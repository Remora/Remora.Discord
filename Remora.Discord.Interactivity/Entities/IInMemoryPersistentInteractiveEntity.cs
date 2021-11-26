//
//  IInMemoryPersistentInteractiveEntity.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.Interactivity;

/// <summary>
/// Represents an interactive entity with in-memory persistent data.
/// </summary>
[PublicAPI]
public interface IInMemoryPersistentInteractiveEntity
{
    /// <summary>
    /// Gets the nonce used to look up the data object.
    /// </summary>
    string Nonce { get; }

    /// <summary>
    /// Gets or sets the persistent data object. This property is used to both provide the cached data object, and to
    /// update it in the memory cache after execution of the interaction.
    /// </summary>
    object UntypedData { get; protected internal set; }
}

/// <summary>
/// Represents an interactive entity with strongly typed in-memory persistent data.
/// </summary>
/// <typeparam name="TEntityData">The type of entity data.</typeparam>
[PublicAPI]
public interface IInMemoryPersistentInteractiveEntity<TEntityData> : IInMemoryPersistentInteractiveEntity
    where TEntityData : notnull
{
    /// <inheritdoc cref="IInMemoryPersistentInteractiveEntity.UntypedData"/>
    TEntityData Data
    {
        get => (TEntityData)this.UntypedData;
        protected set => this.UntypedData = value;
    }

    /// <inheritdoc/>
    object IInMemoryPersistentInteractiveEntity.UntypedData { get; set; }
}
