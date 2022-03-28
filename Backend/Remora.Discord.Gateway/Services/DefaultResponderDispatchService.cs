//
//  DefaultResponderDispatchService.cs
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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Gateway.Services;

/// <summary>
/// Implements the <see cref="IResponderDispatchService"/> interface
/// to dispatch payloads to <see cref="IResponder{TGatewayEvent}"/>
/// types registered in the <see cref="IResponderTypeRepository"/>.
/// </summary>
[PublicAPI]
public class DefaultResponderDispatchService : IResponderDispatchService, IAsyncDisposable
{
    private readonly ILogger<DefaultResponderDispatchService> _logger;
    private readonly IResponderTypeRepository _responderTypeRepository;
    private readonly IServiceProvider _services;

    private readonly Channel<IGatewayEvent> _eventDispatchQueue;
    private readonly Channel<Task<IReadOnlyList<Result>>> _responderFinalizationQueue;
    private readonly Type _dispatcherType;
    private readonly Dictionary<Type, Type> _eventTypeInterfaces;
    private readonly Dictionary<Type, MethodInfo> _dispatchMethods;

    private CancellationTokenSource _dispatchCts;
    private Task<Result>? _finalizerTask;
    private bool _isDisposed;

    /// <inheritdoc />
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultResponderDispatchService"/> class.
    /// </summary>
    /// <param name="logger">The logging provider.</param>
    /// <param name="responderTypeRepository">The payload handler type repository.</param>
    /// <param name="services">The service provider.</param>
    public DefaultResponderDispatchService
    (
        ILogger<DefaultResponderDispatchService> logger,
        IResponderTypeRepository responderTypeRepository,
        IServiceProvider services
    )
    {
        _logger = logger;
        _responderTypeRepository = responderTypeRepository;
        _services = services;

        _eventDispatchQueue = Channel.CreateUnbounded<IGatewayEvent>();
        _responderFinalizationQueue = Channel.CreateUnbounded<Task<IReadOnlyList<Result>>>();
        _dispatcherType = GetType();
        _eventTypeInterfaces = new Dictionary<Type, Type>();
        _dispatchMethods = new Dictionary<Type, MethodInfo>();

        _dispatchCts = new CancellationTokenSource();
    }

    /// <inheritdoc />
    public async ValueTask EnqueueEventAsync<TGatewayEvent>(TGatewayEvent gatewayEvent, CancellationToken ct = default)
        where TGatewayEvent : IGatewayEvent
    {
        await _eventDispatchQueue.Writer.WriteAsync(gatewayEvent, ct);
    }

    /// <inheritdoc />
    public async Task<Result> RunAsync(CancellationToken ct = default)
    {
        if (this.IsRunning)
        {
            return new InvalidOperationError("This dispatch service is already running");
        }

        try
        {
            await Task.Yield();

            // Reset our state
            _dispatchCts.Dispose();
            _dispatchCts = new CancellationTokenSource();

            _finalizerTask?.Dispose();
            _finalizerTask = FinalizerTaskAsync();

            this.IsRunning = true;

            try
            {
                await foreach (var gatewayEvent in _eventDispatchQueue.Reader.ReadAllAsync(ct))
                {
                    var dispatchResult = DispatchEvent(gatewayEvent);
                    if (!dispatchResult.IsSuccess)
                    {
                        return Result.FromError(dispatchResult);
                    }

                    await _responderFinalizationQueue.Writer.WriteAsync(dispatchResult.Entity, ct);

                    if (_finalizerTask.IsCompleted)
                    {
                        return await _finalizerTask;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                 // This is fine, we must be stopping
            }

            return await StopAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of managed resources.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_isDisposed)
        {
            return;
        }

        var stopResult = await StopAsync();
        if (!stopResult.IsSuccess && stopResult.Error is not InvalidOperationError)
        {
            _logger.LogError("Failed to stop the dispatch service when disposing:\n{Error}", stopResult.Error);
        }

        _dispatchCts.Dispose();
        _finalizerTask?.Dispose();

        _isDisposed = true;
    }

    /// <summary>
    /// Cancels any running dispatchers and stops the finalizer task.
    /// </summary>
    /// <returns>The result of the finalizer task.</returns>
    protected virtual async Task<Result> StopAsync()
    {
        if (!this.IsRunning)
        {
            return new InvalidOperationError("Already stopped");
        }

        if (_finalizerTask is null)
        {
            return new InvalidOperationError("Finalizer task is null");
        }

        _dispatchCts.Cancel();
        this.IsRunning = false;

        return await _finalizerTask;
    }

    /// <summary>
    /// Dispatches an event by invoking a typed instance of <see cref="DispatchEventAsync{TGatewayEvent}"/>.
    /// </summary>
    /// <param name="gatewayEvent">The event to dispatch.</param>
    /// <returns>
    /// A result representing the outcome of the dispatch operation. If successful, the result entity contains
    /// a task representing the asynchronous dispatch operation that resolves the execution results of all responders upon completion.
    /// </returns>
    private Result<Task<IReadOnlyList<Result>>> DispatchEvent(IGatewayEvent gatewayEvent)
    {
        var eventType = gatewayEvent.GetType();
        if (!_eventTypeInterfaces.TryGetValue(eventType, out var eventInterfaceType))
        {
            var eventInterfaceTypes = eventType.FindInterfaces
            (
                (m, _) => m.GetInterface(nameof(IGatewayEvent)) is not null,
                null
            );

            if (eventInterfaceTypes.Length != 1)
            {
                return new NotFoundError
                (
                    $"Failed to find an interface deriving from {nameof(IGatewayEvent)} on the {eventType.FullName} type."
                );
            }

            eventInterfaceType = eventInterfaceTypes[0];
            _eventTypeInterfaces[eventType] = eventInterfaceType;
        }

        var dispatchMethod = GetDispatchMethod(eventInterfaceType);
        var responderDispatchTask = dispatchMethod.Invoke(this, new object?[] { gatewayEvent, _dispatchCts.Token });

        return responderDispatchTask is null
            ? new InvalidOperationError("Failed to execute responder dispatch method")
            : (Task<IReadOnlyList<Result>>)responderDispatchTask;
    }

    /// <summary>
    /// Gets a <see cref="MethodInfo"/> representation of the <see cref="DispatchEventAsync{T}"/> method.
    /// </summary>
    /// <param name="abstractType">The abstract type used by responders.</param>
    /// <returns>The method info.</returns>
    private MethodInfo GetDispatchMethod(Type abstractType)
    {
        if (_dispatchMethods.ContainsKey(abstractType))
        {
            return _dispatchMethods[abstractType];
        }

        var dispatchMethod = _dispatcherType.GetMethod
        (
            nameof(DispatchEventAsync),
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (dispatchMethod is null)
        {
            throw new MissingMethodException(nameof(DefaultResponderDispatchService), nameof(DispatchEventAsync));
        }

        dispatchMethod = dispatchMethod.MakeGenericMethod(abstractType);
        _dispatchMethods[abstractType] = dispatchMethod;

        return dispatchMethod;
    }

    /// <summary>
    /// Dispatches an event to all interested responders, collecting the execution results.
    /// </summary>
    /// <typeparam name="TGatewayEvent">The type of the gateway event.</typeparam>
    /// <param name="gatewayEvent">The event to dispatch.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation that resolves the execution results of all responders upon completion.
    /// </returns>
    private async Task<IReadOnlyList<Result>> DispatchEventAsync<TGatewayEvent>
    (
        TGatewayEvent gatewayEvent,
        CancellationToken ct
    ) where TGatewayEvent : IGatewayEvent
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
                        await using var serviceScope = _services.CreateAsyncScope();
                        var responder = (IResponder<TGatewayEvent>)serviceScope.ServiceProvider
                            .GetRequiredService(rt);

                        try
                        {
                            return await responder.RespondAsync(gatewayEvent, ct);
                        }
                        catch (Exception e)
                        {
                            return e;
                        }
                    }
                )
            ).ConfigureAwait(false);

            results.AddRange(groupResults);
        }

        return results;
    }

    private async Task<Result> FinalizerTaskAsync()
    {
        try
        {
            await foreach (var responderTask in _responderFinalizationQueue.Reader.ReadAllAsync(_dispatchCts.Token))
            {
                if (!responderTask.IsCompleted)
                {
                    await _responderFinalizationQueue.Writer.WriteAsync(responderTask, _dispatchCts.Token);
                    continue;
                }

                await FinalizeResponderTask(responderTask);
            }
        }
        catch (OperationCanceledException)
        {
            // This is fine, we must be stopping
        }

        // Finalize any remaining responders on the queue
        while (_responderFinalizationQueue.Reader.TryRead(out var responderTask))
        {
            await FinalizeResponderTask(responderTask);
        }

        return Result.FromSuccess();
    }

    private async ValueTask FinalizeResponderTask(Task<IReadOnlyList<Result>> responderTask)
    {
        try
        {
            var results = await responderTask;

            foreach (var res in results)
            {
                if (res.IsSuccess)
                {
                    continue;
                }

                if (res.Error is ExceptionError ex)
                {
                    _logger.LogError(ex.Exception, "Error in responder");
                    continue;
                }

                _logger.LogError("Error in responder:\n{Error}", res.Error);
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
        {
            // This is fine
        }
        catch (AggregateException aex)
        {
            foreach (var ex in aex.InnerExceptions)
            {
                _logger.LogError(ex, "An error occured while running a responder");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while running a responder");
        }
    }
}
