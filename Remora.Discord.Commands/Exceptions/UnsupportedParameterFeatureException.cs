//
//  UnsupportedParameterFeatureException.cs
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
using Remora.Commands.Signatures;
using Remora.Commands.Trees.Nodes;

namespace Remora.Discord.Commands;

/// <summary>
/// Represents a failure to create a slash command based on an unsupported feature in a command parameter.
/// </summary>
[PublicAPI]
public class UnsupportedParameterFeatureException : UnsupportedFeatureException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedParameterFeatureException"/> class.
    /// </summary>
    /// <param name="message">The user-facing message, if any.</param>
    /// <param name="command">The command node that caused the exception.</param>
    /// <param name="parameter">The parameter that caused the exception.</param>
    /// <param name="innerException">The exception that caused this exception, if any.</param>
    public UnsupportedParameterFeatureException
    (
        string message,
        CommandNode command,
        IParameterShape parameter,
        Exception? innerException = default
    )
        : base(message, command, innerException)
    {
    }
}
