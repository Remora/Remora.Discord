//
//  CacheTests.cs
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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Remora.Discord.Caching.Abstractions;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Caching.Tests;

/// <summary>
/// Tests the functionality of the default in-memory cache.
/// </summary>
public class CacheTests
{
    /// <summary>
    /// A test fixture for caching.
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class Fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheTests"/> class.
    /// </summary>
    public CacheTests()
    {
    }

    /// <summary>
    /// Creates and configures the basic caching services.
    /// </summary>
    /// <param name="configureSettings">
    /// An optional function to configure the <see cref="CacheSettings"/> instance.
    /// </param>
    /// <returns>A newly created <see cref="IServiceProvider"/> with Discord caching services enabled.</returns>
    private static ServiceProvider CreateServices(Action<CacheSettings>? configureSettings = null)
    {
        return new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCaching()
            .Configure<CacheSettings>(settings => configureSettings?.Invoke(settings))
            .BuildServiceProvider(validateScopes: true);
    }

    /// <summary>
    /// Tests whether a cache entry with an absolute expiration value of <see cref="TimeSpan.Zero"/> is never cached.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task DoesNotCacheWithZeroTimeSpan()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = CreateServices(settings =>
            {
                settings.SetAbsoluteExpiration<Fixture>(TimeSpan.Zero);
            })
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        var key = CacheKey.StringKey("dummy");

        await cache.CacheAsync(key, dummyMessage);

        var result = await cache.TryGetValueAsync<Fixture>(key);
        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    /// Tests whether the caching system can store and retrieve multiple cache entries with different keys separately.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanDifferentiateByKey()
    {
        var dummyMessage = new Mock<Fixture>().Object;
        var dummyMessage2 = new Mock<Fixture>().Object;

        var services = CreateServices().CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        var key = CacheKey.StringKey("dummy");
        var key2 = CacheKey.StringKey("dummy2");

        await cache.CacheAsync(key, dummyMessage);
        await cache.CacheAsync(key2, dummyMessage2);

        var result = await cache.TryGetValueAsync<Fixture>(key);
        ResultAssert.Successful(result);

        var result2 = await cache.TryGetValueAsync<Fixture>(key2);
        ResultAssert.Successful(result);

        Assert.Same(dummyMessage, result.Entity);
        Assert.Same(dummyMessage2, result2.Entity);
    }

    /// <summary>
    /// Tests whether the caching system produces the same value that was inserted into it.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanProduceCachedValue()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = CreateServices().CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        var key = CacheKey.StringKey("dummy");

        await cache.CacheAsync(key, dummyMessage);

        var result = await cache.TryGetValueAsync<Fixture>(key);
        ResultAssert.Successful(result);

        Assert.Same(dummyMessage, result.Entity);
    }

    /// <summary>
    /// Tests whether the caching system makes overwritten values available through <see cref="CacheService.TryGetPreviousValueAsync"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanProduceCachedPreviousValue()
    {
        var dummyMessageNew = new Mock<Fixture>().Object;
        var dummyMessageOld = new Mock<Fixture>().Object;

        var services = CreateServices().CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        var key = CacheKey.StringKey("dummy");

        await cache.CacheAsync(key, dummyMessageOld);
        await cache.CacheAsync(key, dummyMessageNew);

        var resultNew = await cache.TryGetValueAsync<Fixture>(key);
        ResultAssert.Successful(resultNew);

        var resultOld = await cache.TryGetPreviousValueAsync<Fixture>(key);
        ResultAssert.Successful(resultOld);

        Assert.Same(dummyMessageNew, resultNew.Entity);
        Assert.Same(dummyMessageOld, resultOld.Entity);
    }

    /// <summary>
    /// Tests whether a cached value does not expire before the specified timeout.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ValueDoesNotExpireBeforeTimeout()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = CreateServices(settings =>
            {
                settings.SetAbsoluteExpiration<Fixture>(TimeSpan.FromMilliseconds(500));
            })
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        var key = CacheKey.StringKey("dummy");

        await cache.CacheAsync(key, dummyMessage);

        var result = await cache.TryGetValueAsync<Fixture>(key);
        ResultAssert.Successful(result);

        Assert.Same(dummyMessage, result.Entity);
    }

    /// <summary>
    /// Tests whether a cached value expires after the specified timeout.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ValueDoesExpireAfterTimeout()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = CreateServices(settings =>
            {
                settings.SetAbsoluteExpiration<Fixture>(TimeSpan.FromMilliseconds(500));
            })
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        var key = CacheKey.StringKey("dummy");

        await cache.CacheAsync(key, dummyMessage);

        await Task.Delay(1000);

        var result = await cache.TryGetValueAsync<Fixture>(key);
        ResultAssert.Unsuccessful(result);
    }
}
