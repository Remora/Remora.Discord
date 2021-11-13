//
//  SelectMenuComponent.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="ISelectMenuComponent" />
[PublicAPI]
public record SelectMenuComponent
(
    string CustomID,
    IReadOnlyList<ISelectOption> Options,
    Optional<string> Placeholder,
    Optional<int> MinValues,
    Optional<int> MaxValues,
    Optional<bool> IsDisabled
) : ISelectMenuComponent, IDefaultedComponent
{
    /// <inheritdoc/>
    ComponentType IComponent.Type => ComponentType.SelectMenu;

    /// <inheritdoc/>
    Optional<string> IComponent.CustomID => this.CustomID;

    /// <inheritdoc/>
    Optional<IReadOnlyList<ISelectOption>> IComponent.Options => new(this.Options);
}
