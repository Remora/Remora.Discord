//
//  InteractionResponder.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Services;
using Remora.Commands.Trees;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Commands.Responders
{
    /// <summary>
    /// Responds to interactions.
    /// </summary>
    [UsedImplicitly]
    public class InteractionResponder : IResponder<IInteractionCreate>
    {
        private readonly CommandService _commandService;
        private readonly IDiscordRestInteractionAPI _interactionAPI;
        private readonly ExecutionEventCollectorService _eventCollector;
        private readonly IServiceProvider _services;
        private readonly ContextInjectionService _contextInjection;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionResponder"/> class.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="interactionAPI">The interaction API.</param>
        /// <param name="eventCollector">The event collector.</param>
        /// <param name="services">The available services.</param>
        /// <param name="contextInjection">The context injection service.</param>
        public InteractionResponder
        (
            CommandService commandService,
            IDiscordRestInteractionAPI interactionAPI,
            ExecutionEventCollectorService eventCollector,
            IServiceProvider services,
            ContextInjectionService contextInjection
        )
        {
            _commandService = commandService;
            _eventCollector = eventCollector;
            _services = services;
            _contextInjection = contextInjection;
            _interactionAPI = interactionAPI;
        }

        /// <inheritdoc />
        public async Task<Result> RespondAsync
        (
            IInteractionCreate? gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent is null)
            {
                return Result.FromSuccess();
            }

            if (!gatewayEvent.Data.HasValue)
            {
                return Result.FromSuccess();
            }

            if (!gatewayEvent.ChannelID.HasValue)
            {
                return Result.FromSuccess();
            }

            var user = gatewayEvent.User.HasValue
                ? gatewayEvent.User.Value
                : gatewayEvent.Member.HasValue
                    ? gatewayEvent.Member.Value.User.HasValue
                        ? gatewayEvent.Member.Value.User.Value
                        : null
                    : null;

            if (user is null)
            {
                return Result.FromSuccess();
            }

            // Signal Discord that we'll be handling this one asynchronously
            var response = new InteractionResponse(InteractionCallbackType.DeferredChannelMessageWithSource);
            var interactionResponse = await _interactionAPI.CreateInteractionResponseAsync
            (
                gatewayEvent.ID,
                gatewayEvent.Token,
                response,
                ct
            );

            if (!interactionResponse.IsSuccess)
            {
                return interactionResponse;
            }

            var interactionData = gatewayEvent.Data.Value!;
            interactionData.UnpackInteraction(out var command, out var parameters);

            var context = new InteractionContext
            (
                gatewayEvent.GuildID,
                gatewayEvent.ChannelID.Value,
                user,
                gatewayEvent.Member,
                gatewayEvent.Token,
                gatewayEvent.ID,
                gatewayEvent.ApplicationID,
                interactionData.Resolved
            );

            // Provide the created context to any services inside this scope
            _contextInjection.Context = context;

            // Run any user-provided pre execution events
            var preExecution = await _eventCollector.RunPreExecutionEvents(context, ct);
            if (!preExecution.IsSuccess)
            {
                return preExecution;
            }

            // Run the actual command
            var searchOptions = new TreeSearchOptions(StringComparison.OrdinalIgnoreCase);
            var executeResult = await _commandService.TryExecuteAsync
            (
                command,
                parameters,
                _services,
                searchOptions: searchOptions,
                ct: ct
            );

            if (!executeResult.IsSuccess)
            {
                return Result.FromError(executeResult);
            }

            // Run any user-provided post execution events
            return await _eventCollector.RunPostExecutionEvents
            (
                context,
                executeResult.Entity,
                ct
            );
        }
    }
}
