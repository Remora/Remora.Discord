//
//  MessageReference.cs
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

using Remora.Discord.API.Abstractions.Messages;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects.Messages
{
    /// <inheritdoc />
    public class MessageReference : IMessageReference
    {
        /// <inheritdoc />
        public Optional<Snowflake> MessageID { get; }

        /// <inheritdoc />
        public Snowflake ChannelID { get; }

        /// <inheritdoc />
        public Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReference"/> class.
        /// </summary>
        /// <param name="messageID">The ID of the referenced message.</param>
        /// <param name="channelID">The ID of the channel the referenced message is in.</param>
        /// <param name="guildID">The ID of the guild the referenced message is in.</param>
        public MessageReference(Optional<Snowflake> messageID, Snowflake channelID, Optional<Snowflake> guildID)
        {
            this.MessageID = messageID;
            this.ChannelID = channelID;
            this.GuildID = guildID;
        }
    }
}
