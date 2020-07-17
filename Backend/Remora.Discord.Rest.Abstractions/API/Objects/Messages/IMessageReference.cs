//
//  IMessageReference.cs
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

using Remora.Discord.Core;

namespace Remora.Discord.Rest.Abstractions.Messages
{
    /// <summary>
    /// Represents a message reference.
    /// </summary>
    public interface IMessageReference
    {
        /// <summary>
        /// Gets the ID of the originating message.
        /// </summary>
        Optional<Snowflake> MessageID { get; }

        /// <summary>
        /// Gets the ID of the originating message's channel.
        /// </summary>
        Snowflake ChannelID { get; }

        /// <summary>
        /// Gets the ID of the originating message's guild.
        /// </summary>
        Optional<Snowflake> GuildID { get; }
    }
}
