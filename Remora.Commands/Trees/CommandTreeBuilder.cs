//
//  CommandTreeBuilder.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Extensions;
using Remora.Commands.Modules;
using Remora.Commands.Trees.Nodes;

namespace Remora.Commands.Trees
{
    /// <summary>
    /// Builds command trees from input command group types.
    /// </summary>
    [PublicAPI]
    public class CommandTreeBuilder
    {
        private readonly List<Type> _registeredModuleTypes = new List<Type>();

        /// <summary>
        /// Registers a module type with the builder.
        /// </summary>
        /// <typeparam name="TModule">The module type.</typeparam>
        public void RegisterModule<TModule>() where TModule : ModuleBase
        {
            if (!_registeredModuleTypes.Contains(typeof(TModule)))
            {
                _registeredModuleTypes.Add(typeof(TModule));
            }
        }

        /// <summary>
        /// Builds a command tree from the registered types.
        /// </summary>
        /// <returns>The command tree.</returns>
        public CommandTree Build()
        {
            var rootChildren = new List<IChildNode>();
            var rootNode = new RootNode(rootChildren);
            rootChildren.AddRange(ToChildNodes(_registeredModuleTypes, rootNode));

            return new CommandTree(rootNode);
        }

        /// <summary>
        /// Parses the given list of module types into a set of child nodes.
        /// </summary>
        /// <remarks>
        /// Child nodes can be either <see cref="GroupNode"/> or <see cref="CommandNode"/> instances, where methods in
        /// the types that have been marked as commands produce command nodes, and nested classes produce group nodes.
        ///
        /// If a nested class does not have a <see cref="GroupAttribute"/>, its subtypes and methods are instead
        /// parented to the containing type.
        /// </remarks>
        /// <param name="moduleTypes">The module types.</param>
        /// <param name="parent">The parent node. For the first invocation, this will be the root node.</param>
        /// <returns>The new children of the parent.</returns>
        private IEnumerable<IChildNode> ToChildNodes(IEnumerable<Type> moduleTypes, IParentNode parent)
        {
            IEnumerable<IGrouping<string, Type>> groups = moduleTypes.GroupBy
            (
                mt => mt.TryGetGroupName(out var name) ? name : string.Empty
            );

            foreach (var group in groups)
            {
                if (group.Key == string.Empty)
                {
                    // Nest these commands and groups directly under the parent
                    foreach (var groupType in group)
                    {
                        var subgroups = groupType.GetNestedTypes().Where(t => typeof(ModuleBase).IsAssignableFrom(t));

                        // Extract submodules and commands
                        foreach (var child in GetModuleCommands(groupType, parent))
                        {
                            yield return child;
                        }

                        foreach (var child in ToChildNodes(subgroups, parent))
                        {
                            yield return child;
                        }
                    }
                }
                else
                {
                    // Nest these commands and groups under a subgroup
                    var groupChildren = new List<IChildNode>();
                    var groupNode = new GroupNode(groupChildren, parent, group.Key);

                    foreach (var groupType in group)
                    {
                        var subgroups = groupType.GetNestedTypes().Where(t => typeof(ModuleBase).IsAssignableFrom(t));

                        // Extract submodules and commands
                        groupChildren.AddRange(GetModuleCommands(groupType, groupNode));
                        groupChildren.AddRange(ToChildNodes(subgroups, groupNode));

                        yield return groupNode;
                    }
                }
            }
        }

        /// <summary>
        /// Parses a set of command nodes from the given type.
        /// </summary>
        /// <param name="moduleType">The module type.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>A set of command nodes.</returns>
        private IEnumerable<CommandNode> GetModuleCommands(Type moduleType, IParentNode parent)
        {
            var methods = moduleType.GetMethods();
            foreach (var method in methods)
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute is null)
                {
                    continue;
                }

                yield return new CommandNode(parent, commandAttribute.Name, moduleType, method);
            }
        }
    }
}
