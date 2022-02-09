//
//  ITextInputComponent.cs
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
/// Currently only valid for <see cref="IModalComponent.Components"/>
/// </remarks>
public interface ITextInputComponent : IMessageComponent
{
    /// <inheritdoc cref="IComponent.Label"/>
    Optional<string> Label { get; }

    /// <inheritdoc cref="IComponent.MinimumLength"/>
    Optional<int> MinimumLength { get; }

    /// <inheritdoc cref="IComponent.MaximumLength"/>
    Optional<int> MaximumLength { get; }

    /// <inheritdoc cref="IComponent.IsRequired"/>
    Optional<bool> IsRequired { get; }

    /// <inheritdoc cref="IComponent.Value"/>
    Optional<string> Value { get; }
}
