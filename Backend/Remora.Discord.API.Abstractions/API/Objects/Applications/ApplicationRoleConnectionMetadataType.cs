//
//  ApplicationRoleConnectionMetadataType.cs
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
/// Enumerates various application role connection metadata types.
/// </summary>
[PublicAPI]
public enum ApplicationRoleConnectionMetadataType
{
    /// <summary>
    /// The metadata value (integer) is less than or equal to the guild's configured value (integer).
    /// </summary>
    IntegerLessThanOrEqual = 1,

    /// <summary>
    /// The metadata value (integer) is greater than or equal to the guild's configured value (integer).
    /// </summary>
    IntegerGreaterThanOrEqual = 2,

    /// <summary>
    /// The metadata value (integer) is equal to the guild's configured value (integer).
    /// </summary>
    IntegerEqual = 3,

    /// <summary>
    /// The metadata value (integer) is not equal to the guild's configured value (integer).
    /// </summary>
    IntegerNotEqual = 4,

    /// <summary>
    /// The metadata value (ISO8601 string) is less than or equal to the guild's configured value (integer; days before current date).
    /// </summary>
    DateTimeLessThanOrEqual = 5,

    /// <summary>
    /// The metadata value (ISO8601 string) is greater than or equal to the guild's configured value (integer; days before current date).
    /// </summary>
    DateTimeGreaterThanOrEqual = 6,

    /// <summary>
    /// The metadata value (integer) is equal to the guild's configured value (integer; 1).
    /// </summary>
    BooleanEqual = 7,

    /// <summary>
    /// The metadata value (integer) is not equal to the guild's configured value (integer; 1).
    /// </summary>
    BooleanNotEqual = 8,
}
