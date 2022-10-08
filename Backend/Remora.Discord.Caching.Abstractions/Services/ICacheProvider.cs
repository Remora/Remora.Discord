//
//  ICacheProvider.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Discord.Caching.Abstractions.Services;

/// <summary>
/// Represents an abstraction between a cache service and it's backing store.
/// </summary>
[PublicAPI]
public interface ICacheProvider
{
    /// <summary>
    /// Caches a value in the backing store.
    /// </summary>
    /// <param name="key">The key to cache the value with.</param>
    /// <param name="instance">The instance of the object to cache.</param>
    /// <param name="absoluteExpiration">The absolute expiration of the value to cache.</param>
    /// <param name="slidingExpiration">The sliding expiration of the value to cache.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="TInstance">The type to cache.</typeparam>
    /// <remarks>Absolute and sliding expirations may be handled differently (or not at all)
    /// depending on the implementation, and are simply a hint to the implementation that the
    /// value should have a pre-determined lifetime in it's backing-store.</remarks>
    /// <returns>A <see cref="ValueTask"/> representing the result of the potentially asynchronous operation.</returns>
    ValueTask CacheAsync<TInstance>
    (
        string key,
        TInstance instance,
        DateTimeOffset? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null,
        CancellationToken ct = default
    )
        where TInstance : class;

    /// <summary>
    /// Retrieves a value from the backing store.
    /// </summary>
    /// <param name="key">The key to retrieve a potential value from the backing store.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="TInstance">The type to return from the backing store, if it exists.</typeparam>
    /// <returns>A <see cref="ValueTask"/> representing the result of the potentially asynchronous action.</returns>
    ValueTask<Result<TInstance>> RetrieveAsync<TInstance>(string key, CancellationToken ct = default)
        where TInstance : class;

    /// <summary>
    /// Evicts a key from the backing store.
    /// </summary>
    /// <param name="key">The key to evict from the backing store.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the result of the potentially asynchronous action.</returns>
    ValueTask<Result> EvictAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Evicts a key from the backing store, returning its current value if it exists.
    /// </summary>
    /// <param name="key">The key to evict from the backing store.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="TInstance">The type to return from the backing store, if it exists.</typeparam>
    /// <returns>A <see cref="ValueTask"/> representing the result of the potentially asynchronous action.</returns>
    ValueTask<Result<TInstance>> EvictAsync<TInstance>(string key, CancellationToken ct = default)
        where TInstance : class;
}
