//
//  EmbedBuilder.cs
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
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Extensions.Embeds
{
    /// <summary>
    /// Provides utilities for building an embed.
    /// </summary>
    public class EmbedBuilder
    {
        /// <summary>
        /// Gets or sets the title of the embed.
        /// </summary>
        [MaxLength(Constants.MaxTitleLength)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets the type of embed. Only <see cref="EmbedType.Rich"/> is available for embeds created by bots.
        /// </summary>
        public EmbedType Type { get; } = EmbedType.Rich;

        /// <summary>
        /// Gets or sets the description of the embed.
        /// </summary>
        [MaxLength(Constants.MaxDescriptionLength)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the url of the embed.
        /// </summary>
        [Url]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp, if any, which should be displayed on the embed.
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; } = null;

        /// <summary>
        /// Gets or sets the color of this embed.
        /// </summary>
        public Color Colour { get; set; } = Constants.DefaultColour;

        /// <summary>
        /// Gets or sets the footer of this embed.
        /// </summary>
        public IEmbedFooter? Footer { get; set; } = null;

        /// <summary>
        /// Gets or sets the image added to this embed.
        /// </summary>
        public IEmbedImage? Image { get; set; } = null;

        /// <summary>
        /// Gets or sets the thumbnail added to this embed.
        /// </summary>
        public IEmbedThumbnail? Thumbnail { get; set; } = null;

        /// <summary>
        /// Gets or sets the author of the embed.
        /// </summary>
        public IEmbedAuthor? Author { get; set; } = null;

        /// <summary>
        /// Gets a read-only list of fields added to this embed.
        /// </summary>
        public IReadOnlyList<IEmbedField> Fields => _fields.AsReadOnly();

        private List<IEmbedField> _fields;

        /// <summary>
        /// Gets the overall length of this embed.
        /// </summary>
        public int Length
        {
            get
            {
                int titleLength = Title.Length;
                int descriptionLength = Description.Length;
                int fieldSum = _fields.Sum(field => field.Name.Length + field.Value.Length);
                int footerLength = Footer?.Text.Length ?? 0;
                int authorLength = Author?.Name.Length ?? 0;

                return titleLength + descriptionLength + fieldSum + footerLength + authorLength;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedBuilder"/> class.
        /// </summary>
        public EmbedBuilder()
            : this(new List<IEmbedField>(Constants.MaxFieldCount))
        {
        }

        private EmbedBuilder(Optional<IReadOnlyList<IEmbedField>> fields)
            : this(fields.HasValue ? new List<IEmbedField>(fields.Value) : new List<IEmbedField>(Constants.MaxFieldCount))
        {
        }

        private EmbedBuilder(List<IEmbedField> fields)
        {
            _fields = fields;
        }

        /// <summary>
        /// Ensures that the overall length of the embed is less than the value of <see cref="Constants.MaxEmbedLength"/>.
        /// </summary>
        /// <returns>Returns a <see cref="Result"/> indicating success or failure of the validation.</returns>
        public Result Ensure()
            => Length < Constants.MaxEmbedLength
            ? Result.FromSuccess()
            : new EmbedError("Embed is too long.");

        /// <summary>
        /// Adds the specified title to this <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        /// Adds the specified description to this <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Adds the specified url to this <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithUrl(string url)
        {
            Url = url;
            return this;
        }

        /// <summary>
        /// Adds a thumbnail with the specified url to this <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="thumbnailUrl">The url of the thumbnail.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithThumbnailUrl(string thumbnailUrl)
        {
            Thumbnail = new EmbedThumbnail(Url: thumbnailUrl);
            return this;
        }

        /// <summary>
        /// Adds an image with the specified url to this <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="imageUrl">The url of the thumbnail.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithImageUrl(string imageUrl)
        {
            Image = new EmbedImage(Url: imageUrl);
            return this;
        }

        /// <summary>
        /// Sets the timestamp of the <see cref="EmbedBuilder"/> to <see cref="DateTimeOffset.UtcNow"/>.
        /// </summary>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithCurrentTimestamp()
        {
            Timestamp = DateTimeOffset.UtcNow;
            return this;
        }

        /// <summary>
        /// Sets the timestamp of the <see cref="EmbedBuilder"/> to the specified timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithTimestamp(DateTimeOffset timestamp)
        {
            Timestamp = timestamp;
            return this;
        }

        /// <summary>
        /// Sets the timestamp of the <see cref="EmbedBuilder"/> to the timestamp specified by the <paramref name="snowflake"/>.
        /// </summary>
        /// <param name="snowflake">The snowflake.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithTimestamp(Snowflake snowflake)
        {
            Timestamp = snowflake.Timestamp;
            return this;
        }

        /// <summary>
        /// Sets the color of the <see cref="EmbedBuilder"/> to the specified color.
        /// </summary>
        /// <param name="colour">The color.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithColour(Color colour)
        {
            Colour = colour;
            return this;
        }

        /// <summary>
        /// Adds an <see cref="EmbedAuthor"/> the <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="name">The author's name.</param>
        /// <param name="url">The author's website.</param>
        /// <param name="iconUrl">The url of the author's icon.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithAuthor
        (
            [MaxLength(Constants.MaxAuthorNameLength)] string name,
            [Url] string url = "",
            [Url] string iconUrl = ""
        )
        {
            Author = new EmbedAuthor(name, url, iconUrl);
            return this;
        }

        /// <summary>
        /// Adds the specified <see cref="IUser"/> as the author of the <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithAuthor(IUser user)
        {
            var avatarUrlResult = CDN.GetUserAvatarUrl(user, imageSize: 256);

            var avatarUrl = avatarUrlResult.IsSuccess
                ? avatarUrlResult.Entity
                : CDN.GetDefaultUserAvatarUrl(user, imageSize: 256).Entity;

            Author = new EmbedAuthor($"{user.Username}${user.Discriminator}", IconUrl: avatarUrl.AbsoluteUri);
            return this;
        }

        /// <summary>
        /// Adds an embed footer to the <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="text">The text of the footer.</param>
        /// <param name="iconUrl">The url of the icon.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithFooter([MaxLength(Constants.MaxFooterTextLength)] string text, [Url] string iconUrl = "")
        {
            Footer = new EmbedFooter(text, iconUrl);
            return this;
        }

        /// <summary>
        /// Adds the specified footer to the <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="footer">The footer.</param>
        /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
        public EmbedBuilder WithFooter(IEmbedFooter footer)
        {
            Footer = footer;
            return this;
        }

        /// <summary>
        /// Attempts to add a field with the specified values to the <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="inline">Whether the field should be shown inline.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        public Result AddField(string name, string value, bool inline = false)
            => AddField(new EmbedField(name, value, inline));

        /// <summary>
        /// Attempts to add the specified <see cref="IEmbedField"/> to the <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="field">The <see cref="IEmbedField"/>.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        public Result AddField(IEmbedField field)
        {
            if (_fields.Count >= Constants.MaxFieldCount)
            {
                return new EmbedError($"Cannot add any more fields to this embed.");
            }

            _fields.Add(field);
            return Result.FromSuccess();
        }

        /// <summary>
        /// Sets the internal field collection to the specified <see cref="ICollection{IEmbedField}"/>.
        /// </summary>
        /// <param name="fields">The collection of fields to use.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        public Result SetFields(ICollection<IEmbedField> fields)
        {
            if (fields.Count >= Constants.MaxFieldCount)
            {
                return new EmbedError($"The specified field collection is too large.");
            }

            _fields = fields.ToList();
            return Result.FromSuccess();
        }

        /// <summary>
        /// Builds the <see cref="EmbedBuilder"/> into a rich embed.
        /// </summary>
        /// <returns>A result containing the built embed or an error indicating failure.</returns>
        public Result<Embed> Build()
            => Length > Constants.MaxEmbedLength
            ? new EmbedError($"The total size of this EmbedBuilder is longer than the maximum allowed length.")
            : new Embed()
            {
                Title = Title,
                Type = Type,
                Description = Description,
                Url = Url,
                Timestamp = Timestamp ?? default(Optional<DateTimeOffset>),
                Colour = Colour,
                Footer = Footer is null ? default : new Optional<IEmbedFooter>(Footer),
                Image = Image is null ? default : new Optional<IEmbedImage>(Image),
                Thumbnail = Thumbnail is null ? default : new Optional<IEmbedThumbnail>(Thumbnail),
                Author = Author is null ? default : new Optional<IEmbedAuthor>(Author),
                Fields = new(Fields)
            };

        /// <summary>
        /// Converts the provided <see cref="IEmbed"/> to an instance of <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <param name="embed">The embed to convert.</param>
        /// <returns>An <see cref="EmbedBuilder"/> with the same properties as the provided embed, where present. The <see cref="EmbedType"/> will be overwritten to <see cref="EmbedType.Rich"/>.</returns>
        public static EmbedBuilder FromEmbed(IEmbed embed)
            => new(embed.Fields)
            {
                Title = embed.Title.HasValue ? embed.Title.Value : string.Empty,
                Description = embed.Description.HasValue ? embed.Description.Value : string.Empty,
                Url = embed.Url.HasValue ? embed.Url.Value : string.Empty,
                Timestamp = embed.Timestamp.HasValue ? embed.Timestamp.Value : null,
                Colour = embed.Colour.HasValue ? embed.Colour.Value : Constants.DefaultColour,
                Footer = embed.Footer.HasValue ? embed.Footer.Value : default,
                Image = embed.Image.HasValue ? embed.Image.Value : default,
                Thumbnail = embed.Thumbnail.HasValue ? embed.Thumbnail.Value : default
            };
    }
}
