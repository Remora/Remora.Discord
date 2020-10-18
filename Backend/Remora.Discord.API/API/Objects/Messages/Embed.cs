//
//  Embed.cs
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
using System.Drawing;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class Embed : IEmbed
    {
        /// <inheritdoc />
        public Optional<string> Title { get; }

        /// <inheritdoc />
        public Optional<EmbedType> Type { get; }

        /// <inheritdoc />
        public Optional<string> Description { get; }

        /// <inheritdoc />
        public Optional<string> Url { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset> Timestamp { get; }

        /// <inheritdoc />
        public Optional<Color> Colour { get; }

        /// <inheritdoc />
        public Optional<IEmbedFooter> Footer { get; }

        /// <inheritdoc />
        public Optional<IEmbedImage> Image { get; }

        /// <inheritdoc />
        public Optional<IEmbedThumbnail> Thumbnail { get; }

        /// <inheritdoc />
        public Optional<IEmbedVideo> Video { get; }

        /// <inheritdoc />
        public Optional<IEmbedProvider> Provider { get; }

        /// <inheritdoc />
        public Optional<IEmbedAuthor> Author { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyList<IEmbedField>> Fields { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Embed"/> class.
        /// </summary>
        /// <param name="title">The title of the embed.</param>
        /// <param name="type">The type of embed.</param>
        /// <param name="description">The embed description.</param>
        /// <param name="url">The embed URL.</param>
        /// <param name="timestamp">The embed timestamp.</param>
        /// <param name="colour">The embed colour.</param>
        /// <param name="footer">The footer of the embed.</param>
        /// <param name="image">The image of the embed.</param>
        /// <param name="thumbnail">The thumbnail of the embed.</param>
        /// <param name="video">The video of the embed.</param>
        /// <param name="provider">The embed provider.</param>
        /// <param name="author">The embed author.</param>
        /// <param name="fields">The fields in the embed.</param>
        public Embed
        (
            Optional<string> title = default,
            Optional<EmbedType> type = default,
            Optional<string> description = default,
            Optional<string> url = default,
            Optional<DateTimeOffset> timestamp = default,
            Optional<Color> colour = default,
            Optional<IEmbedFooter> footer = default,
            Optional<IEmbedImage> image = default,
            Optional<IEmbedThumbnail> thumbnail = default,
            Optional<IEmbedVideo> video = default,
            Optional<IEmbedProvider> provider = default,
            Optional<IEmbedAuthor> author = default,
            Optional<IReadOnlyList<IEmbedField>> fields = default
        )
        {
            this.Title = title;
            this.Type = type;
            this.Description = description;
            this.Url = url;
            this.Timestamp = timestamp;
            this.Colour = colour;
            this.Footer = footer;
            this.Image = image;
            this.Thumbnail = thumbnail;
            this.Video = video;
            this.Provider = provider;
            this.Author = author;
            this.Fields = fields;
        }
    }
}
