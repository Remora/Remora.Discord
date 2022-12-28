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
    /// Gets the name used to build IDs for select menu components.
    /// This may include text, role, channel etc. select menus.
    /// </summary>
    private const string SelectComponentName = "select-menu";

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
    /// Creates an ID string with state information that can be used with button components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <returns>The custom ID.</returns>
    public static string CreateButtonIDWithState(string name, string state)
        => CreateIDWithState(ComponentType.Button, name, state);

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
    /// Creates an ID string with state information that can be used with button components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateButtonIDWithState(string name, string state, params string[] path)
        => CreateIDWithState(ComponentType.Button, name, state, path);

    /// <summary>
    /// Creates an ID string that can be used with select menu components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateSelectMenuID(string name)
        => FormatID(SelectComponentName, name, null, Array.Empty<string>());

    /// <summary>
    /// Creates an ID string with state information that can be used with select menu components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <returns>The custom ID.</returns>
    public static string CreateSelectMenuIDWithState(string name, string state)
        => FormatID(SelectComponentName, name, state, Array.Empty<string>());

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
        => FormatID(SelectComponentName, name, null, path);

    /// <summary>
    /// Creates an ID string with state information that can be used with select menu components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateSelectMenuIDWithState(string name, string state, params string[] path)
        => FormatID(SelectComponentName, name, state, path);

    /// <summary>
    /// Creates an ID string that can be used with modals.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the modal.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateModalID(string name)
        => FormatID("modal", name, null, Array.Empty<string>());

    /// <summary>
    /// Creates an ID string with state information that can be used with modals.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the modal.
    /// </param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <returns>The custom ID.</returns>
    public static string CreateModalIDWithState(string name, string state)
        => FormatID("modal", name, state, Array.Empty<string>());

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
        => FormatID("modal", name, null, path);

    /// <summary>
    /// Creates an ID string with state information that can be used with button components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateModalIDWithState(string name, string state, params string[] path)
        => FormatID("modal", name, state, path);

    /// <summary>
    /// Creates an ID string that can be used with message components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="type">The component type that the ID is for.</param>
    /// <returns>The custom ID.</returns>
    public static string CreateID(string name, ComponentType type)
        => FormatID(type.ToString().Kebaberize(), name, null, Array.Empty<string>());

    /// <summary>
    /// Creates an ID string with state information that can be used with message components.
    /// </summary>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="type">The component type that the ID is for.</param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <returns>The custom ID.</returns>
    public static string CreateIDWithState(string name, ComponentType type, string state)
        => FormatID(type.ToString().Kebaberize(), name, state, Array.Empty<string>());

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
        => FormatID(type.ToString().Kebaberize(), name, null, path);

    /// <summary>
    /// Creates an ID string with state information that can be used with message components.
    /// </summary>
    /// <param name="type">The component type that the ID is for.</param>
    /// <param name="name">
    /// The name used to identify the component. Must be unique among the components in the message.
    /// </param>
    /// <param name="state">The state value passed with the custom ID.</param>
    /// <param name="path">
    /// The group path to the component; that is, the outer groups that must be traversed before reaching the group
    /// where the component's handler is declared.
    /// </param>
    /// <returns>The custom ID.</returns>
    public static string CreateIDWithState(ComponentType type, string name, string state, params string[] path)
        => FormatID(type.ToString().Kebaberize(), name, state, path);

    private static string FormatID(string type, string name, string? state, string[] path)
    {
        var parameters = new[] { type, name }.Concat(path);
        if (state != null)
        {
            parameters = parameters.Concat(new[] { state });
        }

        foreach (var parameter in parameters)
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
        var statePrefix = state != null ? $"{Constants.StatePrefix}{state} " : string.Empty;
        var customID = $"{Constants.InteractionTree}::{statePrefix}{combinedPath}{type}::{name}";
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
