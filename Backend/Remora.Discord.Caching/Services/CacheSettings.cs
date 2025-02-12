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

namespace Remora.Discord.Caching.Services;

/// <summary>
/// Holds various settings for individual cache objects.
/// </summary>
/// <remarks>
/// Cache services should use <see cref="ImmutableCacheSettings"/> instead.
/// </remarks>
[PublicAPI]
public class CacheSettings
{
    private readonly Dictionary<Type, TimeSpan?> _absoluteCacheExpirations = new();
    private readonly Dictionary<Type, TimeSpan?> _slidingCacheExpirations = new();
    private readonly Dictionary<Type, TimeSpan?> _absoluteEvictionCacheExpirations = new();
    private readonly Dictionary<Type, TimeSpan?> _slidingEvictionCacheExpirations = new();
    private readonly HashSet<Type> _configuredTypes = new();
    private readonly HashSet<Type> _configuredEvictionTypes = new();

    /// <summary>
    /// Gets the absolute cache expiration values for various types.
    /// </summary>
    internal IReadOnlyDictionary<Type, TimeSpan?> AbsoluteCacheExpirations => _absoluteCacheExpirations;

    /// <summary>
    /// Gets the sliding cache expiration values for various types.
    /// </summary>
    internal IReadOnlyDictionary<Type, TimeSpan?> SlidingCacheExpirations => _slidingCacheExpirations;

    /// <summary>
    /// Gets the absolute cache expiration values for various types when they have been evicted from the primary cache.
    /// </summary>
    internal IReadOnlyDictionary<Type, TimeSpan?> AbsoluteEvictionCacheExpirations => _absoluteEvictionCacheExpirations;

    /// <summary>
    /// Gets the sliding cache expiration values for various types when they have been evicted from the primary cache.
    /// </summary>
    internal IReadOnlyDictionary<Type, TimeSpan?> SlidingEvictionCacheExpirations => _slidingEvictionCacheExpirations;

    /// <summary>
    /// Gets the default absolute expiration value.
    /// </summary>
    internal TimeSpan? DefaultAbsoluteExpiration { get; private set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets the default sliding expiration value.
    /// </summary>
    internal TimeSpan? DefaultSlidingExpiration { get; private set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets the default absolute expiration value when they have been evicted from the primary cache.
    /// </summary>
    internal TimeSpan? DefaultEvictionAbsoluteExpiration { get; private set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets the default sliding expiration value when they have been evicted from the primary cache.
    /// </summary>
    internal TimeSpan? DefaultEvictionSlidingExpiration { get; private set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets a set of types with custom configured expirations.
    /// </summary>
    internal IReadOnlyCollection<Type> ConfiguredTypes => _configuredTypes;

    /// <summary>
    /// Gets a set of types with custom configured expirations for when they have been evicted from the primary cache.
    /// </summary>
    internal IReadOnlyCollection<Type> ConfiguredEvictionTypes => _configuredEvictionTypes;

    /// <summary>
    /// Sets the default absolute expiration value for types.
    /// </summary>
    /// <param name="defaultAbsoluteExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultAbsoluteExpiration(TimeSpan? defaultAbsoluteExpiration)
    {
        this.DefaultAbsoluteExpiration = defaultAbsoluteExpiration;

        return this;
    }

    /// <summary>
    /// Sets the default sliding expiration value for types.
    /// </summary>
    /// <param name="defaultSlidingExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultSlidingExpiration(TimeSpan? defaultSlidingExpiration)
    {
        this.DefaultSlidingExpiration = defaultSlidingExpiration;

        return this;
    }

    /// <summary>
    /// Sets the default absolute expiration value for types when they have been evicted from the primary cache.
    /// </summary>
    /// <param name="defaultAbsoluteExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultEvictionAbsoluteExpiration(TimeSpan? defaultAbsoluteExpiration)
    {
        this.DefaultAbsoluteExpiration = defaultAbsoluteExpiration;

        return this;
    }

    /// <summary>
    /// Sets the default sliding expiration value for types when they have been evicted from the primary cache.
    /// </summary>
    /// <param name="defaultSlidingExpiration">The default value.</param>
    /// <returns>The settings.</returns>
    public CacheSettings SetDefaultEvictionSlidingExpiration(TimeSpan? defaultSlidingExpiration)
    {
        this.DefaultSlidingExpiration = defaultSlidingExpiration;

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
        _configuredTypes.Add(typeof(TCachedType));
        _configuredEvictionTypes.Add(typeof(TCachedType));
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
        _configuredTypes.Add(typeof(TCachedType));
        _configuredEvictionTypes.Add(typeof(TCachedType));
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
        _configuredEvictionTypes.Add(typeof(TCachedType));
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
        _configuredEvictionTypes.Add(typeof(TCachedType));
        _slidingEvictionCacheExpirations[typeof(TCachedType)] = slidingExpiration;

        return this;
    }
}
