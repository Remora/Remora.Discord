//
//  EmbedImageBuilder.cs
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
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Results;

namespace Remora.Discord.Extensions.Embeds;

/// <summary>
/// A builder which validates and builds an <see cref="EmbedImageBuilder"/>.
/// </summary>
[PublicAPI]
public sealed class EmbedImageBuilder : BuilderBase<EmbedImage>
{
    /// <summary>
    /// Gets or sets the url of the image. Must be a valid url. Null values are not allowed.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedImageBuilder"/> class.
    /// </summary>
    /// <param name="url">The url of the image.</param>
    public EmbedImageBuilder(string url)
    {
        this.Url = url;
    }

    /// <inheritdoc />
    public override Result<EmbedImage> Build()
    {
        var validateResult = Validate();

        return validateResult.IsSuccess
            ? new EmbedImage(this.Url)
            : Result<EmbedImage>.FromError(validateResult);
    }

    /// <inheritdoc />
    public override Result Validate()
    {
        return ValidateUrl(nameof(this.Url), this.Url, false);
    }
}
