//
//  HttpCatCommands.cs
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
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace Remora.Discord.Samples.SlashCommands.Commands
{
    /// <summary>
    /// Responds to a httpcat command.
    /// </summary>
    public class HttpCatCommands : CommandGroup
    {
        private readonly ICommandContext _context;
        private readonly IDiscordRestChannelAPI _channelAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCatCommands"/> class.
        /// </summary>
        /// <param name="channelAPI">The channel API.</param>
        /// <param name="context">The command context.</param>
        public HttpCatCommands
        (
            IDiscordRestChannelAPI channelAPI,
            ICommandContext context
        )
        {
            _channelAPI = channelAPI;
            _context = context;
        }

        /// <summary>
        /// Posts a HTTP error code cat.
        /// </summary>
        /// <param name="httpCode">The HTTP error code.</param>
        /// <returns>The result of the command.</returns>
        [Command("cat")]
        [Description("Posts a cat image that represents the given error code.")]
        public async Task<IResult> PostHttpCatAsync(int httpCode)
        {
            var embedImage = new EmbedImage($"https://http.cat/{httpCode}");
            var embed = new Embed(Image: embedImage);

            var reply = await _channelAPI.CreateMessageAsync
            (
                _context.ChannelID,
                embed: embed,
                ct: this.CancellationToken
            );

            return !reply.IsSuccess
                ? EventResponseResult.FromError(reply)
                : EventResponseResult.FromSuccess();
        }
    }
}
