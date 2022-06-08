//
//  InteractivityResponder.cs
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
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Interactivity.Responders;

/// <summary>
/// Handles dispatch of interaction events to interested entities.
/// </summary>
internal sealed class InteractivityResponder : IResponder<IInteractionCreate>
{
    private readonly IMemoryCache _cache;
    private readonly ContextInjectionService _contextInjectionService;
    private readonly IDiscordRestInteractionAPI _interactionAPI;
    private readonly IServiceProvider _services;
    private readonly InteractivityResponderOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractivityResponder"/> class.
    /// </summary>
    /// <param name="services">The available services.</param>
    /// <param name="contextInjectionService">The context injection service.</param>
    /// <param name="cache">The memory cache.</param>
    /// <param name="interactionAPI">The interaction API.</param>
    /// <param name="options">The responder options.</param>
    public InteractivityResponder
    (
        IServiceProvider services,
        ContextInjectionService contextInjectionService,
        IMemoryCache cache,
        IDiscordRestInteractionAPI interactionAPI,
        IOptions<InteractivityResponderOptions> options
    )
    {
        _services = services;
        _contextInjectionService = contextInjectionService;
        _cache = cache;
        _interactionAPI = interactionAPI;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IInteractionCreate gatewayEvent, CancellationToken ct = default)
    {
        if (gatewayEvent.Type is not (InteractionType.MessageComponent or InteractionType.ModalSubmit))
        {
            return Result.FromSuccess();
        }

        var createContext = gatewayEvent.CreateContext();
        if (!createContext.IsSuccess)
        {
            return (Result)createContext;
        }

        var context = createContext.Entity;
        _contextInjectionService.Context = context;

        return context.Data.Components.HasValue
            ? await HandleModalInteractionAsync(context, ct)
            : await HandleComponentInteractionAsync(context, ct);
    }

    private async Task<Result> HandleModalInteractionAsync(InteractionContext context, CancellationToken ct)
    {
        if (!context.Data.Components.IsDefined(out var components))
        {
            return new InvalidOperationError("The interaction did not contain a custom ID.");
        }

        if (!context.Data.CustomID.IsDefined(out var customID))
        {
            return new InvalidOperationError("The interaction did not contain a custom ID.");
        }

        if (!_options.SuppressAutomaticResponses)
        {
            var response = new InteractionResponse(InteractionCallbackType.DeferredUpdateMessage);
            var createResponse = await _interactionAPI.CreateInteractionResponseAsync
            (
                context.ID,
                context.Token,
                response,
                ct: ct
            );

            if (!createResponse.IsSuccess)
            {
                return createResponse;
            }
        }

        var interactionResults = await RunEntityHandlersAsync<IModalInteractiveEntity>
        (
            null,
            customID,
            (entity, c) => entity.HandleInteractionAsync(context.User, customID, components, c),
            ct
        );

        if (!interactionResults.Any())
        {
            return new NotFoundError
            (
                "No interested interactive entities were found. Did you forget to add any entities interested " +
                $"in modal submissions (with the ID {customID}) to the service collection?"
            );
        }

        return interactionResults.All(r => r.IsSuccess)
            ? Result.FromSuccess()
            : new AggregateError(interactionResults.Where(r => !r.IsSuccess).Cast<IResult>().ToArray());
    }

    private async Task<Result> HandleComponentInteractionAsync(InteractionContext context, CancellationToken ct)
    {
        if (!context.Data.ComponentType.IsDefined(out var componentType))
        {
            return new InvalidOperationError("The interaction did not contain a component type.");
        }

        if (!context.Data.CustomID.IsDefined(out var customID))
        {
            return new InvalidOperationError("The interaction did not contain a custom ID.");
        }

        if (componentType is ComponentType.SelectMenu)
        {
            if (!context.Data.Values.HasValue)
            {
                return new InvalidOperationError("The interaction did not contain any selected values.");
            }
        }

        if (!_options.SuppressAutomaticResponses)
        {
            var response = new InteractionResponse(InteractionCallbackType.DeferredUpdateMessage);
            var createResponse = await _interactionAPI.CreateInteractionResponseAsync
            (
                context.ID,
                context.Token,
                response,
                ct: ct
            );

            if (!createResponse.IsSuccess)
            {
                return createResponse;
            }
        }

        IReadOnlyList<Result> interactionResults;
        switch (componentType)
        {
            case ComponentType.Button:
            {
                interactionResults = await RunEntityHandlersAsync<IButtonInteractiveEntity>
                (
                    componentType,
                    customID,
                    (entity, c) => entity.HandleInteractionAsync(context.User, customID, c),
                    ct
                );

                break;
            }
            case ComponentType.SelectMenu:
            {
                interactionResults = await RunEntityHandlersAsync<ISelectMenuInteractiveEntity>
                (
                    componentType,
                    customID,
                    (entity, c) => entity.HandleInteractionAsync(context.User, customID, context.Data.Values.Value, c),
                    ct
                );

                break;
            }
            case ComponentType.ActionRow:
            default:
            {
                return new InvalidOperationError("An unsupported component type was encountered.");
            }
        }

        if (!interactionResults.Any())
        {
            return new NotFoundError
            (
                "No interested interactive entities were found. Did you forget to add any entities interested " +
                $"in {componentType.Humanize().ToLowerInvariant()} components (with the ID {customID}) to the " +
                "service collection?"
            );
        }

        return interactionResults.All(r => r.IsSuccess)
            ? Result.FromSuccess()
            : new AggregateError(interactionResults.Where(r => !r.IsSuccess).Cast<IResult>().ToArray());
    }

    private async Task<IReadOnlyList<Result>> RunEntityHandlersAsync<TEntity>
    (
        ComponentType? componentType,
        string customID,
        Func<TEntity, CancellationToken, Task<Result>> handler,
        CancellationToken ct = default
    )
        where TEntity : class, IInteractiveEntity
    {
        var entities = _services.GetServices<TEntity>();

        // Null in this list signifies no interest
        var results = await Task.WhenAll(entities.Select<TEntity, Task<Result?>>(async entity =>
        {
            if (entity is InMemoryPersistentInteractiveEntity persistentEntity)
            {
                return await RunPersistentEntityHandlerAsync
                (
                    persistentEntity,
                    componentType,
                    customID,
                    handler,
                    ct
                );
            }

            // It's fine to run without synchronization
            try
            {
                var determineInterest = await entity.IsInterestedAsync(componentType, customID, ct);
                if (!determineInterest.IsSuccess)
                {
                    return (Result)determineInterest;
                }

                if (determineInterest.IsDefined(out var isInterested) && !isInterested)
                {
                    return null;
                }

                return await handler(entity, ct);
            }
            catch (Exception e)
            {
                return e;
            }
        }));

        return results.Where(r => r is not null).Select(r => r!.Value).ToList();
    }

    private async Task<Result?> RunPersistentEntityHandlerAsync<TEntity>
    (
        InMemoryPersistentInteractiveEntity entity,
        ComponentType? componentType,
        string customID,
        Func<TEntity, CancellationToken, Task<Result>> handler,
        CancellationToken ct
    )
        where TEntity : class, IInteractiveEntity
    {
        var dataKey = InMemoryPersistenceHelpers.CreateNonceKey(entity.Nonce);
        if (!_cache.TryGetValue<(SemaphoreSlim, object)>(dataKey, out var value))
        {
            return new NotFoundError
            (
                "No persistent data found for an interactive entity. Did you forget to initialize one when sending " +
                "the original message?"
            );
        }

        var (semaphore, data) = value;

        // Figure out the real data type the entity wants
        var entityDataType = entity.DataType;

        if (!entityDataType.IsInstanceOfType(data))
        {
            // This is a certified "oops" moment
            return new InvalidOperationError
            (
                "The cached persistent data was not compatible with the data type required by the entity that " +
                "requested it. Bug or API misuse?"
            );
        }

        entity.UntypedData = data;
        var determineInterest = await entity.IsInterestedAsync(componentType, customID, ct);
        if (!determineInterest.IsSuccess)
        {
            return (Result)determineInterest;
        }

        if (determineInterest.IsDefined(out var isInterested) && !isInterested)
        {
            return null;
        }

        Result interactionResult;
        try
        {
            // Only one entity instance may operate on an in-memory data object at a time
            await semaphore.WaitAsync(ct);
            interactionResult = await handler((entity as TEntity)!, ct);

            if (interactionResult.IsSuccess)
            {
                _cache.Set(dataKey, (semaphore, entity.UntypedData));
            }
        }
        catch (Exception e)
        {
            return e;
        }
        finally
        {
            if (entity.DeleteData)
            {
                _cache.Remove(dataKey);
                semaphore.Dispose();
            }
            else
            {
                semaphore.Release();
            }
        }

        return interactionResult;
    }
}
