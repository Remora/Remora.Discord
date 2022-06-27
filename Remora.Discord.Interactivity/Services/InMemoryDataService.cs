//
//  InMemoryDataService.cs
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

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Discord.Interactivity.Services;

/// <summary>
/// Manages synchronized access to data.
/// </summary>
/// <typeparam name="TKey">The key type for the stored data.</typeparam>
/// <typeparam name="TData">The data type stored by the service.</typeparam>
[PublicAPI]
public class InMemoryDataService<TKey, TData> where TKey : notnull
{
    /// <summary>
    /// Gets the singleton instance of this service.
    /// </summary>
    public static InMemoryDataService<TKey, TData> Instance { get; } = new();

    private readonly ConcurrentDictionary<TKey, (SemaphoreSlim Semaphore, TData Data)> _paginationData = new();

    private InMemoryDataService()
    {
    }

    /// <summary>
    /// Inserts a new data object into the service.
    /// </summary>
    ///
    /// <param name="key">The key the data object is associated with.</param>
    /// <param name="data">The data object.</param>
    /// <returns>true if the data was successfully added; otherwise, false.</returns>
    public bool TryAddData(TKey key, TData data)
    {
        return _paginationData.TryAdd(key, (new SemaphoreSlim(1, 1), data));
    }

    /// <summary>
    /// Rents the data associated with the given message ID, blocking until a lock can be taken on the data object.
    /// </summary>
    /// <remarks>
    /// The semaphore returned by this method has the lock held on it and must be released once the caller is done with
    /// the object.
    /// </remarks>
    /// <param name="key">The key the data object is associated with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The data and semaphore associated with the message or a <see cref="NotFoundError"/>.</returns>
    public async Task<Result<(SemaphoreSlim Semaphore, TData Data)>> RentData(TKey key, CancellationToken ct = default)
    {
        if (!_paginationData.TryGetValue(key, out var tuple))
        {
            return new NotFoundError();
        }

        var (semaphore, _) = tuple;
        await semaphore.WaitAsync(ct);

        return tuple;
    }

    /// <summary>
    /// Removes the data associated with the given message ID. This method does nothing if no data is associated with
    /// the given message ID.
    /// </summary>
    /// <param name="key">The key the data object is associated with.</param>
    /// <returns>true if the data was successfully removed; otherwise, false.</returns>
    public bool TryRemoveData(TKey key)
    {
        return _paginationData.TryRemove(key, out _);
    }
}
