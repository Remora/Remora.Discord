//
//  ChannelPinsUpdate.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events.Channels
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Gateway.Events.IChannelPinsUpdate" />
    [PublicAPI]
    public class ChannelPinsUpdate : IChannelPinsUpdate
    {
        /// <inheritdoc />
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc />
        public Snowflake ChannelID { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset> LastPinTimestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelPinsUpdate"/> class.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="lastPinTimestamp">The timestamp of the last pin.</param>
        public ChannelPinsUpdate
        (
            Optional<Snowflake> guildID,
            Snowflake channelID,
            Optional<DateTimeOffset> lastPinTimestamp
        )
        {
            this.GuildID = guildID;
            this.ChannelID = channelID;
            this.LastPinTimestamp = lastPinTimestamp;
        }
    }
}
