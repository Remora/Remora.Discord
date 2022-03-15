//
//  ServiceCollectionExtensions.cs
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

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Caching.Services;
using StackExchange.Redis;

namespace Remora.Discord.Caching.Redis.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a redis-backed caching implementations of various API types, overriding the normally non-caching versions.
    /// </summary>
    /// <remarks>
    /// The cache uses a run-of-the-mill <see cref="IDistributedMemoryCache"/>. Cache entry options for any cached type can be
    /// configured using <see cref="IOptions{TOptions}"/>.
    ///
    /// When choosing a cache implementation, it should be noted that <see cref="AddDiscordCaching"/> and
    /// <see cref="AddDiscordRedisCaching"/> cannot be used together.
    ///
    /// </remarks>
    /// <param name="services">The services.</param>
    /// <param name="configureRedisAction">An action to configure the redis cache. If none is specified, a
    /// default connection of localhost:6379 will be used.</param>
    /// <returns>The services, with caching enabled.</returns>
    public static IServiceCollection AddDiscordRedisCaching(this IServiceCollection services, Action<RedisCacheOptions>? configureRedisAction = null)
    {
        configureRedisAction ??= (s) => s.ConfigurationOptions = new ConfigurationOptions
        {
            EndPoints = { { "localhost", 6379 } }
        };

        services.AddStackExchangeRedisCache(configureRedisAction);
        services.AddCachingAPIAndResponders();

        services.TryAddSingleton<ICacheService, RedisCacheService>();
        return services;
    }
}
