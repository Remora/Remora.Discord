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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Interactivity.Responders;

/// <summary>
/// Acts as a base class for interactivity responders.
/// </summary>
public class InteractivityResponder : IResponder<IInteractionCreate>
{
    private readonly IMemoryCache _cache;
    private readonly ContextInjectionService _contextInjectionService;
    private readonly IDiscordRestInteractionAPI _interactionAPI;
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractivityResponder"/> class.
    /// </summary>
    /// <param name="services">The available services.</param>
    /// <param name="contextInjectionService">The context injection service.</param>
    /// <param name="cache">The memory cache.</param>
    /// <param name="interactionAPI">The interaction API.</param>
    protected InteractivityResponder
    (
        IServiceProvider services,
        ContextInjectionService contextInjectionService,
        IMemoryCache cache,
        IDiscordRestInteractionAPI interactionAPI
    )
    {
        _services = services;
        _contextInjectionService = contextInjectionService;
        _cache = cache;
        _interactionAPI = interactionAPI;
    }

    /// <inheritdoc />
    public async Task<Result> RespondAsync(IInteractionCreate gatewayEvent, CancellationToken ct = default)
    {
        if (gatewayEvent.Type != InteractionType.MessageComponent)
        {
            return Result.FromSuccess();
        }

        var createContext = gatewayEvent.CreateContext();
        if (!createContext.IsSuccess)
        {
            return Result.FromError(createContext);
        }

        var context = createContext.Entity;
        _contextInjectionService.Context = context;

        if (!context.Data.ComponentType.IsDefined(out var componentType))
        {
            return new InvalidOperationError("The interaction did not contain a component type.");
        }

        if (!context.Data.CustomID.IsDefined(out var customID))
        {
            return new InvalidOperationError("The interaction did not contain a custom ID.");
        }

        IReadOnlyList<Result> interactionResults;
        switch (componentType)
        {
            case ComponentType.Button:
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

                interactionResults = await RunEntityHandlersAsync<IButtonInteractiveEntity>
                (
                    (entity, c) => entity.HandleInteractionAsync(context.User, customID, c),
                    ct
                );

                break;
            }
            case ComponentType.SelectMenu:
            {
                if (!context.Data.Values.IsDefined(out var values))
                {
                    return new InvalidOperationError("The interaction did not contain selected values.");
                }

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

                interactionResults = await RunEntityHandlersAsync<ISelectMenuInteractiveEntity>
                (
                    (entity, c) => entity.HandleInteractionAsync(context.User, customID, values, c),
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

        return interactionResults.All(r => r.IsSuccess)
            ? Result.FromSuccess()
            : new AggregateError(interactionResults.Where(r => !r.IsSuccess).Cast<IResult>().ToArray());
    }

    private async Task<IReadOnlyList<Result>> RunEntityHandlersAsync<TEntity>
    (
        Func<TEntity, CancellationToken, Task<Result>> handler,
        CancellationToken ct = default
    )
        where TEntity : class
    {
        var entities = _services.GetServices<TEntity>();

        return await Task.WhenAll(entities.Select(async entity =>
        {
            if (entity is not InMemoryPersistentInteractiveEntity persistentEntity)
            {
                try
                {
                    return await handler(entity, ct);
                }
                catch (Exception e)
                {
                    return e;
                }
            }

            var dataKey = InMemoryPersistenceHelpers.CreateNonceKey(persistentEntity.Nonce);
            if (!_cache.TryGetValue<(SemaphoreSlim, object)>(dataKey, out var value))
            {
                return new NotFoundError("No persistent data was found for an interactive entity.");
            }

            var (semaphore, data) = value;

            // Figure out the real data type the entity wants
            var entityDataType = persistentEntity.DataType;

            if (entityDataType != data.GetType())
            {
                // This is a certified "oops" moment
                return new InvalidOperationError
                (
                    "The cached persistent data was not compatible with the data type required by the entity that " +
                    "requested it. Bug or API misuse?"
                );
            }

            persistentEntity.UntypedData = data;

            Result interactionResult;
            try
            {
                // Only one entity instance may operate on an in-memory data object at a time
                await semaphore.WaitAsync(ct);
                interactionResult = await handler(entity, ct);

                if (interactionResult.IsSuccess)
                {
                    _cache.Set(dataKey, (semaphore, persistentEntity.UntypedData));
                }
            }
            catch (Exception e)
            {
                return e;
            }
            finally
            {
                semaphore.Release();
            }

            return interactionResult;
        }));
    }
}
