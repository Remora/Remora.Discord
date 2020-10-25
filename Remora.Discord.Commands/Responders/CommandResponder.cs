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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Remora.Commands.Services;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;

namespace Remora.Discord.Commands.Responders
{
    /// <summary>
    /// Responds to commands.
    /// </summary>
    public class CommandResponder : IResponder<IMessageCreate>, IResponder<IMessageUpdate>
    {
        private readonly CommandService _commandService;
        private readonly ICommandResponderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResponder"/> class.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        /// <param name="options">The command responder options.</param>
        public CommandResponder(CommandService commandService, IOptions<CommandResponderOptions> options)
        {
            _commandService = commandService;
            _options = options.Value;
        }

        /// <inheritdoc/>
        public async Task<EventResponseResult> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if (!(_options.Prefix is null))
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

            return await ExecuteCommandAsync(gatewayEvent.Content, ct);
        }

        /// <inheritdoc/>
        public async Task<EventResponseResult> RespondAsync(IMessageUpdate gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.Content.HasValue)
            {
                return EventResponseResult.FromSuccess();
            }

            if (!(_options.Prefix is null))
            {
                if (!gatewayEvent.Content.Value!.StartsWith(_options.Prefix))
                {
                    return EventResponseResult.FromSuccess();
                }
            }

            if (gatewayEvent.Author.HasValue)
            {
                var author = gatewayEvent.Author.Value!;
                if (author.IsBot.HasValue && author.IsBot.Value)
                {
                    return EventResponseResult.FromSuccess();
                }

                if (author.IsSystem.HasValue && author.IsSystem.Value)
                {
                    return EventResponseResult.FromSuccess();
                }
            }

            return await ExecuteCommandAsync(gatewayEvent.Content.Value!, ct);
        }

        private async Task<EventResponseResult> ExecuteCommandAsync(string content, CancellationToken ct = default)
        {
            var executeResult = await _commandService.TryExecuteAsync(content, ct);
            return executeResult.IsSuccess
                ? EventResponseResult.FromSuccess()
                : EventResponseResult.FromError(executeResult);
        }
    }
}
