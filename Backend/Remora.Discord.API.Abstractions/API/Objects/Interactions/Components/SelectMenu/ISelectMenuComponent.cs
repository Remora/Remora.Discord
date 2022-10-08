//
//  ISelectMenuComponent.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a dropdown of selectable values.
/// </summary>
[PublicAPI]
public interface ISelectMenuComponent : IMessageComponent, IPartialSelectMenuComponent
{
    /// <summary>
    /// Gets the type of the component.
    /// </summary>
    new ComponentType Type { get; }

    /// <summary>
    /// Gets a custom ID for the component, defined by the developer.
    /// </summary>
    new string CustomID { get; }

    /// <summary>
    /// Gets the options in the select menu.
    /// </summary>
    new IReadOnlyList<ISelectOption> Options { get; }

    /// <summary>
    /// Gets the placeholder text for the menu. Max 150 characters.
    /// </summary>
    new Optional<string> Placeholder { get; }

    /// <summary>
    /// Gets the minimum number of options that must be selected.
    /// </summary>
    new Optional<int> MinValues { get; }

    /// <summary>
    /// Gets the maximum number of options that may be selected.
    /// </summary>
    new Optional<int> MaxValues { get; }

    /// <summary>
    /// Gets a value indicating whether the component is disabled.
    /// </summary>
    new Optional<bool> IsDisabled { get; }

    /// <inheritdoc/>
    Optional<ComponentType> IPartialSelectMenuComponent.Type => this.Type;

    /// <inheritdoc/>
    Optional<string> IPartialSelectMenuComponent.CustomID => this.CustomID;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IPartialSelectOption>> IPartialSelectMenuComponent.Options => new(this.Options);

    /// <inheritdoc/>
    Optional<string> IPartialSelectMenuComponent.Placeholder => this.Placeholder;

    /// <inheritdoc/>
    Optional<int> IPartialSelectMenuComponent.MinValues => this.MinValues;

    /// <inheritdoc/>
    Optional<int> IPartialSelectMenuComponent.MaxValues => this.MaxValues;

    /// <inheritdoc/>
    Optional<bool> IPartialSelectMenuComponent.IsDisabled => this.IsDisabled;
}
