//
//  NullLocalizationProvider.cs
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

using System.Collections.Generic;
using System.Globalization;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Defines a localization provider that merely returns the input.
/// </summary>
internal class NullLocalizationProvider : ILocalizationProvider
{
    /// <inheritdoc />
    public string GetTranslation(CultureInfo cultureInfo, string value)
    {
        return value;
    }

    /// <inheritdoc/>
    public string GetTranslationOrDefault(CultureInfo cultureInfo, string value, string? defaultValue = null)
    {
        return value;
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<CultureInfo, string> GetTranslations(string value)
    {
        return new Dictionary<CultureInfo, string>();
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<CultureInfo, string> GetTranslationsOrDefault(string value, string? defaultValue = null)
    {
        return new Dictionary<CultureInfo, string>();
    }
}
