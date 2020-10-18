//
//  ChannelMention.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class ChannelMention : IChannelMention
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public Snowflake GuildID { get; }

        /// <inheritdoc />
        public ChannelType Type { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelMention"/> class.
        /// </summary>
        /// <param name="id">The ID of the mentioned channel.</param>
        /// <param name="guildID">The ID of the guild the channel is in.</param>
        /// <param name="type">The type of channel that was mentioned.</param>
        /// <param name="name">The name of the channel.</param>
        public ChannelMention(Snowflake id, Snowflake guildID, ChannelType type, string name)
        {
            this.ID = id;
            this.GuildID = guildID;
            this.Type = type;
            this.Name = name;
        }
    }
}
