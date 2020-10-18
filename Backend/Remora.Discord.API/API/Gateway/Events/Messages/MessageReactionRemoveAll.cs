//
//  MessageReactionRemoveAll.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc />
    [PublicAPI]
    public class MessageReactionRemoveAll : IMessageReactionRemoveAll
    {
        /// <inheritdoc/>
        public Snowflake ChannelID { get; }

        /// <inheritdoc/>
        public Snowflake MessageID { get; }

        /// <inheritdoc/>
        public Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReactionRemoveAll"/> class.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="guildID">The ID of the guild.</param>
        public MessageReactionRemoveAll(Snowflake channelID, Snowflake messageID, Optional<Snowflake> guildID)
        {
            this.ChannelID = channelID;
            this.MessageID = messageID;
            this.GuildID = guildID;
        }
    }
}
