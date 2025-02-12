//
//  EnumAutocompleteProvider.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;

namespace Remora.Discord.Commands.Autocomplete;

/// <summary>
/// Provides autocompletion suggestions for enums.
/// </summary>
/// <typeparam name="TEnum">The enumeration type.</typeparam>
[PublicAPI]
public class EnumAutocompleteProvider<TEnum> : IAutocompleteProvider<TEnum>
    where TEnum : struct, Enum
{
    private readonly ILocalizationProvider _localizationProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumAutocompleteProvider{TEnum}"/> class.
    /// </summary>
    /// <param name="localizationProvider">The localization provider.</param>
    public EnumAutocompleteProvider(ILocalizationProvider localizationProvider)
    {
        _localizationProvider = localizationProvider;
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync
    (
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default
    )
    {
        var choices = EnumExtensions.GetEnumChoices<TEnum>(_localizationProvider);
        return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
        (
            choices
                .OrderByDescending(choice => Fuzz.Ratio(userInput, choice.Name))
                .Take(25)
                .ToList()
        );
    }
}
