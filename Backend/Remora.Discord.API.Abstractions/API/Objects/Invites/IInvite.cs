//
//  IInvite.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an invite.
/// </summary>
[PublicAPI]
public interface IInvite : IPartialInvite
{
    /// <summary>
    /// Gets the unique invite code.
    /// </summary>
    new string Code { get; }

    /// <summary>
    /// Gets the guild this invite is for.
    /// </summary>
    new Optional<IPartialGuild> Guild { get; }

    /// <summary>
    /// Gets the channel this invite is for.
    /// </summary>
    new IPartialChannel? Channel { get; }

    /// <summary>
    /// Gets the user who created the invite.
    /// </summary>
    new Optional<IUser> Inviter { get; }

    /// <summary>
    /// Gets the type of target for this invite.
    /// </summary>
    new Optional<InviteTarget> TargetType { get; }

    /// <summary>
    /// Gets the target user for this invite.
    /// </summary>
    new Optional<IPartialUser> TargetUser { get; }

    /// <summary>
    /// Gets the embedded application this invite is for.
    /// </summary>
    new Optional<IPartialApplication> TargetApplication { get; }

    /// <summary>
    /// Gets the approximate count of online members. Only present when <see cref="TargetUser"/> is set.
    /// </summary>
    new Optional<int> ApproximatePresenceCount { get; }

    /// <summary>
    /// Gets the approximate count of total members.
    /// </summary>
    new Optional<int> ApproximateMemberCount { get; }

    /// <summary>
    /// Gets the expiration date of this invite.
    /// </summary>
    new Optional<DateTimeOffset?> ExpiresAt { get; }

    /// <summary>
    /// Gets metadata about the scheduled event the invite points to.
    /// </summary>
    new Optional<IGuildScheduledEvent> GuildScheduledEvent { get; }

    /// <inheritdoc/>
    Optional<string> IPartialInvite.Code => this.Code;

    /// <inheritdoc/>
    Optional<IPartialGuild> IPartialInvite.Guild => this.Guild;

    /// <inheritdoc/>
    Optional<IPartialChannel?> IPartialInvite.Channel => new(this.Channel);

    /// <inheritdoc/>
    Optional<IUser> IPartialInvite.Inviter => this.Inviter;

    /// <inheritdoc/>
    Optional<InviteTarget> IPartialInvite.TargetType => this.TargetType;

    /// <inheritdoc/>
    Optional<IPartialUser> IPartialInvite.TargetUser => this.TargetUser;

    /// <inheritdoc/>
    Optional<IPartialApplication> IPartialInvite.TargetApplication => this.TargetApplication;

    /// <inheritdoc/>
    Optional<int> IPartialInvite.ApproximatePresenceCount => this.ApproximatePresenceCount;

    /// <inheritdoc/>
    Optional<int> IPartialInvite.ApproximateMemberCount => this.ApproximateMemberCount;

    /// <inheritdoc/>
    Optional<DateTimeOffset?> IPartialInvite.ExpiresAt => this.ExpiresAt;

    /// <inheritdoc/>
    Optional<IGuildScheduledEvent> IPartialInvite.GuildScheduledEvent => this.GuildScheduledEvent;
}
