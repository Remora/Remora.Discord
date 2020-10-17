//
//  MessageReactionAdd.cs
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
    public class MessageReactionAdd : IMessageReactionAdd
    {
        /// <inheritdoc/>
        public Snowflake UserID { get; }

        /// <inheritdoc/>
        public Snowflake ChannelID { get; }

        /// <inheritdoc/>
        public Snowflake MessageID { get; }

        /// <inheritdoc/>
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc/>
        public Optional<IGuildMember> Member { get; }

        /// <inheritdoc/>
        public IPartialEmoji Emoji { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReactionAdd"/> class.
        /// </summary>
        /// <param name="userID">The ID of the reacting user.</param>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="member">The member information.</param>
        /// <param name="emoji">The emoji.</param>
        public MessageReactionAdd
        (
            Snowflake userID,
            Snowflake channelID,
            Snowflake messageID,
            Optional<Snowflake> guildID,
            Optional<IGuildMember> member,
            IPartialEmoji emoji
        )
        {
            this.UserID = userID;
            this.ChannelID = channelID;
            this.MessageID = messageID;
            this.GuildID = guildID;
            this.Member = member;
            this.Emoji = emoji;
        }
    }
}
