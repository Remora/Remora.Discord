//
//  LocalizationProviderExtensions.cs
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
using System.Linq;
using Remora.Discord.Commands.Services;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="ILocalizationProvider"/> interface.
/// </summary>
public static class LocalizationProviderExtensions
{
    /// <summary>
    /// Gets a mapping of all available localized values for the given input.
    /// </summary>
    /// <param name="provider">The localization provider.</param>
    /// <param name="value">The input value.</param>
    /// <returns>The available localized strings, mapped to the names of their locales.</returns>
    public static IReadOnlyDictionary<string, string> GetStrings(this ILocalizationProvider provider, string value)
    {
        return provider.GetTranslations(value).ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value);
    }
}
