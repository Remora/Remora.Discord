//
//  RootNode.cs
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
    /// Represents the root of a command tree.
    /// </summary>
    [PublicAPI]
    public class RootNode : IParentNode
    {
        /// <inheritdoc/>
        public IReadOnlyList<IChildNode> Children { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RootNode"/> class.
        /// </summary>
        /// <param name="children">The children of this node.</param>
        public RootNode(IReadOnlyList<IChildNode> children)
        {
            this.Children = children;
        }
    }
}
