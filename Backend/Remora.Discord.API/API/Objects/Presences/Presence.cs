//
//  Presence.cs
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
using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Activities;
using Remora.Discord.API.Abstractions.Presence;
using Remora.Discord.API.Abstractions.Users;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class Presence : IPresence
    {
        /// <inheritdoc />
        public IUser User { get; }

        /// <inheritdoc />
        public IReadOnlyList<Snowflake> Roles { get; }

        /// <inheritdoc />
        public IActivity? Game { get; }

        /// <inheritdoc />
        public Snowflake GuildID { get; }

        /// <inheritdoc />
        public ClientStatus Status { get; }

        /// <inheritdoc />
        public IReadOnlyList<IActivity> Activities { get; }

        /// <inheritdoc />
        public IClientStatuses ClientStatus { get; }

        /// <inheritdoc />
        public Optional<DateTime?> PremiumSince { get; }

        /// <inheritdoc />
        public Optional<string?> Nickname { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Presence"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="roles">The roles the user has.</param>
        /// <param name="game">The user's current activity.</param>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="status">The user's current status.</param>
        /// <param name="activities">The user's current activities.</param>
        /// <param name="clientStatus">The user's platform-dependent status.</param>
        /// <param name="premiumSince">The time when the user started boosting the guild.</param>
        /// <param name="nickname">The user's nickname.</param>
        public Presence
        (
            IUser user,
            IReadOnlyList<Snowflake> roles,
            IActivity? game,
            Snowflake guildID,
            ClientStatus status,
            IReadOnlyList<IActivity> activities,
            IClientStatuses clientStatus,
            Optional<DateTime?> premiumSince,
            Optional<string?> nickname
        )
        {
            this.User = user;
            this.Roles = roles;
            this.Game = game;
            this.GuildID = guildID;
            this.Status = status;
            this.Activities = activities;
            this.ClientStatus = clientStatus;
            this.PremiumSince = premiumSince;
            this.Nickname = nickname;
        }
    }
}
