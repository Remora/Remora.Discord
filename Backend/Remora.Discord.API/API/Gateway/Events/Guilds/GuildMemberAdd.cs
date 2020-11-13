//
//  GuildMemberAdd.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc cref="IGuildMemberAdd" />
    [PublicAPI]
    public record GuildMemberAdd : GuildMember, IGuildMemberAdd
    {
        /// <inheritdoc />
        public Snowflake GuildID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildMemberAdd"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="nickname">The user's set nickname.</param>
        /// <param name="roles">The user's assigned roles.</param>
        /// <param name="joinedAt">When the user joined the server.</param>
        /// <param name="premiumSince">When the user started boosting the guild.</param>
        /// <param name="isDeafened">Whether the user is deafened in voice channels.</param>
        /// <param name="isMuted">Whether the user is muted in voice channels.</param>
        /// <param name="guildID">The ID of the guild.</param>
        public GuildMemberAdd
        (
            Optional<IUser> user,
            Optional<string?> nickname,
            IReadOnlyList<Snowflake> roles,
            DateTimeOffset joinedAt,
            Optional<DateTimeOffset?> premiumSince,
            bool isDeafened,
            bool isMuted,
            Snowflake guildID
        )
            : base
            (
                user,
                nickname,
                roles,
                joinedAt,
                premiumSince,
                isDeafened,
                isMuted
            )
        {
            this.GuildID = guildID;
        }
    }
}
