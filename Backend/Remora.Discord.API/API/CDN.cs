//
//  CDN.cs
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
using System.Linq;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;
using Remora.Results;

namespace Remora.Discord.API
{
    /// <summary>
    /// Provides various helper methods for accessing Discord's CDN.
    /// </summary>
    public static class CDN
    {
        /// <summary>
        /// Gets the CDN URI of the given emoji.
        /// </summary>
        /// <remarks>
        /// If the emoji is an animated emoji, the GIF variant of the image is preferred.
        /// </remarks>
        /// <param name="emoji">The emoji.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetEmojiUrl
        (
            IEmoji emoji,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            // Prefer the animated version of emojis, if available
            if (emoji.IsAnimated.HasValue && emoji.IsAnimated.Value)
            {
                if (!imageFormat.HasValue)
                {
                    imageFormat = CDNImageFormat.GIF;
                }
            }

            return emoji.ID is null
                ? new GenericError("The emoji isn't a custom emoji.")
                : GetEmojiUrl(emoji.ID.Value, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given emoji.
        /// </summary>
        /// <param name="emojiID">The ID of the emoji.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetEmojiUrl
        (
            Snowflake emojiID,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat(imageFormat, CDNImageFormat.PNG, CDNImageFormat.GIF);
            if (!formatValidation.IsSuccess)
            {
                return Result<Uri>.FromError(formatValidation);
            }

            imageFormat = formatValidation.Entity;

            var checkImageSize = CheckImageSize(imageSize);
            if (!checkImageSize.IsSuccess)
            {
                return Result<Uri>.FromError(checkImageSize);
            }

            var ub = new UriBuilder(Constants.CDNBaseURL)
            {
                Path = $"emojis/{emojiID}.{imageFormat.Value.ToString().ToLowerInvariant()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        private static Result<Optional<CDNImageFormat>> ValidateOrDefaultImageFormat
        (
            Optional<CDNImageFormat> imageFormat,
            params CDNImageFormat[] acceptedFormats
        )
        {
            if (!imageFormat.HasValue)
            {
                imageFormat = CDNImageFormat.PNG;
            }

            if (acceptedFormats.Contains(imageFormat.Value))
            {
                return imageFormat;
            }

            var formatNames = acceptedFormats.Select(f => f.ToString());

            return new GenericError
            (
                "Unsupported image format. " +
                $"The endpoint supports the following formats: {string.Join(", ", formatNames)}"
            );
        }

        private static Result CheckImageSize(Optional<ushort> imageSize)
        {
            if (!imageSize.HasValue)
            {
                return Result.FromSuccess();
            }

            if (imageSize.Value is < 16 or > 4096)
            {
                return new GenericError("The image size must be between 16 and 4096.");
            }

            var isPowerOfTwo = (imageSize.Value & (imageSize.Value - 1)) == 0;
            if (!isPowerOfTwo)
            {
                return new GenericError("The image size must be a power of two.");
            }

            return Result.FromSuccess();
        }
    }
}
