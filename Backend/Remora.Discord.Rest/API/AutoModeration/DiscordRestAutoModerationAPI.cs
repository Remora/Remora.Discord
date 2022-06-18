//
//  DiscordRestAutoModerationAPI.cs
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

using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestAutoModerationAPI" />
public class DiscordRestAutoModerationAPI : AbstractDiscordRestAPI, IDiscordRestAutoModerationAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestAutoModerationAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestAutoModerationAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc/>
    public Task<Result<IReadOnlyList<IAutoModerationRule>>> ListGuildRulesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IAutoModerationRule>>
        (
            $"guilds/{guildID}/auto-moderation/rules",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc/>
    public Task<Result<IAutoModerationRule>> GetGuildRuleAsync
    (
        Snowflake guildID,
        Snowflake ruleID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IAutoModerationRule>
        (
            $"guilds/{guildID}/auto-moderation/rules/{ruleID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc/>
    public Task<Result<IAutoModerationRule>> CreateGuildRuleAsync
    (
        Snowflake guildID,
        string name,
        AutoModerationEventType eventType,
        AutoModerationTriggerType triggerType,
        Optional<IAutoModerationTriggerMetadata> triggerMetadata,
        IReadOnlyList<IAutoModerationAction> actions,
        Optional<bool?> enabled = default,
        Optional<IReadOnlyList<Snowflake>?> exemptRoles = default,
        Optional<IReadOnlyList<Snowflake>?> exemptChannels = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<IAutoModerationRule>
        (
            $"guilds/{guildID}/auto-moderation/rules",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("event_type", eventType, this.JsonOptions);
                        json.Write("trigger_type", triggerType, this.JsonOptions);
                        json.Write("trigger_metadata", triggerMetadata, this.JsonOptions);
                        json.Write("actions", actions, this.JsonOptions);
                        json.Write("enabled", enabled, this.JsonOptions);
                        json.Write("exempt_roles", exemptRoles, this.JsonOptions);
                        json.Write("exempt_channels", exemptChannels, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc/>
    public Task<Result<IAutoModerationRule>> ModifyGuildRuleAsync
    (
        Snowflake guildID,
        Snowflake ruleID,
        Optional<string> name = default,
        Optional<AutoModerationEventType> eventType = default,
        Optional<AutoModerationTriggerType> triggerType = default,
        Optional<IAutoModerationTriggerMetadata> triggerMetadata = default,
        Optional<IReadOnlyList<IAutoModerationAction>> actions = default,
        Optional<bool?> enabled = default,
        Optional<IReadOnlyList<Snowflake>?> exemptRoles = default,
        Optional<IReadOnlyList<Snowflake>?> exemptChannels = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync<IAutoModerationRule>
        (
            $"guilds/{guildID}/auto-moderation/rules/{ruleID}",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("event_type", eventType, this.JsonOptions);
                        json.Write("trigger_type", triggerType, this.JsonOptions);
                        json.Write("trigger_metadata", triggerMetadata, this.JsonOptions);
                        json.Write("actions", actions, this.JsonOptions);
                        json.Write("enabled", enabled, this.JsonOptions);
                        json.Write("exempt_roles", exemptRoles, this.JsonOptions);
                        json.Write("exempt_channels", exemptChannels, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc/>
    public Task<Result> DeleteGuildRuleAsync
    (
        Snowflake guildID,
        Snowflake ruleID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/auto-moderation/rules/{ruleID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }
}
