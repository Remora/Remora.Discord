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
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
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
        private readonly ConcurrentQueue<Task<EventResponseResult[]>> _runningResponderDispatches;

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
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Holds the task responsible for sending payloads to the gateway.
        /// </summary>
        private Task<GatewaySenderResult> _sendTask;

        /// <summary>
        /// Holds the task responsible for receiving payloads from the gateway.
        /// </summary>
        private Task<GatewayReceiverResult> _receiveTask;

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

            _runningResponderDispatches = new ConcurrentQueue<Task<EventResponseResult[]>>();

            _payloadsToSend = new ConcurrentQueue<IPayload>();
            _receivedPayloads = new ConcurrentQueue<IPayload>();

            _connectionStatus = GatewayConnectionStatus.Offline;

            _tokenSource = new CancellationTokenSource();
            _sendTask = Task.FromResult(GatewaySenderResult.FromSuccess());
            _receiveTask = Task.FromResult(GatewayReceiverResult.FromSuccess());
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A gateway connection result which may or may not have succeeded.</returns>
        public async Task<GatewayConnectionResult> RunAsync(CancellationToken ct)
        {
            try
            {
                if (_connectionStatus != GatewayConnectionStatus.Offline)
                {
                    return GatewayConnectionResult.FromError("Already connected.");
                }

                // Until cancellation has been requested or we hit a fatal error, reconnections should be attempted.
                _tokenSource = new CancellationTokenSource();

                while (!ct.IsCancellationRequested)
                {
                    var iterationResult = await RunConnectionIterationAsync(ct);
                    if (iterationResult.IsSuccess)
                    {
                        continue;
                    }

                    // Something has gone wrong. Close the socket, and handle it
                    // Terminate the send and receive tasks
                    _tokenSource.Cancel();

                    // The results of the send and receive tasks are discarded here, because the iteration result will
                    // contain whichever of them failed if any of them did
                    _ = await _sendTask;
                    _ = await _receiveTask;

                    if (_transportService.IsConnected)
                    {
                        var disconnectResult = await _transportService.DisconnectAsync(ct.IsCancellationRequested, ct);
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

                    if (ct.IsCancellationRequested)
                    {
                        // The user requested a termination, and we don't intend to reconnect.
                        return iterationResult;
                    }

                    switch (iterationResult.GatewayCloseStatus)
                    {
                        case GatewayCloseStatus.SessionTimedOut:
                        case GatewayCloseStatus.RateLimited:
                        case GatewayCloseStatus.InvalidSequence:
                        case GatewayCloseStatus.UnknownError:
                        {
                            // Reconnection is allowed, using a completely new session
                            _sessionID = null;
                            _connectionStatus = GatewayConnectionStatus.Disconnected;

                            continue;
                        }
                    }

                    switch (iterationResult.WebSocketCloseStatus)
                    {
                        case WebSocketCloseStatus.InternalServerError:
                        case WebSocketCloseStatus.EndpointUnavailable:
                        {
                            // Reconnection is allowed, using a completely new session
                            _sessionID = null;
                            _connectionStatus = GatewayConnectionStatus.Disconnected;

                            continue;
                        }
                    }

                    switch (iterationResult.Exception)
                    {
                        case HttpRequestException _:
                        case WebSocketException _:
                        {
                            _log.LogWarning(iterationResult.Exception, "Transient error in gateway client.");

                            // Reconnection is allowed, since this is probably a transient error
                            _connectionStatus = GatewayConnectionStatus.Disconnected;
                            break;
                        }
                        default:
                        {
                            // Something has gone terribly wrong, and we won't keep trying to connect
                            return iterationResult;
                        }
                    }
                }

                var userRequestedDisconnect = await _transportService.DisconnectAsync(false, ct);
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
                return GatewayConnectionResult.FromError(e);
            }
            finally
            {
                // Reconnection is not allowed.
                _sessionID = null;
                _connectionStatus = GatewayConnectionStatus.Offline;
            }

            return GatewayConnectionResult.FromSuccess();
        }

        /// <summary>
        /// Runs a single iteration of the connection loop.
        /// </summary>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A connection result, based on the results of the iteration.</returns>
        private async Task<GatewayConnectionResult> RunConnectionIterationAsync(CancellationToken ct = default)
        {
            switch (_connectionStatus)
            {
                case GatewayConnectionStatus.Offline:
                case GatewayConnectionStatus.Disconnected:
                {
                    _log.LogInformation("Retrieving gateway endpoint...");

                    // Start connecting
                    var getGatewayEndpoint = await _gatewayAPI.GetGatewayBotAsync(ct);
                    if (!getGatewayEndpoint.IsSuccess)
                    {
                        return GatewayConnectionResult.FromError(getGatewayEndpoint);
                    }

                    var gatewayEndpoint = $"{getGatewayEndpoint.Entity.Url}?v=8&encoding=json";
                    if (!Uri.TryCreate(gatewayEndpoint, UriKind.Absolute, out var gatewayUri))
                    {
                        return GatewayConnectionResult.FromError
                        (
                            "Failed to parse the received gateway endpoint."
                        );
                    }

                    _log.LogInformation("Connecting to the gateway...");

                    var transportConnectResult = await _transportService.ConnectAsync(gatewayUri, ct);
                    if (!transportConnectResult.IsSuccess)
                    {
                        return transportConnectResult;
                    }

                    var receiveHello = await _transportService.ReceivePayloadAsync(ct);
                    if (!receiveHello.IsSuccess)
                    {
                        return GatewayConnectionResult.FromError(receiveHello);
                    }

                    if (!(receiveHello.Entity is IPayload<IHello> hello))
                    {
                        // Not receiving a hello is a non-recoverable error
                        return GatewayConnectionResult.FromError
                        (
                            "The first payload from the gateway was not a hello. Rude!"
                        );
                    }

                    // Set up the send task
                    var heartbeatInterval = hello.Data.HeartbeatInterval;

                    _sendTask = Task.Factory.StartNew
                    (
                        () => GatewaySenderAsync(heartbeatInterval, _tokenSource.Token),
                        TaskCreationOptions.LongRunning
                    ).Unwrap();

                    // Attempt to connect or resume
                    var connectResult = await AttemptConnectionAsync(ct);
                    if (!connectResult.IsSuccess)
                    {
                        return connectResult;
                    }

                    // Now, set up the receive task and start receiving events normally
                    _receiveTask = Task.Factory.StartNew
                    (
                        () => GatewayReceiverAsync(_tokenSource.Token),
                        TaskCreationOptions.LongRunning
                    ).Unwrap();

                    _log.LogInformation("Connected.");

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
                        UnwrapAndDispatchEvent(payload, _tokenSource.Token);
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
                            return GatewayConnectionResult.FromError(sendResult);
                        }
                    }

                    if (_receiveTask.IsCompleted)
                    {
                        var receiveResult = await _receiveTask;
                        if (!receiveResult.IsSuccess)
                        {
                            return GatewayConnectionResult.FromError(receiveResult);
                        }
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(10), ct);
                    break;
                }
            }

            if (!_shouldReconnect)
            {
                return GatewayConnectionResult.FromSuccess();
            }

            _log.LogInformation("Reconnection requested by the gateway; terminating session...");

            // Terminate the send and receive tasks
            _tokenSource.Cancel();

            // The results of the send and receive tasks are discarded here, because we know that it's going to be a
            // cancellation
            _ = await _sendTask;
            _ = await _receiveTask;

            var disconnectResult = await _transportService.DisconnectAsync(true, ct);
            if (!disconnectResult.IsSuccess)
            {
                return disconnectResult;
            }

            // Set up the state for the new connection
            _tokenSource = new CancellationTokenSource();
            _connectionStatus = GatewayConnectionStatus.Disconnected;

            return GatewayConnectionResult.FromSuccess();
        }

        /// <summary>
        /// Finalizes the given running responder, awaiting it and logging its results.
        /// </summary>
        /// <param name="runningResponderDispatch">The running responder dispatch.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task FinalizeResponderDispatchAsync(Task<EventResponseResult[]> runningResponderDispatch)
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

                    if (responderResult.Exception is null)
                    {
                        _log.LogWarning
                        (
                            $"Error in gateway event responder: {responderResult.ErrorReason}"
                        );
                    }
                    else
                    {
                        _log.LogWarning
                        (
                            $"Error in gateway event responder.\n{responderResult.Exception}"
                        );
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

                    _log.LogWarning($"Error in gateway event responder.\n{e}");
                }
            }
            catch (Exception e)
            {
                _log.LogWarning($"Error in gateway event responder.\n{e}");
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
                    $"The given payload of type {payloadType} was not compatible with the event dispatcher."
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
                    $"The given payload of type {payloadType} was not compatible with the event dispatcher."
                );

                return;
            }

            var boundDispatchMethod = dispatchMethod.MakeGenericMethod(payloadInterfaceType.GetGenericArguments());
            var dispatchTask = boundDispatchMethod.Invoke(this, new object?[] { payload, ct });
            if (dispatchTask is null)
            {
                throw new InvalidOperationException();
            }

            _runningResponderDispatches.Enqueue((Task<EventResponseResult[]>)dispatchTask);
        }

        /// <summary>
        /// Dispatches the given event to all relevant gateway event responders.
        /// </summary>
        /// <param name="gatewayEvent">The event to dispatch.</param>
        /// <param name="ct">The cancellation token to use.</param>
        /// <typeparam name="TGatewayEvent">The gateway event.</typeparam>
        private async Task<EventResponseResult[]> DispatchEventAsync<TGatewayEvent>
        (
            IPayload<TGatewayEvent> gatewayEvent,
            CancellationToken ct = default
        )
            where TGatewayEvent : IGatewayEvent
        {
            var responderTypes = _responderTypeRepository.GetResponderTypes<TGatewayEvent>();
            if (responderTypes.Count == 0)
            {
                return Array.Empty<EventResponseResult>();
            }

            return await Task.WhenAll
            (
                responderTypes.Select(async rt =>
                {
                    using var serviceScope = _services.CreateScope();
                    var responder = (IResponder<TGatewayEvent>)serviceScope.ServiceProvider.GetService(rt);

                    try
                    {
                        return await responder.RespondAsync(gatewayEvent.Data, ct);
                    }
                    catch (Exception e)
                    {
                        return EventResponseResult.FromError(e);
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
        }

        /// <summary>
        /// Attempts to identify or resume the gateway connection.
        /// </summary>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A connection result which may or may not have succeeded.</returns>
        private Task<GatewayConnectionResult> AttemptConnectionAsync(CancellationToken ct = default)
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
        private async Task<GatewayConnectionResult> CreateNewSessionAsync(CancellationToken ct = default)
        {
            _log.LogInformation("Creating a new session...");

            var shardInformation = _gatewayOptions.ShardIdentification is null
                ? default
                : new Optional<IShardIdentification>(_gatewayOptions.ShardIdentification);

            SubmitCommandAsync
            (
                new Identify
                (
                    _tokenStore.Token,
                    _gatewayOptions.ConnectionProperties,
                    Intents: _gatewayOptions.Intents,
                    Compress: false,
                    Shard: shardInformation
                )
            );

            while (true)
            {
                var receiveReady = await _transportService.ReceivePayloadAsync(ct);
                if (!receiveReady.IsSuccess)
                {
                    return GatewayConnectionResult.FromError(receiveReady);
                }

                if (receiveReady.Entity is IPayload<IHeartbeatAcknowledge>)
                {
                    continue;
                }

                if (!(receiveReady.Entity is IPayload<IReady> ready))
                {
                    return GatewayConnectionResult.FromError
                    (
                        "The payload after identification was not a Ready payload."
                    );
                }

                _sessionID = ready.Data.SessionID;
                break;
            }

            return GatewayConnectionResult.FromSuccess();
        }

        /// <summary>
        /// Resumes an existing session with the gateway, replaying missed events.
        /// </summary>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A connection result which may or may not have succeeded.</returns>
        private async Task<GatewayConnectionResult> ResumeExistingSessionAsync(CancellationToken ct = default)
        {
            if (_sessionID is null)
            {
                return GatewayConnectionResult.FromError("There's no previous session to resume.");
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
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return GatewayConnectionResult.FromError("Operation was cancelled.");
                }

                var receiveEvent = await _transportService.ReceivePayloadAsync(ct);
                if (!receiveEvent.IsSuccess)
                {
                    return GatewayConnectionResult.FromError(receiveEvent);
                }

                if (receiveEvent.Entity is IPayload<IHeartbeatAcknowledge>)
                {
                    continue;
                }

                if (receiveEvent.Entity is IPayload<IInvalidSession>)
                {
                    _log.LogInformation($"Resume rejected by the gateway.");

                    await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(1000, 5000)), ct);
                    return await CreateNewSessionAsync(ct);
                }

                if (receiveEvent.Entity is IPayload<IResumed>)
                {
                    break;
                }

                _receivedPayloads.Enqueue(receiveEvent.Entity);
            }

            return GatewayConnectionResult.FromSuccess();
        }

        /// <summary>
        /// This method acts as the main entrypoint for the gateway sender task. It processes payloads that are
        /// submitted by the application to the gateway, sending them to it.
        /// </summary>
        /// <param name="heartbeatInterval">The interval at which heartbeats should be sent.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A sender result which may or may not have been successful. A failed result indicates that something
        /// has gone wrong when sending a payload, and that the connection has been deemed nonviable. A nonviable
        /// connection should be either terminated, reestablished, or resumed as appropriate.</returns>
        private async Task<GatewaySenderResult> GatewaySenderAsync
        (
            TimeSpan heartbeatInterval,
            CancellationToken ct = default
        )
        {
            try
            {
                DateTime? lastHeartbeat = null;
                while (!ct.IsCancellationRequested)
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
                        if (lastHeartbeatAck.HasValue && lastHeartbeatAck < lastHeartbeat)
                        {
                            return GatewaySenderResult.FromError
                            (
                                "The server did not respond in time with a heartbeat acknowledgement.",
                                GatewayCloseStatus.SessionTimedOut
                            );
                        }

                        // 32-bit reads are atomic, so this is fine
                        var lastSequenceNumber = _lastSequenceNumber;

                        var heartbeatPayload = new Payload<IHeartbeat>
                        (
                            new Heartbeat
                            (
                                lastSequenceNumber == 0 ? (long?)null : lastSequenceNumber
                            )
                        );

                        var sendHeartbeat = await _transportService.SendPayloadAsync(heartbeatPayload, ct);

                        if (!sendHeartbeat.IsSuccess)
                        {
                            return GatewaySenderResult.FromError(sendHeartbeat);
                        }

                        lastHeartbeat = DateTime.UtcNow;
                    }

                    // Check if there are any user-submitted payloads to send
                    if (!_payloadsToSend.TryDequeue(out var payload))
                    {
                        // Let's sleep for a little while
                        var maxSleepTime = (lastHeartbeat.Value + heartbeatInterval - safetyMargin) - now;
                        var sleepTime = TimeSpan.FromMilliseconds(Math.Clamp(100, 0, maxSleepTime.TotalMilliseconds));

                        await Task.Delay(sleepTime, ct);
                        continue;
                    }

                    var sendResult = await _transportService.SendPayloadAsync(payload, ct);
                    if (sendResult.IsSuccess)
                    {
                        continue;
                    }

                    // Normal closures are okay
                    return sendResult.WebSocketCloseStatus == WebSocketCloseStatus.NormalClosure
                        ? GatewaySenderResult.FromSuccess()
                        : GatewaySenderResult.FromError(sendResult);
                }

                return GatewaySenderResult.FromSuccess();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is a success
                return GatewaySenderResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return GatewaySenderResult.FromError(ex);
            }
        }

        /// <summary>
        /// This method acts as the main entrypoint for the gateway receiver task. It processes payloads that are
        /// sent from the gateway to the application, submitting them to it.
        /// </summary>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A receiver result which may or may not have been successful. A failed result indicates that
        /// something has gone wrong when receiving a payload, and that the connection has been deemed nonviable. A
        /// nonviable connection should be either terminated, reestablished, or resumed as appropriate.</returns>
        private async Task<GatewayReceiverResult> GatewayReceiverAsync(CancellationToken ct = default)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var receivedPayload = await _transportService.ReceivePayloadAsync(ct);
                    if (!receivedPayload.IsSuccess)
                    {
                        // Normal closures are okay
                        return receivedPayload.WebSocketCloseStatus == WebSocketCloseStatus.NormalClosure
                            ? GatewayReceiverResult.FromSuccess()
                            : GatewayReceiverResult.FromError(receivedPayload);
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
                        case IPayload<IReconnect> _:
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
                        case IPayload<IHeartbeat> _:
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

                return GatewayReceiverResult.FromSuccess();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is a success
                return GatewayReceiverResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return GatewayReceiverResult.FromError(ex);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _tokenSource.Dispose();
        }
    }
}
