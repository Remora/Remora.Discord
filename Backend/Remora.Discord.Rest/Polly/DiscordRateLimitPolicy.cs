//
//  DiscordRateLimitPolicy.cs
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
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.API;

namespace Remora.Discord.Rest.Polly;

/// <summary>
/// Represents a Discord rate limiting policy.
/// </summary>
internal class DiscordRateLimitPolicy : AsyncPolicy<HttpResponseMessage>
{
    /// <summary>
    /// Maps endpoint names to their corresponding bucket IDs.
    /// </summary>
    /// <remarks>
    /// If an endpoint is not in this dictionary, it either does not use a shared bucket, or it is the first request for
    /// this endpoint.
    /// </remarks>
    private readonly ConcurrentDictionary<string, string> _endpointBuckets;

    /// <summary>
    /// Maps tokens to their corresponding bucket.
    /// </summary>
    /// <remarks>
    /// If a token is not in this dictionary, it is the first request for this token.
    /// </remarks>
    private readonly ConcurrentDictionary<string, RateLimitBucket> _globalRateLimitBuckets;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRateLimitPolicy"/> class.
    /// </summary>
    private DiscordRateLimitPolicy()
    {
        _endpointBuckets = new ConcurrentDictionary<string, string>();
        _globalRateLimitBuckets = new ConcurrentDictionary<string, RateLimitBucket>();
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> ImplementationAsync
    (
        Func<Context, CancellationToken, Task<HttpResponseMessage>> action,
        Context context,
        CancellationToken cancellationToken,
        bool continueOnCapturedContext
    )
    {
        if (!context.TryGetValue("endpoint", out var rawEndpoint) || rawEndpoint is not string endpoint)
        {
            throw new InvalidOperationException("No endpoint set.");
        }

        if (!context.TryGetValue("cache", out var rawCache) || rawCache is not ICacheProvider cache)
        {
            throw new InvalidOperationException("No cache provider set.");
        }

        var now = DateTimeOffset.UtcNow;

        // Determine whether this request is exempt from global rate limits
        var isExemptFromGlobalRateLimits = false;
        if (context.TryGetValue("exempt-from-global-rate-limits", out var rawExempt) && rawExempt is bool isExempt)
        {
            isExemptFromGlobalRateLimits = isExempt;
        }

        // First, take a token from the global limits
        if (!isExemptFromGlobalRateLimits)
        {
            if (!context.TryGetValue("token", out var rawToken) || rawToken is not string token)
            {
                token = "unauthorized";
            }

            var globalBucketIdentifier = $"global:{token}";

            if (!_globalRateLimitBuckets.TryGetValue(globalBucketIdentifier, out var globalRateLimitBucket))
            {
                globalRateLimitBucket = new RateLimitBucket
                (
                    Constants.GlobalRateLimit,
                    Constants.GlobalRateLimit,
                    DateTimeOffset.UtcNow + TimeSpan.FromSeconds(1),
                    globalBucketIdentifier
                );
                _globalRateLimitBuckets.TryAdd(globalBucketIdentifier, globalRateLimitBucket);
            }

            // Check if we need to reset the global limits
            if (globalRateLimitBucket.ResetsAt < now)
            {
                await globalRateLimitBucket.ResetAsync(now + TimeSpan.FromSeconds(1));
            }

            if (!await globalRateLimitBucket.TryTakeAsync())
            {
                var rateLimitedResponse = new HttpResponseMessage(HttpStatusCode.TooManyRequests);

                var delay = globalRateLimitBucket.ResetsAt - now;
                rateLimitedResponse.Headers.RetryAfter = new RetryConditionHeaderValue(delay);

                return rateLimitedResponse;
            }
        }

        // Then, try to take one from the local bucket
        if (!_endpointBuckets.TryGetValue(endpoint, out var bucketIdentifier))
        {
            bucketIdentifier = endpoint;
        }

        var getValue = await cache.RetrieveAsync<RateLimitBucket>(bucketIdentifier, cancellationToken);
        if (getValue.IsDefined(out var rateLimitBucket))
        {
            // We don't reset route-specific rate limits ourselves; that's the responsibility of the returned headers
            // from Discord
            if (!await rateLimitBucket.TryTakeAsync())
            {
                var rateLimitedResponse = new HttpResponseMessage(HttpStatusCode.TooManyRequests);

                var delay = rateLimitBucket.ResetsAt - now;
                rateLimitedResponse.Headers.RetryAfter = new RetryConditionHeaderValue(delay);

                return rateLimitedResponse;
            }
        }

        // The request can proceed without hitting rate limits, and we've taken a token
        var requestAction = action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);

        var response = await requestAction;
        if (!RateLimitBucket.TryParse(response.Headers, out var newLimits))
        {
            return response;
        }

        if (newLimits.ID is null)
        {
            // No shared bucket for this endpoint; clear any old shared information
            _endpointBuckets.TryRemove(endpoint, out _);

            // use the endpoint and not any mapped identifier, plus an expiration so we don't leak transient rate limits
            // over time
            await cache.CacheAsync
            (
                endpoint,
                newLimits,
                newLimits.ResetsAt + TimeSpan.FromSeconds(1),
                ct: cancellationToken
            );

            return response;
        }

        // Shared bucket
        // save the endpoint-to-id mapping
        _endpointBuckets.AddOrUpdate(endpoint, _ => newLimits.ID, (_, _) => newLimits.ID);
        await cache.CacheAsync
        (
            newLimits.ID,
            newLimits,
            newLimits.ResetsAt + TimeSpan.FromSeconds(1),
            ct: cancellationToken
        );

        if (newLimits.ID != bucketIdentifier)
        {
            // evict the old endpoint-specific or shared bucket
            await cache.EvictAsync(bucketIdentifier, cancellationToken);
        }

        return response;
    }

    /// <summary>
    /// Creates a new instance of the policy.
    /// </summary>
    /// <returns>The policy.</returns>
    public static DiscordRateLimitPolicy Create() => new();
}
