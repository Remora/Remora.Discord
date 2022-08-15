//
//  PageFactory.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Pagination.Extensions;

namespace Remora.Discord.Pagination;

/// <summary>
/// Factory class for creating page collections from various sources.
/// </summary>
[PublicAPI]
public static class PageFactory
{
    /// <summary>
    /// Creates a set of embed pages from a collection of embed fields.
    /// </summary>
    /// <param name="fields">The fields to paginate.</param>
    /// <param name="maxFieldsPerPage">The maximum number of embed fields per page.</param>
    /// <param name="description">The description to display on each page.</param>
    /// <param name="pageBase">The base layout for the page.</param>
    /// <returns>The paginated embed.</returns>
    public static List<Embed> FromFields
    (
        IEnumerable<IEmbedField> fields,
        uint maxFieldsPerPage = 5,
        string description = "",
        Embed? pageBase = null
    )
    {
        pageBase ??= new Embed
        {
            Description = description
        };

        var pageBaseLength = pageBase.CalculateEmbedLength();
        var enumeratedFields = fields.ToList();

        // Build the pages
        var pages = new List<Embed>();
        var currentPageFields = new List<IEmbedField>();
        if (pageBase.Fields.HasValue)
        {
            currentPageFields.AddRange(pageBase.Fields.Value);
        }

        foreach (var field in enumeratedFields)
        {
            var fieldContentLength = field.Name.Length + field.Value.Length;
            if (currentPageFields.Count >= maxFieldsPerPage || pageBaseLength + fieldContentLength >= 1300)
            {
                pages.Add(pageBase with { Fields = new List<IEmbedField>(currentPageFields) });
                currentPageFields.Clear();
            }

            currentPageFields.Add(field);
        }

        if (currentPageFields.Count <= 0)
        {
            return pages;
        }

        // Stick the remaining ones on the end
        pages.Add(pageBase with { Fields = new List<IEmbedField>(currentPageFields) });
        currentPageFields.Clear();

        return pages;
    }
}
