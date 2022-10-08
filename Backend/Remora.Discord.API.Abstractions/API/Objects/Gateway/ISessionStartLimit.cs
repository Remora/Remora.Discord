//
//  ISessionStartLimit.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about session start limits.
/// </summary>
[PublicAPI]
public interface ISessionStartLimit
{
    /// <summary>
    /// Gets the total number of session starts the user is allowed.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the remaining number of session starts the user is allowed.
    /// </summary>
    int Remaining { get; }

    /// <summary>
    /// Gets the time (in milliseconds) after which the limit resets.
    /// </summary>
    TimeSpan ResetAfter { get; }

    /// <summary>
    /// Gets the maximum number of concurrent identify messages that may be sent if the bot is sharded.
    ///
    /// The logic behind this value is slightly complex. Given a maximum concurrency of 16 and 16 shards, all 16
    /// shards may identify concurrently. However, given a maximum concurrency of 16 and 32 shards, the batching
    /// properties of this value come into play. Concurrently identifying shards 0 through 15 would be allowed, but
    /// not shards 0 through 8 and 16 through 24.
    ///
    /// Effectively, only shards with different rate limit keys may concurrently identify, where the key is
    /// calculated as follows.
    /// <code>
    /// rate_limit_key = shard_id % max_concurrency
    /// </code>
    /// </summary>
    int MaxConcurrency { get; }
}
