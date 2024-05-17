//
//  DiscordRestPollAPI.cs
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
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestPollAPI" />
[PublicAPI]
public class DiscordRestPollAPI : AbstractDiscordRestAPI, IDiscordRestPollAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestPollAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestPollAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual async Task<Result<IPollAnswerVoters>> GetAnswerVotersAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        int answerID,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        if (limit is { HasValue: true, Value: > 100 or < 1 })
        {
            return new ArgumentOutOfRangeError
            (
                nameof(limit),
                "The user limit must be between 1 and 100."
            );
        }

        return await this.RestHttpClient.GetAsync<IPollAnswerVoters>
        (
            $"channels/{channelID}/polls/{messageID}/answers/{answerID}",
            b =>
            {
                if (after.TryGet(out var realAfter))
                {
                    b.AddQueryParameter("after", realAfter.ToString());
                }

                if (limit.TryGet(out var realLimit))
                {
                    b.AddQueryParameter("limit", realLimit.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IMessage>> EndPollAsync
    (
        Snowflake channelID,
        Snowflake messageID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<IMessage>
        (
            $"channels/{channelID}/polls/{messageID}/expire",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }
}
