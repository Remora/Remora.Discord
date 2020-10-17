//
//  PartialGuildMember.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class PartialGuildMember : IPartialGuildMember
    {
        /// <inheritdoc />
        public Optional<IUser> User { get; }

        /// <inheritdoc />
        public Optional<string?> Nickname { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<Snowflake>> Roles { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset> JoinedAt { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset?> PremiumSince { get; }

        /// <inheritdoc />
        public Optional<bool> IsDeafened { get; }

        /// <inheritdoc />
        public Optional<bool> IsMuted { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialGuildMember"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="nickname">The user's set nickname.</param>
        /// <param name="roles">The user's assigned roles.</param>
        /// <param name="joinedAt">When the user joined the server.</param>
        /// <param name="premiumSince">When the user started boosting the guild.</param>
        /// <param name="isDeafened">Whether the user is deafened in voice channels.</param>
        /// <param name="isMuted">Whether the user is muted in voice channels.</param>
        public PartialGuildMember
        (
            Optional<IUser> user,
            Optional<string?> nickname,
            Optional<IReadOnlyList<Snowflake>> roles,
            Optional<DateTimeOffset> joinedAt,
            Optional<DateTimeOffset?> premiumSince,
            Optional<bool> isDeafened,
            Optional<bool> isMuted
        )
        {
            this.User = user;
            this.Nickname = nickname;
            this.Roles = roles;
            this.JoinedAt = joinedAt;
            this.PremiumSince = premiumSince;
            this.IsDeafened = isDeafened;
            this.IsMuted = isMuted;
        }
    }
}
