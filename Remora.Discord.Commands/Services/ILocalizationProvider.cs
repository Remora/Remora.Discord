//
//  ILocalizationProvider.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Represents the public API of a service that can provide translated strings.
/// </summary>
[PublicAPI]
public interface ILocalizationProvider
{
    /// <summary>
    /// Gets the translated string for the given culture.
    /// </summary>
    /// <param name="cultureInfo">The culture to get the translation for.</param>
    /// <param name="value">The string to translate.</param>
    /// <returns>The translated string, or the input if no translation exists.</returns>
    string? GetTranslation(CultureInfo cultureInfo, string value);

    /// <summary>
    /// Gets the translated string for the given culture.
    /// </summary>
    /// <param name="cultureInfo">The culture to get the translation for.</param>
    /// <param name="value">The string to translate.</param>
    /// <param name="defaultValue">
    /// The default value to return. If no value is provided, the original input will be used.
    /// </param>
    /// <returns>The translated string, or the default if no translation exists.</returns>
    string GetTranslationOrDefault(CultureInfo cultureInfo, string value, string? defaultValue = null);

    /// <summary>
    /// Gets a mapping of all available localized values for the given input.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The available localized strings, mapped to their locales.</returns>
    IReadOnlyDictionary<CultureInfo, string> GetTranslations(string value);

    /// <summary>
    /// Gets a mapping of all available localized values for the given input, or the default value if no translation
    /// exists.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="defaultValue">
    /// The default value to return. If no value is provided, the original input will be used.
    /// </param>
    /// <returns>The available localized strings, mapped to their locales.</returns>
    IReadOnlyDictionary<CultureInfo, string> GetTranslationsOrDefault(string value, string? defaultValue = null);
}
