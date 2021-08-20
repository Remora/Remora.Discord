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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Commands.Services;
using Remora.Commands.Tokenization;
using Remora.Commands.Trees;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Commands.Responders
{
    /// <summary>
    /// Responds to commands.
    /// </summary>
    [PublicAPI]
    public class CommandResponder : IResponder<IMessageCreate>, IResponder<IMessageUpdate>
    {
        private readonly CommandService _commandService;
        private readonly ICommandResponderOptions _options;
        private readonly ExecutionEventCollectorService _eventCollector;
        private readonly IServiceProvider _services;
        private readonly ContextInjectionService _contextInjection;

        private readonly TokenizerOptions _tokenizerOptions;
        private readonly TreeSearchOptions _treeSearchOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResponder"/> class.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="options">The command responder options.</param>
        /// <param name="eventCollector">The event collector.</param>
        /// <param name="services">The available services.</param>
        /// <param name="contextInjection">The injection service.</param>
        /// <param name="tokenizerOptions">The tokenizer options.</param>
        /// <param name="treeSearchOptions">The tree search options.</param>
        public CommandResponder
        (
            CommandService commandService,
            IOptions<CommandResponderOptions> options,
            ExecutionEventCollectorService eventCollector,
            IServiceProvider services,
            ContextInjectionService contextInjection,
            IOptions<TokenizerOptions> tokenizerOptions,
            IOptions<TreeSearchOptions> treeSearchOptions
        )
        {
            _commandService = commandService;
            _services = services;
            _contextInjection = contextInjection;
            _eventCollector = eventCollector;
            _options = options.Value;

            _tokenizerOptions = tokenizerOptions.Value;
            _treeSearchOptions = treeSearchOptions.Value;
        }

        /// <inheritdoc/>
        public virtual async Task<Result> RespondAsync
        (
            IMessageCreate? gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent is null)
            {
                return Result.FromSuccess();
            }

            if (_options.Prefix is not null)
            {
                if (!gatewayEvent.Content.StartsWith(_options.Prefix))
                {
                    return Result.FromSuccess();
                }
            }

            var author = gatewayEvent.Author;
            if (author.IsBot.IsDefined(out var isBot) && isBot)
            {
                return Result.FromSuccess();
            }

            if (author.IsSystem.IsDefined(out var isSystem) && isSystem)
            {
                return Result.FromSuccess();
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
                    gatewayEvent.ApplicationID,
                    gatewayEvent.MessageReference,
                    gatewayEvent.Flags,
                    gatewayEvent.ReferencedMessage,
                    gatewayEvent.Interaction,
                    gatewayEvent.Thread,
                    gatewayEvent.Components,
                    gatewayEvent.StickerItems
                )
            );

            return await ExecuteCommandAsync(gatewayEvent.Content, context, ct);
        }

        /// <inheritdoc/>
        public virtual async Task<Result> RespondAsync
        (
            IMessageUpdate? gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent is null)
            {
                return Result.FromSuccess();
            }

            if (!gatewayEvent.ID.IsDefined(out var messageID))
            {
                return Result.FromSuccess();
            }

            if (!gatewayEvent.ChannelID.IsDefined(out var channelID))
            {
                return Result.FromSuccess();
            }

            if (!gatewayEvent.Content.IsDefined(out var content))
            {
                return Result.FromSuccess();
            }

            if (_options.Prefix is not null)
            {
                if (!content.StartsWith(_options.Prefix))
                {
                    return Result.FromSuccess();
                }
            }

            if (!gatewayEvent.Author.IsDefined(out var author))
            {
                return Result.FromSuccess();
            }

            if (author.IsBot.IsDefined(out var isBot) && isBot)
            {
                return Result.FromSuccess();
            }

            if (author.IsSystem.IsDefined(out var isSystem) && isSystem)
            {
                return Result.FromSuccess();
            }

            var context = new MessageContext
            (
                channelID,
                author,
                messageID,
                gatewayEvent
            );

            return await ExecuteCommandAsync(content, context, ct);
        }

        /// <summary>
        /// Executes the actual command.
        /// </summary>
        /// <param name="content">The contents of the message.</param>
        /// <param name="commandContext">The command context.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        protected virtual async Task<Result> ExecuteCommandAsync
        (
            string content,
            ICommandContext commandContext,
            CancellationToken ct = default
        )
        {
            // Provide the created context to any services inside this scope
            _contextInjection.Context = commandContext;

            // Strip off the prefix
            if (_options.Prefix is not null)
            {
                content = content
                [
                    (content.IndexOf(_options.Prefix, StringComparison.Ordinal) + _options.Prefix.Length)..
                ];
            }

            // Run any user-provided pre execution events
            var preExecution = await _eventCollector.RunPreExecutionEvents(_services, commandContext, ct);
            if (!preExecution.IsSuccess)
            {
                return preExecution;
            }

            // Run the actual command
            var executeResult = await _commandService.TryExecuteAsync
            (
                content,
                _services,
                tokenizerOptions: _tokenizerOptions,
                searchOptions: _treeSearchOptions,
                ct: ct
            );

            // Run any user-provided post execution events, passing along either the result of the command itself, or if
            // execution failed, the reason why
            return await _eventCollector.RunPostExecutionEvents
            (
                _services,
                commandContext,
                executeResult.IsSuccess ? executeResult.Entity : executeResult,
                ct
            );
        }
    }
}
