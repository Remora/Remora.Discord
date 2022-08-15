//
//  IPartialButtonComponent.cs
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
/// Represents a partial button component.
/// </summary>
[PublicAPI]
public interface IPartialButtonComponent : IPartialMessageComponent
{
    /// <inheritdoc cref="IButtonComponent.Type" />
    Optional<ComponentType> Type { get; }

    /// <inheritdoc cref="IButtonComponent.Style"/>
    Optional<ButtonComponentStyle> Style { get; }

    /// <inheritdoc cref="IButtonComponent.Label"/>
    Optional<string> Label { get; }

    /// <inheritdoc cref="IButtonComponent.Emoji"/>
    Optional<IPartialEmoji> Emoji { get; }

    /// <inheritdoc cref="IButtonComponent.CustomID"/>
    Optional<string> CustomID { get; }

    /// <inheritdoc cref="IButtonComponent.URL"/>
    Optional<string> URL { get; }

    /// <inheritdoc cref="IButtonComponent.IsDisabled"/>
    Optional<bool> IsDisabled { get; }
}
