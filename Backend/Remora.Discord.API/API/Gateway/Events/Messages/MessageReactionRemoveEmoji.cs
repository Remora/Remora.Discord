//
//  MessageReactionRemoveEmoji.cs
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

using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc />
    public class MessageReactionRemoveEmoji : IMessageReactionRemoveEmoji
    {
        /// <inheritdoc/>
        public Snowflake ChannelID { get; }

        /// <inheritdoc/>
        public Snowflake MessageID { get; }

        /// <inheritdoc/>
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc/>
        public IEmoji Emoji { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReactionRemoveEmoji"/> class.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="emoji">The emoji.</param>
        public MessageReactionRemoveEmoji
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<Snowflake> guildID,
            IEmoji emoji
        )
        {
            this.ChannelID = channelID;
            this.MessageID = messageID;
            this.GuildID = guildID;
            this.Emoji = emoji;
        }
    }
}
