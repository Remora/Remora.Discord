//
//  RedisCacheProvider.cs
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Remora.Discord.Caching.Abstractions;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Results;

namespace Remora.Discord.Caching.Redis.Services;

/// <summary>
/// Handles cache insert/evict operations for various types, using Redis as a backing-store.
/// </summary>
[PublicAPI]
public class RedisCacheProvider : ICacheProvider
{
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheProvider"/> class.
    /// </summary>
    /// <param name="cache">The redis cache.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    public RedisCacheProvider(IDistributedCache cache, IOptionsMonitor<JsonSerializerOptions> jsonOptions)
    {
        _cache = cache;
        _jsonOptions = jsonOptions.Get("Discord");
    }

    /// <inheritdoc cref="ICacheProvider.CacheAsync{TInstance}"/>
    /// <remarks>
    /// It should be noted that in this implementation of <see cref="ICacheProvider.CacheAsync{TInstance}"/>,
    /// there is a strong reliance on the fact that the entity being cached is trivially serializable to JSON.
    ///
    /// In the event that this is not the case, this method can be overridden in a derived class to provide
    /// a more apt transformation of incoming data.
    /// </remarks>
    public virtual async ValueTask CacheAsync<TInstance>
    (
        CacheKey key,
        TInstance instance,
        CacheEntryOptions options,
        CancellationToken ct = default
    )
        where TInstance : class
    {
        if (options.AbsoluteExpiration >= DateTimeOffset.UtcNow)
        {
            return;
        }

        var serialized = JsonSerializer.SerializeToUtf8Bytes(instance, _jsonOptions);

        await _cache.SetAsync(key.ToCanonicalString(), serialized, options, ct);
    }

    /// <inheritdoc cref="ICacheProvider.RetrieveAsync{TInstance}"/>
    /// <remarks>
    /// It should be noted that in this implementation of <see cref="ICacheProvider.RetrieveAsync{TInstance}"/>,
    /// there is a strong reliance on the fact that the entity being cached is trivially deserializable from JSON.
    ///
    /// In the event that this is not the case, this method can be overridden in a derived class to provide
    /// a more apt transformation of outgoing data.
    /// </remarks>
    public virtual async ValueTask<Result<TInstance>> RetrieveAsync<TInstance>
    (
        CacheKey key,
        CancellationToken ct = default
    )
        where TInstance : class
    {
        var keyString = key.ToCanonicalString();

        var value = await _cache.GetAsync(keyString, ct);

        if (value is null)
        {
            return new NotFoundError($"The given key \"{key}\" held no value in the cache.");
        }

        await _cache.RefreshAsync(keyString, ct);

        var deserialized = JsonSerializer.Deserialize<TInstance>(value, _jsonOptions);

        return deserialized;
    }

    /// <inheritdoc cref="ICacheProvider.EvictAsync" />
    public async ValueTask<Result> EvictAsync(CacheKey key, CancellationToken ct = default)
    {
        var keyString = key.ToCanonicalString();

        var existingValue = await _cache.GetAsync(keyString, ct);

        if (existingValue is null)
        {
            return new NotFoundError($"The given key \"{key}\" held no value in the cache.");
        }

        await _cache.RemoveAsync(keyString, ct);

        return Result.FromSuccess();
    }

    /// <inheritdoc cref="ICacheProvider.EvictAsync{TInstance}"/>
    /// <remarks>
    /// It should be noted that in this implementation of <see cref="ICacheProvider.EvictAsync{TInstance}"/>,
    /// there is a strong reliance on the fact that the entity being cached is trivially deserializable from JSON.
    ///
    /// In the event that this is not the case, this method can be overridden in a derived class to provide
    /// a more apt transformation of evicted data.
    /// </remarks>
    public virtual async ValueTask<Result<TInstance>> EvictAsync<TInstance>
    (
        CacheKey key,
        CancellationToken ct = default
    )
        where TInstance : class
    {
        var keyString = key.ToCanonicalString();

        var existingValue = await _cache.GetAsync(keyString, ct);

        if (existingValue is null)
        {
            return new NotFoundError($"The given key \"{key}\" held no value in the cache.");
        }

        await _cache.RemoveAsync(keyString, ct);

        var deserialized = JsonSerializer.Deserialize<TInstance>(existingValue, _jsonOptions);

        return deserialized;
    }
}
