//
//  Ready.cs
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

using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <summary>
    /// Represents the Ready event.
    /// </summary>
    public class Ready : IReady
    {
        /// <inheritdoc />
        public int Version { get; }

        /// <inheritdoc />
        public IUser User { get; }

        /// <inheritdoc />
        public IReadOnlyList<IUnavailableGuild> Guilds { get; }

        /// <inheritdoc />
        public string SessionID { get; }

        /// <inheritdoc />
        public Optional<IShardIdentification> Shard { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ready"/> class.
        /// </summary>
        /// <param name="version">The gateway version.</param>
        /// <param name="user">The user object.</param>
        /// <param name="guilds">The guilds the bot is in.</param>
        /// <param name="sessionID">The session ID.</param>
        /// <param name="shard">The sharding information.</param>
        public Ready
        (
            int version,
            IUser user,
            IReadOnlyList<IUnavailableGuild> guilds,
            string sessionID,
            Optional<IShardIdentification> shard
        )
        {
            this.Version = version;
            this.User = user;
            this.Guilds = guilds;
            this.SessionID = sessionID;
            this.Shard = shard;
        }
    }
}
