//
//  VerificationLevel.cs
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
/// Enumerates the various verification levels.
/// </summary>
[PublicAPI]
public enum VerificationLevel
{
    /// <summary>
    /// No restrictions.
    /// </summary>
    None = 0,

    /// <summary>
    /// Users must have a verified email.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Users must have been registered longer than 5 minutes.
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Users must be a member of the server for longer than 10 minutes.
    /// </summary>
    High = 3,

    /// <summary>
    /// Users must have a verified phone number.
    /// </summary>
    VeryHigh = 4
}
