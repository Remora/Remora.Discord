//
//  ITextInputComponent.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a text-based input component.
/// </summary>
[PublicAPI]
public interface ITextInputComponent : IMessageComponent, IPartialTextInputComponent
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
    /// Gets the text input's style.
    /// </summary>
    new TextInputStyle Style { get; }

    /// <summary>
    /// Gets the label on the text input. Max 45 characters.
    /// </summary>
    new string Label { get; }

    /// <summary>
    /// Gets the minimum length of the input, maximum of 4000.
    /// </summary>
    /// <remarks>
    /// Must be greater than zero and less than or equal to <see cref="MaxLength"/> if specified. The global maximum is
    /// 4000 characters, regardless of what <see cref="MaxLength"/> is set to.
    /// </remarks>
    new Optional<int> MinLength { get; }

    /// <summary>
    /// Gets the maximum length of the input, maximum of 4000.
    /// </summary>
    /// <remarks>
    /// Must be greater than zero and greater than or equal to <see cref="MinLength"/> if specified. The global maximum
    /// is 4000 characters, regardless of what <see cref="MaxLength"/> is set to.
    /// </remarks>
    new Optional<int> MaxLength { get; }

    /// <summary>
    /// Gets whether this field must be filled out. Defaults to true.
    /// </summary>
    new Optional<bool> IsRequired { get; }

    /// <summary>
    /// Gets the pre-filled value for the text field. Maximum 4000 characters.
    /// </summary>
    new Optional<string> Value { get; }

    /// <summary>
    /// Gets the placeholder text displayed if the input field is empty.
    /// </summary>
    new Optional<string> Placeholder { get; }

    /// <inheritdoc/>
    Optional<ComponentType> IPartialTextInputComponent.Type => this.Type;

    /// <inheritdoc/>
    Optional<string> IPartialTextInputComponent.CustomID => this.CustomID;

    /// <inheritdoc/>
    Optional<TextInputStyle> IPartialTextInputComponent.Style => this.Style;

    /// <inheritdoc/>
    Optional<string> IPartialTextInputComponent.Label => this.Label;

    /// <inheritdoc/>
    Optional<int> IPartialTextInputComponent.MinLength => this.MinLength;

    /// <inheritdoc/>
    Optional<int> IPartialTextInputComponent.MaxLength => this.MaxLength;

    /// <inheritdoc/>
    Optional<bool> IPartialTextInputComponent.IsRequired => this.IsRequired;

    /// <inheritdoc/>
    Optional<string> IPartialTextInputComponent.Value => this.Value;

    /// <inheritdoc/>
    Optional<string> IPartialTextInputComponent.Placeholder => this.Placeholder;
}
