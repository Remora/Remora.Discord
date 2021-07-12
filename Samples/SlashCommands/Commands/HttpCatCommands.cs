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

using System.ComponentModel;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Samples.SlashCommands.Commands
{
    /// <summary>
    /// Responds to a httpcat command.
    /// </summary>
    public class HttpCatCommands : CommandGroup
    {
        private readonly InteractionContext _context;
        private readonly IDiscordRestWebhookAPI _webhookAPI;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCatCommands"/> class.
        /// </summary>
        /// <param name="webhookAPI">The webhook API.</param>
        /// <param name="context">The command context.</param>
        public HttpCatCommands
        (
            IDiscordRestWebhookAPI webhookAPI,
            InteractionContext context
        )
        {
            _webhookAPI = webhookAPI;
            _context = context;
        }

        /// <summary>
        /// Posts a HTTP error code cat.
        /// </summary>
        /// <param name="httpCode">The HTTP error code.</param>
        /// <returns>The result of the command.</returns>
        [Command("cat")]
        [Description("Posts a cat image that represents the given error code.")]
        public async Task<IResult> PostHttpCatAsync([Description("The HTTP code.")] int httpCode)
        {
            var embedImage = new EmbedImage($"https://http.cat/{httpCode}");
            var embed = new Embed(Image: embedImage);

            var reply = await _webhookAPI.CreateFollowupMessageAsync
            (
                _context.ApplicationID,
                _context.Token,
                embeds: new[] { embed },
                ct: this.CancellationToken
            );

            return !reply.IsSuccess
                ? Result.FromError(reply)
                : Result.FromSuccess();
        }

        /// <summary>
        /// Posts a HTTP error code cat.
        /// </summary>
        /// <param name="catUser">The user to cattify.</param>
        /// <returns>The result of the command.</returns>
        [Command("user-cat")]
        [Description("Posts a cat image that matches the user.")]
        public Task<IResult> PostUserHttpCatAsync([Description("The user to cattify")] IGuildMember catUser)
        {
            if (!catUser.User.HasValue)
            {
                return Task.FromResult<IResult>
                (
                    Result.FromError(new InvalidOperationError("No user field in the guild member??"))
                );
            }

            var modulo = (int)(catUser.User.Value.ID.Value % 999);
            return PostHttpCatAsync(modulo);
        }
    }
}
