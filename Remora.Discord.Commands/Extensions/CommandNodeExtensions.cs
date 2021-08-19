//
//  CommandNodeExtensions.cs
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

using System;
using System.Reflection;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;

namespace Remora.Discord.Commands.Extensions
{
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
        /// <returns>A custom attribute that matches <typeparamref name="T"/>, or <c>null</c> if no such attribute is found.</returns>
        public static T? FindCustomAttributeOnLocalTree<T>(this CommandNode node) where T : Attribute
        {
            // Attempt to first check for ephemerailty on the command itself
            T? attr = node.CommandMethod.GetCustomAttribute<T>();

            if (attr is null)
            {
                // Traverse each parent group node, until we find the root node
                IParentNode p = node.Parent;
                while (p is GroupNode g && attr is null)
                {
                    p = g.Parent;

                    // See if any of the types in this group have expressed ephemerality
                    foreach (Type t in g.GroupTypes)
                    {
                        attr = t.GetCustomAttribute<T>();

                        if (attr is not null)
                        {
                            return attr;
                        }
                    }
                }
            }

            return null;
        }
    }
}
