//
//  NGetTextLocalizationProvider.cs
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
using NGettext;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Acts as a container and provider for localized string catalogues.
/// </summary>
/// <param name="LocalizationCatalogues">The available localization catalogues.</param>
internal record NGetTextLocalizationProvider(IReadOnlyDictionary<CultureInfo, Catalog> LocalizationCatalogues)
    : ILocalizationProvider
{
    /// <summary>
    /// Gets the translated string for the given culture.
    /// </summary>
    /// <param name="cultureInfo">The culture to get the translation for.</param>
    /// <param name="value">The string to translate.</param>
    /// <returns>The translated string, or null if no translation exists.</returns>
    public string? GetTranslation(CultureInfo cultureInfo, string value)
    {
        return this.LocalizationCatalogues.TryGetValue(cultureInfo, out var catalog)
            ? catalog.GetStringDefault(value, null)
            : value;
    }

    /// <inheritdoc/>
    public string GetTranslationOrDefault(CultureInfo cultureInfo, string value, string? defaultValue = null)
    {
        return this.LocalizationCatalogues.TryGetValue(cultureInfo, out var catalog)
            ? catalog.GetStringDefault(value, defaultValue ?? value)
            : value;
    }

    /// <summary>
    /// Gets a mapping of all available localized values for the given input.
    /// </summary>
    /// <remarks>
    /// If no translation exists for the given value, no dictionary member will be present for the culture.
    /// </remarks>
    /// <param name="value">The input value.</param>
    /// <returns>The available localized strings, mapped to their locales.</returns>
    public IReadOnlyDictionary<CultureInfo, string> GetTranslations(string value)
    {
        var dictionary = new Dictionary<CultureInfo, string>();
        foreach (var (cultureInfo, catalog) in this.LocalizationCatalogues)
        {
            var translation = catalog.GetStringDefault(value, null);
            if (translation is null)
            {
                continue;
            }

            dictionary.Add(cultureInfo, translation);
        }

        return dictionary;
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<CultureInfo, string> GetTranslationsOrDefault(string value, string? defaultValue = null)
    {
        var dictionary = new Dictionary<CultureInfo, string>();
        foreach (var (cultureInfo, catalog) in this.LocalizationCatalogues)
        {
            var translation = catalog.GetStringDefault(value, defaultValue ?? value);
            if (translation is null)
            {
                continue;
            }

            dictionary.Add(cultureInfo, translation);
        }

        return dictionary;
    }
}
