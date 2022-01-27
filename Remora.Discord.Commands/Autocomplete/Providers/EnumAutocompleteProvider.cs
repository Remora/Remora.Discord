//
//  EnumAutocompleteProvider.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Extensions;

namespace Remora.Discord.Commands.Autocomplete;

/// <summary>
/// Provides autocompletion suggestions for enums.
/// </summary>
/// <typeparam name="TEnum">The enumeration type.</typeparam>
public class EnumAutocompleteProvider<TEnum> : IAutocompleteProvider<TEnum>
    where TEnum : struct, Enum
{
    private readonly ILogger<EnumAutocompleteProvider<TEnum>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumAutocompleteProvider{TEnum}"/> class.
    /// </summary>
    /// <param name="logger">The logging instance for this type.</param>
    public EnumAutocompleteProvider(ILogger<EnumAutocompleteProvider<TEnum>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync
    (
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default
    )
    {
        var getChoices = EnumExtensions.GetEnumChoices<TEnum>();
        if (getChoices.IsDefined(out var choices))
        {
            return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
            (
                choices
                    .OrderByDescending(choice => Fuzz.Ratio(userInput, choice.Name))
                    .Take(25)
                    .ToList()
            );
        }

        _logger.LogWarning
        (
            "No autocomplete suggestions available for enumeration \"{TypeName}\": {Message}",
            typeof(TEnum).Name,
            getChoices.Error!.Message
        );

        return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
        (
            Array.Empty<IApplicationCommandOptionChoice>()
        );
    }
}
