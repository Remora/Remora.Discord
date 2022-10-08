//
//  IThreadMember.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a thread member.
/// </summary>
[PublicAPI]
public interface IThreadMember
{
    /// <summary>
    /// Gets the ID of the thread.
    /// </summary>
    Optional<Snowflake> ID { get; }

    /// <summary>
    /// Gets the ID of the user.
    /// </summary>
    Optional<Snowflake> UserID { get; }

    /// <summary>
    /// Gets the time the current user last joined the thread.
    /// </summary>
    DateTimeOffset JoinTimestamp { get; }

    /// <summary>
    /// Gets any user-thread settings.
    /// </summary>
    ThreadMemberFlags Flags { get; }

    /// <summary>
    /// Gets the guild member object related to the thread member.
    /// </summary>
    /// <remarks>This field is typically only set in <see cref="IThreadMembersUpdate"/> events.</remarks>
    Optional<IGuildMember> Member { get; }

    /// <summary>
    /// Gets the presence information related to the thread member.
    /// </summary>
    /// <remarks>This field is typically only set in <see cref="IThreadMembersUpdate"/> events.</remarks>
    Optional<IPartialPresence?> Presence { get; }
}
