//
//  LocalizationProvider.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using NGettext;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Acts as a container and provider for localized string catalogues.
/// </summary>
/// <param name="LocalizationCatalogues">The available localization catalogues.</param>
public record LocalizationProvider(IReadOnlyDictionary<CultureInfo, ICatalog> LocalizationCatalogues)
{
    /// <summary>
    /// Gets a mapping of all available localized values for the given input.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The available localized strings, mapped to the names of their locales.</returns>
    public IReadOnlyDictionary<string, string> GetStrings(string value)
    {
        var dictionary = new Dictionary<string, string>();
        foreach (var (cultureInfo, catalog) in this.LocalizationCatalogues)
        {
            dictionary.Add(cultureInfo.Name, catalog.GetString(value));
        }

        return dictionary;
    }
}
