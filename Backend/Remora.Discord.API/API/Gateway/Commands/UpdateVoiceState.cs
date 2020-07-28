//
//  UpdateVoiceState.cs
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

using Remora.Discord.API.Abstractions.Commands;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Commands
{
    /// <summary>
    /// Represents a command to update the voice state of the client.
    /// </summary>
    public class UpdateVoiceState : IUpdateVoiceState
    {
        /// <inheritdoc />
        public Snowflake GuildID { get; }

        /// <inheritdoc />
        public Snowflake? ChannelID { get; }

        /// <inheritdoc />
        public bool IsSelfMuted { get; }

        /// <inheritdoc />
        public bool IsSelfDeafened { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateVoiceState"/> class.
        /// </summary>
        /// <param name="guildID">The guild ID.</param>
        /// <param name="isSelfMuted">Whether the client is muted.</param>
        /// <param name="isSelfDeafened">Whether the client is deafened.</param>
        /// <param name="channelID">The ID of the channel to move to, or null to disconnect.</param>
        public UpdateVoiceState
        (
            Snowflake guildID,
            bool isSelfMuted,
            bool isSelfDeafened,
            Snowflake? channelID = null
        )
        {
            this.GuildID = guildID;
            this.ChannelID = channelID;
            this.IsSelfMuted = isSelfMuted;
            this.IsSelfDeafened = isSelfDeafened;
        }
    }
}
