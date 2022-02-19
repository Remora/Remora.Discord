//
//  IDefaultedPartialComponent.cs
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
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects;

/// <summary>
/// Acts as a shim for concrete subcomponent types, providing default implementations for inaccessible properties.
/// </summary>
internal interface IDefaultedPartialComponent : IPartialComponent
{
    /// <inheritdoc cref="IPartialComponent.Type"/>
    ComponentType IPartialComponent.Type => default;

    /// <inheritdoc cref="IPartialComponent.CustomID"/>
    Optional<string> IPartialComponent.CustomID => default;

    /// <inheritdoc cref="IPartialComponent.IsDisabled"/>
    Optional<bool> IPartialComponent.IsDisabled => default;

    /// <inheritdoc cref="IPartialComponent.Style"/>
    Optional<OneOf<ButtonComponentStyle, TextInputStyle>> IPartialComponent.Style => default;

    /// <inheritdoc cref="IPartialComponent.Label"/>
    Optional<string> IPartialComponent.Label => default;

    /// <inheritdoc cref="IPartialComponent.Emoji"/>
    Optional<IPartialEmoji> IPartialComponent.Emoji => default;

    /// <inheritdoc cref="IPartialComponent.URL"/>
    Optional<string> IPartialComponent.URL => default;

    /// <inheritdoc cref="IPartialComponent.Options"/>
    Optional<IReadOnlyList<ISelectOption>> IPartialComponent.Options => default;

    /// <inheritdoc cref="IPartialComponent.Placeholder"/>
    Optional<string> IPartialComponent.Placeholder => default;

    /// <inheritdoc cref="IPartialComponent.MinValues"/>
    Optional<int> IPartialComponent.MinValues => default;

    /// <inheritdoc cref="IPartialComponent.MaxValues"/>
    Optional<int> IPartialComponent.MaxValues => default;

    /// <inheritdoc cref="IPartialComponent.Components"/>
    Optional<IReadOnlyList<IPartialMessageComponent>> IPartialComponent.Components => default;

    /// <inheritdoc cref="IPartialComponent.MinLength"/>
    Optional<int> IPartialComponent.MinLength => default;

    /// <inheritdoc cref="IPartialComponent.MaxLength"/>
    Optional<int> IPartialComponent.MaxLength => default;

    /// <inheritdoc cref="IPartialComponent.IsRequired"/>
    Optional<bool> IPartialComponent.IsRequired => default;

    /// <inheritdoc cref="IPartialComponent.Value"/>
    Optional<string> IPartialComponent.Value => default;
}
