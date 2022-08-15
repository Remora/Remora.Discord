//
//  IGuildMembersChunk.cs
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
/// Represents a chunk of guild members.
/// </summary>
[PublicAPI]
public interface IGuildMembersChunk : IGatewayEvent
{
    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the members in this chunk.
    /// </summary>
    IReadOnlyList<IGuildMember> Members { get; }

    /// <summary>
    /// Gets the index of this chunk in the expected chunks of the response.
    /// </summary>
    int ChunkIndex { get; }

    /// <summary>
    /// Gets the total number of expected chunks for this response.
    /// </summary>
    int ChunkCount { get; }

    /// <summary>
    /// Gets a list of guild members that were not found.
    /// </summary>
    Optional<IReadOnlyList<Snowflake>> NotFound { get; }

    /// <summary>
    /// Gets the presences of the returned members.
    /// </summary>
    Optional<IReadOnlyList<IPresence>> Presences { get; }

    /// <summary>
    /// Gets the nonce used in the original request.
    /// </summary>
    Optional<string> Nonce { get; }
}
