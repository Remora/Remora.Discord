//
//  AnsiBackgroundColor.cs
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
/// Discord supported ANSI background colors.
/// </summary>
/// <remarks>Discord ANSI colors are based on Solarized palette.</remarks>
[PublicAPI]
public enum AnsiBackgroundColor
{
    /// <summary>
    /// A base03 background color from Solarized palette.
    /// </summary>
    /// <remarks>Color Hex is #002b36.</remarks>
    Base03 = 40,

    /// <summary>
    /// A orange background color.
    /// </summary>
    /// <remarks>Color Hex is #cb4b16.</remarks>
    Orange = 41,

    /// <summary>
    /// A base01 background color from Solarized palette.
    /// </summary>
    /// <remarks>Color Hex is #586e75.</remarks>
    Base01 = 42,

    /// <summary>
    /// A base00 background color from Solarized palette.
    /// </summary>
    /// <remarks>Color Hex is #657b83.</remarks>
    Base00 = 43,

    /// <summary>
    /// A base0 background color from Solarized palette.
    /// </summary>
    /// <remarks>Color Hex is #839496.</remarks>
    Base0 = 44,

    /// <summary>
    /// A violet background color.
    /// </summary>
    /// <remarks>Color Hex is #6c71c4.</remarks>
    Violet = 45,

    /// <summary>
    /// A base1 background color from Solarized palette.
    /// </summary>
    /// <remarks>Color Hex is #93a1a1.</remarks>
    Base1 = 46,

    /// <summary>
    /// A base3 background color from Solarized palette.
    /// </summary>
    /// <remarks>Color Hex is #fdf6e3.</remarks>
    Base3 = 47,

    /// <summary>
    /// No background color.
    /// </summary>
    None = 49
}
