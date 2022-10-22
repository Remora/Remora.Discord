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
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Caching.Tests;

/// <summary>
/// Tests injection of various command contexts.
/// </summary>
public class CacheTests
{
    /// <summary>
    /// A test fixture for caching.
    /// </summary>
    public class Fixture
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheTests"/> class.
    /// </summary>
    public CacheTests()
    {
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="ICommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task DoesNotCacheWithZeroTimeSpan()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCaching()
            .Configure<CacheSettings>(settings =>
            {
                settings.SetAbsoluteExpiration<Fixture>(TimeSpan.Zero);
            })
            .BuildServiceProvider(true)
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        await cache.CacheAsync("dummy", dummyMessage);

        var result = await cache.TryGetValueAsync<Fixture>("dummy");
        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="ICommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanDifferentiateByKey()
    {
        var dummyMessage = new Mock<Fixture>().Object;
        var dummyMessage2 = new Mock<Fixture>().Object;

        var services = new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCaching()
            .BuildServiceProvider(true)
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        await cache.CacheAsync("dummy", dummyMessage);
        await cache.CacheAsync("dummy2", dummyMessage2);

        var result = await cache.TryGetValueAsync<Fixture>("dummy");
        ResultAssert.Successful(result);

        var result2 = await cache.TryGetValueAsync<Fixture>("dummy2");
        ResultAssert.Successful(result);

        Assert.Same(dummyMessage, result.Entity);
        Assert.Same(dummyMessage2, result2.Entity);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="ICommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanProduceCachedValue()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCaching()
            .BuildServiceProvider(true)
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        await cache.CacheAsync("dummy", dummyMessage);

        var result = await cache.TryGetValueAsync<Fixture>("dummy");
        ResultAssert.Successful(result);

        Assert.Same(dummyMessage, result.Entity);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="ICommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ValueDoesNotExpireBeforeTimeout()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCaching()
            .Configure<CacheSettings>(settings =>
            {
                settings.SetAbsoluteExpiration<Fixture>(TimeSpan.FromMilliseconds(500));
            })
            .BuildServiceProvider(true)
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        await cache.CacheAsync("dummy", dummyMessage);

        var result = await cache.TryGetValueAsync<Fixture>("dummy");
        ResultAssert.Successful(result);

        Assert.Same(dummyMessage, result.Entity);
    }

    /// <summary>
    /// Tests whether a command in a group that requires an <see cref="ICommandContext"/> can be executed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ValueDoesExpireAfterTimeout()
    {
        var dummyMessage = new Mock<Fixture>().Object;

        var services = new ServiceCollection()
            .AddDiscordRest(_ => ("dummy", DiscordTokenType.Bot))
            .AddDiscordCaching()
            .Configure<CacheSettings>(settings =>
            {
                settings.SetAbsoluteExpiration<Fixture>(TimeSpan.FromMilliseconds(500));
            })
            .BuildServiceProvider(true)
            .CreateScope().ServiceProvider;

        var cache = services.GetRequiredService<CacheService>();

        await cache.CacheAsync("dummy", dummyMessage);

        await Task.Delay(1000);

        var result = await cache.TryGetValueAsync<Fixture>("dummy");
        ResultAssert.Unsuccessful(result);
    }
}
