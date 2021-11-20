//
//  IDiscordRestGuildScheduledEventAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Guild Scheduled Event API.
/// </summary>
public interface IDiscordRestGuildScheduledEventAPI
{
    /// <summary>
    /// Gets a list of scheduled events for the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="withUserCount">Whether subscribed user counts should be included.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IGuildScheduledEvent>>> ListScheduledEventsForGuildAsync
    (
        Snowflake guildID,
        Optional<bool> withUserCount = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new scheduled event in the guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild to create the event in.</param>
    /// <param name="channelID">
    /// The channel ID of the scheduled event, if it is for a stage instance or voice channel.
    /// </param>
    /// <param name="entityMetadata">The entity metadata, if any.</param>
    /// <param name="name">The name of the event.</param>
    /// <param name="privacyLevel">The privacy level of the event.</param>
    /// <param name="scheduledStartTime">The time at which the event is scheduled to start.</param>
    /// <param name="scheduledEndTime">The time at which the event is scheduled to end, if any.</param>
    /// <param name="description">The description of the event, if any.</param>
    /// <param name="entityType">The entity type of the event.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IGuildScheduledEvent>> CreateGuildScheduledEventAsync
    (
        Snowflake guildID,
        Optional<Snowflake> channelID,
        Optional<IGuildScheduledEventEntityMetadata> entityMetadata,
        string name,
        GuildScheduledEventPrivacyLevel privacyLevel,
        DateTimeOffset scheduledStartTime,
        Optional<DateTimeOffset> scheduledEndTime,
        Optional<string> description,
        GuildScheduledEventEntityType entityType,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a specific scheduled event in the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="eventID">The ID of the event.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuildScheduledEvent>> GetGuildScheduledEventAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given scheduled event.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="eventID">The ID of the event.</param>
    /// <param name="channelID">The new channel ID associated with the event.</param>
    /// <param name="entityMetadata">The new entity metadata.</param>
    /// <param name="name">The new name of the event.</param>
    /// <param name="privacyLevel">The new privacy level of the event.</param>
    /// <param name="scheduledStartTime">The new start time of the event.</param>
    /// <param name="scheduledEndTime">The new end time of the event.</param>
    /// <param name="description">The new description of the event.</param>
    /// <param name="entityType">The new entity type associated with the event.</param>
    /// <param name="status">The new status of the event.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IGuildScheduledEvent>> ModifyGuildScheduledEventAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        Optional<Snowflake> channelID = default,
        Optional<IGuildScheduledEventEntityMetadata> entityMetadata = default,
        Optional<string> name = default,
        Optional<GuildScheduledEventPrivacyLevel> privacyLevel = default,
        Optional<DateTimeOffset> scheduledStartTime = default,
        Optional<DateTimeOffset> scheduledEndTime = default,
        Optional<string> description = default,
        Optional<GuildScheduledEventEntityType> entityType = default,
        Optional<GuildScheduledEventStatus> status = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given event.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="eventID">The ID of the event.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteGuildScheduledEventAsync(Snowflake guildID, Snowflake eventID, CancellationToken ct = default);

    /// <summary>
    /// Gets a list of users subscribed to an event.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="eventID">The ID of the event.</param>
    /// <param name="limit">The maximum number of users to return.</param>
    /// <param name="withMember">Whether member information should be included.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuildScheduledEventUsersResponse>> GetGuildScheduledEventUsersAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        Optional<int> limit = default,
        Optional<bool> withMember = default,
        CancellationToken ct = default
    );
}
