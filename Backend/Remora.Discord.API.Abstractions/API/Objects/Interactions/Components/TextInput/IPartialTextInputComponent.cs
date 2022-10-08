//
//  IPartialTextInputComponent.cs
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
/// Represents a partial text-based input component.
/// </summary>
[PublicAPI]
public interface IPartialTextInputComponent : IPartialMessageComponent
{
    /// <inheritdoc cref="ITextInputComponent.Type" />
    Optional<ComponentType> Type { get; }

    /// <inheritdoc cref="ITextInputComponent.CustomID"/>
    Optional<string> CustomID { get; }

    /// <inheritdoc cref="ITextInputComponent.Style"/>
    Optional<TextInputStyle> Style { get; }

    /// <inheritdoc cref="ITextInputComponent.Label"/>
    Optional<string> Label { get; }

    /// <inheritdoc cref="ITextInputComponent.MinLength"/>
    Optional<int> MinLength { get; }

    /// <inheritdoc cref="ITextInputComponent.MaxLength"/>
    Optional<int> MaxLength { get; }

    /// <inheritdoc cref="ITextInputComponent.IsRequired"/>
    Optional<bool> IsRequired { get; }

    /// <inheritdoc cref="ITextInputComponent.Value"/>
    Optional<string> Value { get; }

    /// <inheritdoc cref="ITextInputComponent.Placeholder"/>
    Optional<string> Placeholder { get; }
}
