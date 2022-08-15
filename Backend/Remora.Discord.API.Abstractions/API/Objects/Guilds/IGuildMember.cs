//
//  IGuildMember.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about a guild member.
/// </summary>
[PublicAPI]
public interface IGuildMember : IPartialGuildMember
{
    /// <summary>
    /// Gets the user this guild member represents.
    /// </summary>
    new Optional<IUser> User { get; }

    /// <summary>
    /// Gets the user's guild nickname.
    /// </summary>
    new Optional<string?> Nickname { get; }

    /// <summary>
    /// Gets the member's guild avatar hash.
    /// </summary>
    new Optional<IImageHash?> Avatar { get; }

    /// <summary>
    /// Gets the roles the user has.
    /// </summary>
    new IReadOnlyList<Snowflake> Roles { get; }

    /// <summary>
    /// Gets when the user joined the guild.
    /// </summary>
    new DateTimeOffset JoinedAt { get; }

    /// <summary>
    /// Gets when the user started boosting the guild.
    /// </summary>
    new Optional<DateTimeOffset?> PremiumSince { get; }

    /// <summary>
    /// Gets a value indicating whether the user is deafened in voice channels.
    /// </summary>
    new bool IsDeafened { get; }

    /// <summary>
    /// Gets a value indicating whether the user is muted in voice channels.
    /// </summary>
    new bool IsMuted { get; }

    /// <summary>
    /// Gets a value indicating whether the user has passed the guild membership screening requirements.
    /// </summary>
    new Optional<bool?> IsPending { get; }

    /// <summary>
    /// Gets the total permissions of the member in a channel, including overrides.
    /// </summary>
    new Optional<IDiscordPermissionSet> Permissions { get; }

    /// <summary>
    /// Gets the <see cref="DateTimeOffset"/> until the user has communication disabled.
    /// </summary>
    new Optional<DateTimeOffset?> CommunicationDisabledUntil { get; }

    /// <inheritdoc/>
    Optional<IUser> IPartialGuildMember.User => this.User;

    /// <inheritdoc/>
    Optional<string?> IPartialGuildMember.Nickname => this.Nickname;

    /// <inheritdoc />
    Optional<IImageHash?> IPartialGuildMember.Avatar => this.Avatar;

    /// <inheritdoc/>
    Optional<IReadOnlyList<Snowflake>> IPartialGuildMember.Roles => new(this.Roles);

    /// <inheritdoc/>
    Optional<DateTimeOffset> IPartialGuildMember.JoinedAt => this.JoinedAt;

    /// <inheritdoc/>
    Optional<DateTimeOffset?> IPartialGuildMember.PremiumSince => this.PremiumSince;

    /// <inheritdoc/>
    Optional<bool> IPartialGuildMember.IsDeafened => this.IsDeafened;

    /// <inheritdoc/>
    Optional<bool> IPartialGuildMember.IsMuted => this.IsMuted;

    /// <inheritdoc/>
    Optional<bool?> IPartialGuildMember.IsPending => this.IsPending;

    /// <inheritdoc/>
    Optional<IDiscordPermissionSet> IPartialGuildMember.Permissions => this.Permissions;

    /// <inheritdoc/>
    Optional<DateTimeOffset?> IPartialGuildMember.CommunicationDisabledUntil => this.CommunicationDisabledUntil;
}
