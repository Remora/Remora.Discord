//
//  CommandNodeExtensions.cs
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
using System.Reflection;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="CommandNode"/> type.
/// </summary>
public static class CommandNodeExtensions
{
    /// <summary>
    /// Gets the command type of the given node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The command type.</returns>
    public static ApplicationCommandType GetCommandType(this CommandNode node)
    {
        var attribute = node.CommandMethod.GetCustomAttribute<CommandTypeAttribute>();
        return attribute?.Type ?? ApplicationCommandType.ChatInput;
    }

    /// <summary>
    /// Finds the first custom attribute on the given command or any of its ancestors.
    /// </summary>
    /// <typeparam name="T">The type of attribute to search for.</typeparam>
    /// <param name="node">The command node.</param>
    /// <param name="includeAncestors">
    /// Indicates that ancestors of the command should also be search for the attribute.
    /// </param>
    /// <returns>
    /// A custom attribute that matches <typeparamref name="T"/>, or <c>null</c> if no such attribute is found.
    /// </returns>
    public static T? FindCustomAttributeOnLocalTree<T>
    (
        this CommandNode node,
        bool includeAncestors = true
    ) where T : Attribute
    {
        // Attempt to first find the attribute on the command itself
        var attribute = node.CommandMethod.GetCustomAttribute<T>();

        if (attribute is not null || !includeAncestors)
        {
            return attribute;
        }

        // Traverse each parent group node, until we find the root node
        var parent = node.Parent;
        while (parent is GroupNode group)
        {
            parent = group.Parent;

            // See if any of the types in this group been decorated with the attribute
            foreach (var t in group.GroupTypes)
            {
                attribute = t.GetCustomAttribute<T>();

                if (attribute is not null)
                {
                    return attribute;
                }
            }
        }

        return null;
    }
}
