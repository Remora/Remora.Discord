//
//  BoundCommandNode.cs
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
using Remora.Commands.Trees.Nodes;

namespace Remora.Commands.Signatures
{
    /// <summary>
    /// Represents a command node that has been bound to a set of tokens.
    /// </summary>
    public class BoundCommandNode
    {
        /// <summary>
        /// Gets the base node.
        /// </summary>
        public CommandNode Node { get; }

        /// <summary>
        /// Gets the bound parameters.
        /// </summary>
        public IReadOnlyList<BoundParameterShape> BoundParameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundCommandNode"/> class.
        /// </summary>
        /// <param name="node">The command node.</param>
        /// <param name="boundParameters">The bound parameters.</param>
        public BoundCommandNode(CommandNode node, IReadOnlyList<BoundParameterShape> boundParameters)
        {
            this.Node = node;
            this.BoundParameters = boundParameters;
        }
    }
}
