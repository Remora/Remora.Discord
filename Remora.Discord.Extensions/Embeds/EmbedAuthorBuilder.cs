//
//  EmbedAuthorBuilder.cs
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

using System.ComponentModel.DataAnnotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Extensions.Embeds
{
    /// <summary>
    /// A builder which validates and builds an <see cref="EmbedAuthor"/>.
    /// </summary>
    public sealed class EmbedAuthorBuilder : BuilderBase<EmbedAuthor>
    {
        /// <summary>
        /// Gets or sets the author's name.
        /// </summary>
        [Required]
        [MaxLength(EmbedConstants.MaxAuthorNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the author's website.
        /// </summary>
        [Url]
        public Optional<string> Url { get; set; }

        /// <summary>
        /// Gets or sets the author's icon url.
        /// </summary>
        [Url]
        public Optional<string> IconUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedAuthorBuilder"/> class.
        /// </summary>
        /// <param name="name">The author's name.</param>
        /// <param name="url">The author's website.</param>
        /// <param name="iconUrl">The author's icon url.</param>
        public EmbedAuthorBuilder(string name, Optional<string> url = default, Optional<string> iconUrl = default)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
        }

        /// <inheritdoc />
        public override Result<EmbedAuthor> Build()
        {
            var validationResult = this.Validate();

            return validationResult.IsSuccess
                ? new EmbedAuthor(Name, Url, IconUrl)
                : Result<EmbedAuthor>.FromError(validationResult);
        }

        /// <summary>
        /// Converts an existing embed author into an <see cref="EmbedAuthorBuilder"/>.
        /// </summary>
        /// <param name="author">The author of the embed.</param>
        /// <returns>A new <see cref="EmbedAuthorBuilder"/> based on the provided author.</returns>
        public static EmbedAuthorBuilder FromAuthor(IEmbedAuthor author)
            => new(author.Name, author.Url, author.IconUrl);
    }
}
