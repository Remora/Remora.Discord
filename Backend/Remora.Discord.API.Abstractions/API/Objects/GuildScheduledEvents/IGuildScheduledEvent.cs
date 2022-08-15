//
//  IGuildScheduledEvent.cs
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
/// Represents a scheduled event in a guild.
/// </summary>
[PublicAPI]
public interface IGuildScheduledEvent
{
    /// <summary>
    /// Gets the ID of the scheduled event.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the ID of the guild the event belongs to.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the ID of the channel the event will be hosted in, or <value>null</value> if the event is an external
    /// event.
    /// </summary>
    Snowflake? ChannelID { get; }

    /// <summary>
    /// Gets the ID of the user that created the scheduled event.
    /// </summary>
    Optional<Snowflake?> CreatorID { get; }

    /// <summary>
    /// Gets the name of the scheduled event.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the scheduled event, if any.
    /// </summary>
    Optional<string?> Description { get; }

    /// <summary>
    /// Gets the time the scheduled event will start.
    /// </summary>
    DateTimeOffset ScheduledStartTime { get; }

    /// <summary>
    /// Gets the time the scheduled event will end, or <value>null</value> if the event does not have a scheduled end
    /// time.
    /// </summary>
    DateTimeOffset? ScheduledEndTime { get; }

    /// <summary>
    /// Gets the privacy level of the scheduled event.
    /// </summary>
    GuildScheduledEventPrivacyLevel PrivacyLevel { get; }

    /// <summary>
    /// Gets the status of the scheduled event.
    /// </summary>
    GuildScheduledEventStatus Status { get; }

    /// <summary>
    /// Gets the type of the hosting entity associated with a scheduled event, e.g. voice channel or stage channel.
    /// </summary>
    GuildScheduledEventEntityType EntityType { get; }

    /// <summary>
    /// Gets any additional ID associated with the hosting entity.
    /// </summary>
    Snowflake? EntityID { get; }

    /// <summary>
    /// Gets the entity metadata, if any.
    /// </summary>
    IGuildScheduledEventEntityMetadata? EntityMetadata { get; }

    /// <summary>
    /// Gets the user that created the event, if any.
    /// </summary>
    Optional<IUser> Creator { get; }

    /// <summary>
    /// Gets the number of users subscribed to the scheduled event.
    /// </summary>
    Optional<int> UserCount { get; }

    /// <summary>
    /// Gets the cover image of the scheduled event.
    /// </summary>
    IImageHash? Image { get; }
}
