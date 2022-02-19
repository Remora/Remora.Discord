//
//  IPartialTextInputComponent.cs
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

using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a text-based input component.
/// </summary>
/// <remarks>
/// Currently only valid for modal components..
/// </remarks>
public interface IPartialTextInputComponent : IPartialMessageComponent
{
    /// <inheritdoc cref="IPartialComponent.CustomID"/>
    string CustomID { get; }

    /// <inheritdoc cref="IPartialComponent.Style"/>
    Optional<TextInputStyle> Style { get; }

    /// <inheritdoc cref="IPartialComponent.Label"/>
    Optional<string> Label { get; }

    /// <inheritdoc cref="IPartialComponent.MinLength"/>
    Optional<int> MinLength { get; }

    /// <inheritdoc cref="IPartialComponent.MaxLength"/>
    Optional<int> MaxLength { get; }

    /// <inheritdoc cref="IPartialComponent.IsRequired"/>
    Optional<bool> IsRequired { get; }

    /// <inheritdoc cref="IPartialComponent.Value"/>
    Optional<string> Value { get; }

    /// <inheritdoc cref="IPartialComponent.Placeholder"/>
    Optional<string> Placeholder { get; }
}
