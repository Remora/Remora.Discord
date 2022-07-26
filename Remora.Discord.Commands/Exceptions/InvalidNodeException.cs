//
//  InvalidNodeException.cs
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
using JetBrains.Annotations;
using OneOf;
using Remora.Commands.Trees.Nodes;

namespace Remora.Discord.Commands;

/// <summary>
/// Represents a fault related to a command tree node that was considered invalid for some reason in some context.
/// </summary>
[PublicAPI]
public class InvalidNodeException : Exception
{
    /// <summary>
    /// Gets the node that caused the exception.
    /// </summary>
    public OneOf<RootNode, IChildNode> Node { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidNodeException"/> class.
    /// </summary>
    /// <param name="message">The user-facing message, if any.</param>
    /// <param name="node">The node that caused the exception, if any.</param>
    /// <param name="innerException">The exception that caused this exception, if any.</param>
    public InvalidNodeException
    (
        string message,
        IChildNode node,
        Exception? innerException = default
    )
        : base(message, innerException)
    {
        this.Node = OneOf<RootNode, IChildNode>.FromT1(node);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidNodeException"/> class.
    /// </summary>
    /// <param name="message">The user-facing message, if any.</param>
    /// <param name="node">The node that caused the exception, if any.</param>
    /// <param name="innerException">The exception that caused this exception, if any.</param>
    public InvalidNodeException
    (
        string message,
        RootNode node,
        Exception? innerException = default
    )
        : base(message, innerException)
    {
        this.Node = node;
    }
}
