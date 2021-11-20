//
//  DiscordRestGuildScheduledEventAPI.cs
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;
using Remora.Discord.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="IDiscordRestGuildScheduledEventAPI"/>
public class DiscordRestGuildScheduledEventAPI : AbstractDiscordRestAPI, IDiscordRestGuildScheduledEventAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestGuildScheduledEventAPI"/> class.
    /// </summary>
    /// <param name="discordHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The json options.</param>
    public DiscordRestGuildScheduledEventAPI
    (
        DiscordHttpClient discordHttpClient,
        IOptions<JsonSerializerOptions> jsonOptions
    )
        : base(discordHttpClient, jsonOptions)
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
        return this.DiscordHttpClient.GetAsync<IReadOnlyList<IGuildScheduledEvent>>
        (
            $"guilds/{guildID}/scheduled-events",
            b =>
            {
                if (withUserCount.HasValue)
                {
                    b.AddQueryParameter("with_user_count", withUserCount.Value.ToString());
                }
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

        return await this.DiscordHttpClient.PostAsync<IGuildScheduledEvent>
        (
            $"guilds/{guildID}/scheduled-events",
            b => b.WithJson
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
                }
            ),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IGuildScheduledEvent>> GetGuildScheduledEventAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        CancellationToken ct = default
    )
    {
        return this.DiscordHttpClient.GetAsync<IGuildScheduledEvent>
        (
            $"guilds/{guildID}/scheduled-events/{eventID}",
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IGuildScheduledEvent>> ModifyGuildScheduledEventAsync
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

        return await this.DiscordHttpClient.PatchAsync<IGuildScheduledEvent>
        (
            $"guilds/{guildID}/scheduled-events/{eventID}",
            b => b.WithJson
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
                }
            ),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result> DeleteGuildScheduledEventAsync(Snowflake guildID, Snowflake eventID, CancellationToken ct = default)
    {
        return this.DiscordHttpClient.DeleteAsync($"guilds/{guildID}/scheduled-events/{eventID}", ct: ct);
    }

    /// <inheritdoc />
    public Task<Result<IGuildScheduledEventUsersResponse>> GetGuildScheduledEventUsersAsync
    (
        Snowflake guildID,
        Snowflake eventID,
        Optional<int> limit = default,
        Optional<bool> withMember = default,
        CancellationToken ct = default
    )
    {
        return this.DiscordHttpClient.GetAsync<IGuildScheduledEventUsersResponse>
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
            },
            ct: ct
        );
    }
}
