//
//  IComponent.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents the raw view of a message component, with all fields from all types. This is the on-wire Discord
/// format.
/// </summary>
[PublicAPI]
public interface IComponent
{
    /// <summary>
    /// Gets the component's type.
    /// </summary>
    ComponentType Type { get; }

    /// <summary>
    /// Gets the components nested under this component.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="IActionRowComponent"/>s.
    /// </remarks>
    Optional<IReadOnlyList<IMessageComponent>> Components { get; }

    /// <summary>
    /// Gets the button's style.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="IButtonComponent"/>s.
    /// </remarks>
    Optional<ButtonComponentStyle> Style { get; }

    /// <summary>
    /// Gets the label on the button.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="IButtonComponent"/>s.
    /// </remarks>
    Optional<string> Label { get; }

    /// <summary>
    /// Gets the emoji displayed in the button.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="IButtonComponent"/>s.
    /// </remarks>
    Optional<IPartialEmoji> Emoji { get; }

    /// <summary>
    /// Gets a custom ID for the component, defined by the developer.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="IButtonComponent"/>s.
    /// </remarks>
    Optional<string> CustomID { get; }

    /// <summary>
    /// Gets the URL used for link-style buttons.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="IButtonComponent"/>s.
    /// </remarks>
    Optional<string> URL { get; }

    /// <summary>
    /// Gets a value indicating whether the component is disabled.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="IButtonComponent"/>s and <see cref="ISelectMenuComponent"/>s.
    /// </remarks>
    Optional<bool> IsDisabled { get; }

    /// <summary>
    /// Gets the options in a select menu.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="ISelectMenuComponent"/>s.
    /// </remarks>
    Optional<IReadOnlyList<ISelectOption>> Options { get; }

    /// <summary>
    /// Gets the placeholder text for a component.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="ISelectMenuComponent"/>s.
    /// </remarks>
    Optional<string> Placeholder { get; }

    /// <summary>
    /// Gets the minimum number of options that must be selected.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="ISelectMenuComponent"/>s.
    /// </remarks>
    Optional<int> MinValues { get; }

    /// <summary>
    /// Gets the maximum number of options that may be selected.
    /// </summary>
    /// <remarks>
    /// Valid for <see cref="ISelectMenuComponent"/>s.
    /// </remarks>
    Optional<int> MaxValues { get; }
}
