//
//  DiscordGatewayClient.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Gateway.Services;
using Remora.Discord.Gateway.Transport;
using Remora.Results;
using static System.Net.WebSockets.WebSocketCloseStatus;

namespace Remora.Discord.Gateway
{
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
        private readonly IResponderTypeRepository _responderTypeRepository;

        /// <summary>
        /// Holds payloads that have been submitted by the application, but have not yet been sent to the gateway.
        /// </summary>
        private readonly ConcurrentQueue<IPayload> _payloadsToSend;

        /// <summary>
        /// Holds payloads that have been received by the gateway, but not yet distributed to the application.
        /// </summary>
        private readonly ConcurrentQueue<IPayload> _receivedPayloads;

        /// <summary>
        /// Holds the currently running responders.
        /// </summary>
        private readonly ConcurrentQueue<Task<IReadOnlyList<Result>>> _runningResponderDispatches;

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
        /// Holds the time when the last heartbeat acknowledgement was received, using
        /// <see cref="DateTime.ToBinary()"/>.
        /// </summary>
        private long _lastReceivedHeartbeatAck;

        /// <summary>
        /// Holds the session ID.
        /// </summary>
        private string? _sessionID;

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
        /// Initializes a new instance of the <see cref="DiscordGatewayClient"/> class.
        /// </summary>
        /// <param name="gatewayAPI">The gateway API.</param>
        /// <param name="transportService">The payload transport service.</param>
        /// <param name="gatewayOptions">The gateway options.</param>
        /// <param name="tokenStore">The token store.</param>
        /// <param name="random">An entropy source.</param>
        /// <param name="log">The logging instance.</param>
        /// <param name="services">The available services.</param>
        /// <param name="responderTypeRepository">The responder type repository.</param>
        public DiscordGatewayClient
        (
            IDiscordRestGatewayAPI gatewayAPI,
            IPayloadTransportService transportService,
            IOptions<DiscordGatewayClientOptions> gatewayOptions,
            ITokenStore tokenStore,
            Random random,
            ILogger<DiscordGatewayClient> log,
            IServiceProvider services,
            IResponderTypeRepository responderTypeRepository
        )
        {
            _gatewayAPI = gatewayAPI;
            _transportService = transportService;
            _gatewayOptions = gatewayOptions.Value;
            _tokenStore = tokenStore;
            _random = random;
            _log = log;
            _services = services;
            _responderTypeRepository = responderTypeRepository;

            _runningResponderDispatches = new ConcurrentQueue<Task<IReadOnlyList<Result>>>();

            _payloadsToSend = new ConcurrentQueue<IPayload>();
            _receivedPayloads = new ConcurrentQueue<IPayload>();

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
        public void SubmitCommandAsync<TCommand>(TCommand commandPayload) where TCommand : IGatewayCommand
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
                    return new GenericError("Already connected.");
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
                        var disconnectResult = await _transportService.DisconnectAsync(stopRequested.IsCancellationRequested, stopRequested);
                        if (!disconnectResult.IsSuccess)
                        {
                            // Couldn't disconnect cleanly :(
                            return disconnectResult;
                        }
                    }

                    // Finish up the responders
                    foreach (var runningResponder in _runningResponderDispatches)
                    {
                        await FinalizeResponderDispatchAsync(runningResponder);
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
                _connectionStatus = GatewayConnectionStatus.Offline;
            }

            // Reconnection is not allowed at this point.
            _sessionID = null;
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
                case ExceptionError exe:
                {
                    switch (exe.Exception)
                    {
                        case HttpRequestException or WebSocketException:
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
                            new GenericError("Failed to get the gateway endpoint."),
                            getGatewayEndpoint
                        );
                    }

                    var gatewayEndpoint = $"{getGatewayEndpoint.Entity.Url}?v=9&encoding=json";
                    if (!Uri.TryCreate(gatewayEndpoint, UriKind.Absolute, out var gatewayUri))
                    {
                        return new GenericError
                        (
                            "Failed to parse the received gateway endpoint."
                        );
                    }

                    _log.LogInformation("Connecting to the gateway...");

                    var transportConnectResult = await _transportService.ConnectAsync(gatewayUri, stopRequested);
                    if (!transportConnectResult.IsSuccess)
                    {
                        return transportConnectResult;
                    }

                    var receiveHello = await _transportService.ReceivePayloadAsync(stopRequested);
                    if (!receiveHello.IsSuccess)
                    {
                        return Result.FromError(new GenericError("Failed to receive the Hello payload."), receiveHello);
                    }

                    if (!(receiveHello.Entity is IPayload<IHello> hello))
                    {
                        // Not receiving a hello is a non-recoverable error
                        return new GenericError
                        (
                            "The first payload from the gateway was not a hello. Rude!"
                        );
                    }

                    UnwrapAndDispatchEvent(receiveHello.Entity, _disconnectRequestedSource.Token);

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
                    _receiveTask = GatewayReceiverAsync(_disconnectRequestedSource.Token);

                    _log.LogInformation("Connected");

                    _shouldReconnect = false;
                    _isSessionResumable = false;
                    _lastReceivedHeartbeatAck = 0;

                    _connectionStatus = GatewayConnectionStatus.Connected;

                    break;
                }
                case GatewayConnectionStatus.Connected:
                {
                    // Process received events and dispatch them to the application
                    if (_receivedPayloads.TryDequeue(out var payload))
                    {
                        UnwrapAndDispatchEvent(payload, _disconnectRequestedSource.Token);
                    }

                    // Unpack one of the running responders, if any are pending
                    if (_runningResponderDispatches.TryDequeue(out var runningResponder))
                    {
                        if (runningResponder.IsCompleted)
                        {
                            await FinalizeResponderDispatchAsync(runningResponder);
                        }
                        else
                        {
                            _runningResponderDispatches.Enqueue(runningResponder);
                        }
                    }

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

                    await Task.Delay(TimeSpan.FromMilliseconds(10), stopRequested);
                    break;
                }
            }

            if (!_shouldReconnect)
            {
                return Result.FromSuccess();
            }

            _log.LogInformation("Reconnection requested by the gateway; terminating session...");

            // Terminate the send and receive tasks
            _disconnectRequestedSource.Cancel();

            // The results of the send and receive tasks are discarded here, because we know that it's going to be a
            // cancellation
            _ = await _sendTask;
            _ = await _receiveTask;

            var disconnectResult = await _transportService.DisconnectAsync(true, stopRequested);
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
        /// Finalizes the given running responder, awaiting it and logging its results.
        /// </summary>
        /// <param name="runningResponderDispatch">The running responder dispatch.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task FinalizeResponderDispatchAsync(Task<IReadOnlyList<Result>> runningResponderDispatch)
        {
            try
            {
                var responderResults = await runningResponderDispatch;
                foreach (var responderResult in responderResults)
                {
                    if (responderResult.IsSuccess)
                    {
                        continue;
                    }

                    switch (responderResult.Error)
                    {
                        case ExceptionError exe:
                        {
                            _log.LogWarning
                            (
                                exe.Exception,
                                "Error in gateway event responder: {Exception}",
                                exe.Message
                            );

                            break;
                        }
                        default:
                        {
                            _log.LogWarning
                            (
                                "Error in gateway event responder.\n{Reason}",
                                responderResult.Error.Message
                            );

                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Pass, this is fine
            }
            catch (AggregateException aex)
            {
                foreach (var e in aex.InnerExceptions)
                {
                    if (e is OperationCanceledException)
                    {
                        continue;
                    }

                    _log.LogWarning("Error in gateway event responder.\n{Exception}", e);
                }
            }
            catch (Exception e)
            {
                _log.LogWarning("Error in gateway event responder.\n{Exception}", e);
            }
        }

        /// <summary>
        /// Unwraps the given payload into its typed representation, dispatching all events for it.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="ct">The cancellation token for the dispatched event.</param>
        private void UnwrapAndDispatchEvent(IPayload payload, CancellationToken ct = default)
        {
            var dispatchMethod = GetType().GetMethod
            (
                nameof(DispatchEventAsync),
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (dispatchMethod is null)
            {
                throw new MissingMethodException(nameof(DiscordGatewayClient), nameof(DispatchEventAsync));
            }

            var payloadType = payload.GetType();
            if (!payloadType.IsGenericType)
            {
                _log.LogWarning
                (
                    "The given payload of type {PayloadType} was not compatible with the event dispatcher",
                    payloadType
                );

                return;
            }

            var payloadInterfaceType = payloadType.GetInterfaces().FirstOrDefault
            (
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPayload<>)
            );

            if (payloadInterfaceType is null)
            {
                _log.LogWarning
                (
                    "The given payload of type {PayloadType} was not compatible with the event dispatcher",
                    payloadType
                );

                return;
            }

            var boundDispatchMethod = dispatchMethod.MakeGenericMethod(payloadInterfaceType.GetGenericArguments());
            var dispatchTask = boundDispatchMethod.Invoke(this, new object?[] { payload, ct });
            if (dispatchTask is null)
            {
                throw new InvalidOperationException();
            }

            _runningResponderDispatches.Enqueue((Task<IReadOnlyList<Result>>)dispatchTask);
        }

        /// <summary>
        /// Dispatches the given event to all relevant gateway event responders.
        /// </summary>
        /// <param name="gatewayEvent">The event to dispatch.</param>
        /// <param name="ct">The cancellation token to use.</param>
        /// <typeparam name="TGatewayEvent">The gateway event.</typeparam>
        private async Task<IReadOnlyList<Result>> DispatchEventAsync<TGatewayEvent>
        (
            IPayload<TGatewayEvent> gatewayEvent,
            CancellationToken ct = default
        )
            where TGatewayEvent : IGatewayEvent
        {
            // Batch up the responders according to their groups
            var responderGroups = new[]
            {
                _responderTypeRepository.GetEarlyResponderTypes<TGatewayEvent>(),
                _responderTypeRepository.GetResponderTypes<TGatewayEvent>(),
                _responderTypeRepository.GetLateResponderTypes<TGatewayEvent>(),
            };

            // Run through the groups in order
            var results = new List<Result>();
            foreach (var responderGroup in responderGroups)
            {
                var groupResults = await Task.WhenAll
                (
                    responderGroup.Select
                    (
                        async rt =>
                        {
                            using var serviceScope = _services.CreateScope();
                            var responder = (IResponder<TGatewayEvent>)serviceScope.ServiceProvider
                                .GetRequiredService(rt);

                            try
                            {
                                return await responder.RespondAsync(gatewayEvent.Data, ct);
                            }
                            catch (Exception e)
                            {
                                return e;
                            }
                            finally
                            {
                                // Suspicious type conversions are disabled here, since the user-defined responders may
                                // implement IDisposable or IAsyncDisposable.

                                // ReSharper disable once SuspiciousTypeConversion.Global
                                if (responder is IDisposable disposable)
                                {
                                    disposable.Dispose();
                                }

                                // ReSharper disable once SuspiciousTypeConversion.Global
                                if (responder is IAsyncDisposable asyncDisposable)
                                {
                                    await asyncDisposable.DisposeAsync();
                                }
                            }
                        }
                    )
                ).ConfigureAwait(false);

                results.AddRange(groupResults);
            }

            return results;
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
            _log.LogInformation("Creating a new session...");

            var shardInformation = _gatewayOptions.ShardIdentification is null
                ? default
                : new Optional<IShardIdentification>(_gatewayOptions.ShardIdentification);

            var initialPresence = _gatewayOptions.Presence is null
                ? default
                : new Optional<IUpdatePresence>(_gatewayOptions.Presence);

            SubmitCommandAsync
            (
                new Identify
                (
                    _tokenStore.Token,
                    _gatewayOptions.ConnectionProperties,
                    Intents: _gatewayOptions.Intents,
                    Compress: false,
                    Shard: shardInformation,
                    Presence: initialPresence
                )
            );

            while (true)
            {
                var receiveReady = await _transportService.ReceivePayloadAsync(ct);
                if (!receiveReady.IsSuccess)
                {
                    return Result.FromError(receiveReady);
                }

                if (receiveReady.Entity is IPayload<IHeartbeatAcknowledge>)
                {
                    continue;
                }

                if (!(receiveReady.Entity is IPayload<IReady> ready))
                {
                    return new GenericError
                    (
                        "The payload after identification was not a Ready payload."
                    );
                }

                UnwrapAndDispatchEvent(receiveReady.Entity, _disconnectRequestedSource.Token);

                _sessionID = ready.Data.SessionID;
                break;
            }

            return Result.FromSuccess();
        }

        /// <summary>
        /// Resumes an existing session with the gateway, replaying missed events.
        /// </summary>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A connection result which may or may not have succeeded.</returns>
        private async Task<Result> ResumeExistingSessionAsync(CancellationToken ct = default)
        {
            if (_sessionID is null)
            {
                return new GenericError("There's no previous session to resume.");
            }

            _log.LogInformation("Resuming existing session...");

            SubmitCommandAsync
            (
                new Resume
                (
                    _tokenStore.Token,
                    _sessionID,
                    _lastSequenceNumber
                )
            );

            // Push resumed events onto the queue
            var resuming = true;
            while (resuming)
            {
                if (ct.IsCancellationRequested)
                {
                    return new GenericError("Operation was cancelled.");
                }

                var receiveEvent = await _transportService.ReceivePayloadAsync(ct);
                if (!receiveEvent.IsSuccess)
                {
                    return Result.FromError(new GenericError("Failed to receive a payload."), receiveEvent);
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
                        UnwrapAndDispatchEvent(receiveEvent.Entity, _disconnectRequestedSource.Token);

                        resuming = false;
                        break;
                    }
                }

                _receivedPayloads.Enqueue(receiveEvent.Entity);
            }

            return Result.FromSuccess();
        }

        /// <summary>
        /// This method acts as the main entrypoint for the gateway sender task. It processes payloads that are
        /// submitted by the application to the gateway, sending them to it.
        /// </summary>
        /// <param name="heartbeatInterval">The interval at which heartbeats should be sent.</param>
        /// <param name="disconnectRequested">A token for requests to disconnect the socket..</param>
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
                DateTime? lastHeartbeat = null;
                while (!disconnectRequested.IsCancellationRequested)
                {
                    var lastReceivedHeartbeatAck = Interlocked.Read(ref _lastReceivedHeartbeatAck);
                    var lastHeartbeatAck = lastReceivedHeartbeatAck > 0
                        ? DateTime.FromBinary(lastReceivedHeartbeatAck)
                        : (DateTime?)null;

                    // Heartbeat, if required
                    var now = DateTime.UtcNow;
                    var safetyMargin = _gatewayOptions.GetTrueHeartbeatSafetyMargin(heartbeatInterval);

                    if (lastHeartbeat is null || now - lastHeartbeat >= heartbeatInterval - safetyMargin)
                    {
                        if (lastHeartbeatAck < lastHeartbeat)
                        {
                            return new GatewayError
                            (
                                "The server did not respond in time with a heartbeat acknowledgement."
                            );
                        }

                        // 32-bit reads are atomic, so this is fine
                        var lastSequenceNumber = _lastSequenceNumber;

                        var heartbeatPayload = new Payload<IHeartbeat>
                        (
                            new Heartbeat
                            (
                                lastSequenceNumber == 0 ? null : lastSequenceNumber
                            )
                        );

                        var sendHeartbeat = await _transportService.SendPayloadAsync(heartbeatPayload, disconnectRequested);

                        if (!sendHeartbeat.IsSuccess)
                        {
                            return Result.FromError(new GenericError("Failed to send a heartbeat."), sendHeartbeat);
                        }

                        lastHeartbeat = DateTime.UtcNow;
                    }

                    // Check if there are any user-submitted payloads to send
                    if (!_payloadsToSend.TryDequeue(out var payload))
                    {
                        // Let's sleep for a little while
                        var maxSleepTime = lastHeartbeat.Value + heartbeatInterval - safetyMargin - now;
                        var sleepTime = TimeSpan.FromMilliseconds(Math.Clamp(100, 0, maxSleepTime.TotalMilliseconds));

                        await Task.Delay(sleepTime, disconnectRequested);
                        continue;
                    }

                    var sendResult = await _transportService.SendPayloadAsync(payload, disconnectRequested);
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
        /// <param name="disconnectRequested">A token for requests to disconnect the socket.</param>
        /// <returns>A receiver result which may or may not have been successful. A failed result indicates that
        /// something has gone wrong when receiving a payload, and that the connection has been deemed nonviable. A
        /// nonviable connection should be either terminated, reestablished, or resumed as appropriate.</returns>
        private async Task<Result> GatewayReceiverAsync(CancellationToken disconnectRequested)
        {
            await Task.Yield();

            try
            {
                while (!disconnectRequested.IsCancellationRequested)
                {
                    var receivedPayload = await _transportService.ReceivePayloadAsync(disconnectRequested);
                    if (!receivedPayload.IsSuccess)
                    {
                        // Normal closures are okay
                        if (receivedPayload.Error is GatewayWebSocketError { CloseStatus: NormalClosure })
                        {
                            return Result.FromSuccess();
                        }

                        return Result.FromError(receivedPayload);
                    }

                    // Update the sequence number
                    if (receivedPayload.Entity is IEventPayload eventPayload)
                    {
                        Interlocked.Exchange(ref _lastSequenceNumber, eventPayload.SequenceNumber);
                    }

                    // Update the ack timestamp
                    if (receivedPayload.Entity is IPayload<IHeartbeatAcknowledge>)
                    {
                        Interlocked.Exchange(ref _lastReceivedHeartbeatAck, DateTime.UtcNow.ToBinary());
                    }

                    // Enqueue the payload for dispatch
                    _receivedPayloads.Enqueue(receivedPayload.Entity);

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
                            SubmitCommandAsync(new HeartbeatAcknowledge());
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

        /// <inheritdoc />
        public void Dispose()
        {
            _disconnectRequestedSource.Dispose();
        }
    }
}
