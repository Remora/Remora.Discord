//
//  ApplicationCommandOptionType.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various option types.
/// </summary>
[PublicAPI]
public enum ApplicationCommandOptionType
{
    /// <summary>
    /// The option is not a value; rather, it is a subcommand.
    /// </summary>
    SubCommand = 1,

    /// <summary>
    /// The option is not a value; rather, it is a subgroup.
    /// </summary>
    SubCommandGroup = 2,

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
    Number = 10,

    /// <summary>
    /// The option is an attachment (an image, document, etc).
    /// </summary>
    Attachment = 11
}
