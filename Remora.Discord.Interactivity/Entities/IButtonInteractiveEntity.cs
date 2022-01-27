//
//  IButtonInteractiveEntity.cs
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
/// Represents an entity that responds to button interactions.
/// </summary>
[PublicAPI]
public interface IButtonInteractiveEntity : IInteractiveEntity
{
    /// <summary>
    /// Handles a button interaction; that is, a user pressed a button attached to a message.
    /// </summary>
    /// <param name="user">The user who pressed the button.</param>
    /// <param name="customID">The button's own unique ID.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> HandleInteractionAsync(IUser user, string customID, CancellationToken ct = default);
}
