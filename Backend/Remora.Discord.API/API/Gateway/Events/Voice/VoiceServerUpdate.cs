//
//  VoiceServerUpdate.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc />
    public class VoiceServerUpdate : IVoiceServerUpdate
    {
        /// <inheritdoc/>
        public string Token { get; }

        /// <inheritdoc/>
        public Snowflake GuildID { get; }

        /// <inheritdoc/>
        public string Endpoint { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceServerUpdate"/> class.
        /// </summary>
        /// <param name="token">The voice connection token.</param>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="endpoint">The voice server host.</param>
        public VoiceServerUpdate(string token, Snowflake guildID, string endpoint)
        {
            this.Token = token;
            this.GuildID = guildID;
            this.Endpoint = endpoint;
        }
    }
}
