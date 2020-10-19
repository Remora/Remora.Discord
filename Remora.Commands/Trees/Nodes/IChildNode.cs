//
//  IChildNode.cs
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

using JetBrains.Annotations;

namespace Remora.Commands.Trees.Nodes
{
    /// <summary>
    /// Defines the public interface of a child node.
    /// </summary>
    [PublicAPI]
    public interface IChildNode
    {
        /// <summary>
        /// Gets the parent of this node.
        /// </summary>
        public IParentNode Parent { get; }

        /// <summary>
        /// Gets the key value of this node. This value is not guaranteed to be unique among its siblings.
        /// </summary>
        public string Key { get; }
    }
}
