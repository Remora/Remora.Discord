//
//  IAutocompleteProvider.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

#pragma warning disable SA1402

namespace Remora.Discord.Commands.Autocomplete;

/// <summary>
/// Glue interface for type-specific autocomplete providers.
/// </summary>
/// <typeparam name="T">The type the provider suggests autocompletion for.</typeparam>
[PublicAPI]
public interface IAutocompleteProvider<T> : IAutocompleteProvider
{
    /// <summary>
    /// Gets the type the provider suggests autocompletion for.
    /// </summary>
    Type Type => typeof(T);

    /// <inheritdoc/>
    string IAutocompleteProvider.Identity => $"autocomplete::type::{this.Type.FullName ?? this.Type.Name}";
}

/// <summary>
/// Represents the public interface of an autocomplete provider.
/// </summary>
[PublicAPI]
public interface IAutocompleteProvider
{
    /// <summary>
    /// Gets the identity of the autocomplete provider.
    /// </summary>
    string Identity { get; }

    /// <summary>
    /// Gets a set of autocomplete suggestions based on provided user input.
    /// </summary>
    /// <param name="options">The other options in the command being invoked.</param>
    /// <param name="userInput">The user's current input.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The suggested options.</returns>
    ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync
    (
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default
    );
}
