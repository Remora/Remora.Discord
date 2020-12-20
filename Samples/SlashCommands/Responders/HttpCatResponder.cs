//
//  HttpCatResponder.cs
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
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;

namespace Remora.Discord.Samples.SlashCommands.Responders
{
    /// <summary>
    /// Responds to a httpcat command.
    /// </summary>
    public class HttpCatResponder : IResponder<IInteractionCreate>, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscordRestInteractionAPI _interactionAPI;
        private readonly IDiscordRestChannelAPI _channelAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCatResponder"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The http client factory.</param>
        /// <param name="channelAPI">The channel API.</param>
        /// <param name="interactionAPI">The interaction API.</param>
        public HttpCatResponder
        (
            IHttpClientFactory httpClientFactory,
            IDiscordRestChannelAPI channelAPI,
            IDiscordRestInteractionAPI interactionAPI
        )
        {
            _httpClient = httpClientFactory.CreateClient();
            _channelAPI = channelAPI;
            _interactionAPI = interactionAPI;
        }

        /// <inheritdoc/>
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

            if (gatewayEvent.Type != InteractionType.ApplicationCommand)
            {
                return EventResponseResult.FromSuccess();
            }

            // Thanks, we'll deal with this on our own
            var asyncHandlingReply = await _interactionAPI.CreateInteractionResponseAsync
            (
                gatewayEvent.ID,
                gatewayEvent.Token,
                new InteractionResponse(InteractionResponseType.Acknowledge, default),
                ct
            );

            if (!asyncHandlingReply.IsSuccess)
            {
                return EventResponseResult.FromError(asyncHandlingReply);
            }

            if (!gatewayEvent.Data.HasValue)
            {
                return EventResponseResult.FromSuccess();
            }

            var commandData = gatewayEvent.Data.Value!;
            if (commandData.Name != "cat")
            {
                return EventResponseResult.FromSuccess();
            }

            if (!commandData.Options.HasValue)
            {
                return EventResponseResult.FromError("No options were passed, but at least one was expected.");
            }

            var commandOptions = commandData.Options.Value!;
            if (commandOptions.Count != 1)
            {
                return EventResponseResult.FromError("Exactly one option was expected.");
            }

            var codeOption = commandOptions.Single();
            if (codeOption.Name != "code")
            {
                return EventResponseResult.FromError("The option name was invalid.");
            }

            if (!codeOption.Value.HasValue)
            {
                return EventResponseResult.FromError("The option value was invalid.");
            }

            var optionValue = codeOption.Value.Value!;
            if (!optionValue.TryPickT2(out var code, out _))
            {
                return EventResponseResult.FromError("The value wasn't an integer.");
            }

            var embedImage = new EmbedImage($"https://http.cat/{code}");
            var embed = new Embed(Image: embedImage);

            var reply = await _channelAPI.CreateMessageAsync(gatewayEvent.ChannelID, embed: embed, ct: ct);
            return !reply.IsSuccess
                ? EventResponseResult.FromError(asyncHandlingReply)
                : EventResponseResult.FromSuccess();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
