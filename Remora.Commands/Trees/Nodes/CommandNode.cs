//
//  CommandNode.cs
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
using JetBrains.Annotations;

namespace Remora.Commands.Trees.Nodes
{
    /// <summary>
    /// Represents a command in a command group.
    /// </summary>
    [PublicAPI]
    public class CommandNode : IChildNode
    {
        /// <summary>
        /// Gets the module type that the command is in.
        /// </summary>
        public Type ModuleType { get; }

        /// <summary>
        /// Gets the method that the command invokes.
        /// </summary>
        public MethodInfo CommandMethod { get; }

        /// <inheritdoc/>
        public IParentNode Parent { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// This key value represents the name of the command, which terminates the command prefix.
        /// </remarks>
        public string Key { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNode"/> class.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="key">The key value of the command node.</param>
        /// <param name="moduleType">The module type that the command is in.</param>
        /// <param name="commandMethod">The method that the command invokes.</param>
        public CommandNode(IParentNode parent, string key, Type moduleType, MethodInfo commandMethod)
        {
            this.Parent = parent;
            this.Key = key;
            this.ModuleType = moduleType;
            this.CommandMethod = commandMethod;
        }
    }
}
