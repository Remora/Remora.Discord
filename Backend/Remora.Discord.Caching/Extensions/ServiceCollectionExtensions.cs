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

using System;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.API;
using Remora.Discord.Caching.Responders;
using Remora.Discord.Caching.Services;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Responders;
using Remora.Rest;
using StackExchange.Redis;

namespace Remora.Discord.Caching.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds caching implementations of various API types, overriding the normally non-caching versions.
    /// </summary>
    /// <remarks>
    /// The cache uses a run-of-the-mill <see cref="IMemoryCache"/>. Cache entry options for any cached type can be
    /// configured using <see cref="IOptions{CacheSettings}"/>.
    ///
    /// When choosing a cache implementation, it should be noted that <see cref="AddDiscordCaching"/> and
    /// <see cref="AddDiscordRedisCaching"/> cannot be used together.
    /// </remarks>
    /// <param name="services">The services.</param>
    /// <returns>The services, with caching enabled.</returns>
    public static IServiceCollection AddDiscordCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.TryAddSingleton<ICacheService, CacheService>();

        services.AddCachingAPIAndResponders();

        return services;
    }

    /// <summary>
    /// Adds a redis-backed caching implementations of various API types, overriding the normally non-caching versions.
    /// </summary>
    /// <remarks>
    /// The cache uses a run-of-the-mill <see cref="IDistributedMemoryCache"/>. Cache entry options for any cached type can be
    /// configured using <see cref="IOptions{CacheSettings}"/>.
    ///
    /// When choosing a cache implementation, it should be noted that <see cref="AddDiscordCaching"/> and
    /// <see cref="AddDiscordRedisCaching"/> cannot be used together.
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

    private static IServiceCollection AddCachingAPIAndResponders(this IServiceCollection services)
    {
        services.AddOptions<CacheSettings>();

        services
            .Replace(ServiceDescriptor.Transient<IDiscordRestChannelAPI>(s => new CachingDiscordRestChannelAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestEmojiAPI>(s => new CachingDiscordRestEmojiAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestGuildAPI>(s => new CachingDiscordRestGuildAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestInteractionAPI>(s => new CachingDiscordRestInteractionAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestInviteAPI>(s => new CachingDiscordRestInviteAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestOAuth2API>(s => new CachingDiscordRestOAuth2API
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestTemplateAPI>(s => new CachingDiscordRestTemplateAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestUserAPI>(s => new CachingDiscordRestUserAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestVoiceAPI>(s => new CachingDiscordRestVoiceAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )))
            .Replace(ServiceDescriptor.Transient<IDiscordRestWebhookAPI>(s => new CachingDiscordRestWebhookAPI
            (
                s.GetRequiredService<IRestHttpClient>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
                s.GetRequiredService<CacheService>()
            )));

        services
            .AddResponder<EarlyCacheResponder>(ResponderGroup.Early)
            .AddResponder<LateCacheResponder>(ResponderGroup.Late);

        return services;
    }
}
