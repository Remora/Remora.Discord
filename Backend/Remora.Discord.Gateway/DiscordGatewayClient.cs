//
//  DiscordGatewayClient.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Gateway.Services;
using Remora.Discord.Gateway.Transport;
using Remora.Discord.Rest;
using Remora.Rest.Core;
using Remora.Results;
using static System.Net.WebSockets.WebSocketCloseStatus;

namespace Remora.Discord.Gateway;

/// <summary>
/// Represents a Discord Gateway client.
/// </summary>
[PublicAPI]
public class DiscordGatewayClient : IDisposable
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DiscordGatewayClient> _log;

    private readonly IDiscordRestGatewayAPI _gatewayAPI;
    private readonly DiscordGatewayClientOptions _gatewayOptions;
    private readonly ITokenStore _tokenStore;
    private readonly Random _random;

    private readonly ResponderDispatchService _responderDispatch;

    /// <summary>
    /// Holds payloads that have been submitted by the application, but have not yet been sent to the gateway.
    /// </summary>
    private readonly ConcurrentQueue<IPayload> _payloadsToSend;

    /// <summary>
    /// Holds the websocket.
    /// </summary>
    private readonly IPayloadTransportService _transportService;

    /// <summary>
    /// Holds the connection status.
    /// </summary>
    private GatewayConnectionStatus _connectionStatus;

    /// <summary>
    /// Holds the last sequence number received by the gateway client.
    /// </summary>
    private int _lastSequenceNumber;

    /// <summary>
    /// Holds the time when the last heartbeat was sent, using <see cref="DateTime.ToBinary"/>.
    /// </summary>
    private long _lastSentHeartbeat;

    /// <summary>
    /// Holds the time when the last event acknowledgement was received, encoded using
    /// <see cref="DateTime.ToBinary()"/>.
    /// </summary>
    private long _lastReceivedEvent;

    /// <summary>
    /// Holds the time when the last heartbeat acknowledgement was received, encoded using
    /// <see cref="DateTime.ToBinary()"/>.
    /// </summary>
    private long _lastReceivedHeartbeatAck;

    /// <summary>
    /// Holds the session ID.
    /// </summary>
    private string? _sessionID;

    /// <summary>
    /// Holds the resume gateway URL.
    /// </summary>
    private string? _resumeGatewayUrl;

    /// <summary>
    /// Holds the cancellation token source for internal operations.
    /// </summary>
    private CancellationTokenSource _disconnectRequestedSource;

    /// <summary>
    /// Holds the task responsible for sending payloads to the gateway.
    /// </summary>
    private Task<Result> _sendTask;

    /// <summary>
    /// Holds the task responsible for receiving payloads from the gateway.
    /// </summary>
    private Task<Result> _receiveTask;

    /// <summary>
    /// Holds a value indicating that the client should reconnect and resume at its earliest convenience.
    /// </summary>
    private bool _shouldReconnect;

    /// <summary>
    /// Holds a value indicating whether the client's current session is resumable.
    /// </summary>
    private bool _isSessionResumable;

    /// <summary>
    /// Gets the time taken for the gateway to respond to the last heartbeat, providing an estimate of round-trip latency.
    /// Will return zero until the first heartbeat has occurred.
    /// </summary>
    public TimeSpan Latency { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordGatewayClient"/> class.
    /// </summary>
    /// <param name="gatewayAPI">The gateway API.</param>
    /// <param name="transportService">The payload transport service.</param>
    /// <param name="gatewayOptions">The gateway options.</param>
    /// <param name="tokenStore">The token store.</param>
    /// <param name="random">An entropy source.</param>
    /// <param name="log">The logging instance.</param>
    /// <param name="services">The available services.</param>
    /// <param name="responderDispatch">The responder dispatch service.</param>
    public DiscordGatewayClient
    (
        IDiscordRestGatewayAPI gatewayAPI,
        IPayloadTransportService transportService,
        IOptions<DiscordGatewayClientOptions> gatewayOptions,
        ITokenStore tokenStore,
        Random random,
        ILogger<DiscordGatewayClient> log,
        IServiceProvider services,
        ResponderDispatchService responderDispatch
    )
    {
        _gatewayAPI = gatewayAPI;
        _transportService = transportService;
        _gatewayOptions = gatewayOptions.Value;
        _tokenStore = tokenStore;
        _random = random;
        _log = log;
        _services = services;
        _responderDispatch = responderDispatch;

        _payloadsToSend = new ConcurrentQueue<IPayload>();

        _connectionStatus = GatewayConnectionStatus.Offline;

        _disconnectRequestedSource = new CancellationTokenSource();
        _sendTask = Task.FromResult(Result.FromSuccess());
        _receiveTask = Task.FromResult(Result.FromSuccess());
    }

    /// <summary>
    /// Submits a command to the gateway, enqueueing it for sending.
    /// </summary>
    /// <param name="commandPayload">The command to send.</param>
    /// <typeparam name="TCommand">The type of command to send.</typeparam>
    public void SubmitCommand<TCommand>(TCommand commandPayload) where TCommand : IGatewayCommand
    {
        var payload = new Payload<TCommand>(commandPayload);
        _payloadsToSend.Enqueue(payload);
    }

    /// <summary>
    /// Starts and connects the gateway client.
    /// </summary>
    /// <remarks>
    /// This task will not complete until cancelled (or faulted), maintaining the connection for the duration of it.
    ///
    /// If the gateway client encounters a fatal problem during the execution of this task, it will return with a
    /// failed result. If a shutdown is requested, it will gracefully terminate the connection and return a
    /// successful result.
    /// </remarks>
    /// <param name="stopRequested">A token by which the caller can request this method to stop.</param>
    /// <returns>A gateway connection result which may or may not have succeeded.</returns>
    public async Task<Result> RunAsync(CancellationToken stopRequested)
    {
        try
        {
            if (_connectionStatus != GatewayConnectionStatus.Offline)
            {
                return new InvalidOperationError("Already connected.");
            }

            // Until cancellation has been requested or we hit a fatal error, reconnections should be attempted.
            _disconnectRequestedSource.Dispose();
            _disconnectRequestedSource = new CancellationTokenSource();

            while (!stopRequested.IsCancellationRequested)
            {
                var iterationResult = await RunConnectionIterationAsync(stopRequested);
                if (iterationResult.IsSuccess)
                {
                    continue;
                }

                // Something has gone wrong. Close the socket, and handle it
                // Terminate the send and receive tasks
                _disconnectRequestedSource.Cancel();

                // The results of the send and receive tasks are discarded here, because the iteration result will
                // contain whichever of them failed if any of them did
                _ = await _sendTask;
                _ = await _receiveTask;

                if (_transportService.IsConnected)
                {
                    var disconnectResult = await _transportService.DisconnectAsync(!stopRequested.IsCancellationRequested, stopRequested);
                    if (!disconnectResult.IsSuccess)
                    {
                        // Couldn't disconnect cleanly :(
                        return disconnectResult;
                    }
                }

                if (stopRequested.IsCancellationRequested)
                {
                    // The user requested a termination, and we don't intend to reconnect.
                    return iterationResult;
                }

                if (ShouldReconnect(iterationResult, out var shouldTerminate, out var withNewSession))
                {
                    if (withNewSession)
                    {
                        _sessionID = null;
                        _resumeGatewayUrl = null;
                        _connectionStatus = GatewayConnectionStatus.Disconnected;
                    }
                    else
                    {
                        _connectionStatus = GatewayConnectionStatus.Disconnected;
                    }
                }
                else if (shouldTerminate)
                {
                    return iterationResult;
                }

                // This token's been cancelled, we'll need a new one to reconnect.
                _disconnectRequestedSource.Dispose();
                _disconnectRequestedSource = new CancellationTokenSource();
            }

            var userRequestedDisconnect = await _transportService.DisconnectAsync(false, stopRequested);
            if (!userRequestedDisconnect.IsSuccess)
            {
                // Couldn't disconnect cleanly :(
                return userRequestedDisconnect;
            }
        }
        catch (OperationCanceledException)
        {
            // Pass, this is fine
        }
        catch (Exception e)
        {
            return e;
        }
        finally
        {
            _sessionID = null;
            _resumeGatewayUrl = null;
            _connectionStatus = GatewayConnectionStatus.Offline;
        }

        // Reconnection is not allowed at this point.
        _sessionID = null;
        _resumeGatewayUrl = null;
        _connectionStatus = GatewayConnectionStatus.Offline;

        return Result.FromSuccess();
    }

    // ReSharper disable once CyclomaticComplexity
    // Complexity level is unavoidable in this case; many different cases to handle.
    private bool ShouldReconnect
    (
        Result iterationResult,
        out bool shouldTerminate,
        out bool withNewSession
    )
    {
        shouldTerminate = false;
        withNewSession = false;

        switch (iterationResult.Error)
        {
            case GatewayDiscordError gde:
            {
                _log.LogWarning
                (
                    "Remote transient gateway error: {Error}",
                    gde.Message
                );

                switch (gde.CloseStatus)
                {
                    case GatewayCloseStatus.UnknownError:
                    case GatewayCloseStatus.UnknownOpcode:
                    case GatewayCloseStatus.DecodeError:
                    case GatewayCloseStatus.AlreadyAuthenticated:
                    case GatewayCloseStatus.RateLimited:
                    {
                        return true;
                    }
                    case GatewayCloseStatus.NotAuthenticated:
                    case GatewayCloseStatus.InvalidSequence:
                    case GatewayCloseStatus.SessionTimedOut:
                    {
                        withNewSession = true;
                        return true;
                    }
                    case GatewayCloseStatus.AuthenticationFailed:
                    case GatewayCloseStatus.InvalidShard:
                    case GatewayCloseStatus.ShardingRequired:
                    case GatewayCloseStatus.InvalidAPIVersion:
                    case GatewayCloseStatus.InvalidIntents:
                    case GatewayCloseStatus.DisallowedIntent:
                    {
                        shouldTerminate = true;
                        return false;
                    }
                }

                break;
            }
            case GatewayWebSocketError gwe:
            {
                _log.LogWarning
                (
                    "Transient gateway transport layer error: {Error}",
                    gwe.Message
                );

                switch (gwe.CloseStatus)
                {
                    case InternalServerError:
                    case EndpointUnavailable:
                    {
                        withNewSession = true;
                        return true;
                    }
                }

                break;
            }
            case GatewayError(var message, var isSessionResumable, var isCritical):
            {
                // We'll try reconnecting on non-critical internal errors
                if (!isCritical)
                {
                    _log.LogWarning
                    (
                        "Local transient gateway error: {Error}",
                        message
                    );

                    withNewSession = !isSessionResumable;
                    return true;
                }

                _log.LogError
                (
                    "Local unrecoverable gateway error: {Error}",
                    message
                );

                shouldTerminate = true;
                return false;
            }
            case ExceptionError exe:
            {
                switch (exe.Exception)
                {
                    case HttpRequestException or WebSocketException or TimeoutRejectedException:
                    {
                        _log.LogWarning
                        (
                            exe.Exception,
                            "Transient error in gateway client: {Exception}",
                            exe.Message
                        );

                        return true;
                    }
                    default:
                    {
                        shouldTerminate = true;
                        return false;
                    }
                }
            }
        }

        // We don't know what happened... try reconnecting?
        return true;
    }

    /// <summary>
    /// Runs a single iteration of the connection loop.
    /// </summary>
    /// <param name="stopRequested">A token for requests to stop the outer run loop.</param>
    /// <returns>A connection result, based on the results of the iteration.</returns>
    private async Task<Result> RunConnectionIterationAsync(CancellationToken stopRequested)
    {
        switch (_connectionStatus)
        {
            case GatewayConnectionStatus.Offline:
            case GatewayConnectionStatus.Disconnected:
            {
                _log.LogInformation("Retrieving gateway endpoint...");

                // Start connecting
                var getGatewayEndpoint = await _gatewayAPI.GetGatewayBotAsync(stopRequested);
                if (!getGatewayEndpoint.IsSuccess)
                {
                    return Result.FromError
                    (
                        new GatewayError("Failed to get the gateway endpoint.", false, true),
                        getGatewayEndpoint
                    );
                }

                var endpointInformation = getGatewayEndpoint.Entity;
                if (endpointInformation.Shards.IsDefined(out var shards))
                {
                    if (shards > 1 && _gatewayOptions.ShardIdentification?.ShardCount != shards)
                    {
                        _log.LogInformation
                        (
                            "Discord suggests {Shards} shards for this bot, but your sharding configuration does " +
                            "not match this. Consider switching to a sharded topology of at least that many shards",
                            shards
                        );
                    }
                }

                if (endpointInformation.SessionStartLimit.IsDefined(out var startLimit))
                {
                    if (_sessionID is null)
                    {
                        if (startLimit.Remaining <= 0)
                        {
                            _log.LogWarning
                            (
                                "No further sessions may be started right now for this bot. Waiting {Time} for " +
                                "the limits to reset...",
                                startLimit.ResetAfter
                            );

                            await Task.Delay(startLimit.ResetAfter, stopRequested);
                            return new GatewayError("Session start limits reached; retrying...", false, false);
                        }

                        _log.LogInformation
                        (
                            "Starting a new session ({Remaining} session starts remaining of {Total}; limits " +
                            "reset in {Time})",
                            startLimit.Remaining,
                            startLimit.Total,
                            startLimit.ResetAfter
                        );
                    }
                    else
                    {
                        _log.LogInformation
                        (
                            "Resuming an existing session ({Remaining} new session starts remaining of {Total}; " +
                            "limits reset in {Time})",
                            startLimit.Remaining,
                            startLimit.Total,
                            startLimit.ResetAfter
                        );
                    }
                }
                else
                {
                    _log.LogWarning
                    (
                        "There are no session start limits available for this connection. Rate limits may be " +
                        "unexpectedly hit"
                    );
                }

                var gatewayEndpointUrl = _resumeGatewayUrl is not null && _isSessionResumable
                    ? _resumeGatewayUrl
                    : getGatewayEndpoint.Entity.Url;

                var gatewayEndpoint = $"{gatewayEndpointUrl}?v={(int)DiscordAPIVersion.V10}&encoding=json";
                if (!Uri.TryCreate(gatewayEndpoint, UriKind.Absolute, out var gatewayUri))
                {
                    return new GatewayError
                    (
                        "Failed to parse the received gateway endpoint.",
                        false,
                        true
                    );
                }

                _log.LogInformation("Connecting to the gateway...");

                var transportConnectResult = await _transportService.ConnectAsync(gatewayUri, stopRequested);
                if (!transportConnectResult.IsSuccess)
                {
                    return transportConnectResult;
                }

                var timeoutPolicy = Policy.TimeoutAsync<Result<IPayload>>(TimeSpan.FromSeconds(5));

                var receiveHello = await timeoutPolicy.ExecuteAsync
                (
                    c => _transportService.ReceivePayloadAsync(c),
                    stopRequested
                );

                if (!receiveHello.IsSuccess)
                {
                    return Result.FromError
                    (
                        new GatewayError("Failed to receive the Hello payload.", false, true),
                        receiveHello
                    );
                }

                if (receiveHello.Entity is not IPayload<IHello> hello)
                {
                    // Not receiving a hello is a non-recoverable error
                    return new GatewayError
                    (
                        "The first payload from the gateway was not a hello. Rude!",
                        false,
                        true
                    );
                }

                var dispatch = await _responderDispatch.DispatchAsync
                (
                    receiveHello.Entity,
                    _disconnectRequestedSource.Token
                );

                if (!dispatch.IsSuccess)
                {
                    return dispatch;
                }

                // Set up the send task
                var heartbeatInterval = hello.Data.HeartbeatInterval;

                _sendTask = GatewaySenderAsync(heartbeatInterval, _disconnectRequestedSource.Token);

                // Attempt to connect or resume
                var connectResult = await AttemptConnectionAsync(stopRequested);
                if (!connectResult.IsSuccess)
                {
                    return connectResult;
                }

                // Now, set up the receive task and start receiving events normally
                _receiveTask = GatewayReceiverAsync(heartbeatInterval, _disconnectRequestedSource.Token);

                _log.LogInformation("Connected");

                _shouldReconnect = false;
                _isSessionResumable = false;
                _lastReceivedHeartbeatAck = 0;
                _lastReceivedEvent = 0;

                _connectionStatus = GatewayConnectionStatus.Connected;

                break;
            }
            case GatewayConnectionStatus.Connected:
            {
                // Check the send and receive tasks for errors
                if (_sendTask.IsCompleted)
                {
                    var sendResult = await _sendTask;
                    if (!sendResult.IsSuccess)
                    {
                        return sendResult;
                    }
                }

                if (_receiveTask.IsCompleted)
                {
                    var receiveResult = await _receiveTask;
                    if (!receiveResult.IsSuccess)
                    {
                        return receiveResult;
                    }
                }

                // No need to check for errors like crazy
                await Task.Delay(TimeSpan.FromMilliseconds(100), stopRequested);
                break;
            }
        }

        if (!stopRequested.IsCancellationRequested)
        {
            if (!_shouldReconnect)
            {
                return Result.FromSuccess();
            }

            _log.LogInformation("Reconnection requested by the gateway; terminating session...");
        }

        // Terminate the send and receive tasks
        _disconnectRequestedSource.Cancel();

        // The results of the send and receive tasks are discarded here, because we know that it's going to be a
        // cancellation
        _ = await _sendTask;
        _ = await _receiveTask;

        var disconnectResult = await _transportService.DisconnectAsync(!stopRequested.IsCancellationRequested, stopRequested);
        if (!disconnectResult.IsSuccess)
        {
            return disconnectResult;
        }

        // Set up the state for the new connection
        _disconnectRequestedSource.Dispose();
        _disconnectRequestedSource = new CancellationTokenSource();
        _connectionStatus = GatewayConnectionStatus.Disconnected;

        return Result.FromSuccess();
    }

    /// <summary>
    /// Attempts to identify or resume the gateway connection.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A connection result which may or may not have succeeded.</returns>
    private Task<Result> AttemptConnectionAsync(CancellationToken ct = default)
    {
        if (_sessionID is null || !_isSessionResumable)
        {
            // We've never connected before, or the current session isn't resumable
            return CreateNewSessionAsync(ct);
        }

        return ResumeExistingSessionAsync(ct);
    }

    /// <summary>
    /// Creates a new session with the gateway, identifying the client.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A connection result which may or may not have succeeded.</returns>
    private async Task<Result> CreateNewSessionAsync(CancellationToken ct = default)
    {
        try
        {
            _log.LogInformation("Creating a new session...");

            var shardInformation = _gatewayOptions.ShardIdentification is null
                ? default
                : new Optional<IShardIdentification>(_gatewayOptions.ShardIdentification);

            var initialPresence = _gatewayOptions.Presence is null
                ? default
                : new Optional<IUpdatePresence>(_gatewayOptions.Presence);

            SubmitCommand
            (
                new Identify
                (
                    _tokenStore.Token,
                    _gatewayOptions.ConnectionProperties,
                    false,
                    _gatewayOptions.LargeThreshold,
                    shardInformation,
                    initialPresence,
                    _gatewayOptions.Intents
                )
            );

            var timeoutPolicy = Policy.TimeoutAsync<Result<IPayload>>(TimeSpan.FromSeconds(5));

            while (true)
            {
                var receiveReady = await timeoutPolicy.ExecuteAsync(c => _transportService.ReceivePayloadAsync(c), ct);
                if (!receiveReady.IsSuccess)
                {
                    return Result.FromError(receiveReady);
                }

                switch (receiveReady.Entity)
                {
                    case IPayload<IHeartbeatAcknowledge>:
                    {
                        continue;
                    }
                    case IPayload<IReconnect>:
                    {
                        return new GatewayError
                        (
                            "The newly created session was invalidated by Discord.",
                            false,
                            false
                        );
                    }
                    case IPayload<IInvalidSession> invalidSession:
                    {
                        return new GatewayError
                        (
                            "The newly created session was invalidated by Discord.",
                            invalidSession.Data.IsResumable,
                            false
                        );
                    }
                    case IPayload<IReady> ready:
                    {
                        var dispatch = await _responderDispatch.DispatchAsync
                        (
                            ready,
                            _disconnectRequestedSource.Token
                        );

                        if (!dispatch.IsSuccess)
                        {
                            return dispatch;
                        }

                        _sessionID = ready.Data.SessionID;
                        _resumeGatewayUrl = ready.Data.ResumeGatewayUrl;

                        return Result.FromSuccess();
                    }
                    default:
                    {
                        _log.LogTrace("Payload Body: {Body}", JsonSerializer.Serialize(receiveReady.Entity));

                        return new GatewayError
                        (
                            $"The payload after identification was not a Ready payload.{Environment.NewLine}" +
                            $"\tExpected: {typeof(IPayload<IReady>).FullName}{Environment.NewLine}" +
                            $"\tActual: {receiveReady.Entity.GetType().FullName}",
                            false,
                            true
                        );
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <summary>
    /// Resumes an existing session with the gateway, replaying missed events.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A connection result which may or may not have succeeded.</returns>
    private async Task<Result> ResumeExistingSessionAsync(CancellationToken ct = default)
    {
        try
        {
            if (_sessionID is null)
            {
                return new InvalidOperationError("There's no previous session to resume.");
            }

            _log.LogInformation("Resuming existing session...");

            SubmitCommand
            (
                new Resume
                (
                    _tokenStore.Token,
                    _sessionID,
                    _lastSequenceNumber
                )
            );

            var timeoutPolicy = Policy.TimeoutAsync<Result<IPayload>>(TimeSpan.FromSeconds(5));

            // Push resumed events onto the queue
            var resuming = true;
            while (resuming)
            {
                if (ct.IsCancellationRequested)
                {
                    return new GatewayError("Operation was cancelled.", false, false);
                }

                var receiveEvent = await timeoutPolicy.ExecuteAsync(c => _transportService.ReceivePayloadAsync(c), ct);
                if (!receiveEvent.IsSuccess)
                {
                    return Result.FromError(new GatewayError("Failed to receive a payload.", true, false), receiveEvent);
                }

                switch (receiveEvent.Entity)
                {
                    case IPayload<IHeartbeatAcknowledge>:
                    {
                        continue;
                    }
                    case IPayload<IInvalidSession>:
                    {
                        _log.LogInformation("Resume rejected by the gateway");

                        await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(1000, 5000)), ct);
                        return await CreateNewSessionAsync(ct);
                    }
                    case IPayload<IResumed>:
                    {
                        resuming = false;
                        break;
                    }
                }

                var dispatch = await _responderDispatch.DispatchAsync
                (
                    receiveEvent.Entity,
                    _disconnectRequestedSource.Token
                );

                if (!dispatch.IsSuccess)
                {
                    return dispatch;
                }
            }

            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <summary>
    /// This method acts as the main entrypoint for the gateway sender task. It processes payloads that are
    /// submitted by the application to the gateway, sending them to it.
    /// </summary>
    /// <param name="heartbeatInterval">The interval at which heartbeats should be sent.</param>
    /// <param name="disconnectRequested">A token for requests to disconnect the socket.</param>
    /// <returns>A sender result which may or may not have been successful. A failed result indicates that something
    /// has gone wrong when sending a payload, and that the connection has been deemed nonviable. A nonviable
    /// connection should be either terminated, reestablished, or resumed as appropriate.</returns>
    private async Task<Result> GatewaySenderAsync
    (
        TimeSpan heartbeatInterval,
        CancellationToken disconnectRequested
    )
    {
        await Task.Yield();

        try
        {
            // Figure out how many slots we need to reserve for heartbeats
            var rateLimitWindow = TimeSpan.FromSeconds(60);
            var heartbeatsPerRateLimitWindow = (int)Math.Ceiling(rateLimitWindow / heartbeatInterval);

            int maxEventsPerRateLimitWindow;
            if (heartbeatsPerRateLimitWindow + _gatewayOptions.HeartbeatHeadroom >= 120)
            {
                _log.LogWarning
                (
                    "Unreasonable heartbeat interval ({Interval}) requested - not reserving any slots",
                    heartbeatInterval
                );

                maxEventsPerRateLimitWindow = 120;
            }
            else
            {
                maxEventsPerRateLimitWindow = 120 - heartbeatsPerRateLimitWindow - _gatewayOptions.HeartbeatHeadroom;
            }

            var rateLimitPolicy = Policy.RateLimitAsync<Result>
            (
                maxEventsPerRateLimitWindow,
                rateLimitWindow,
                _gatewayOptions.CommandBurstRate,
                (retryAfter, _) => new RetryAfterError(retryAfter)
            );

            while (!disconnectRequested.IsCancellationRequested)
            {
                var lastSentHeartbeatTime = ReadTimeAtomic(ref _lastSentHeartbeat);

                var now = DateTime.UtcNow;
                var safetyMargin = _gatewayOptions.GetTrueHeartbeatSafetyMargin(heartbeatInterval);

                var needsHeartbeat = lastSentHeartbeatTime is null ||
                                     now - lastSentHeartbeatTime >= heartbeatInterval - safetyMargin;

                Result sendResult;

                // Heartbeats are prioritized over user-submitted payloads, and are sent without using the rate limit
                // policy to avoid timing issues.
                if (needsHeartbeat)
                {
                    var lastReceivedEventTime = ReadTimeAtomic(ref _lastReceivedEvent);
                    var lastHeartbeatAckTime = ReadTimeAtomic(ref _lastReceivedHeartbeatAck);

                    var isConnectionSilent = lastHeartbeatAckTime < lastSentHeartbeatTime &&
                                             now - lastReceivedEventTime >= heartbeatInterval - safetyMargin;

                    if (isConnectionSilent)
                    {
                        return new GatewayError
                        (
                            "The server did not respond in time with a heartbeat acknowledgement or an incoming event.",
                            true,
                            false
                        );
                    }

                    // 32-bit reads are atomic, so this is fine
                    var lastSequenceNumber = _lastSequenceNumber;

                    var heartbeatPayload = new Payload<IHeartbeat>(new Heartbeat
                    (
                        lastSequenceNumber == 0 ? null : lastSequenceNumber
                    ));

                    sendResult = await _transportService.SendPayloadAsync(heartbeatPayload, disconnectRequested);
                    WriteTimeAtomic(ref _lastSentHeartbeat, DateTime.UtcNow);
                }
                else
                {
                    if (!_payloadsToSend.TryPeek(out var userPayload))
                    {
                        // Sleep for a little bit
                        await Task.Delay(CalculateAllowedSleepTime(heartbeatInterval), disconnectRequested);
                        continue;
                    }

                    while (true)
                    {
                        sendResult = await rateLimitPolicy.ExecuteAsync
                        (
                            () => _transportService.SendPayloadAsync(userPayload, disconnectRequested)
                        );

                        if (sendResult.IsSuccess)
                        {
                            // Dequeue the peeked payload, now that we've sent it
                            _payloadsToSend.TryDequeue(out _);
                            break;
                        }

                        if (sendResult.Error is RetryAfterError rae)
                        {
                            var allowedSleepTime = CalculateAllowedSleepTime(heartbeatInterval);
                            if (rae.RetryAfter >= allowedSleepTime)
                            {
                                // Won't have time to send this until we have to heartbeat again... give up for now
                                await Task.Delay(allowedSleepTime, disconnectRequested);

                                sendResult = Result.FromSuccess();
                                break;
                            }

                            await Task.Delay(rae.RetryAfter, disconnectRequested);
                            continue;
                        }

                        break;
                    }
                }

                if (sendResult.IsSuccess)
                {
                    continue;
                }

                // Normal closures are okay
                return sendResult.Error is GatewayWebSocketError { CloseStatus: NormalClosure }
                    ? Result.FromSuccess()
                    : sendResult;
            }

            return Result.FromSuccess();
        }
        catch (OperationCanceledException)
        {
            // Cancellation is a success
            return Result.FromSuccess();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <summary>
    /// This method acts as the main entrypoint for the gateway receiver task. It processes payloads that are
    /// sent from the gateway to the application, submitting them to it.
    /// </summary>
    /// <param name="heartbeatInterval">The interval at which heartbeats should be sent.</param>
    /// <param name="disconnectRequested">A token for requests to disconnect the socket.</param>
    /// <returns>A receiver result which may or may not have been successful. A failed result indicates that
    /// something has gone wrong when receiving a payload, and that the connection has been deemed nonviable. A
    /// nonviable connection should be either terminated, reestablished, or resumed as appropriate.</returns>
    private async Task<Result> GatewayReceiverAsync(TimeSpan heartbeatInterval, CancellationToken disconnectRequested)
    {
        await Task.Yield();

        try
        {
            var timeoutPolicy = Policy.TimeoutAsync<Result<IPayload>>(heartbeatInterval * 2);

            while (!disconnectRequested.IsCancellationRequested)
            {
                var receivedPayload = await timeoutPolicy.ExecuteAsync
                (
                    c => _transportService.ReceivePayloadAsync(c),
                    disconnectRequested
                );

                var receivedAt = DateTime.UtcNow;

                if (!receivedPayload.IsSuccess)
                {
                    // Normal closures are okay
                    return receivedPayload.Error is GatewayWebSocketError { CloseStatus: NormalClosure }
                        ? Result.FromSuccess()
                        : Result.FromError(receivedPayload);
                }

                // Update the sequence number
                if (receivedPayload.Entity is IEventPayload eventPayload)
                {
                    Interlocked.Exchange(ref _lastSequenceNumber, eventPayload.SequenceNumber);
                    WriteTimeAtomic(ref _lastReceivedEvent, receivedAt);
                }

                // Update the ack timestamp
                if (receivedPayload.Entity is IPayload<IHeartbeatAcknowledge>)
                {
                    WriteTimeAtomic(ref _lastReceivedHeartbeatAck, receivedAt);

                    // Update the latency
                    var lastSentHeartbeat = ReadTimeAtomic(ref _lastSentHeartbeat);
                    if (lastSentHeartbeat != null)
                    {
                        this.Latency = receivedAt - lastSentHeartbeat.Value;
                    }
                }

                // Enqueue the payload for dispatch
                var dispatch = await _responderDispatch.DispatchAsync
                (
                    receivedPayload.Entity,
                    _disconnectRequestedSource.Token
                );

                if (!dispatch.IsSuccess)
                {
                    return dispatch;
                }

                // Signal the governor task that a reconnection is requested, if necessary.
                switch (receivedPayload.Entity)
                {
                    case IPayload<IReconnect>:
                    {
                        _shouldReconnect = true;
                        _isSessionResumable = true;

                        break;
                    }
                    case IPayload<IInvalidSession> invalidSession:
                    {
                        _shouldReconnect = true;
                        _isSessionResumable = invalidSession.Data.IsResumable;

                        break;
                    }
                    case IPayload<IHeartbeat>:
                    {
                        // 32-bit reads are atomic, so this is fine
                        var lastSequenceNumber = _lastSequenceNumber;

                        var heartbeat = new Heartbeat
                        (
                            lastSequenceNumber == 0 ? null : lastSequenceNumber
                        );

                        SubmitCommand(heartbeat);
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }

                break;
            }

            return Result.FromSuccess();
        }
        catch (OperationCanceledException)
        {
            // Cancellation is a success
            return Result.FromSuccess();
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private static DateTime? ReadTimeAtomic(ref long field)
    {
        var binary = Interlocked.Read(ref field);
        return binary > 0
            ? DateTime.FromBinary(binary)
            : null;
    }

    private static void WriteTimeAtomic(ref long field, DateTime value)
    {
        Interlocked.Exchange(ref field, value.ToBinary());
    }

    private TimeSpan CalculateAllowedSleepTime(TimeSpan heartbeatInterval)
    {
        var lastSentHeartbeatBinary = Interlocked.Read(ref _lastSentHeartbeat);
        var lastSentHeartbeat = lastSentHeartbeatBinary > 0
            ? DateTime.FromBinary(lastSentHeartbeatBinary)
            : (DateTime?)null;

        var now = DateTime.UtcNow;
        var safetyMargin = _gatewayOptions.GetTrueHeartbeatSafetyMargin(heartbeatInterval);

        var maxSleepTime = lastSentHeartbeat + heartbeatInterval - safetyMargin - now;

        return maxSleepTime is null
            ? safetyMargin
            : maxSleepTime.Value <= TimeSpan.Zero
                ? TimeSpan.Zero
                : TimeSpan.FromMilliseconds(Math.Clamp(100, 0, maxSleepTime.Value.TotalMilliseconds));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _disconnectRequestedSource.Dispose();
    }
}
