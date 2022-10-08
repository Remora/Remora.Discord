//
//  IButtonComponent.cs
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
/// Represents a button component.
/// </summary>
[PublicAPI]
public interface IButtonComponent : IMessageComponent, IPartialButtonComponent
{
    /// <summary>
    /// Gets the type of the component.
    /// </summary>
    new ComponentType Type { get; }

    /// <summary>
    /// Gets the button's style.
    /// </summary>
    new ButtonComponentStyle Style { get; }

    /// <summary>
    /// Gets the label on the button.
    /// </summary>
    new Optional<string> Label { get; }

    /// <summary>
    /// Gets the emoji displayed in the button.
    /// </summary>
    new Optional<IPartialEmoji> Emoji { get; }

    /// <summary>
    /// Gets a custom ID for the component, defined by the developer.
    /// </summary>
    new Optional<string> CustomID { get; }

    /// <summary>
    /// Gets the URL used for link-style buttons.
    /// </summary>
    new Optional<string> URL { get; }

    /// <summary>
    /// Gets a value indicating whether the component is disabled.
    /// </summary>
    new Optional<bool> IsDisabled { get; }

    /// <inheritdoc/>
    Optional<ComponentType> IPartialButtonComponent.Type => this.Type;

    /// <inheritdoc/>
    Optional<ButtonComponentStyle> IPartialButtonComponent.Style => this.Style;

    /// <inheritdoc/>
    Optional<string> IPartialButtonComponent.Label => this.Label;

    /// <inheritdoc/>
    Optional<IPartialEmoji> IPartialButtonComponent.Emoji => this.Emoji;

    /// <inheritdoc/>
    Optional<string> IPartialButtonComponent.CustomID => this.CustomID;

    /// <inheritdoc/>
    Optional<string> IPartialButtonComponent.URL => this.URL;

    /// <inheritdoc/>
    Optional<bool> IPartialButtonComponent.IsDisabled => this.IsDisabled;
}
