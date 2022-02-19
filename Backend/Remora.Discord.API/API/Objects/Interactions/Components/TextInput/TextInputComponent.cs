//
//  TextInputComponent.cs
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

using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Objects.ITextInputComponent" />
public record TextInputComponent
(
    string CustomID,
    TextInputStyle Style,
    string Label,
    Optional<int> MinLength,
    Optional<int> MaxLength,
    Optional<bool> IsRequired,
    Optional<string> Value,
    Optional<string> Placeholder
) : ITextInputComponent, IDefaultedComponent
{
    /// <inheritdoc cref="IPartialComponent.Type" />
    ComponentType IPartialComponent.Type => ComponentType.TextInput;

    /// <inheritdoc cref="IPartialComponent.CustomID" />
    Optional<string> IPartialComponent.CustomID => this.CustomID;

    /// <inheritdoc cref="IPartialComponent.Style" />
    Optional<OneOf<ButtonComponentStyle, TextInputStyle>> IPartialComponent.Style => new(this.Style);

    /// <inheritdoc cref="IPartialComponent.Label" />
    Optional<string> IPartialComponent.Label => this.Label;
}
