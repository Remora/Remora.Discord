//
//  MessageExtensions.cs
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Results;

namespace Remora.Discord.Extensions.Extensions
{
    /// <summary>
    /// Adds extensions to <see cref="IMessage"/> instances.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Determines whether a given message was sent by a user.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by a user; otherwise, <c>False</c>.</returns>
        public static bool IsUserMessage(this IMessage message)
            => message is { Type: MessageType.Default or MessageType.InlineReply } && !message.IsWebhookMessage() && !message.IsBotMessage();

        /// <summary>
        /// Determines whether a given message was sent by a webhook.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by a webhook; otherwise, <c>False</c>.</returns>
        public static bool IsWebhookMessage(this IMessage message)
            => message.WebhookID.IsDefined();

        /// <summary>
        /// Determines whether a given message was sent by a bot.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by a bot; otherwise, <c>False</c>.</returns>
        public static bool IsBotMessage(this IMessage message)
            => message.Author.IsBot.IsDefined(out var isBot) && isBot;

        private const string JumpUrlBase = "https://discord.com/channels/";

        /// <summary>
        /// Retrieves a jump url for the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channelAPI">A <see cref="IDiscordRestChannelAPI"/> used to get channel information.</param>
        /// <returns>A result containing a jump link to the specified message as a string.</returns>
        public static async Task<Result<string>> GetJumpUrlAsync(this IMessage message, IDiscordRestChannelAPI channelAPI)
        {
            var channelResult = await channelAPI.GetChannelAsync(message.ChannelID);

            if (!channelResult.IsSuccess)
            {
                return Result<string>.FromError(channelResult);
            }

            var guildMessage = message.GuildID.IsDefined(out var guildId);
            var channelID = channelResult.Entity.ID.Value;
            if (!channelResult.Entity.Name.IsDefined(out var channelName))
            {
                channelName = message.ChannelID.ToString();
            }

            var paths = new[]
            {
                guildMessage ? guildId.Value.ToString() : string.Empty,
                channelID.ToString(),
                message.ID.Value.ToString()
            };

            // A System.IO.Path.Combine-like method but for Urls.
            string jumpUrl = new Uri(paths.Aggregate(JumpUrlBase, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/')))).AbsoluteUri;

            // TODO: Use Format Utilities
            return $"[#{channelName}]({jumpUrl})";
        }
    }
}
