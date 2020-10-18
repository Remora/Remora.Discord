//
//  IGuildMember.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents information about a guild member.
    /// </summary>
    [PublicAPI]
    public interface IGuildMember
    {
        /// <summary>
        /// Gets the user this guild member represents.
        /// </summary>
        Optional<IUser> User { get; }

        /// <summary>
        /// Gets the user's guild nickname.
        /// </summary>
        Optional<string?> Nickname { get; }

        /// <summary>
        /// Gets the roles the user has.
        /// </summary>
        IReadOnlyList<Snowflake> Roles { get; }

        /// <summary>
        /// Gets when the user joined the guild.
        /// </summary>
        DateTimeOffset JoinedAt { get; }

        /// <summary>
        /// Gets when the user started boosting the guild.
        /// </summary>
        Optional<DateTimeOffset?> PremiumSince { get; }

        /// <summary>
        /// Gets a value indicating whether the user is deafened in voice channels.
        /// </summary>
        bool IsDeafened { get; }

        /// <summary>
        /// Gets a value indicating whether the user is muted in voice channels.
        /// </summary>
        bool IsMuted { get; }
    }
}
