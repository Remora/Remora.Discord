//
//  CustomIDHelpers.cs
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

using Humanizer;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Interactivity;

/// <summary>
/// Contains various helper methods for creating component ID strings.
/// </summary>
[PublicAPI]
public static class CustomIDHelpers
{
    /// <summary>
    /// Creates an ID string that can be used with button components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateButtonID(string name) => CreateID(name, ComponentType.Button);

    /// <summary>
    /// Creates an ID string that can be used with select menu components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateSelectMenuID(string name) => CreateID(name, ComponentType.SelectMenu);

    /// <summary>
    /// Creates an ID string that can be used with modals.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the modal.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateModalID(string name) => $"{Constants.InteractionTree}::modal::{name}";

    /// <summary>
    /// Creates an ID string that can be used with message components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="type">The component type that the ID is for.</param>
    /// <returns>The custom ID.</returns>
    public static string CreateID(string name, ComponentType type)
    {
        return $"{Constants.InteractionTree}::{type.ToString().Kebaberize()}::{name}";
    }
}
