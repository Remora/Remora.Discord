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

using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Samples.HttpCat.Responders
{
    /// <summary>
    /// Responds to a httpcat command.
    /// </summary>
    public class HttpCatResponder : IResponder<IMessageCreate>
    {
        private readonly IDiscordRestChannelAPI _channelAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCatResponder"/> class.
        /// </summary>
        /// <param name="channelAPI">The channel API.</param>
        public HttpCatResponder(IDiscordRestChannelAPI channelAPI)
        {
            _channelAPI = channelAPI;
        }

        /// <inheritdoc/>
        public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if (!gatewayEvent.Content.StartsWith('!'))
            {
                return Result.FromSuccess();
            }

            var statusCode = gatewayEvent.Content[1..];

            if (!int.TryParse(statusCode, out var code))
            {
                return new GenericError("Could not parse an integer.");
            }

            var embedImage = new EmbedImage($"https://http.cat/{code}");
            var embed = new Embed(Image: embedImage);

            var reply = await _channelAPI.CreateMessageAsync(gatewayEvent.ChannelID, embeds: new[] { embed }, ct: ct);
            return !reply.IsSuccess
                ? Result.FromError(reply)
                : Result.FromSuccess();
        }
    }
}
