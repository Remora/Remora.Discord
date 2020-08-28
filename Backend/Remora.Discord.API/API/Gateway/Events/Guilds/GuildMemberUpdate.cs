//
//  GuildMemberUpdate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc />
    public class GuildMemberUpdate : IGuildMemberUpdate
    {
        /// <inheritdoc />
        public Snowflake GuildID { get; }

        /// <inheritdoc/>
        public IReadOnlyList<Snowflake> Roles { get; }

        /// <inheritdoc/>
        public IUser User { get; }

        /// <inheritdoc/>
        public Optional<string?> Nickname { get; }

        /// <inheritdoc/>
        public DateTimeOffset JoinedAt { get; }

        /// <inheritdoc/>
        public Optional<DateTimeOffset?> PremiumSince { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildMemberUpdate"/> class.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="roles">The user's roles.</param>
        /// <param name="user">The user.</param>
        /// <param name="nickname">The user's nickname.</param>
        /// <param name="joinedAt">When the user joined.</param>
        /// <param name="premiumSince">When the user started boosting the guild.</param>
        public GuildMemberUpdate
        (
            Snowflake guildID,
            IReadOnlyList<Snowflake> roles,
            IUser user,
            Optional<string?> nickname,
            DateTimeOffset joinedAt,
            Optional<DateTimeOffset?> premiumSince
        )
        {
            this.GuildID = guildID;
            this.Roles = roles;
            this.User = user;
            this.Nickname = nickname;
            this.JoinedAt = joinedAt;
            this.PremiumSince = premiumSince;
        }
    }
}
