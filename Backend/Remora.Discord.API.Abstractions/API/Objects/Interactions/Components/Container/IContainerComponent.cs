//
//  IContainerComponent.cs
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

using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// A container for V2 components.
/// </summary>
[PublicAPI]
public interface IContainerComponent : IMessageComponent, IPartialContainerComponent
{
    /// <summary>
    /// Gets the type of the component.
    /// </summary>
    new ComponentType Type { get; }

    /// <summary>
    /// Gets the components of the container.
    /// </summary>
    new IReadOnlyList<IMessageComponent> Components { get; }

    /// <summary>
    /// Gets whether the container is spoilered.
    /// </summary>
    new Optional<bool> IsSpoiler { get; }

    /// <summary>
    /// Gets the accent colour of the container.
    /// </summary>
    new Optional<Color?> AccentColour { get; }

    /// <inheritdoc/>
    Optional<ComponentType> IPartialContainerComponent.Type => this.Type;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IPartialMessageComponent>> IPartialContainerComponent.Components => new(this.Components);

    /// <inheritdoc/>
    Optional<bool> IPartialContainerComponent.IsSpoiler => this.IsSpoiler;

    /// <inheritdoc/>
    Optional<Color?> IPartialContainerComponent.AccentColour => this.AccentColour;
}
