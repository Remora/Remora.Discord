//
//  IGuildCreate.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents the creation of a guild. This event is sent in one of three scenarios:
///     1. When a user is initially connecting to lazily load and backfill information for all unavailable guilds
///        sent in the <see cref="IReady"/> event.
///     2. When a guild becomes available again to the client.
///     3. When the current user joins a guild.
/// </summary>
[PublicAPI]
public interface IGuildCreate : IGatewayEvent, IGuild
{
    /// <summary>
    /// Gets the time when the current user joined the guild.
    /// </summary>
    Optional<DateTimeOffset> JoinedAt { get; }

    /// <summary>
    /// Gets a value indicating whether this is considered a large guild.
    /// </summary>
    Optional<bool> IsLarge { get; }

    /// <summary>
    /// Gets a value indicating whether the guild is unavailable due to an outage.
    /// </summary>
    Optional<bool> IsUnavailable { get; }

    /// <summary>
    /// Gets the number of members in the guild.
    /// </summary>
    Optional<int> MemberCount { get; }

    /// <summary>
    /// Gets the states of members currently in voice channels.
    /// </summary>
    Optional<IReadOnlyList<IPartialVoiceState>> VoiceStates { get; }

    /// <summary>
    /// Gets the members in the guild.
    /// </summary>
    Optional<IReadOnlyList<IGuildMember>> Members { get; }

    /// <summary>
    /// Gets the channels in the guild.
    /// </summary>
    Optional<IReadOnlyList<IChannel>> Channels { get; }

    /// <summary>
    /// Gets the threads in the guild.
    /// </summary>
    Optional<IReadOnlyList<IChannel>> Threads { get; }

    /// <summary>
    /// Gets the presences of the members in the guild.
    /// </summary>
    Optional<IReadOnlyList<IPartialPresence>> Presences { get; }

    /// <summary>
    /// Gets the stage instances in the guild.
    /// </summary>
    Optional<IReadOnlyList<IStageInstance>> StageInstances { get; }

    /// <summary>
    /// Gets the scheduled events in the guild.
    /// </summary>
    Optional<IReadOnlyList<IGuildScheduledEvent>> GuildScheduledEvents { get; }
}
