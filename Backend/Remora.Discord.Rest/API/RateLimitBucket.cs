//
//  RateLimitBucket.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Remora.Discord.Rest.API;

/// <summary>
/// Represents a rate limit bucket for an endpoint.
/// </summary>
[PublicAPI]
internal class RateLimitBucket
{
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Gets the total limit of the bucket.
    /// </summary>
    public int Limit { get; }

    /// <summary>
    /// Gets the remaining requests in the bucket.
    /// </summary>
    public int Remaining { get; private set; }

    /// <summary>
    /// Gets the time when the bucket resets.
    /// </summary>
    public DateTimeOffset ResetsAt { get; private set; }

    /// <summary>
    /// Gets the ID of the bucket.
    /// </summary>
    public string? ID { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitBucket"/> class.
    /// </summary>
    /// <param name="limit">The bucket limit.</param>
    /// <param name="remaining">The remaining requests.</param>
    /// <param name="resetsAt">The time when the bucket resets.</param>
    /// <param name="id">The ID of the bucket.</param>
    public RateLimitBucket(int limit, int remaining, DateTimeOffset resetsAt, string? id)
    {
        this.Limit = limit;
        this.Remaining = remaining;
        this.ResetsAt = resetsAt;
        this.ID = id;

        _semaphore = new SemaphoreSlim(1);
    }

    /// <summary>
    /// Attempts to parse a rate limit bucket from the response headers.
    /// </summary>
    /// <param name="headers">The response headers.</param>
    /// <param name="result">The resulting rate limit bucket.</param>
    /// <returns>true if a bucket was successfully parsed; otherwise, false.</returns>
    public static bool TryParse(HttpResponseHeaders headers, [NotNullWhen(true)] out RateLimitBucket? result)
    {
        result = null;

        try
        {
            if (!headers.TryGetValues(Constants.RateLimitHeaderName, out var rawLimit))
            {
                return false;
            }

            if (!int.TryParse(rawLimit.SingleOrDefault(), out var limit))
            {
                return false;
            }

            if (!headers.TryGetValues(Constants.RateLimitRemainingHeaderName, out var rawRemaining))
            {
                return false;
            }

            if (!int.TryParse(rawRemaining.SingleOrDefault(), out var remaining))
            {
                return false;
            }

            if (!headers.TryGetValues(Constants.RateLimitResetHeaderName, out var rawReset))
            {
                return false;
            }

            if (!double.TryParse(rawReset.SingleOrDefault(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var resetsAtEpoch))
            {
                return false;
            }

            var id = headers.GetValues(Constants.RateLimitBucketHeaderName).SingleOrDefault();
            var resetsAt = DateTimeOffset.UnixEpoch + TimeSpan.FromSeconds(resetsAtEpoch);

            result = new RateLimitBucket(limit, remaining, resetsAt, id);
            return true;
        }
        catch (InvalidOperationException)
        {
            // More than one element in a sequence that expected one and only one
            return false;
        }
    }

    /// <summary>
    /// Resets the rate limit bucket, setting a new reset time.
    /// </summary>
    /// <param name="nextReset">The time at which the bucket should reset again.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ResetAsync(DateTimeOffset nextReset)
    {
        if (nextReset < this.ResetsAt)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(nextReset),
                "The next reset has to be in the future from the perspective of the rate limit bucket."
            );
        }

        try
        {
            await _semaphore.WaitAsync();

            this.Remaining = this.Limit;
            this.ResetsAt = nextReset;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Attempts to take a token from the bucket.
    /// </summary>
    /// <returns>true if a token was successfully taken from the bucket; otherwise, false.</returns>
    public async Task<bool> TryTakeAsync()
    {
        try
        {
            await _semaphore.WaitAsync();

            if (this.Remaining <= 0)
            {
                // Optimistic allowance; the bucket should have reset by now
                return this.ResetsAt < DateTimeOffset.UtcNow;
            }

            this.Remaining -= 1;
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
