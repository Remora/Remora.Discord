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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Remora.Commands.Services;
using Remora.Commands.Trees;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;

namespace Remora.Discord.Commands.Responders
{
    /// <summary>
    /// Responds to interactions.
    /// </summary>
    public class InteractionResponder : IResponder<IInteractionCreate>
    {
        private readonly CommandService _commandService;
        private readonly IDiscordRestInteractionAPI _interactionAPI;
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionResponder"/> class.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="interactionAPI">The interaction API.</param>
        /// <param name="services">The available services.</param>
        public InteractionResponder
        (
            CommandService commandService,
            IDiscordRestInteractionAPI interactionAPI,
            IServiceProvider services
        )
        {
            _commandService = commandService;
            _services = services;
            _interactionAPI = interactionAPI;
        }

        /// <inheritdoc />
        public async Task<EventResponseResult> RespondAsync
        (
            IInteractionCreate? gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent is null)
            {
                return EventResponseResult.FromSuccess();
            }

            if (!gatewayEvent.Data.HasValue)
            {
                return EventResponseResult.FromSuccess();
            }

            if (!gatewayEvent.Member.User.HasValue)
            {
                return EventResponseResult.FromSuccess();
            }

            // Signal Discord that we'll be handling this one asynchronously
            var response = new InteractionResponse(InteractionResponseType.Acknowledge, default);
            var interactionResponse = await _interactionAPI.CreateInteractionResponseAsync
            (
                gatewayEvent.ID,
                gatewayEvent.Token,
                response,
                ct
            );

            if (!interactionResponse.IsSuccess)
            {
                return EventResponseResult.FromError(interactionResponse);
            }

            var interactionData = gatewayEvent.Data.Value!;
            var (command, parameters) = UnpackInteraction(interactionData);

            var context = new InteractionContext
            (
                gatewayEvent.ChannelID,
                gatewayEvent.Member.User.Value!,
                gatewayEvent.Member,
                gatewayEvent.Token,
                gatewayEvent.ID,
                gatewayEvent.GuildID
            );

            var searchOptions = new TreeSearchOptions(StringComparison.OrdinalIgnoreCase);

            var executeResult = await _commandService.TryExecuteAsync
            (
                command,
                parameters,
                _services,
                new object[] { context },
                searchOptions,
                ct
            );

            return executeResult.IsSuccess
                ? EventResponseResult.FromSuccess()
                : EventResponseResult.FromError(executeResult);
        }

        private (string Command, IReadOnlyDictionary<string, IReadOnlyList<string>> Parameters) UnpackInteraction
        (
            IApplicationCommandInteractionData commandData
        )
        {
            if (!commandData.Options.HasValue)
            {
                return (commandData.Name, new Dictionary<string, IReadOnlyList<string>>());
            }

            var commandStringBuilder = new StringBuilder();
            commandStringBuilder.Append(commandData.Name);

            var (command, parameters) = UnpackInteractionOptions(commandData.Options.Value!);
            commandStringBuilder.Append(" ");
            commandStringBuilder.Append(command);

            return (commandStringBuilder.ToString(), parameters);
        }

        private (string Command, IReadOnlyDictionary<string, IReadOnlyList<string>> Parameters) UnpackInteractionOptions
        (
            IReadOnlyList<IApplicationCommandInteractionDataOption> options,
            StringBuilder? commandStringBuilder = null,
            Dictionary<string, IReadOnlyList<string>>? parameters = null
        )
        {
            commandStringBuilder ??= new StringBuilder();
            parameters ??= new Dictionary<string, IReadOnlyList<string>>();

            if (options.Count > 1)
            {
                // multiple parameters
                foreach (var option in options)
                {
                    UnpackInteractionParameter(parameters, option);
                }
            }
            else
            {
                var singleOption = options.Single();
                if (singleOption.Value.HasValue)
                {
                    // A single parameter
                    UnpackInteractionParameter(parameters, singleOption);
                }
                else if (singleOption.Options.HasValue)
                {
                    // A nested group
                    var nestedOptions = singleOption.Options.Value!;

                    commandStringBuilder.Append(" ");
                    commandStringBuilder.Append(singleOption.Name);

                    UnpackInteractionOptions(nestedOptions, commandStringBuilder, parameters);
                }
                else
                {
                    // A parameterless command
                    commandStringBuilder.Append(" ");
                    commandStringBuilder.Append(singleOption.Name);
                }
            }

            return (commandStringBuilder.ToString().Trim(), parameters);
        }

        private static void UnpackInteractionParameter
        (
            IDictionary<string, IReadOnlyList<string>> parameters,
            IApplicationCommandInteractionDataOption option
        )
        {
            if (!option.Value.HasValue)
            {
                throw new InvalidOperationException();
            }

            var values = new List<string>();
            var optionValue = option.Value.Value!;
            if (optionValue.Value is ICollection collection)
            {
                values.AddRange(collection.Cast<object>().Select(o => o.ToString()));
            }
            else
            {
                values.Add(optionValue.Value.ToString());
            }

            parameters.Add(option.Name, values);
        }
    }
}
