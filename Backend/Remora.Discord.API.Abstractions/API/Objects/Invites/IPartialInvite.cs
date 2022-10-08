//
//  IPartialInvite.cs
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
/// Represents a partial invite.
/// </summary>
[PublicAPI]
public interface IPartialInvite
{
    /// <inheritdoc cref="IInvite.Code" />
    Optional<string> Code { get; }

    /// <inheritdoc cref="IInvite.Guild" />
    Optional<IPartialGuild> Guild { get; }

    /// <inheritdoc cref="IInvite.Channel" />
    Optional<IPartialChannel?> Channel { get; }

    /// <inheritdoc cref="IInvite.Inviter" />
    Optional<IUser> Inviter { get; }

    /// <inheritdoc cref="IInvite.TargetType" />
    Optional<InviteTarget> TargetType { get; }

    /// <inheritdoc cref="IInvite.TargetUser" />
    Optional<IPartialUser> TargetUser { get; }

    /// <inheritdoc cref="IInvite.TargetApplication" />
    Optional<IPartialApplication> TargetApplication { get; }

    /// <inheritdoc cref="IInvite.ApproximatePresenceCount" />
    Optional<int> ApproximatePresenceCount { get; }

    /// <inheritdoc cref="IInvite.ApproximateMemberCount" />
    Optional<int> ApproximateMemberCount { get; }

    /// <inheritdoc cref="IInvite.ExpiresAt" />
    Optional<DateTimeOffset?> ExpiresAt { get; }

    /// <inheritdoc cref="IInvite.GuildScheduledEvent" />
    Optional<IGuildScheduledEvent> GuildScheduledEvent { get; }
}
