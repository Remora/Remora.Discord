//
//  StringExtensions.cs
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

using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods to the <see cref="string"/> type.
/// </summary>
[PublicAPI]
public static class StringExtensions
{
    private static readonly Regex _unmentionRegex = new("(\\d+)>$", RegexOptions.Compiled);

    /// <summary>
    /// Removes Discord mention markdown from a string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The unmentioned string.</returns>
    public static string Unmention(this string value)
    {
        if (value.Length <= 0)
        {
            return value;
        }

        if (char.IsDigit(value[0]))
        {
            return value;
        }

        var regexMatches = _unmentionRegex.Match(value);
        return !regexMatches.Success
            ? value
            : regexMatches.Groups[1].Value;
    }
}
