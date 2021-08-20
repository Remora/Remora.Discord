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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Commands.Results;
using Remora.Commands.Services;
using Remora.Commands.Signatures;
using Remora.Commands.Tokenization;
using Remora.Commands.Trees;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
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
    [PublicAPI]
    public class InteractionResponder : IResponder<IInteractionCreate>
    {
        private readonly CommandService _commandService;
        private readonly InteractionResponderOptions _options;
        private readonly IDiscordRestInteractionAPI _interactionAPI;
        private readonly ExecutionEventCollectorService _eventCollector;
        private readonly IServiceProvider _services;
        private readonly ContextInjectionService _contextInjection;

        private readonly TokenizerOptions _tokenizerOptions;
        private readonly TreeSearchOptions _treeSearchOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionResponder"/> class.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="options">The options.</param>
        /// <param name="interactionAPI">The interaction API.</param>
        /// <param name="eventCollector">The event collector.</param>
        /// <param name="services">The available services.</param>
        /// <param name="contextInjection">The context injection service.</param>
        /// <param name="tokenizerOptions">The tokenizer options.</param>
        /// <param name="treeSearchOptions">The tree search options.</param>
        public InteractionResponder
        (
            CommandService commandService,
            IOptions<InteractionResponderOptions> options,
            IDiscordRestInteractionAPI interactionAPI,
            ExecutionEventCollectorService eventCollector,
            IServiceProvider services,
            ContextInjectionService contextInjection,
            IOptions<TokenizerOptions> tokenizerOptions,
            IOptions<TreeSearchOptions> treeSearchOptions
        )
        {
            _commandService = commandService;
            _options = options.Value;
            _eventCollector = eventCollector;
            _services = services;
            _contextInjection = contextInjection;
            _interactionAPI = interactionAPI;

            _tokenizerOptions = tokenizerOptions.Value;
            _treeSearchOptions = treeSearchOptions.Value;
        }

        /// <inheritdoc />
        public virtual async Task<Result> RespondAsync
        (
            IInteractionCreate? gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent is null)
            {
                return Result.FromSuccess();
            }

            if (gatewayEvent.Type != InteractionType.ApplicationCommand)
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

            var interactionData = gatewayEvent.Data.Value;
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
                interactionData
            );

            // Provide the created context to any services inside this scope
            _contextInjection.Context = context;

            Result<BoundCommandNode> findCommandResult = FindCommandNode(command, parameters);
            if (!findCommandResult.IsSuccess)
            {
                return await _eventCollector.RunPostExecutionEvents
                (
                    _services,
                    context,
                    findCommandResult,
                    ct
                );
            }

            if (!_options.SuppressAutomaticResponses)
            {
                // Signal Discord that we'll be handling this one asynchronously
                var response = new InteractionResponse(InteractionCallbackType.DeferredChannelMessageWithSource);

                EphemeralAttribute? ephemeralAttribute = findCommandResult.Entity.Node.FindCustomAttributeOnLocalTree<EphemeralAttribute>();
                if (ephemeralAttribute?.IsEphemeral == true)
                {
                    response = response with { Data = new InteractionCallbackData(Flags: InteractionCallbackDataFlags.Ephemeral) };
                }

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
            }

            // Run any user-provided pre execution events
            var preExecution = await _eventCollector.RunPreExecutionEvents(_services, context, ct);
            if (!preExecution.IsSuccess)
            {
                return preExecution;
            }

            // Run the actual command
            var executeResult = await _commandService.TryExecuteAsync
            (
                findCommandResult.Entity,
                _services,
                ct: ct
            );

            // Run any user-provided post execution events, passing along either the result of the command itself, or if
            // execution failed, the reason why
            return await _eventCollector.RunPostExecutionEvents
            (
                _services,
                context,
                executeResult.IsSuccess ? executeResult.Entity : executeResult,
                ct
            );
        }

        /// <summary>
        /// Attempts to find a command in the command tree.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandParameters">The parameters of the command.</param>
        /// <returns>A <see cref="Result{BoundCommandNode}"/> indicating if the command was successfully found, and containing the command node if so.</returns>
        private Result<BoundCommandNode> FindCommandNode(string commandName, IReadOnlyDictionary<string, IReadOnlyList<string>> commandParameters)
        {
            TreeSearchOptions searchOptions = new(StringComparison.OrdinalIgnoreCase);
            List<BoundCommandNode> commands = _commandService.Tree.Search(commandName, commandParameters, searchOptions: searchOptions).ToList();

            if (commands.Count == 0)
            {
                return new CommandNotFoundError(commandName);
            }

            if (commands.Count > 1)
            {
                return new AmbiguousCommandInvocationError();
            }

            return commands.Single();
        }
    }
}
