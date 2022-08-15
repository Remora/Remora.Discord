//
//  ApplicationCommandTypeExtensions.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extensions for the <see cref="ApplicationCommandType"/> interface.
/// </summary>
[PublicAPI]
public static class ApplicationCommandTypeExtensions
{
    /// <summary>
    /// Maps the <see cref="ApplicationCommandType" /> to a command parameter name.
    /// </summary>
    /// <param name="commandType">The command type.</param>
    /// <returns>The parameter type the <see cref="ApplicationCommandType"/> is mapped to.</returns>
    /// <exception cref="NotSupportedException">Thrown if the <see cref="ApplicationCommandType"/> is not supported.</exception>
    public static string AsParameterName(this ApplicationCommandType commandType)
    {
        return commandType switch {
            ApplicationCommandType.Message => "message",
            ApplicationCommandType.User => "user",
            _ => throw new NotSupportedException($"Command type {commandType} is not supported as parameter name")
        };
    }
}
