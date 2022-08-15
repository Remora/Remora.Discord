//
//  EmbedBuilder.cs
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
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Discord.Extensions.Errors;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Extensions.Embeds;

/// <summary>
/// Provides utilities for building an embed.
/// </summary>
[PublicAPI]
public class EmbedBuilder : BuilderBase<Embed>
{
    /// <summary>
    /// Gets or sets the title of the embed.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the embed.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the url of the embed.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the timestamp, if any, which should be displayed on the embed.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the color of this embed.
    /// </summary>
    public Color? Colour { get; set; }

    /// <summary>
    /// Gets or sets the footer of this embed.
    /// </summary>
    public EmbedFooterBuilder? Footer { get; set; }

    /// <summary>
    /// Gets or sets the image added to this embed.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail added to this embed.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Gets or sets the author of the embed.
    /// </summary>
    public EmbedAuthorBuilder? Author { get; set; }

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
            var titleLength = this.Title?.Length ?? 0;
            var descriptionLength = this.Description?.Length ?? 0;
            var fieldSum = _fields.Sum(field => field.Name.Length + field.Value.Length);
            var footerLength = this.Footer?.Text.Length ?? 0;
            var authorLength = this.Author?.Name.Length ?? 0;

            return titleLength + descriptionLength + fieldSum + footerLength + authorLength;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedBuilder"/> class.
    /// </summary>
    public EmbedBuilder()
        : this(new List<IEmbedField>(EmbedConstants.MaxFieldCount))
    {
    }

    private EmbedBuilder(Optional<IReadOnlyList<IEmbedField>> fields)
        : this(fields.HasValue ? new List<IEmbedField>(fields.Value) : new List<IEmbedField>(EmbedConstants.MaxFieldCount))
    {
    }

    private EmbedBuilder(List<IEmbedField> fields)
    {
        _fields = fields;
    }

    /// <summary>
    /// Ensures that the overall length of the embed is less than the value of <see cref="EmbedConstants.MaxEmbedLength"/>.
    /// </summary>
    /// <returns>Returns a <see cref="Result"/> indicating success or failure of the validation.</returns>
    public override Result Validate()
    {
        var validateTitleResult = ValidateLength(nameof(this.Title), this.Title, EmbedConstants.MaxTitleLength, true);
        if (!validateTitleResult.IsSuccess)
        {
            return validateTitleResult;
        }

        var validateDescriptionResult = ValidateLength(nameof(this.Description), this.Description, EmbedConstants.MaxDescriptionLength, true);
        if (!validateDescriptionResult.IsSuccess)
        {
            return validateDescriptionResult;
        }

        var validateUrlResult = ValidateUrl(nameof(this.Url), this.Url, true);
        if (!validateUrlResult.IsSuccess)
        {
            return validateUrlResult;
        }

        // If there is no footer, just default to success.
        var validateFooterResult = this.Footer?.Validate() ?? Result.FromSuccess();
        if (!validateFooterResult.IsSuccess)
        {
            return validateFooterResult;
        }

        var validateImageResult = ValidateUrl(nameof(this.ImageUrl), this.ImageUrl, true);
        if (!validateImageResult.IsSuccess)
        {
            return validateImageResult;
        }

        var validateThumbnailResult = ValidateUrl(nameof(this.ThumbnailUrl), this.ThumbnailUrl, true);
        if (!validateThumbnailResult.IsSuccess)
        {
            return validateThumbnailResult;
        }

        var validateAuthorResult = this.Author?.Validate() ?? Result.FromSuccess();
        if (!validateAuthorResult.IsSuccess)
        {
            return validateAuthorResult;
        }

        if (this.Fields.Count >= EmbedConstants.MaxFieldCount)
        {
            return new ArgumentOutOfRangeError(nameof(this.Fields), $"There are too many fields in this collection. Expected: <{EmbedConstants.MaxFieldCount}. Actual: {this.Fields.Count}.");
        }

        return this.Length > EmbedConstants.MaxEmbedLength
            ? new ValidationError(nameof(this.Length), "The overall embed length is too long.")
            : Result.FromSuccess();
    }

    /// <summary>
    /// Adds the specified title to this <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithTitle(string title)
    {
        this.Title = title;
        return this;
    }

    /// <summary>
    /// Adds the specified description to this <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithDescription(string description)
    {
        this.Description = description;
        return this;
    }

    /// <summary>
    /// Adds the specified url to this <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithUrl(string url)
    {
        this.Url = url;
        return this;
    }

    /// <summary>
    /// Adds a thumbnail with the specified url to this <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="thumbnailUrl">The url of the thumbnail.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithThumbnailUrl(string thumbnailUrl)
    {
        this.ThumbnailUrl = thumbnailUrl;
        return this;
    }

    /// <summary>
    /// Adds an image with the specified url to this <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="imageUrl">The url of the thumbnail.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithImageUrl(string imageUrl)
    {
        this.ImageUrl = imageUrl;
        return this;
    }

    /// <summary>
    /// Sets the timestamp of the <see cref="EmbedBuilder"/> to <see cref="DateTimeOffset.UtcNow"/>.
    /// </summary>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithCurrentTimestamp()
    {
        this.Timestamp = DateTimeOffset.UtcNow;
        return this;
    }

    /// <summary>
    /// Sets the timestamp of the <see cref="EmbedBuilder"/> to the specified timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithTimestamp(DateTimeOffset timestamp)
    {
        this.Timestamp = timestamp;
        return this;
    }

    /// <summary>
    /// Sets the timestamp of the <see cref="EmbedBuilder"/> to the timestamp specified by the <paramref name="snowflake"/>.
    /// </summary>
    /// <param name="snowflake">The snowflake.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithTimestamp(Snowflake snowflake)
    {
        this.Timestamp = snowflake.Timestamp;
        return this;
    }

    /// <summary>
    /// Sets the color of the <see cref="EmbedBuilder"/> to the specified color.
    /// </summary>
    /// <param name="colour">The color.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithColour(Color colour)
    {
        this.Colour = colour;
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
        string name,
        string? url = null,
        string? iconUrl = null
    )
    {
        this.Author = new EmbedAuthorBuilder(name, url, iconUrl);
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

        this.Author = new EmbedAuthorBuilder($"{user.Username}#{user.Discriminator}", iconUrl: avatarUrl.AbsoluteUri);
        return this;
    }

    /// <summary>
    /// Adds an embed footer to the <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="text">The text of the footer.</param>
    /// <param name="iconUrl">The url of the icon.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithFooter(string text, string? iconUrl = null)
    {
        this.Footer = new EmbedFooterBuilder(text, iconUrl);
        return this;
    }

    /// <summary>
    /// Adds the specified footer to the <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="footer">The footer.</param>
    /// <returns>The current <see cref="EmbedBuilder"/> for chaining.</returns>
    public EmbedBuilder WithFooter(IEmbedFooter footer)
    {
        this.Footer = EmbedFooterBuilder.FromFooter(footer);
        return this;
    }

    /// <summary>
    /// Attempts to add a field with the specified values to the <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="inline">Whether the field should be shown inline.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    public Result<EmbedBuilder> AddField(string name, string value, bool inline = false)
        => AddField(new EmbedField(name, value, inline));

    /// <summary>
    /// Attempts to add the specified <see cref="IEmbedField"/> to the <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="field">The <see cref="IEmbedField"/>.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    public Result<EmbedBuilder> AddField(IEmbedField field)
    {
        if (_fields.Count >= EmbedConstants.MaxFieldCount)
        {
            return new ValidationError(nameof(this.Fields), "Cannot add any more fields to this embed.");
        }

        _fields.Add(field);
        return Result<EmbedBuilder>.FromSuccess(this);
    }

    /// <summary>
    /// Sets the internal field collection to the specified <see cref="ICollection{IEmbedField}"/>.
    /// </summary>
    /// <param name="fields">The collection of fields to use.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    public Result<EmbedBuilder> SetFields(ICollection<IEmbedField> fields)
    {
        if (fields.Count >= EmbedConstants.MaxFieldCount)
        {
            return new ArgumentOutOfRangeError(nameof(fields), $"There are too many fields in this collection. Expected: <{EmbedConstants.MaxFieldCount}. Actual: {fields.Count}.");
        }

        _fields = fields.ToList();
        return Result<EmbedBuilder>.FromSuccess(this);
    }

    /// <summary>
    /// Validates and builds the <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <returns>A result containing the built <see cref="Embed"/> or an error indicating failure.</returns>
    public override Result<Embed> Build()
    {
        var validationResult = Validate();

        if (!validationResult.IsSuccess)
        {
            return Result<Embed>.FromError(validationResult);
        }

        var footerResult = this.Footer?.Build();
        var authorResult = this.Author?.Build();

        return new Embed
        {
            Title = this.Title ?? default(Optional<string>),
            Type = EmbedType.Rich,
            Description = this.Description ?? default(Optional<string>),
            Url = this.Url ?? default(Optional<string>),
            Timestamp = this.Timestamp ?? default(Optional<DateTimeOffset>),
            Colour = this.Colour ?? default(Optional<Color>),
            Image = this.ImageUrl is null ? default(Optional<IEmbedImage>) : new EmbedImage(this.ImageUrl),
            Thumbnail = this.ThumbnailUrl is null ? default(Optional<IEmbedThumbnail>) : new EmbedThumbnail(this.ThumbnailUrl),
            Author = authorResult is { IsSuccess: true } author
                ? author.Entity
                : default(Optional<IEmbedAuthor>),
            Footer = footerResult is { IsSuccess: true } footer
                ? footer.Entity
                : default(Optional<IEmbedFooter>),
            Fields = new(this.Fields)
        };
    }

    /// <summary>
    /// Converts the provided <see cref="IEmbed"/> to an instance of <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="embed">The embed to convert.</param>
    /// <returns>An <see cref="EmbedBuilder"/> with the same properties as the provided embed, where present. The <see cref="EmbedType"/> will be overwritten to <see cref="EmbedType.Rich"/>.</returns>
    public static EmbedBuilder FromEmbed(IEmbed embed) => new(embed.Fields)
    {
        Title = embed.Title.HasValue ? embed.Title.Value : null,
        Description = embed.Description.HasValue ? embed.Description.Value : null,
        Url = embed.Url.HasValue ? embed.Url.Value : null,
        Timestamp = embed.Timestamp.HasValue ? embed.Timestamp.Value : null,
        Colour = embed.Colour.HasValue ? embed.Colour.Value : null,
        ImageUrl = embed.Image.HasValue ? embed.Image.Value.Url : null,
        ThumbnailUrl = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : null,
        Author = embed.Author.HasValue ? EmbedAuthorBuilder.FromAuthor(embed.Author.Value) : default,
        Footer = embed.Footer.HasValue ? EmbedFooterBuilder.FromFooter(embed.Footer.Value) : default
    };
}
