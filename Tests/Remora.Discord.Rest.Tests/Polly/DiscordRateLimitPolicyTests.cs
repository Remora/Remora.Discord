//
//  DiscordRateLimitPolicyTests.cs
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

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Polly;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Caching;
using Remora.Discord.Rest.Polly;
using Xunit;

namespace Remora.Discord.Rest.Tests.Polly;

/// <summary>
/// Tests the <see cref="DiscordRateLimitPolicy" /> class.
/// </summary>
public class DiscordRateLimitPolicyTests
{
    private static ICacheProvider CreateCacheProvider()
    {
        var options = Options.Create(new MemoryCacheOptions());
        var cache = new MemoryCache(options);
        return new MemoryCacheProvider(cache);
    }

    private static DiscordRateLimitPolicy CreatePolicy()
    {
        return DiscordRateLimitPolicy.Create();
    }

    private static Context CreateContext
    (
        string endpoint,
        ICacheProvider cache,
        bool isExemptFromGlobalLimits,
        string? token = default
    )
    {
        var context = new Context
        {
            { "endpoint", endpoint },
            { "cache", cache },
            { "exempt-from-global-rate-limits", isExemptFromGlobalLimits }
        };

        if (token is not null)
        {
            context.Add("token", token);
        }

        return context;
    }

    /// <summary>
    /// Tests whether global rate limit is enforced correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task GlobalRateLimitEnforcedCorrectly()
    {
        var policy = CreatePolicy();
        var cache = CreateCacheProvider();

        var contextWithToken = CreateContext("dummy", cache, false, "MySecretToken");

        var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);

        for (var i = 1; i <= Constants.GlobalRateLimit + 1; ++i)
        {
            var policyResponse = await policy.ExecuteAsync(_ => Task.FromResult(apiResponse), contextWithToken);
            Assert.Equal(i <= Constants.GlobalRateLimit ? HttpStatusCode.OK : HttpStatusCode.TooManyRequests, policyResponse.StatusCode);
        }
    }

    /// <summary>
    /// Tests whether global rate limit per token is enforced correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task GlobalRateLimitPerTokenEnforcedCorrectly()
    {
        var policy = CreatePolicy();
        var cache = CreateCacheProvider();

        var contextWithoutToken = CreateContext("dummy", cache, false);
        var contextWithToken = CreateContext("dummy", cache, false, "MySecretToken");

        var apiResponse = new HttpResponseMessage(HttpStatusCode.OK);
        HttpResponseMessage policyResponse;

        // Trigger global rate limit for token
        for (var i = 1; i <= Constants.GlobalRateLimit + 1; ++i)
        {
            policyResponse = await policy.ExecuteAsync(_ => Task.FromResult(apiResponse), contextWithToken);
            Assert.Equal(i <= Constants.GlobalRateLimit ? HttpStatusCode.OK : HttpStatusCode.TooManyRequests, policyResponse.StatusCode);
        }

        // Verify that requests without token is not rate limited.
        policyResponse = await policy.ExecuteAsync(_ => Task.FromResult(apiResponse), contextWithoutToken);
        Assert.Equal(HttpStatusCode.OK, policyResponse.StatusCode);

        // Verify that requests with token is still rate limited.
        policyResponse = await policy.ExecuteAsync(_ => Task.FromResult(apiResponse), contextWithToken);
        Assert.Equal(HttpStatusCode.TooManyRequests, policyResponse.StatusCode);
    }
}
