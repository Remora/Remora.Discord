//
//  PartialPresence.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class PartialPresence : IPartialPresence
    {
        /// <inheritdoc />
        public Optional<IPartialUser> User { get; }

        /// <inheritdoc />
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc />
        public Optional<ClientStatus> Status { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IActivity>?> Activities { get; }

        /// <inheritdoc />
        public Optional<IClientStatuses> ClientStatus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialPresence"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="status">The user's current status.</param>
        /// <param name="activities">The user's current activities.</param>
        /// <param name="clientStatus">The user's platform-dependent status.</param>
        public PartialPresence
        (
            Optional<IPartialUser> user,
            Optional<Snowflake> guildID,
            Optional<ClientStatus> status,
            Optional<IReadOnlyList<IActivity>?> activities,
            Optional<IClientStatuses> clientStatus
        )
        {
            this.User = user;
            this.GuildID = guildID;
            this.Status = status;
            this.Activities = activities;
            this.ClientStatus = clientStatus;
        }
    }
}
