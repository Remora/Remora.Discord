//
//  IInteractiveEntity.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Results;

namespace Remora.Discord.Interactivity;

/// <summary>
/// Marker interface for interactive entities.
/// </summary>
[PublicAPI]
public interface IInteractiveEntity
{
    /// <summary>
    /// Determines if the entity is interested in an interaction for the given component type and custom ID.
    /// </summary>
    /// <remarks>
    /// An interested entity will be allowed to handle the interaction, and more than one entity may indicate
    /// interest for a single interaction. Multiple interested entities will run in parallel, though access to
    /// persistent in-memory entity data will be synchronized.
    /// </remarks>
    /// <param name="componentType">
    /// The component type, or null if the interaction is not bound to a particular component type. This is generally
    /// the case for modal interactions.
    /// </param>
    /// <param name="customID">The custom ID.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>true if the entity is interested; otherwise, false.</returns>
    Task<Result<bool>> IsInterestedAsync(ComponentType? componentType, string customID, CancellationToken ct = default);
}
