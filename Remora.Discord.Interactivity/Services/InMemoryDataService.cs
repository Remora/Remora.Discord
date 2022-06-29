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

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Discord.Interactivity.Services;

/// <summary>
/// Manages synchronized access to data.
/// </summary>
/// <remarks>
/// While <typeparamref name="TKey"/> should be a simple dictionary key, <seealso cref="TData"/> may be any complex type
/// such as classes, structures, or records. If the data type implements <see cref="IDisposable"/> or
/// <see cref="IAsyncDisposable"/>, the instance will also be disposed upon removal.
/// </remarks>
/// <typeparam name="TKey">The key type for the stored data.</typeparam>
/// <typeparam name="TData">The data type stored by the service.</typeparam>
[PublicAPI]
public class InMemoryDataService<TKey, TData> : IAsyncDisposable where TKey : notnull
{
    /// <summary>
    /// Gets the singleton instance of this service.
    /// </summary>
    public static InMemoryDataService<TKey, TData> Instance { get; } = new();

    private readonly ConcurrentDictionary<TKey, (SemaphoreSlim Semaphore, TData Data)> _data = new();
    private bool _isDisposed;

    private InMemoryDataService()
    {
    }

    /// <summary>
    /// Inserts a new data object into the service.
    /// </summary>
    /// <remarks>
    /// After inserting the value, any further access to the data is invalid. If you come from a C++ or Rust background,
    /// consider the value as having been moved into this method.
    /// </remarks>
    /// <param name="key">The key the data object is associated with.</param>
    /// <param name="data">The data object.</param>
    /// <returns>true if the data was successfully added; otherwise, false.</returns>
    public bool TryAddData(TKey key, TData data)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The data service has been disposed of and cannot be used.");
        }

        return _data.TryAdd(key, (new SemaphoreSlim(1, 1), data));
    }

    /// <summary>
    /// Leases the data associated with the given key, blocking until a lock can be taken on the data object.
    /// </summary>
    /// <remarks>
    /// The semaphore returned by this method has the lock held on it and must be released once the caller is done with
    /// the object.
    /// </remarks>
    /// <param name="key">The key the data object is associated with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The data and semaphore associated with the data or a <see cref="NotFoundError"/>.</returns>
    public async Task<Result<DataLease<TKey, TData>>> LeaseDataAsync(TKey key, CancellationToken ct = default)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The data service has been disposed of and cannot be used.");
        }

        if (!_data.TryGetValue(key, out var tuple))
        {
            return new NotFoundError();
        }

        var (semaphore, data) = tuple;
        await semaphore.WaitAsync(ct);

        return new DataLease<TKey, TData>(this, key, semaphore, data);
    }

    /// <summary>
    /// Rents the data associated with the given key, blocking until a lock can be taken on the data object.
    /// </summary>
    /// <remarks>
    /// The semaphore returned by this method has the lock held on it and must be released once the caller is done with
    /// the object.
    /// </remarks>
    /// <param name="key">The key the data object is associated with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The data and semaphore associated with the data or a <see cref="NotFoundError"/>.</returns>
    [Obsolete("Use LeaseDataAsync instead.", true)]
    public async Task<Result<(SemaphoreSlim Semaphore, TData Data)>> RentDataAsync
    (
        TKey key,
        CancellationToken ct = default
    )
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The data service has been disposed of and cannot be used.");
        }

        if (!_data.TryGetValue(key, out var tuple))
        {
            return new NotFoundError();
        }

        var (semaphore, _) = tuple;
        await semaphore.WaitAsync(ct);

        return tuple;
    }

    /// <summary>
    /// Removes the data associated with the given key. This method does nothing if no data is associated with
    /// the given key.
    /// </summary>
    /// <param name="key">The key the data object is associated with.</param>
    /// <returns>true if the data was successfully removed; otherwise, false.</returns>
    public bool TryRemoveData(TKey key)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The data service has been disposed of and cannot be used.");
        }

        if (!_data.TryRemove(key, out var tuple))
        {
            return false;
        }

        var (semaphore, data) = tuple;

        if (data is IAsyncDisposable and not IDisposable)
        {
            throw new InvalidOperationException
            (
                $"Unable to synchronously dispose of the held data belonging to key {key}."
            );
        }

        if (data is IDisposable disposableData)
        {
            disposableData.Dispose();
        }

        if (data is IAsyncDisposable and not IDisposable)
        {
            throw new InvalidOperationException
            (
                $"Unable to synchronously dispose of the held data belonging to key {key}."
            );
        }

        semaphore.Dispose();
        return true;
    }

    /// <summary>
    /// Removes the data associated with the given key. This method does nothing if no data is associated with
    /// the given key.
    /// </summary>
    /// <param name="key">The key the data object is associated with.</param>
    /// <returns>true if the data was successfully removed; otherwise, false.</returns>
    public async ValueTask<bool> TryRemoveDataAsync(TKey key)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The data service has been disposed of and cannot be used.");
        }

        if (!_data.TryRemove(key, out var tuple))
        {
            return false;
        }

        var (semaphore, data) = tuple;

        if (data is IAsyncDisposable asyncDisposableData)
        {
            await asyncDisposableData.DisposeAsync();
        }

        if (data is IDisposable disposableData)
        {
            disposableData.Dispose();
        }

        semaphore.Dispose();
        return true;
    }

    /// <summary>
    /// Deletes the data, disposing of the semaphore and, if necessary, the data.
    /// </summary>
    /// <param name="key">The key the data object is associated with.</param>
    /// <returns>true if the data was successfully removed; otherwise, false..</returns>
    internal async ValueTask<bool> DeleteDataAsync(TKey key)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The data service has been disposed of and cannot be used.");
        }

        if (!_data.TryRemove(key, out var tuple))
        {
            // already gone
            return false;
        }

        var (semaphore, data) = tuple;
        if (data is IAsyncDisposable asyncDisposableData)
        {
            await asyncDisposableData.DisposeAsync();
        }

        if (data is IDisposable disposableData)
        {
            disposableData.Dispose();
        }

        semaphore.Dispose();
        return true;
    }

    /// <summary>
    /// Updates the data identified by the given key.
    /// </summary>
    /// <remarks>
    /// The semaphore is not released by this method.</remarks>
    /// <param name="key">The key.</param>
    /// <param name="newData">The new data.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the key is not found or the semaphore is not held.
    /// </exception>
    internal void UpdateData(TKey key, TData newData)
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException("The data service has been disposed of and cannot be used.");
        }

        if (!_data.TryGetValue(key, out var existingData))
        {
            throw new InvalidOperationException("No data with that key could be found.");
        }

        var (existingSemaphore, _) = existingData;
        if (existingSemaphore.CurrentCount != 0)
        {
            throw new InvalidOperationException("The semaphore is not currently held, and you do not own the data.");
        }

        if (!_data.TryUpdate(key, (existingSemaphore, newData), existingData))
        {
            throw new InvalidOperationException
            (
                "Something updated the data while we were working on it. Concurrency bug?"
            );
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        var keys = _data.Keys.ToList();
        foreach (var key in keys)
        {
            await DeleteDataAsync(key);
        }
    }
}
