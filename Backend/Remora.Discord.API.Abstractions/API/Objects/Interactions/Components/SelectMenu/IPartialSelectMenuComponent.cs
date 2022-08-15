//
//  IPartialSelectMenuComponent.cs
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
/// Represents a partial dropdown of selectable values.
/// </summary>
[PublicAPI]
public interface IPartialSelectMenuComponent : IPartialMessageComponent
{
    /// <inheritdoc cref="ISelectMenuComponent.Type" />
    Optional<ComponentType> Type { get; }

    /// <inheritdoc cref="ISelectMenuComponent.CustomID"/>
    Optional<string> CustomID { get; }

    /// <inheritdoc cref="ISelectMenuComponent.Options"/>
    Optional<IReadOnlyList<IPartialSelectOption>> Options { get; }

    /// <inheritdoc cref="ISelectMenuComponent.Placeholder"/>
    Optional<string> Placeholder { get; }

    /// <inheritdoc cref="ISelectMenuComponent.MinValues"/>
    Optional<int> MinValues { get; }

    /// <inheritdoc cref="ISelectMenuComponent.MaxValues"/>
    Optional<int> MaxValues { get; }

    /// <inheritdoc cref="ISelectMenuComponent.IsDisabled"/>
    Optional<bool> IsDisabled { get; }
}
