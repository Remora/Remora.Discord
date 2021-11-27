//
//  InMemoryPersistentInteractiveEntity.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.Interactivity;

/// <summary>
/// Represents an interactive entity with in-memory persistent data.
/// </summary>
[PublicAPI]
public abstract class InMemoryPersistentInteractiveEntity
{
    /// <summary>
    /// Gets the nonce used to look up the data object.
    /// </summary>
    public abstract string Nonce { get; }

    /// <summary>
    /// Gets or sets the persistent data object. This property is used to both provide the cached data object, and to
    /// update it in the memory cache after execution of the interaction.
    /// </summary>
    internal abstract object UntypedData { get; set; }

    /// <summary>
    /// Gets the data type.
    /// </summary>
    internal abstract Type DataType { get; }
}

/// <summary>
/// Represents an interactive entity with strongly typed in-memory persistent data.
/// </summary>
/// <typeparam name="TEntityData">The type of entity data.</typeparam>
[PublicAPI]
public abstract class InMemoryPersistentInteractiveEntity<TEntityData> : InMemoryPersistentInteractiveEntity
    where TEntityData : notnull
{
    /// <inheritdoc/>
    internal sealed override object UntypedData { get; set; } = null!;

    /// <inheritdoc/>
    internal sealed override Type DataType => typeof(TEntityData);

    /// <inheritdoc cref="InMemoryPersistentInteractiveEntity.UntypedData"/>
    protected TEntityData Data
    {
        get => (TEntityData)this.UntypedData;
        set => this.UntypedData = value;
    }
}
