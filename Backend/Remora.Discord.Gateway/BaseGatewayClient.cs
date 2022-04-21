//
//  BaseGatewayClient.cs
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
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Gateway.Services;
using Remora.Discord.Gateway.Transport;
using Remora.Results;

namespace Remora.Discord.Gateway;

/// <summary>
/// Provides an abstract implementation of an <see cref="IGatewayClient"/>
/// that must be extended with relevant connection and resumption flows.
/// </summary>
[PublicAPI]
public abstract class BaseGatewayClient : IGatewayClient, IAsyncDisposable
{
    private readonly ILogger<BaseGatewayClient> _logger;
    private readonly Channel<IPayload> _payloadSendQueue;
    private readonly ConcurrentQueue<IPayload> _priorityPayloadSendQueue;
    private readonly Dictionary<Guid, IGatewayCommand> _preShutdownCommands;

    private Task<Result>? _sendTask;
    private Task<Result>? _receiveTask;
    private Task<Result>? _responderDispatchTask;
    private volatile bool _hasStarted;
    private volatile bool _isStopping;
    private CancellationTokenSource _dispatchCts;

    /// <summary>
    /// Gets the dispatch service.
    /// </summary>
    protected IResponderDispatchService DispatchService { get; }

    /// <summary>
    /// Gets the payload transport service.
    /// </summary>
    protected IPayloadTransportService TransportService { get; }

    /// <summary>
    /// Gets the cancellation token source for internal operations.
    /// </summary>
    protected CancellationTokenSource DisconnectRequestedSource { get; private set; }

    /// <inheritdoc />
    public TimeSpan Latency { get; protected set; }

    /// <inheritdoc />
    public GatewayConnectionStatus Status { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseGatewayClient"/> class.
    /// </summary>
    /// <param name="logger">The logging provider.</param>
    /// <param name="transportService">The payload transport service.</param>
    /// <param name="dispatchService">The gateway event dispatch service.</param>
    protected BaseGatewayClient
    (
        ILogger<BaseGatewayClient> logger,
        IPayloadTransportService transportService,
        IResponderDispatchService dispatchService
    )
    {
        _logger = logger;
        _payloadSendQueue = Channel.CreateUnbounded<IPayload>();
        _priorityPayloadSendQueue = new ConcurrentQueue<IPayload>();
        _preShutdownCommands = new Dictionary<Guid, IGatewayCommand>();
        this.DispatchService = dispatchService;
        this.TransportService = transportService;

        _dispatchCts = new CancellationTokenSource();
        this.DisconnectRequestedSource = new CancellationTokenSource();
    }

    /// <inheritdoc />
    public virtual async Task<Result> RunAsync(CancellationToken ct = default)
    {
        if (_hasStarted || _isStopping)
        {
            return new InvalidOperationError("Already running.");
        }

        try
        {
            _dispatchCts.Dispose();
            _dispatchCts = new CancellationTokenSource();

            _responderDispatchTask?.Dispose();
            _responderDispatchTask = this.DispatchService.RunAsync(_dispatchCts.Token);

            while (!ct.IsCancellationRequested)
            {
                switch (this.Status)
                {
                    case GatewayConnectionStatus.Offline:
                    case GatewayConnectionStatus.Disconnected:
                    {
                        // We may have been switched to a disconnected state
                        // in response to a websocket message, so cleanup.
                        // Any errors as a result of this stop operation are either:
                        // 1. Gateway errors from the last run cycle, which will
                        // have already been acted on OR will result in immediate errors
                        // during the connection process.
                        // 2. Library errors, which shouldn't exist :P.
                        // Regardless, we don't need to return if stopping fails.
                        await StopAsync(true);

                        _hasStarted = true;
                        this.DisconnectRequestedSource.Dispose();
                        this.DisconnectRequestedSource = new CancellationTokenSource();

                        var connectResult = await ConnectAsync(ct);
                        if (!connectResult.IsSuccess)
                        {
                            _logger.LogWarning
                            (
                                "Failed to connect to the gateway. Connection may be reattempted\n{Error}",
                                connectResult.Error
                            );

                            if (!await StopAndCheckResultForReconnectionAsync(connectResult))
                            {
                                return connectResult;
                            }

                            continue;
                        }

                        if (_sendTask is null)
                        {
                            await StopAsync(false);
                            throw new InvalidOperationException
                            (
                                "Implementing type must call " + nameof(ConnectToGatewayAndBeginSendTaskAsync) + " when connecting"
                            );
                        }

                        _receiveTask = GatewayReceiverAsync(this.DisconnectRequestedSource.Token);

                        break;
                    }
                    case GatewayConnectionStatus.Connected:
                    {
                        // Restart the dispatcher service if it has stopped
                        if (_responderDispatchTask.IsCompleted)
                        {
                            var stopResult = await _responderDispatchTask;
                            _logger.LogError("The responder dispatch service stopped with the error {Error}. Restarting it...", stopResult.Error);

                            _responderDispatchTask.Dispose();
                            _responderDispatchTask = this.DispatchService.RunAsync(_dispatchCts.Token);
                        }

                        if (_sendTask!.IsCompleted)
                        {
                            var stopResult = await _sendTask;
                            if (!await StopAndCheckResultForReconnectionAsync(stopResult))
                            {
                                return stopResult;
                            }

                            break;
                        }

                        if (_receiveTask!.IsCompleted)
                        {
                            var stopResult = await _receiveTask;
                            if (!await StopAndCheckResultForReconnectionAsync(stopResult))
                            {
                                return stopResult;
                            }

                            break;
                        }

                        // Take a breath
                        try
                        {
                            await Task.Delay(10, ct);
                        }
                        catch (TaskCanceledException)
                        {
                            // We're wrapping up;
                        }

                        break;
                    }
                }
            }

            // Queue up the pre-shutdown commands
            foreach (var command in _preShutdownCommands.Values)
            {
                await EnqueueCommandAsync(command, CancellationToken.None);
            }

            // Wait for all the pre-shutdown commands to be sent
            while (_payloadSendQueue.Reader.TryPeek(out _))
            {
                await Task.Delay(10, CancellationToken.None);
            }

            // Ensure the last command on the queue is really sent
            await Task.Delay(20, CancellationToken.None);

            // Properly shutdown now
            return await StopAsync(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public virtual async ValueTask EnqueueCommandAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : IGatewayCommand
    {
        var payload = new Payload<TCommand>(command);
        await _payloadSendQueue.Writer.WriteAsync(payload, ct);
    }

    /// <inheritdoc />
    public Guid RegisterPreShutdownCommand<TCommand>(TCommand command)
        where TCommand : IGatewayCommand
    {
        var id = Guid.NewGuid();
        _preShutdownCommands.Add(id, command);

        return id;
    }

    /// <inheritdoc />
    public void DeregisterPreShutdownCommand(Guid id)
    {
        _preShutdownCommands.Remove(id);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Connects (or reconnects, depending on the current <see cref="Status"/>)
    /// to the gateway.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A result representing the outcome of the operation.</returns>
    protected abstract Task<Result> ConnectAsync(CancellationToken ct);

    /// <summary>
    /// Gets a value indicating if the gateway connection should be re-established.
    /// </summary>
    /// <param name="iterationResult">The result of the last connection iteration.</param>
    /// <param name="withNewSession">Defines whether a resume or reconnect should be attempted.</param>
    /// <returns>A value indicating whether or not the connection should be re-established.</returns>
    protected abstract bool ShouldReconnect(Result iterationResult, out bool withNewSession);

    /// <summary>
    /// Provides an internal means to handle payloads as they are received.
    /// Useful, for example, for updating the heartbeat state.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>A value indicating whether or not the payload should be dispatched.</returns>
    protected abstract bool ProcessPayload(IPayload payload);

    /// <summary>
    /// Sends a heartbeat if required. Any heartbeats should be
    /// enqueued using <see cref="EnqueuePriorityCommand{TCommand}"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> representing the outcome of the operation, and containing the maximum
    /// safe amount of time needed until the next heartbeat if the operation was successful.
    /// </returns>
    protected abstract Result<TimeSpan> SendHeartbeat();

    /// <summary>
    /// Enqueues a command to the priority queue. These will be dispatched
    /// before all other commands, and block the send task until all have
    /// been dispatched, so use this only for commands that must be dispatched
    /// immediately.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    protected void EnqueuePriorityCommand<TCommand>(TCommand command)
        where TCommand : IGatewayCommand
    {
        var payload = new Payload<TCommand>(command);
        _priorityPayloadSendQueue.Enqueue(payload);
    }

    /// <summary>
    /// Connects the websocket and begins the send task.
    /// </summary>
    /// <param name="gatewayUri">The URI of the voice gateway.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A <see cref="Result"/> indicating the outcome of the operation.</returns>
    protected async Task<Result> ConnectToGatewayAndBeginSendTaskAsync
    (
        Uri gatewayUri,
        CancellationToken ct
    )
    {
        var connectResult = await this.TransportService.ConnectAsync(gatewayUri, ct).ConfigureAwait(false);
        if (!connectResult.IsSuccess)
        {
            return connectResult;
        }

        _sendTask = GatewaySenderAsync(this.DisconnectRequestedSource.Token);

        return Result.FromSuccess();
    }

    /// <summary>
    /// Stops the client by cancelling the <see cref="DisconnectRequestedSource"/>,
    /// disconnecting the transport service and waiting on all running tasks to complete.
    /// </summary>
    /// <param name="reconnectionIntended">A value indicating whether or not the connection will be re-attempted after this stoppage.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task<Result> StopAsync(bool reconnectionIntended)
    {
        if (_isStopping || !_hasStarted)
        {
            _logger.LogWarning("The gateway service is already stopping/stopped! _isStopping: {IsStopping} | _isRunning: {IsRunning}", _isStopping, _hasStarted);
            return Result.FromSuccess();
        }

        _isStopping = true;
        var failedResults = new List<IResult>();

        this.DisconnectRequestedSource.Cancel();

        if (this.TransportService.IsConnected)
        {
            var disconnectResult = await this.TransportService.DisconnectAsync(reconnectionIntended);
            if (!disconnectResult.IsSuccess && !disconnectResult.HasCancellationError())
            {
                _logger.LogError("Failed to disconnect the transport service: {Error}", disconnectResult.Error);
                failedResults.Add(disconnectResult);
            }
        }

        if (_sendTask is not null)
        {
            var sendResult = await _sendTask;
            if (!sendResult.IsSuccess && !sendResult.HasCancellationError())
            {
                _logger.LogError("An error occured in the send task: {Error}", sendResult.Error);
                failedResults.Add(sendResult);
            }
        }

        if (_receiveTask is not null)
        {
            var receiveResult = await _receiveTask;
            if (!receiveResult.IsSuccess && !receiveResult.HasCancellationError())
            {
                _logger.LogError("An error occured in the receive task: {Error}", receiveResult.Error);
                failedResults.Add(receiveResult);
            }
        }

        if (!reconnectionIntended && _responderDispatchTask is not null)
        {
            _dispatchCts.Cancel();
            var dispatchResult = await _responderDispatchTask;

            if (!dispatchResult.IsSuccess && !dispatchResult.HasCancellationError())
            {
                _logger.LogError("An error occured in the responder dispatch task: {Error}", dispatchResult.Error);
                failedResults.Add(dispatchResult);
            }
        }

        this.Status = reconnectionIntended
            ? GatewayConnectionStatus.Disconnected
            : GatewayConnectionStatus.Offline;

        _isStopping = false;
        _hasStarted = false;

        return failedResults.Count > 0
            ? new AggregateError(failedResults)
            : Result.FromSuccess();
    }

    /// <summary>
    /// Stops the client and disposes of managed resources..
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        await StopAsync(false);

        _sendTask?.Dispose();
        _receiveTask?.Dispose();
        _responderDispatchTask?.Dispose();
        _dispatchCts.Dispose();
        this.DisconnectRequestedSource.Dispose();
    }

    /// <summary>
    /// This method acts as the main entrypoint for the gateway receiver task. It receives and processes payloads from the gateway.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>
    /// A receiver result which may or may not have been successful. A failed result indicates that
    /// something has gone wrong when receiving a payload, and that the connection has been deemed nonviable. A
    /// nonviable connection should be either terminated, reestablished, or resumed as appropriate.
    /// </returns>
    private async Task<Result> GatewayReceiverAsync(CancellationToken ct)
    {
        try
        {
            await Task.Yield();

            while (!ct.IsCancellationRequested)
            {
                var receiveResult = await this.TransportService.ReceivePayloadAsync(ct).ConfigureAwait(false);

                if (!receiveResult.IsSuccess)
                {
                    // While this indicates a potential error in our understanding
                    // of the gateway, it isn't something we can reliable fail on,
                    // as Discord gateways can send undocumented and/or internal-use
                    // only payloads.
                    if (receiveResult.Error is UnrecognisedPayloadError upe)
                    {
                        _logger.LogWarning
                        (
                            "Received an unknown payload: {RawJson}",
                            upe.RawPayloadJson
                        );

                        continue;
                    }

                    // Normal closures are okay
                    return receiveResult.Error is GatewayWebSocketError { CloseStatus: WebSocketCloseStatus.NormalClosure }
                        ? Result.FromSuccess()
                        : Result.FromError(receiveResult);
                }

                if (!ProcessPayload(receiveResult.Entity))
                {
                    continue;
                }

                if (receiveResult.Entity is IPayload<IGatewayEvent> gatewayEventPayload)
                {
                    await this.DispatchService.EnqueueEventAsync(gatewayEventPayload.Data, ct);
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
    /// This method acts as the main entrypoint for the gateway sender task.
    /// It processes and sends submitted payloads to the gateway, and calls <see cref="SendHeartbeat"/>.
    /// </summary>
    /// <param name="ct">A token for requests to disconnect the socket.</param>
    /// <returns>
    /// A result which may or may not have been successful. A failed result indicates that something
    /// has gone wrong when sending a payload, and that the connection has been deemed nonviable. A nonviable
    /// connection should be either terminated, reestablished, or resumed as appropriate.
    /// </returns>
    private async Task<Result> GatewaySenderAsync(CancellationToken ct)
    {
        try
        {
            await Task.Yield();

            while (!ct.IsCancellationRequested)
            {
                var heartbeatResult = SendHeartbeat();
                if (!heartbeatResult.IsDefined(out var maxSleepTime))
                {
                    return Result.FromError(heartbeatResult);
                }

                // Dispatch any priority commands
                while (_priorityPayloadSendQueue.TryDequeue(out var priorityPayload))
                {
                    var prioritySendResult = await this.TransportService.SendPayloadAsync(priorityPayload, ct);
                    if (!prioritySendResult.IsSuccess)
                    {
                        return prioritySendResult;
                    }
                }

                // Check if there are any user-submitted payloads to send
                if (!_payloadSendQueue.Reader.TryRead(out var payload))
                {
                    // Let's sleep for a little while
                    var sleepTime = TimeSpan.FromMilliseconds(Math.Clamp(100, 0, maxSleepTime.TotalMilliseconds));
                    await Task.Delay(sleepTime, ct);
                    continue;
                }

                var sendResult = await this.TransportService.SendPayloadAsync(payload, ct);
                if (!sendResult.IsSuccess)
                {
                    return sendResult;
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
    /// Checks if a result should allow the client to reconnect, then stops the client.
    /// </summary>
    /// <param name="operationResult">The result.</param>
    /// <returns>A value indicating whether or not to attempt a reconnection.</returns>
    private async Task<bool> StopAndCheckResultForReconnectionAsync(Result operationResult)
    {
        if (operationResult.HasCancellationError())
        {
            // A shutdown has been requested
            return false;
        }

        var shouldReconnect = ShouldReconnect(operationResult, out var withNewSession);
        await StopAsync(shouldReconnect);

        if (!shouldReconnect)
        {
            return false;
        }

        if (withNewSession)
        {
            _logger.LogDebug("Errored result resulted in restarting with a new session");
            this.Status = GatewayConnectionStatus.Offline;
        }

        return true;
    }
}
