//
//  CacheSettings.cs
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
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Remora.Discord.Caching.Services;

/// <summary>
/// Holds various settings for individual cache objects.
/// </summary>
[PublicAPI]
public class CacheSettings
{
    /// <summary>
    /// Holds absolute cache expiration values for various types.
    /// </summary>
    private readonly Dictionary<Type, TimeSpan?> _absoluteCacheExpirations = new();

    /// <summary>
    /// Holds sliding cache expiration values for various types.
    /// </summary>
    private readonly Dictionary<Type, TimeSpan?> _slidingCacheExpirations = new();

    /// <summary>
    /// Holds absolute cache expiration values for various types when they have been evicted from the primary cache.
    /// </summary>
    private readonly Dictionary<Type, TimeSpan?> _absoluteEvictionCacheExpirations = new();

    /// <summary>
    /// Holds sliding cache expiration values for various types when they have been evicted from the primary cache.
    /// </summary>
    private readonly Dictionary<Type, TimeSpan?> _slidingEvictionCacheExpirations = new();

    /// <summary>
    /// Holds the default absolute expiration value.
    /// </summary>
    private TimeSpan? _defaultAbsoluteExpiration = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Holds the default sliding expiration value.
    /// </summary>
    private TimeSpan? _defaultSlidingExpiration = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Holds the default absolute expiration value when they have been evicted from the primary cache.
    /// </summary>
    private TimeSpan? _defaultEvictionAbsoluteExpiration = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Holds the default sliding expiration value when they have been evicted from the primary cache.
    /// </summary>
    private TimeSpan? _defaultEvictionSlidingExpiration = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Sets the default absolute expiration value for types.
    /// </summary>
    /// <param name="defaultAbsoluteExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultAbsoluteExpiration(TimeSpan? defaultAbsoluteExpiration)
    {
        _defaultAbsoluteExpiration = defaultAbsoluteExpiration;
        return this;
    }

    /// <summary>
    /// Sets the default sliding expiration value for types.
    /// </summary>
    /// <param name="defaultSlidingExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultSlidingExpiration(TimeSpan? defaultSlidingExpiration)
    {
        _defaultSlidingExpiration = defaultSlidingExpiration;
        return this;
    }

    /// <summary>
    /// Sets the default absolute expiration value for types when they have been evicted from the primary cache.
    /// </summary>
    /// <param name="defaultAbsoluteExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultEvictionAbsoluteExpiration(TimeSpan? defaultAbsoluteExpiration)
    {
        _defaultAbsoluteExpiration = defaultAbsoluteExpiration;
        return this;
    }

    /// <summary>
    /// Sets the default sliding expiration value for types when they have been evicted from the primary cache.
    /// </summary>
    /// <param name="defaultSlidingExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultEvictionSlidingExpiration(TimeSpan? defaultSlidingExpiration)
    {
        _defaultSlidingExpiration = defaultSlidingExpiration;
        return this;
    }

    /// <summary>
    /// Sets the absolute cache expiration for the given type.
    /// </summary>
    /// <remarks>
    /// This method also sets the expiration time for evicted values to the same value, provided no other expiration
    /// time has already been set.
    /// </remarks>
    /// <param name="absoluteExpiration">
    /// The absolute expiration value. If the value is null, cached values will be kept indefinitely.
    /// </param>
    /// <typeparam name="TCachedType">The cached type.</typeparam>
    /// <returns>The settings.</returns>
    public CacheSettings SetAbsoluteExpiration<TCachedType>(TimeSpan? absoluteExpiration)
    {
        _absoluteCacheExpirations[typeof(TCachedType)] = absoluteExpiration;
        _absoluteEvictionCacheExpirations.TryAdd(typeof(TCachedType), absoluteExpiration);

        return this;
    }

    /// <summary>
    /// Sets the sliding cache expiration for the given type.
    /// </summary>
    /// <remarks>
    /// This method also sets the expiration time for evicted values to the same value, provided no other expiration
    /// time has already been set.
    /// </remarks>
    /// <param name="slidingExpiration">
    /// The sliding expiration value. If the value is null, cached values will be kept indefinitely.
    /// </param>
    /// <typeparam name="TCachedType">The cached type.</typeparam>
    /// <returns>The settings.</returns>
    public CacheSettings SetSlidingExpiration<TCachedType>(TimeSpan? slidingExpiration)
    {
        _slidingCacheExpirations[typeof(TCachedType)] = slidingExpiration;
        _slidingEvictionCacheExpirations.TryAdd(typeof(TCachedType), slidingExpiration);
        return this;
    }

    /// <summary>
    /// Sets the absolute cache expiration for the given type when it has been evicted from the primary cache.
    /// </summary>
    /// <param name="absoluteExpiration">
    /// The absolute expiration value. If the value is null, cached values will be kept indefinitely.
    /// </param>
    /// <typeparam name="TCachedType">The cached type.</typeparam>
    /// <returns>The settings.</returns>
    public CacheSettings SetEvictionAbsoluteExpiration<TCachedType>(TimeSpan? absoluteExpiration)
    {
        _absoluteEvictionCacheExpirations[typeof(TCachedType)] = absoluteExpiration;
        return this;
    }

    /// <summary>
    /// Sets the sliding cache expiration for the given type when it has been evicted from the primary cache.
    /// </summary>
    /// <param name="slidingExpiration">
    /// The sliding expiration value. If the value is null, cached values will be kept indefinitely.
    /// </param>
    /// <typeparam name="TCachedType">The cached type.</typeparam>
    /// <returns>The settings.</returns>
    public CacheSettings SetEvictionSlidingExpiration<TCachedType>(TimeSpan? slidingExpiration)
    {
        _slidingEvictionCacheExpirations[typeof(TCachedType)] = slidingExpiration;
        return this;
    }

    /// <summary>
    /// Gets the absolute expiration time in the cache for the given type, or a default value if one does not exist.
    /// </summary>
    /// <param name="defaultExpiration">The default expiration. Defaults to 30 seconds.</param>
    /// <typeparam name="T">The cached type.</typeparam>
    /// <returns>The absolute expiration time.</returns>
    public TimeSpan? GetAbsoluteExpirationOrDefault<T>(TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultAbsoluteExpiration;
        return GetAbsoluteExpirationOrDefault(typeof(T), defaultExpiration);
    }

    /// <summary>
    /// Gets the absolute expiration time in the cache for the given type, or a default value if one does not exist.
    /// </summary>
    /// <param name="cachedType">The cached type.</param>
    /// <param name="defaultExpiration">The default expiration. Defaults to 30 seconds.</param>
    /// <returns>The absolute expiration time.</returns>
    public TimeSpan? GetAbsoluteExpirationOrDefault(Type cachedType, TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultAbsoluteExpiration;
        return _absoluteCacheExpirations.TryGetValue(cachedType, out var absoluteExpiration)
            ? absoluteExpiration
            : defaultExpiration;
    }

    /// <summary>
    /// Gets the absolute expiration time in the cache for the given type when it has been evicted from the primary
    /// cache, or a default value if one does not exist.
    /// </summary>
    /// <param name="defaultExpiration">The default expiration. Defaults to 30 seconds.</param>
    /// <typeparam name="T">The cached type.</typeparam>
    /// <returns>The absolute expiration time.</returns>
    public TimeSpan? GetEvictionAbsoluteExpirationOrDefault<T>(TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultEvictionAbsoluteExpiration;
        return GetEvictionAbsoluteExpirationOrDefault(typeof(T), defaultExpiration);
    }

    /// <summary>
    /// Gets the absolute expiration time in the cache for the given type when it has been evicted from the primary
    /// cache, or a default value if one does not exist.
    /// </summary>
    /// <param name="cachedType">The cached type.</param>
    /// <param name="defaultExpiration">The default expiration. Defaults to 30 seconds.</param>
    /// <returns>The absolute expiration time.</returns>
    public TimeSpan? GetEvictionAbsoluteExpirationOrDefault(Type cachedType, TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultEvictionAbsoluteExpiration;
        return _absoluteEvictionCacheExpirations.TryGetValue(cachedType, out var absoluteExpiration)
            ? absoluteExpiration
            : defaultExpiration;
    }

    /// <summary>
    /// Gets the sliding expiration time in the cache for the given type, or a default value if one does not exist.
    /// </summary>
    /// <param name="defaultExpiration">The default expiration. Defaults to 10 seconds.</param>
    /// <typeparam name="T">The cached type.</typeparam>
    /// <returns>The sliding expiration time.</returns>
    public TimeSpan? GetSlidingExpirationOrDefault<T>(TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultSlidingExpiration;
        return GetSlidingExpirationOrDefault(typeof(T), defaultExpiration);
    }

    /// <summary>
    /// Gets the sliding expiration time in the cache for the given type, or a default value if one does not exist.
    /// </summary>
    /// <param name="cachedType">The cached type.</param>
    /// <param name="defaultExpiration">The default expiration. Defaults to 10 seconds.</param>
    /// <returns>The sliding expiration time.</returns>
    public TimeSpan? GetSlidingExpirationOrDefault(Type cachedType, TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultSlidingExpiration;
        return _slidingCacheExpirations.TryGetValue(cachedType, out var slidingExpiration)
            ? slidingExpiration
            : defaultExpiration;
    }

    /// <summary>
    /// Gets the sliding expiration time in the cache for the given type when it has been evicted from the primary
    /// cache, or a default value if one does not exist.
    /// </summary>
    /// <param name="defaultExpiration">The default expiration. Defaults to 10 seconds.</param>
    /// <typeparam name="T">The cached type.</typeparam>
    /// <returns>The sliding expiration time.</returns>
    public TimeSpan? GetEvictionSlidingExpirationOrDefault<T>(TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultEvictionSlidingExpiration;
        return GetEvictionSlidingExpirationOrDefault(typeof(T), defaultExpiration);
    }

    /// <summary>
    /// Gets the sliding expiration time in the cache for the given type when it has been evicted from the primary
    /// cache, or a default value if one does not exist.
    /// </summary>
    /// <param name="cachedType">The cached type.</param>
    /// <param name="defaultExpiration">The default expiration. Defaults to 10 seconds.</param>
    /// <returns>The sliding expiration time.</returns>
    public TimeSpan? GetEvictionSlidingExpirationOrDefault(Type cachedType, TimeSpan? defaultExpiration = null)
    {
        defaultExpiration ??= _defaultEvictionSlidingExpiration;
        return _slidingEvictionCacheExpirations.TryGetValue(cachedType, out var slidingExpiration)
            ? slidingExpiration
            : defaultExpiration;
    }

    /// <summary>
    /// Gets a set of cache options, with expirations relative to now.
    /// </summary>
    /// <typeparam name="T">The cache entry type.</typeparam>
    /// <returns>The entry options.</returns>
    public MemoryCacheEntryOptions GetEntryOptions<T>()
    {
        var cacheOptions = new MemoryCacheEntryOptions();

        var absoluteExpiration = GetAbsoluteExpirationOrDefault<T>();
        if (absoluteExpiration is not null)
        {
            cacheOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
        }

        var slidingExpiration = GetSlidingExpirationOrDefault<T>();
        if (slidingExpiration is not null)
        {
            cacheOptions.SetSlidingExpiration(slidingExpiration.Value);
        }

        return cacheOptions;
    }

    /// <summary>
    /// Gets a set of cache options for an evicted value, with expirations relative to now.
    /// </summary>
    /// <typeparam name="T">The cache entry type.</typeparam>
    /// <returns>The entry options.</returns>
    public MemoryCacheEntryOptions GetEvictionEntryOptions<T>()
    {
        var cacheOptions = new MemoryCacheEntryOptions();

        var absoluteExpiration = GetEvictionAbsoluteExpirationOrDefault<T>();
        if (absoluteExpiration is not null)
        {
            cacheOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
        }

        var slidingExpiration = GetEvictionSlidingExpirationOrDefault<T>();
        if (slidingExpiration is not null)
        {
            cacheOptions.SetSlidingExpiration(slidingExpiration.Value);
        }

        return cacheOptions;
    }

    /// <summary>
    /// Gets a set of distributed cache options, with expirations relative to now.
    /// </summary>
    /// <typeparam name="T">The cache entry type.</typeparam>
    /// <returns>The entry options.</returns>
    public DistributedCacheEntryOptions GetDistributedEntryOptions<T>()
    {
        var cacheOptions = new DistributedCacheEntryOptions();

        var absoluteExpiration = GetAbsoluteExpirationOrDefault<T>();
        if (absoluteExpiration is not null)
        {
            cacheOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
        }

        var slidingExpiration = GetSlidingExpirationOrDefault<T>();
        if (slidingExpiration is not null && absoluteExpiration is not null)
        {
            cacheOptions.SetSlidingExpiration(slidingExpiration.Value);
        }

        return cacheOptions;
    }

    /// <summary>
    /// Gets a set of distributed cache options for an evicted value, with expirations relative to now.
    /// </summary>
    /// <typeparam name="T">The cache entry type.</typeparam>
    /// <returns>The entry options.</returns>
    public DistributedCacheEntryOptions GetEvictionDistributedEntryOptions<T>()
    {
        var cacheOptions = new DistributedCacheEntryOptions();

        var absoluteExpiration = GetEvictionAbsoluteExpirationOrDefault<T>();
        if (absoluteExpiration is not null)
        {
            cacheOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
        }

        var slidingExpiration = GetEvictionSlidingExpirationOrDefault<T>();
        if (slidingExpiration is not null && absoluteExpiration is not null)
        {
            cacheOptions.SetSlidingExpiration(slidingExpiration.Value);
        }

        return cacheOptions;
    }

    /// <summary>
    /// Gets a set of distributed cache options, with expirations relative to now.
    /// </summary>
    /// <typeparam name="T">The cache entry type.</typeparam>
    /// <returns>The entry options.</returns>
    [Obsolete($"Use {nameof(GetDistributedEntryOptions)} instead.")]
    public DistributedCacheEntryOptions GetRedisEntryOptions<T>() => GetDistributedEntryOptions<T>();
}
