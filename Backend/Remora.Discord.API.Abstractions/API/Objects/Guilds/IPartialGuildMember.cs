//
//  IPartialGuildMember.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents partial information about a guild member.
/// </summary>
[PublicAPI]
public interface IPartialGuildMember
{
    /// <inheritdoc cref="IGuildMember.User" />
    Optional<IUser> User { get; }

    /// <inheritdoc cref="IGuildMember.Nickname" />
    Optional<string?> Nickname { get; }

    /// <inheritdoc cref="IGuildMember.Avatar"/>
    Optional<IImageHash?> Avatar { get; }

    /// <inheritdoc cref="IGuildMember.Roles" />
    Optional<IReadOnlyList<Snowflake>> Roles { get; }

    /// <inheritdoc cref="IGuildMember.JoinedAt" />
    Optional<DateTimeOffset> JoinedAt { get; }

    /// <inheritdoc cref="IGuildMember.PremiumSince" />
    Optional<DateTimeOffset?> PremiumSince { get; }

    /// <inheritdoc cref="IGuildMember.IsDeafened" />
    Optional<bool> IsDeafened { get; }

    /// <inheritdoc cref="IGuildMember.IsMuted" />
    Optional<bool> IsMuted { get; }

    /// <inheritdoc cref="IGuildMember.IsPending" />
    Optional<bool?> IsPending { get; }

    /// <inheritdoc cref="IGuildMember.Permissions" />
    Optional<IDiscordPermissionSet> Permissions { get; }

    /// <inheritdoc cref="IGuildMember.CommunicationDisabledUntil"/>
    Optional<DateTimeOffset?> CommunicationDisabledUntil { get; }
}
