//
//  IGuildMemberUpdate.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents a user being updated in the guild.
/// </summary>
[PublicAPI]
public interface IGuildMemberUpdate : IGatewayEvent
{
    /// <summary>
    /// Gets the ID of the guild the member is in.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the roles the user has.
    /// </summary>
    IReadOnlyList<Snowflake> Roles { get; }

    /// <summary>
    /// Gets the user.
    /// </summary>
    IUser User { get; }

    /// <summary>
    /// Gets the user's nickname, if they have one set.
    /// </summary>
    Optional<string?> Nickname { get; }

    /// <summary>
    /// Gets the member's guild avatar hash.
    /// </summary>
    Optional<IImageHash?> Avatar { get; }

    /// <summary>
    /// Gets the date when the user joined the guild.
    /// </summary>
    DateTimeOffset? JoinedAt { get; }

    /// <summary>
    /// Gets the date when the user started boosting the guild.
    /// </summary>
    Optional<DateTimeOffset?> PremiumSince { get; }

    /// <summary>
    /// Gets a value indicating whether the user has not yet passed the screening requirements.
    /// </summary>
    Optional<bool> IsPending { get; }

    /// <summary>
    /// Gets a value indicating whether the user is deafened in voice channels.
    /// </summary>
    Optional<bool> IsDeafened { get; }

    /// <summary>
    /// Gets a value indicating whether the user is muted in voice channels.
    /// </summary>
    Optional<bool> IsMuted { get; }

    /// <summary>
    /// Gets the <see cref="DateTimeOffset"/> until the user has communication disabled.
    /// </summary>
    Optional<DateTimeOffset?> CommunicationDisabledUntil { get; }
}
