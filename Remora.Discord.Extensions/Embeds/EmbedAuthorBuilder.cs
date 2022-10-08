//
//  EmbedAuthorBuilder.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Extensions.Embeds;

/// <summary>
/// A builder which validates and builds an <see cref="EmbedAuthor"/>.
/// </summary>
[PublicAPI]
public sealed class EmbedAuthorBuilder : BuilderBase<EmbedAuthor>
{
    /// <summary>
    /// Gets or sets the author's name. Must be shorter than or equal to <see cref="EmbedConstants.MaxAuthorNameLength"/> in length.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the author's website. Provide <c>null</c> if no url is needed.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the author's icon url. Provide <c>null</c> if no url is needed.
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedAuthorBuilder"/> class.
    /// </summary>
    /// <param name="name">The author's name.</param>
    /// <param name="url">The author's website.</param>
    /// <param name="iconUrl">The author's icon url.</param>
    public EmbedAuthorBuilder(string name, string? url = null, string? iconUrl = null)
    {
        this.Name = name;
        this.Url = url;
        this.IconUrl = iconUrl;
    }

    /// <inheritdoc />
    public override Result<EmbedAuthor> Build()
    {
        var validationResult = Validate();

        return validationResult.IsSuccess
            ? new EmbedAuthor(this.Name, this.Url ?? default(Optional<string>), this.IconUrl ?? default(Optional<string>))
            : Result<EmbedAuthor>.FromError(validationResult);
    }

    /// <summary>
    /// Converts an existing embed author into an <see cref="EmbedAuthorBuilder"/>.
    /// </summary>
    /// <param name="author">The author of the embed.</param>
    /// <returns>A new <see cref="EmbedAuthorBuilder"/> based on the provided author.</returns>
    public static EmbedAuthorBuilder FromAuthor(IEmbedAuthor author)
        => new(author.Name, author.Url.HasValue ? author.Url.Value : null, author.IconUrl.HasValue ? author.Url.Value : null);

    /// <inheritdoc/>
    public override Result Validate()
    {
        var nameResult = ValidateLength(nameof(this.Name), this.Name, EmbedConstants.MaxAuthorNameLength, false);
        if (!nameResult.IsSuccess)
        {
            return nameResult;
        }

        var urlResult = ValidateUrl(nameof(this.Url), this.Url, true);
        if (!urlResult.IsSuccess)
        {
            return urlResult;
        }

        var iconResult = ValidateUrl(nameof(this.IconUrl), this.IconUrl, true);
        return iconResult;
    }
}
