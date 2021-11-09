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
using Humanizer;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace Remora.Discord.Commands.Autocomplete;

/// <summary>
/// Provides autocompletion suggestions for enums.
/// </summary>
/// <typeparam name="TEnum">The enumeration type.</typeparam>
public class EnumAutocompleteProvider<TEnum> : IAutocompleteProvider<TEnum>
    where TEnum : struct, Enum
{
    private static readonly string[] Names = Enum.GetNames(typeof(TEnum));

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync
    (
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default
    )
    {
        return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>
        (
            Names
                .OrderByDescending(n => Fuzz.Ratio(userInput, n))
                .Take(25)
                .Select(n => new ApplicationCommandOptionChoice(n.Humanize().Transform(To.TitleCase), n))
                .ToList()
        );
    }
}
