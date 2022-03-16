//
//  ResponderDispatchService.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace Remora.Discord.Gateway.Services;

/// <summary>
/// Manages dispatch and processing of gateway payloads.
/// </summary>
public class ResponderDispatchService : IAsyncDisposable
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ResponderDispatchService> _log;
    private readonly IResponderTypeRepository _responderTypeRepository;

    private readonly Dictionary<Type, Type> _cachedInterfaceTypeArguments;
    private readonly Dictionary<Type, MethodInfo> _cachedDispatchMethods;

    private bool _isRunning;
    private CancellationTokenSource? _dispatchCancellationSource;
    private Task? _dispatcher;
    private Task? _finalizer;
    private Channel<IPayload>? _payloadsToDispatch;
    private Channel<Task<IReadOnlyList<Result>>>? _respondersToFinalize;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponderDispatchService"/> class.
    /// </summary>
    /// <param name="services">The available services.</param>
    /// <param name="responderTypeRepository">The responder type repository.</param>
    /// <param name="log">The logging instance for this type.</param>
    public ResponderDispatchService
    (
        IServiceProvider services,
        IResponderTypeRepository responderTypeRepository,
        ILogger<ResponderDispatchService> log
    )
    {
        _services = services;
        _responderTypeRepository = responderTypeRepository;
        _log = log;

        _cachedInterfaceTypeArguments = new();
        _cachedDispatchMethods = new();
    }

    /// <summary>
    /// Dispatches the given payload to interested responders. If the service is stopped with pending dispatches, the
    /// pending payloads will be dropped. Any payloads that have been dispatched (that is, a call to this method has
    /// returned a successful result) will be allowed to run to completion.
    /// </summary>
    /// <param name="payload">The payload to dispatch.</param>
    /// <param name="ct">
    /// The cancellation token for this operation. Note that this is *not* the cancellation token which will be passed
    /// to any instantiated responders.
    /// </param>
    /// <returns>A result which may or may not have succeeded.</returns>
    public async Task<Result> DispatchAsync(IPayload payload, CancellationToken ct = default)
    {
        if (!_isRunning)
        {
            return new NotRunningError();
        }

        if (_dispatchCancellationSource is null)
        {
            throw new InvalidOperationException();
        }

        if (_payloadsToDispatch is null)
        {
            throw new InvalidOperationException();
        }

        try
        {
            await _payloadsToDispatch.Writer.WriteAsync(payload, ct);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Starts the dispatch service, beginning acceptance of payloads for dispatch.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    public Result Start()
    {
        if (_isRunning)
        {
            return new AlreadyStartedError();
        }

        _dispatchCancellationSource = new();
        _payloadsToDispatch = Channel.CreateBounded<IPayload>
        (
            new BoundedChannelOptions(100) { FullMode = BoundedChannelFullMode.Wait }
        );

        _respondersToFinalize = Channel.CreateUnbounded<Task<IReadOnlyList<Result>>>();

        _dispatcher = Task.Run(DispatcherTaskAsync, _dispatchCancellationSource.Token);
        _finalizer = Task.Run(FinalizerTaskAsync, _dispatchCancellationSource.Token);

        _isRunning = true;
        return Result.FromSuccess();
    }

    /// <summary>
    /// Stops the dispatch service, finishing any pending payloads.
    /// </summary>
    /// <returns>A result which may or may not have succeeded.</returns>
    public async Task<Result> StopAsync()
    {
        if (!_isRunning)
        {
            return new NotRunningError();
        }

        if (_dispatcher is null)
        {
            throw new InvalidOperationException();
        }

        if (_finalizer is null)
        {
            throw new InvalidOperationException();
        }

        if (_dispatchCancellationSource is null)
        {
            throw new InvalidOperationException();
        }

        if (_payloadsToDispatch is null)
        {
            throw new InvalidOperationException();
        }

        if (_respondersToFinalize is null)
        {
            throw new InvalidOperationException();
        }

        // Stop!
        _dispatchCancellationSource.Cancel();
        _payloadsToDispatch.Writer.Complete();

        // Wait for everything to actually stop...
        await _dispatcher;
        await _finalizer;

        // Reset state so we can start again
        _dispatchCancellationSource = null;

        _dispatcher = null;
        _finalizer = null;

        _payloadsToDispatch = null;
        _respondersToFinalize = null;

        _isRunning = false;
        return Result.FromSuccess();
    }

    /// <summary>
    /// Runs the main loop of the dispatcher task.
    /// </summary>
    private async Task DispatcherTaskAsync()
    {
        if (_dispatchCancellationSource is null)
        {
            throw new InvalidOperationException();
        }

        if (_payloadsToDispatch is null)
        {
            throw new InvalidOperationException();
        }

        if (_respondersToFinalize is null)
        {
            throw new InvalidOperationException();
        }

        while (!_dispatchCancellationSource.Token.IsCancellationRequested)
        {
            var payload = await _payloadsToDispatch.Reader.ReadAsync(_dispatchCancellationSource.Token);
            var dispatch = UnwrapAndDispatchEvent(payload, _dispatchCancellationSource.Token);
            if (!dispatch.IsSuccess)
            {
                _log.LogWarning("Failed to dispatch payload: {Reason}", dispatch.Error.Message);
                continue;
            }

            await _respondersToFinalize.Writer.WriteAsync(dispatch.Entity, _dispatchCancellationSource.Token);
        }

        // Finish up remaining dispatches
        await _payloadsToDispatch.Reader.Completion;
        await foreach (var payload in _payloadsToDispatch.Reader.ReadAllAsync())
        {
            var dispatch = UnwrapAndDispatchEvent(payload, _dispatchCancellationSource.Token);
            if (!dispatch.IsSuccess)
            {
                _log.LogWarning("Failed to dispatch payload: {Reason}", dispatch.Error.Message);
                continue;
            }

            await _respondersToFinalize.Writer.WriteAsync(dispatch.Entity, _dispatchCancellationSource.Token);
        }

        _respondersToFinalize.Writer.Complete();
    }

    /// <summary>
    /// Runs the main loop of the finalizer task.
    /// </summary>
    private async Task FinalizerTaskAsync()
    {
        if (_dispatchCancellationSource is null)
        {
            throw new InvalidOperationException();
        }

        if (_respondersToFinalize is null)
        {
            throw new InvalidOperationException();
        }

        while (!_dispatchCancellationSource.Token.IsCancellationRequested)
        {
            var responderResults = await await _respondersToFinalize.Reader.ReadAsync
            (
                _dispatchCancellationSource.Token
            );

            FinalizeResponderDispatch(responderResults);
        }

        await _respondersToFinalize.Reader.Completion;
        await foreach (var responderResults in _respondersToFinalize.Reader.ReadAllAsync())
        {
            FinalizeResponderDispatch(await responderResults);
        }
    }

    /// <summary>
    /// Unwraps the given payload into its typed representation, dispatching all events for it.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="ct">The cancellation token for the dispatched event.</param>
    private Result<Task<IReadOnlyList<Result>>> UnwrapAndDispatchEvent(IPayload payload, CancellationToken ct = default)
    {
        if (!_cachedInterfaceTypeArguments.TryGetValue(payload.GetType(), out var interfaceArgument))
        {
            var payloadType = payload.GetType();
            if (!payloadType.IsGenericType)
            {
                _log.LogWarning
                (
                    "The given payload of type {PayloadType} was not compatible with the event dispatcher",
                    payloadType
                );

                return new InvalidOperationError();
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

                return new InvalidOperationError();
            }

            interfaceArgument = payloadInterfaceType.GetGenericArguments()[0];
            _cachedInterfaceTypeArguments.Add(payload.GetType(), interfaceArgument);
        }

        if (!_cachedDispatchMethods.TryGetValue(interfaceArgument, out var boundDispatchMethod))
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

            boundDispatchMethod = dispatchMethod.MakeGenericMethod(interfaceArgument);
            _cachedDispatchMethods.Add(interfaceArgument, boundDispatchMethod);
        }

        var responderTask = Task.Run
        (
            () =>
            {
                var task = boundDispatchMethod.Invoke(this, new object?[] { payload, ct })
                           ?? throw new InvalidOperationException();

                return (Task<IReadOnlyList<Result>>)task;
            },
            ct
        );

        return responderTask;
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
    /// Finalizes the given dispatch results, logging any potential problems.
    /// </summary>
    /// <param name="responderResults">The results from the responder.</param>
    private void FinalizeResponderDispatch(IEnumerable<Result> responderResults)
    {
        try
        {
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
                            responderResult.Error!.Message
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

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_isRunning)
        {
            return;
        }

        var stopDispatch = await StopAsync();
        if (!stopDispatch.IsSuccess)
        {
            _log.LogError("Failed to stop the dispatch service in DisposeAsync. Panic!");
        }
    }
}
