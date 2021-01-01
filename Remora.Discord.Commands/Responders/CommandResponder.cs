//
//  CommandResponder.cs
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Commands.Services;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace Remora.Discord.Commands.Responders
{
    /// <summary>
    /// Responds to commands.
    /// </summary>
    public class CommandResponder : IResponder<IMessageCreate>, IResponder<IMessageUpdate>
    {
        private readonly CommandService _commandService;
        private readonly ICommandResponderOptions _options;
        private readonly ExecutionEventCollectorService _eventCollector;
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResponder"/> class.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="options">The command responder options.</param>
        /// <param name="eventCollector">The event collector.</param>
        /// <param name="services">The available services.</param>
        public CommandResponder
        (
            CommandService commandService,
            IOptions<CommandResponderOptions> options,
            ExecutionEventCollectorService eventCollector,
            IServiceProvider services
        )
        {
            _commandService = commandService;
            _services = services;
            _eventCollector = eventCollector;
            _options = options.Value;
        }

        /// <inheritdoc/>
        public async Task<EventResponseResult> RespondAsync
        (
            IMessageCreate? gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent is null)
            {
                return EventResponseResult.FromSuccess();
            }

            if (_options.Prefix is not null)
            {
                if (!gatewayEvent.Content.StartsWith(_options.Prefix))
                {
                    return EventResponseResult.FromSuccess();
                }
            }

            var author = gatewayEvent.Author;
            if (author.IsBot.HasValue && author.IsBot.Value)
            {
                return EventResponseResult.FromSuccess();
            }

            if (author.IsSystem.HasValue && author.IsSystem.Value)
            {
                return EventResponseResult.FromSuccess();
            }

            var context = new MessageContext
            (
                gatewayEvent.ChannelID,
                author,
                gatewayEvent.ID,
                new PartialMessage
                (
                    gatewayEvent.ID,
                    gatewayEvent.ChannelID,
                    gatewayEvent.GuildID,
                    new Optional<IUser>(gatewayEvent.Author),
                    gatewayEvent.Member,
                    gatewayEvent.Content,
                    gatewayEvent.Timestamp,
                    gatewayEvent.EditedTimestamp,
                    gatewayEvent.IsTTS,
                    gatewayEvent.MentionsEveryone,
                    new Optional<IReadOnlyList<IUserMention>>(gatewayEvent.Mentions),
                    new Optional<IReadOnlyList<Snowflake>>(gatewayEvent.MentionedRoles),
                    gatewayEvent.MentionedChannels,
                    new Optional<IReadOnlyList<IAttachment>>(gatewayEvent.Attachments),
                    new Optional<IReadOnlyList<IEmbed>>(gatewayEvent.Embeds),
                    gatewayEvent.Reactions,
                    gatewayEvent.Nonce,
                    gatewayEvent.IsPinned,
                    gatewayEvent.WebhookID,
                    gatewayEvent.Type,
                    gatewayEvent.Activity,
                    gatewayEvent.Application,
                    gatewayEvent.MessageReference,
                    gatewayEvent.Flags
                )
            );

            return await ExecuteCommandAsync(gatewayEvent.Content, context, ct);
        }

        /// <inheritdoc/>
        public async Task<EventResponseResult> RespondAsync
        (
            IMessageUpdate? gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent is null)
            {
                return EventResponseResult.FromSuccess();
            }

            if (!gatewayEvent.Content.HasValue)
            {
                return EventResponseResult.FromSuccess();
            }

            if (_options.Prefix is not null)
            {
                if (!gatewayEvent.Content.Value!.StartsWith(_options.Prefix))
                {
                    return EventResponseResult.FromSuccess();
                }
            }

            if (!gatewayEvent.Author.HasValue)
            {
                return EventResponseResult.FromSuccess();
            }

            var author = gatewayEvent.Author.Value!;
            if (author.IsBot.HasValue && author.IsBot.Value)
            {
                return EventResponseResult.FromSuccess();
            }

            if (author.IsSystem.HasValue && author.IsSystem.Value)
            {
                return EventResponseResult.FromSuccess();
            }

            var context = new MessageContext
            (
                gatewayEvent.ChannelID.Value,
                author,
                gatewayEvent.ID.Value,
                gatewayEvent
            );

            return await ExecuteCommandAsync(gatewayEvent.Content.Value!, context, ct);
        }

        private async Task<EventResponseResult> ExecuteCommandAsync
        (
            string content,
            ICommandContext commandContext,
            CancellationToken ct = default
        )
        {
            // Strip off the prefix
            if (_options.Prefix is not null)
            {
                content = content.Substring
                (
                    content.IndexOf(_options.Prefix, StringComparison.Ordinal) + _options.Prefix.Length
                );
            }

            var additionalParameters = new object[] { commandContext };

            // Run any user-provided pre execution events
            var preExecution = await _eventCollector.RunPreExecutionEvents(commandContext, ct);
            if (!preExecution.IsSuccess)
            {
                return EventResponseResult.FromError(preExecution);
            }

            // Run the actual command
            var executeResult = await _commandService.TryExecuteAsync
            (
                content,
                _services,
                additionalParameters,
                ct: ct
            );

            if (!executeResult.IsSuccess)
            {
                return EventResponseResult.FromError(executeResult);
            }

            // Run any user-provided post execution events
            var postExecution = await _eventCollector.RunPostExecutionEvents
            (
                commandContext,
                executeResult.InnerResult!,
                ct
            );

            if (!postExecution.IsSuccess)
            {
                return EventResponseResult.FromError(postExecution);
            }

            return EventResponseResult.FromSuccess();
        }
    }
}
