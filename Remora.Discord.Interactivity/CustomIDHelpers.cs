//
//  CustomIDHelpers.cs
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

using System;
using System.Linq;
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
    public static string CreateButtonID(string name)
        => CreateID(ComponentType.Button, name);

    /// <summary>
    /// Creates an ID string that can be used with button components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateButtonID(string name, params string[] path)
        => CreateID(ComponentType.Button, name, path);

    /// <summary>
    /// Creates an ID string that can be used with select menu components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateSelectMenuID(string name)
        => CreateID(name, ComponentType.SelectMenu);

    /// <summary>
    /// Creates an ID string that can be used with select menu components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateSelectMenuID(string name, params string[] path)
        => CreateID(ComponentType.SelectMenu, name, path);

    /// <summary>
    /// Creates an ID string that can be used with modals.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the modal.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateModalID(string name)
        => FormatID("modal", name, Array.Empty<string>());

    /// <summary>
    /// Creates an ID string that can be used with button components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateModalID(string name, params string[] path)
        => FormatID("modal", name, path);

    /// <summary>
    /// Creates an ID string that can be used with message components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="type">The component type that the ID is for.</param>
    /// <returns>The custom ID.</returns>
    public static string CreateID(string name, ComponentType type)
        => FormatID(type.ToString().Kebaberize(), name, Array.Empty<string>());

    /// <summary>
    /// Creates an ID string that can be used with message components.
    /// </summary>
    /// <param name="type">The component type that the ID is for.</param>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateID(ComponentType type, string name, params string[] path)
        => FormatID(type.ToString().Kebaberize(), name, path);

    private static string FormatID(string type, string name, string[] path)
    {
        foreach (var parameter in new[] { type, name }.Concat(path))
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException("Parameters must consist of some non-whitespace characters.");
            }

            if (parameter.Any(char.IsWhiteSpace))
            {
                throw new ArgumentException("Parameters may not contain whitespace.");
            }
        }

        var combinedPath = path.Length > 0 ? $"{string.Join(' ', path)} " : string.Empty;
        var customID = $"{Constants.InteractionTree}::{combinedPath}{type}::{name}";
        if (customID.Length <= 100)
        {
            return customID;
        }

        var reductionRequired = customID.Length - 100;
        throw new ArgumentException
        (
            $"The final ID is too long. Reduce your parameter lengths by at least {reductionRequired} characters."
        );
    }
}
