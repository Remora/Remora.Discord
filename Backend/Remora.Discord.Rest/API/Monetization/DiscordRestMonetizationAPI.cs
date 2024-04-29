//
//  DiscordRestMonetizationAPI.cs
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
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestMonetizationAPI" />
[PublicAPI]
public class DiscordRestMonetizationAPI : AbstractDiscordRestAPI, IDiscordRestMonetizationAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestMonetizationAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestMonetizationAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IEntitlement>>> ListEntitlementsAsync
    (
        Snowflake applicationID,
        Optional<Snowflake> userID = default,
        Optional<IReadOnlyList<Snowflake>> skuIDs = default,
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        Optional<Snowflake> guildID = default,
        Optional<bool> excludeEnded = default,
        CancellationToken ct = default
    ) => this.RestHttpClient.GetAsync<IReadOnlyList<IEntitlement>>
    (
        $"applications/{applicationID}/entitlements",
        b => b
            .AddQueryParameter("user_id", userID)
            .AddQueryParameter("sku_ids", skuIDs.Map(ids => string.Join(',', ids.Select(id => id.ToString()))))
            .AddQueryParameter("before", before)
            .AddQueryParameter("after", after)
            .AddQueryParameter("limit", limit)
            .AddQueryParameter("guild_id", guildID)
            .AddQueryParameter("exclude_ended", excludeEnded)
            .WithRateLimitContext(this.RateLimitCache),
        ct: ct
    );

    /// <inheritdoc />
    public Task<Result<IPartialEntitlement>> CreateTestEntitlementAsync
    (
        Snowflake applicationID,
        Snowflake skuID,
        Snowflake ownerID,
        EntitlementOwnerType ownerType,
        CancellationToken ct = default
    ) => this.RestHttpClient.PostAsync<IPartialEntitlement>
    (
        $"applications/{applicationID}/entitlements",
        b => b
            .WithJson
            (
                json =>
                {
                    json.Write("sku_id", skuID, this.JsonOptions);
                    json.Write("owner_id", ownerID, this.JsonOptions);
                    json.Write("owner_type", (int)ownerType, this.JsonOptions);
                }
            )
            .WithRateLimitContext(this.RateLimitCache),
        ct: ct
    );

    /// <inheritdoc />
    public Task<Result> DeleteTestEntitlementAsync
    (
        Snowflake applicationID,
        Snowflake entitlementID,
        CancellationToken ct = default
    ) => this.RestHttpClient.DeleteAsync
    (
        $"applications/{applicationID}/entitlements/{entitlementID}",
        b => b.WithRateLimitContext(this.RateLimitCache),
        ct: ct
    );

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<ISKU>>> ListSKUsAsync(Snowflake applicationID, CancellationToken ct = default)
        => this.RestHttpClient.GetAsync<IReadOnlyList<ISKU>>
    (
        $"applications/{applicationID}/skus",
        b => b.WithRateLimitContext(this.RateLimitCache),
        ct: ct
    );
}
