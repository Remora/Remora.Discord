//
//  TypeHint.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Attributes;

/// <summary>
/// Enumerates supported type hints for slash command parameters. The values in this enum map to
/// <see cref="ApplicationCommandOptionType"/>, and can be considered a subset of that enum.
/// </summary>
[PublicAPI]
public enum TypeHint
{
    /// <summary>
    /// The option is a string.
    /// </summary>
    String = 3,

    /// <summary>
    /// The option is an integer.
    /// </summary>
    Integer = 4,

    /// <summary>
    /// The option is a boolean.
    /// </summary>
    Boolean = 5,

    /// <summary>
    /// The option is a user reference.
    /// </summary>
    User = 6,

    /// <summary>
    /// The option is a channel reference.
    /// </summary>
    Channel = 7,

    /// <summary>
    /// The option is a role reference.
    /// </summary>
    Role = 8,

    /// <summary>
    /// The option is some type of mentionable object (member, role, channel, etc).
    /// </summary>
    Mentionable = 9,

    /// <summary>
    /// The option is a floating-point number (double precision).
    /// </summary>
    Number = 10
}
