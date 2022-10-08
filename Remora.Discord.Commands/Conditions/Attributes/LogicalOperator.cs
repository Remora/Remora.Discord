//
//  LogicalOperator.cs
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

namespace Remora.Discord.Commands.Conditions;

/// <summary>
/// Enumerates various logical operators.
/// </summary>
[PublicAPI]
public enum LogicalOperator
{
    /// <summary>
    /// AND, that is, all of the inputs must be logically true.
    /// </summary>
    And,

    /// <summary>
    /// NOT, that is, all of the inputs must be logically false.
    /// </summary>
    Not,

    /// <summary>
    /// OR, that is, one or more of the inputs must be logically true.
    /// </summary>
    Or,

    /// <summary>
    /// XOR, that is, one and only one of the inputs must be logically true.
    /// </summary>
    Xor
}
