//
//  PaginatedEmbedFactory.cs
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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Results;

namespace Remora.Discord.Pagination;

/// <summary>
/// Factory class for creating paginated embeds from various sources.
/// </summary>
[PublicAPI]
public static class PaginatedEmbedFactory
{
    /// <summary>
    /// Creates a simple paginated list from a collection of items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="pageBuilder">A function that builds a page for a single value in the collection.</param>
    /// <param name="emptyCollectionDescription">The description to use when the collection is empty.</param>
    /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
    /// <returns>The paginated embed.</returns>
    public static IReadOnlyList<IEmbed> PagesFromCollection<TItem>
    (
        IReadOnlyList<TItem> items,
        Func<TItem, Embed> pageBuilder,
        string emptyCollectionDescription = "There's nothing here."
    )
    {
        List<Embed> pages = new();
        if (!items.Any())
        {
            var eb = new Embed
            {
                Description = emptyCollectionDescription
            };

            pages.Add(eb);
        }
        else
        {
            pages.AddRange(items.Select(pageBuilder));
        }

        return pages;
    }

    /// <summary>
    /// Creates a simple paginated list from a collection of items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="pageBuilder">A function that builds a page for a single value in the collection.</param>
    /// <param name="emptyCollectionDescription">The description to use when the collection is empty.</param>
    /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
    /// <returns>The paginated embed.</returns>
    public static async Task<IReadOnlyList<Result<Embed>>> PagesFromCollectionAsync<TItem>
    (
        IReadOnlyList<TItem> items,
        Func<TItem, Task<Result<Embed>>> pageBuilder,
        string emptyCollectionDescription = "There's nothing here."
    )
    {
        List<Result<Embed>> pages = new();
        if (!items.Any())
        {
            var eb = new Embed
            {
                Description = emptyCollectionDescription
            };

            pages.Add(eb);
        }
        else
        {
            pages.AddRange(await Task.WhenAll(items.Select(async i => await pageBuilder(i))));
        }

        return pages;
    }

    /// <summary>
    /// Creates a simple paginated list from a collection of items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="titleSelector">A function that selects the title for each field.</param>
    /// <param name="valueSelector">A function that selects the value for each field.</param>
    /// <param name="emptyCollectionDescription">The description to use when the collection is empty.</param>
    /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
    /// <returns>The paginated embed.</returns>
    public static IReadOnlyList<Embed> SimpleFieldsFromCollection<TItem>
    (
        IReadOnlyList<TItem> items,
        Func<TItem, string> titleSelector,
        Func<TItem, string> valueSelector,
        string emptyCollectionDescription = "There's nothing here."
    )
    {
        List<Embed> pages = new();
        if (!items.Any())
        {
            var eb = new Embed
            {
                Description = emptyCollectionDescription
            };

            pages.Add(eb);
        }
        else
        {
            var fields = items.Select
            (
                i =>
                    new EmbedField
                    (
                        string.IsNullOrWhiteSpace(titleSelector(i)) ? "Not set" : titleSelector(i),
                        string.IsNullOrWhiteSpace(valueSelector(i)) ? "Not set" : valueSelector(i)
                    )
            );

            pages.AddRange(PageFactory.FromFields(fields));
        }

        return pages;
    }
}
