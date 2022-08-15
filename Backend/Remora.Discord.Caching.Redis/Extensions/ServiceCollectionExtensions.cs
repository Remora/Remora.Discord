//
//  ServiceCollectionExtensions.cs
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
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Caching.Redis.Services;
using StackExchange.Redis;

namespace Remora.Discord.Caching.Redis.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a redis-backed caching implementations of various API types, overriding the normally non-caching versions.
    /// </summary>
    /// <remarks>
    /// The cache uses a run-of-the-mill <see cref="IDistributedCache"/>. Cache entry options for any cached type can be
    /// configured using <see cref="IOptions{TOptions}"/>.
    ///
    /// When choosing a cache implementation, it should be noted that choosing this will override the backing store for
    /// caching REST clients and responders.
    ///
    /// It is also very important to know that the stock implementation of <see cref="ICacheProvider"/> that this method
    /// adds uses JSON to store values for convenience and consistency with Remora's entities. If JSON is not a
    /// desirable format, the caching methods in <see cref="RedisCacheProvider"/> can be provided, or a custom
    /// implementation of <see cref="ICacheProvider"/> can be added to the container.
    ///
    /// If using a custom cache provider, it is necessary to add it after calling this method, or it will be overridden
    /// by the provider implementation this method adds.
    /// </remarks>
    /// <param name="services">The services.</param>
    /// <param name="configureRedisAction">An action to configure the redis cache. If none is specified, a
    /// default connection of localhost:6379 will be used.</param>
    /// <returns>The services, with caching enabled.</returns>
    public static IServiceCollection AddDiscordRedisCaching
    (
        this IServiceCollection services,
        Action<RedisCacheOptions>? configureRedisAction = null
    )
    {
        configureRedisAction ??= s => s.ConfigurationOptions = new ConfigurationOptions
        {
            EndPoints = { { "localhost", 6379 } }
        };

        services.AddDiscordCaching();
        services.AddStackExchangeRedisCache(configureRedisAction);

        services.TryAddSingleton<RedisCacheProvider>();
        services.AddSingleton<ICacheProvider>(s => s.GetRequiredService<RedisCacheProvider>());
        return services;
    }
}
