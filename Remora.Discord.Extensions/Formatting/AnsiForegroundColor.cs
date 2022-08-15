//
//  AnsiForegroundColor.cs
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

namespace Remora.Discord.Extensions.Formatting;

/// <summary>
/// Discord supported ANSI foreground colors.
/// </summary>
/// <remarks>Discord ANSI colors are based on Solarized palette.</remarks>
[PublicAPI]
public enum AnsiForegroundColor
{
    /// <summary>
    /// A black foreground color.
    /// </summary>
    /// <remarks>Color Hex is #4f545c, when a background is set then #073642 is used.</remarks>
    Black = 30,

    /// <summary>
    /// A red foreground color.
    /// </summary>
    /// <remarks>Color Hex is #dc322f.</remarks>
    Red = 31,

    /// <summary>
    /// A green foreground color.
    /// </summary>
    /// <remarks>Color Hex is #859900.</remarks>
    Green = 32,

    /// <summary>
    /// A yellow foreground color.
    /// </summary>
    /// <remarks>Color Hex is #b58900.</remarks>
    Yellow = 33,

    /// <summary>
    /// A blue foreground color.
    /// </summary>
    /// <remarks>Color Hex is #268bd2.</remarks>
    Blue = 34,

    /// <summary>
    /// A magenta foreground color.
    /// </summary>
    /// <remarks>Color Hex is #d33682.</remarks>
    Magenta = 35,

    /// <summary>
    /// A cyan foreground color.
    /// </summary>
    /// <remarks>Color Hex is #2aa198.</remarks>
    Cyan = 36,

    /// <summary>
    /// A white foreground color.
    /// </summary>
    /// <remarks>Color Hex is #ffffff, when a background is set then #eee8d5 is used.</remarks>
    White = 37,

    /// <summary>
    /// No foreground color.
    /// </summary>
    None = 39
}
