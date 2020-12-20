//
//  MessageContext.cs
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

using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.Commands.Contexts
{
    /// <summary>
    /// Represents contextual information about a message.
    /// </summary>
    public class MessageContext
    {
        /// <summary>
        /// Gets the ID of the message.
        /// </summary>
        public Snowflake MessageID { get; }

        /// <summary>
        /// Gets the ID of the channel the message was sent in.
        /// </summary>
        public Snowflake ChannelID { get; }

        /// <summary>
        /// Gets the full partial message that the event received.
        /// </summary>
        public IPartialMessage Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContext"/> class.
        /// </summary>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="message">The partial message.</param>
        public MessageContext(Snowflake messageID, Snowflake channelID, IPartialMessage message)
        {
            this.MessageID = messageID;
            this.ChannelID = channelID;
            this.Message = message;
        }
    }
}
