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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Errors;
using Remora.Discord.API.Extensions;
using Remora.Discord.Core;
using Remora.Results;

namespace Remora.Discord.API
{
    /// <summary>
    /// Provides various helper methods for accessing Discord's CDN.
    /// </summary>
    [PublicAPI]
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
            if (emoji.ID is null)
            {
                return new UnsupportedArgumentError("The emoji isn't a custom emoji.");
            }

            if (imageFormat.HasValue)
            {
                return GetEmojiUrl(emoji.ID.Value, imageFormat, imageSize);
            }

            // Prefer the animated version of emojis, if available
            if (emoji.IsAnimated.IsDefined(out var isAnimated) && isAnimated)
            {
                imageFormat = CDNImageFormat.GIF;
            }

            return GetEmojiUrl(emoji.ID.Value, imageFormat, imageSize);
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
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP,
                CDNImageFormat.GIF
            );

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
                Path = $"emojis/{emojiID}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's icon.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildIconUrl
        (
            IGuild guild,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            if (guild.Icon is null)
            {
                return new ImageNotFoundError();
            }

            // Prefer the animated version, if available
            if (guild.Icon.HasGif && !imageFormat.HasValue)
            {
                imageFormat = CDNImageFormat.GIF;
            }

            return GetGuildIconUrl(guild.ID, guild.Icon, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's icon.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="iconHash">The image hash of the guild's icon.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildIconUrl
        (
            Snowflake guildID,
            IImageHash iconHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP,
                CDNImageFormat.GIF
            );

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
                Path = $"icons/{guildID}/{iconHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's splash.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildSplashUrl
        (
            IGuild guild,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            if (guild.Splash is null)
            {
                return new ImageNotFoundError();
            }

            return GetGuildSplashUrl(guild.ID, guild.Splash, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's splash.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="splashHash">The image hash of the guild's splash.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildSplashUrl
        (
            Snowflake guildID,
            IImageHash splashHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"splashes/{guildID}/{splashHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's discovery splash.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildDiscoverySplashUrl
        (
            IGuild guild,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            if (guild.DiscoverySplash is null)
            {
                return new ImageNotFoundError();
            }

            return GetGuildDiscoverySplashUrl(guild.ID, guild.DiscoverySplash, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's discovery splash.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="discoverySplashHash">The image hash of the guild's discovery splash.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildDiscoverySplashUrl
        (
            Snowflake guildID,
            IImageHash discoverySplashHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"discovery-splashes/{guildID}/{discoverySplashHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's banner.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildBannerUrl
        (
            IGuild guild,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            if (guild.Banner is null)
            {
                return new ImageNotFoundError();
            }

            return GetGuildBannerUrl(guild.ID, guild.Banner, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given guild's banner.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="bannerHash">The image hash of the guild's banner.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetGuildBannerUrl
        (
            Snowflake guildID,
            IImageHash bannerHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"banners/{guildID}/{bannerHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given user's banner.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetUserBannerUrl
        (
            IUser user,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            return user.Banner.IsDefined(out var banner)
                ? GetUserBannerUrl(user.ID, banner, imageFormat, imageSize)
                : new ImageNotFoundError();
        }

        /// <summary>
        /// Gets the CDN URI of the given user's banner.
        /// </summary>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="bannerHash">The image hash of the user's banner.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetUserBannerUrl
        (
            Snowflake userID,
            IImageHash bannerHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP,
                CDNImageFormat.GIF
            );

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
                Path = $"banners/{userID}/{bannerHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given user's default avatar.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetDefaultUserAvatarUrl
        (
            IUser user,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            return GetDefaultUserAvatarUrl(user.Discriminator, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given user's default avatar.
        /// </summary>
        /// <param name="userDiscriminator">The user's discriminator.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetDefaultUserAvatarUrl
        (
            ushort userDiscriminator,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG
            );

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

            var discriminatorModulus = userDiscriminator % 5;
            var ub = new UriBuilder(Constants.CDNBaseURL)
            {
                Path = $"embed/avatars/{discriminatorModulus}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given user's avatar.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetUserAvatarUrl
        (
            IUser user,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            if (user.Avatar is null)
            {
                return new ImageNotFoundError();
            }

            // Prefer the animated version, if available
            if (user.Avatar.HasGif && !imageFormat.HasValue)
            {
                imageFormat = CDNImageFormat.GIF;
            }

            return GetUserAvatarUrl(user.ID, user.Avatar, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given user's avatar.
        /// </summary>
        /// <param name="userID">The ID of the team.</param>
        /// <param name="avatarHash">The image hash of the user's avatar.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetUserAvatarUrl
        (
            Snowflake userID,
            IImageHash avatarHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP,
                CDNImageFormat.GIF
            );

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
                Path = $"avatars/{userID}/{avatarHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given application's icon.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetApplicationIconUrl
        (
            IApplication application,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            if (application.Icon is null)
            {
                return new ImageNotFoundError();
            }

            return GetApplicationIconUrl(application.ID, application.Icon, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given application's icon.
        /// </summary>
        /// <param name="applicationID">The ID of the application.</param>
        /// <param name="iconHash">The image hash of the application's icon.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetApplicationIconUrl
        (
            Snowflake applicationID,
            IImageHash iconHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"app-icons/{applicationID}/{iconHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given application's cover.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetApplicationCoverUrl
        (
            IApplication application,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            return application.CoverImage.IsDefined(out var coverImage)
                ? GetApplicationCoverUrl(application.ID, coverImage, imageFormat, imageSize)
                : new ImageNotFoundError();
        }

        /// <summary>
        /// Gets the CDN URI of the given application's cover.
        /// </summary>
        /// <param name="applicationID">The ID of the application.</param>
        /// <param name="coverHash">The image hash of the application's cover.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetApplicationCoverUrl
        (
            Snowflake applicationID,
            IImageHash coverHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"app-icons/{applicationID}/{coverHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given application's given asset.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="assetID">The asset ID.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetApplicationAssetUrl
        (
            IApplication application,
            string assetID,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        ) => GetApplicationAssetUrl(application.ID, assetID, imageFormat, imageSize);

        /// <summary>
        /// Gets the CDN URI of the given application's given asset.
        /// </summary>
        /// <param name="applicationID">The ID of the application.</param>
        /// <param name="assetID">The asset ID.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetApplicationAssetUrl
        (
            Snowflake applicationID,
            string assetID,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"app-assets/{applicationID}/{assetID}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given achievement's icon.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="achievementID">The ID of the achievement.</param>
        /// <param name="iconHash">The image hash of the icon.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetAchievementIconUrl
        (
            IApplication application,
            Snowflake achievementID,
            IImageHash iconHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        ) => GetAchievementIconUrl(application.ID, achievementID, iconHash, imageFormat, imageSize);

        /// <summary>
        /// Gets the CDN URI of the given achievement's icon.
        /// </summary>
        /// <param name="applicationID">The ID of the application.</param>
        /// <param name="achievementID">The ID of the achievement.</param>
        /// <param name="iconHash">The image hash of the achievement's icon.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetAchievementIconUrl
        (
            Snowflake applicationID,
            Snowflake achievementID,
            IImageHash iconHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"app-assets/{applicationID}/achievements/{achievementID}/icons/{iconHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given sticker pack's banner.
        /// </summary>
        /// <param name="stickerPack">The sticker pack.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetStickerPackBannerUrl
        (
            IStickerPack stickerPack,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        ) => GetStickerPackBannerUrl(stickerPack.BannerAssetID, imageFormat, imageSize);

        /// <summary>
        /// Gets the CDN URI of the given sticker pack's banner.
        /// </summary>
        /// <param name="bannerAssetId">The asset ID of the sticker pack.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetStickerPackBannerUrl
        (
            Snowflake bannerAssetId,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                // Yes, all stickers are stored under this hardcoded application. This is intentional.
                Path = $"app-assets/710982414301790216/store/{bannerAssetId}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given team's icon.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetTeamIconUrl
        (
            ITeam team,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            if (team.Icon is null)
            {
                return new ImageNotFoundError();
            }

            return GetTeamIconUrl(team.ID, team.Icon, imageFormat, imageSize);
        }

        /// <summary>
        /// Gets the CDN URI of the given team's icon.
        /// </summary>
        /// <param name="teamID">The ID of the team.</param>
        /// <param name="iconHash">The image hash of the team's icon.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetTeamIconUrl
        (
            Snowflake teamID,
            IImageHash iconHash,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.JPEG,
                CDNImageFormat.WebP
            );

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
                Path = $"team-icons/{teamID}/{iconHash.Value}.{imageFormat.Value.ToFileExtension()}"
            };

            if (imageSize.HasValue)
            {
                ub.Query = $"size={imageSize.Value}";
            }

            return ub.Uri;
        }

        /// <summary>
        /// Gets the CDN URI of the given sticker.
        /// </summary>
        /// <param name="sticker">The sticker pack.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetStickerUrl
        (
            ISticker sticker,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        ) => GetStickerUrl(sticker.ID, imageFormat, imageSize);

        /// <summary>
        /// Gets the CDN URI of the given sticker.
        /// </summary>
        /// <param name="stickerId">The ID of the sticker.</param>
        /// <param name="imageFormat">The requested image format.</param>
        /// <param name="imageSize">The requested image size. May be any power of two between 16 and 4096.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        public static Result<Uri> GetStickerUrl
        (
            Snowflake stickerId,
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var formatValidation = ValidateOrDefaultImageFormat
            (
                imageFormat,
                CDNImageFormat.PNG,
                CDNImageFormat.Lottie
            );

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
                Path = $"stickers/{stickerId}.{imageFormat.Value.ToFileExtension()}"
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

            return new UnsupportedImageFormatError(acceptedFormats);
        }

        private static Result CheckImageSize(Optional<ushort> imageSize)
        {
            if (!imageSize.HasValue)
            {
                return Result.FromSuccess();
            }

            if (imageSize.Value is < 16 or > 4096)
            {
                return new ImageSizeOutOfRangeError();
            }

            var isPowerOfTwo = (imageSize.Value & (imageSize.Value - 1)) == 0;
            if (!isPowerOfTwo)
            {
                return new ImageSizeNotPowerOfTwoError();
            }

            return Result.FromSuccess();
        }
    }
}
