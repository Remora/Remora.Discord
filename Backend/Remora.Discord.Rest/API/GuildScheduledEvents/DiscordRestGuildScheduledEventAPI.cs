//
//  DiscordRestGuildScheduledEventAPI.cs
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
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Rest.Utility;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="IDiscordRestGuildScheduledEventAPI"/>
public class DiscordRestGuildScheduledEventAPI : AbstractDiscordRestAPI, IDiscordRestGuildScheduledEventAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestGuildScheduledEventAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestGuildScheduledEventAPI(IRestHttpClient restHttpClient, JsonSerializerOptions jsonOptions, ICacheProvider rateLimitCache)
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IGuildScheduledEvent>>> ListScheduledEventsForGuildAsync
    (
        Snowflake guildID,
        Optional<bool> withUserCount = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IGuildScheduledEvent>>
        (
            $"guilds/{guildID}/scheduled-events",
            b =>
            {
                if (withUserCount.HasValue)
                {
                    b.AddQueryParameter("with_user_count", withUserCount.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IGuildScheduledEvent>> CreateGuildScheduledEventAsync
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
        Optional<Stream> image,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        if (name.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 100 characters in length."
            );
        }

        if (description.HasValue && description.Value.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The description must be between 1 and 100 characters in length."
            );
        }

        if
        (
            entityMetadata.HasValue &&
            entityMetadata.Value.Location.HasValue &&
            entityMetadata.Value.Location.Value.Length is < 1 or > 100
        )
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The location metadata must be between 1 and 100 characters in length."
            );
        }

        var packImage = await ImagePacker.PackImageAsync(image!, ct);
        if (!packImage.IsSuccess)
        {
            return Result<IGuildScheduledEvent>.FromError(packImage);
        }

        Optional<string> imageData = packImage.Entity!;

        return await this.RestHttpClient.PostAsync<IGuildScheduledEvent>
        (
            $"guilds/{guildID}/scheduled-events",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("channel_id", channelID, this.JsonOptions);
                        json.Write("entity_metadata", entityMetadata, this.JsonOptions);
                        json.Write("name", name, this.JsonOptions);
                        json.Write("privacy_level", privacyLevel, this.JsonOptions);
                        json.Write("scheduled_start_time", scheduledStartTime, this.JsonOptions);
                        json.Write("scheduled_end_time", scheduledEndTime, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write("entity_type", entityType, this.JsonOptions);
                        json.Write("image", imageData, this.JsonOptions);
                    }
                );

                b.AddAuditLogReason(reason);
                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IGuildScheduledEvent>> GetGuildScheduledEventAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        Optional<bool> withUserCount = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuildScheduledEvent>
        (
            $"guilds/{guildID}/scheduled-events/{eventID}",
            b =>
            {
                if (withUserCount.HasValue)
                {
                    b.AddQueryParameter("with_user_count", withUserCount.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IGuildScheduledEvent>> ModifyGuildScheduledEventAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        Optional<Snowflake> channelID = default,
        Optional<IGuildScheduledEventEntityMetadata?> entityMetadata = default,
        Optional<string> name = default,
        Optional<GuildScheduledEventPrivacyLevel> privacyLevel = default,
        Optional<DateTimeOffset> scheduledStartTime = default,
        Optional<DateTimeOffset> scheduledEndTime = default,
        Optional<string?> description = default,
        Optional<GuildScheduledEventEntityType> entityType = default,
        Optional<GuildScheduledEventStatus> status = default,
        Optional<Stream> image = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        if (name.HasValue && name.Value.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 100 characters in length."
            );
        }

        if (description.IsDefined(out var realDescription) && realDescription.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The description must be between 1 and 100 characters in length."
            );
        }

        if
        (
            entityMetadata.IsDefined(out var metadata) &&
            metadata.Location.HasValue &&
            metadata.Location.Value.Length is < 1 or > 100
        )
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The location metadata must be between 1 and 100 characters in length."
            );
        }

        var packImage = await ImagePacker.PackImageAsync(image!, ct);
        if (!packImage.IsSuccess)
        {
            return Result<IGuildScheduledEvent>.FromError(packImage);
        }

        Optional<string> imageData = packImage.Entity!;

        return await this.RestHttpClient.PatchAsync<IGuildScheduledEvent>
        (
            $"guilds/{guildID}/scheduled-events/{eventID}",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("channel_id", channelID, this.JsonOptions);
                        json.Write("entity_metadata", entityMetadata, this.JsonOptions);
                        json.Write("name", name, this.JsonOptions);
                        json.Write("privacy_level", privacyLevel, this.JsonOptions);
                        json.Write("scheduled_start_time", scheduledStartTime, this.JsonOptions);
                        json.Write("scheduled_end_time", scheduledEndTime, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write("entity_type", entityType, this.JsonOptions);
                        json.Write("status", status, this.JsonOptions);
                        json.Write("image", imageData, this.JsonOptions);
                    }
                );

                b.AddAuditLogReason(reason);
                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result> DeleteGuildScheduledEventAsync(Snowflake guildID, Snowflake eventID, CancellationToken ct = default)
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/scheduled-events/{eventID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IGuildScheduledEventUser>>> GetGuildScheduledEventUsersAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        Optional<int> limit = default,
        Optional<bool> withMember = default,
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        CancellationToken ct = default
    )
    {
        if (limit.HasValue && limit.Value is < 1 or >= 100)
        {
            return new ArgumentOutOfRangeError(nameof(limit), "The limit must be between 1 and 100.");
        }

        return await this.RestHttpClient.GetAsync<IReadOnlyList<IGuildScheduledEventUser>>
        (
            $"guilds/{guildID}/scheduled-events/{eventID}/users",
            b =>
            {
                if (limit.HasValue)
                {
                    b.AddQueryParameter("limit", limit.Value.ToString());
                }

                if (withMember.HasValue)
                {
                    b.AddQueryParameter("with_member", withMember.Value.ToString());
                }

                if (before.HasValue)
                {
                    b.AddQueryParameter("before", before.Value.ToString());
                }

                if (after.HasValue)
                {
                    b.AddQueryParameter("after", after.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }
}
