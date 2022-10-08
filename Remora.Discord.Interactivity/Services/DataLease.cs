//
//  DataLease.cs
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

namespace Remora.Discord.Interactivity.Services;

/// <summary>
/// Represents exclusive leased access to a piece of data. An asynchronous lock is held on the data while the lease is
/// active. The data is updated and the lock released when the lease is disposed.
/// </summary>
/// <typeparam name="TKey">The key type of the leased data.</typeparam>
/// <typeparam name="TData">The data type of the leased data.</typeparam>
[PublicAPI]
public class DataLease<TKey, TData> : IAsyncDisposable where TKey : notnull
{
    private readonly InMemoryDataService<TKey, TData> _dataService;
    private readonly TKey _key;
    private readonly SemaphoreSlim _semaphore;

    private bool _shouldDelete;
    private bool _isDisposed;
    private TData _data;

    /// <summary>
    /// Gets or sets the data associated with the lease.
    /// </summary>
    public TData Data
    {
        get => _isDisposed
            ? throw new ObjectDisposedException("The data lease has expired.")
            : _data;
        set
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("The data lease has expired.");
            }

            _data = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataLease{TKey, TData}"/> class.
    /// </summary>
    /// <param name="dataService">The data service.</param>
    /// <param name="key">The key associated with the data.</param>
    /// <param name="semaphore">The semaphore associated with the data.</param>
    /// <param name="data">The data.</param>
    internal DataLease(InMemoryDataService<TKey, TData> dataService, TKey key, SemaphoreSlim semaphore, TData data)
    {
        _dataService = dataService;
        _key = key;
        _semaphore = semaphore;
        _data = data;
    }

    /// <summary>
    /// Marks the leased data for deletion upon disposal.
    /// </summary>
    public void Delete()
    {
        _shouldDelete = true;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        GC.SuppressFinalize(this);

        if (_shouldDelete)
        {
            var couldDelete = await _dataService.DeleteDataAsync(_key);
            if (couldDelete)
            {
                return;
            }

            // manual cleanup, someone must have deleted the data while we were using it
            if (_data is IAsyncDisposable asyncDisposableData)
            {
                await asyncDisposableData.DisposeAsync();
            }

            if (_data is IDisposable disposableData)
            {
                disposableData.Dispose();
            }

            _semaphore.Dispose();
            return;
        }

        _dataService.UpdateData(_key, _data);
        _semaphore.Release();
    }
}
