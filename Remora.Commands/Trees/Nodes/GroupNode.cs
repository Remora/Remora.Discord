//
//  GroupNode.cs
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

namespace Remora.Commands.Trees.Nodes
{
    /// <summary>
    /// Represents a command group.
    /// </summary>
    /// <remarks>
    /// Command groups may contain either other groups for deeper nesting, or leaf nodes in the form of commands.
    /// </remarks>
    [PublicAPI]
    public class GroupNode : IParentNode, IChildNode
    {
        /// <inheritdoc/>
        public IReadOnlyList<IChildNode> Children { get; }

        /// <inheritdoc/>
        public IParentNode Parent { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// This key represents the name of the group, from which command prefixes are formed.
        /// </remarks>
        public string Key { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupNode"/> class.
        /// </summary>
        /// <param name="children">The child nodes of the group node.</param>
        /// <param name="parent">The parent of the group node.</param>
        /// <param name="key">The key value for the group node.</param>
        public GroupNode(IReadOnlyList<IChildNode> children, IParentNode parent, string key)
        {
            this.Children = children;
            this.Parent = parent;
            this.Key = key;
        }
    }
}
