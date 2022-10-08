//
//  IThreadMembersUpdate.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents an update to the member list of a thread.
/// </summary>
[PublicAPI]
public interface IThreadMembersUpdate : IGatewayEvent
{
    /// <summary>
    /// Gets the ID of the thread.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the approximate member count of members in the thread. Stops counting after 50.
    /// </summary>
    int MemberCount { get; }

    /// <summary>
    /// Gets the users who were added to the thread.
    /// </summary>
    Optional<IReadOnlyList<IThreadMember>> AddedMembers { get; }

    /// <summary>
    /// Gets the IDs of the users who were removed from the thread.
    /// </summary>
    Optional<IReadOnlyList<Snowflake>> RemovedMemberIDs { get; }
}
