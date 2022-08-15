//
//  CDNTests.cs
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
using Moq;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Errors;
using Remora.Discord.API.Objects;
using Remora.Discord.API.Tests.TestBases;
using Remora.Rest.Core;
using Remora.Results;
using Xunit;

namespace Remora.Discord.API.Tests;

/// <summary>
/// Tests the <see cref="CDN"/> class.
/// </summary>
public class CDNTests
{
    /// <summary>
    /// Tests the <see cref="GetEmojiUrl"/> method and its
    /// overloads.
    /// </summary>
    public class GetEmojiUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmojiUrl"/> class.
        /// </summary>
        public GetEmojiUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/emojis/0"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.GIF, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when an animated emoji is provided.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedEmoji()
        {
            var expected = new Uri
            (
                this.ValidUriWithoutExtension + ".gif"
            );

            var emoji = new Emoji(DiscordSnowflake.New(0), null, IsAnimated: true);
            var getActual = CDN.GetEmojiUrl(emoji);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether the correct address is returned when an animated emoji is provided, but a custom format is
        /// explicitly requested.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedEmojiAndCustomFormat()
        {
            var expected = new Uri
            (
                this.ValidUriWithoutExtension + ".png"
            );

            var emoji = new Emoji(DiscordSnowflake.New(0), null, IsAnimated: true);
            var getActual = CDN.GetEmojiUrl(emoji, CDNImageFormat.PNG);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether an unsuccessful result is returned if the emoji isn't a custom emoji.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfEmojiIsNotCustomEmoji()
        {
            var emoji = new Emoji(null, "booga");
            var getActual = CDN.GetEmojiUrl(emoji);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<UnsupportedArgumentError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var emoji = new Emoji(DiscordSnowflake.New(0), null);
            yield return CDN.GetEmojiUrl(emoji, imageFormat, imageSize);
            yield return CDN.GetEmojiUrl(emoji.ID!.Value, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetGuildIconUrl(IGuild, Optional{CDNImageFormat}, Optional{ushort})"/> method and
    /// its overloads.
    /// </summary>
    public class GetGuildIconUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildIconUrl"/> class.
        /// </summary>
        public GetGuildIconUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/icons/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP, CDNImageFormat.GIF }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has an animated image.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedImage()
        {
            var expected = new Uri("https://cdn.discordapp.com/icons/0/a_1.gif");

            var guildID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("a_1");

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Icon).Returns(imageHash);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;

            var getActual = CDN.GetGuildIconUrl(guild);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has an animated image, but a custom
        /// format is explicitly requested.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedImageAndCustomFormat()
        {
            var expected = new Uri("https://cdn.discordapp.com/icons/0/a_1.png");

            var guildID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("a_1");

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Icon).Returns(imageHash);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;

            var getActual = CDN.GetGuildIconUrl(guild, CDNImageFormat.PNG);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var guildID = DiscordSnowflake.New(0);

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Icon).Returns((IImageHash?)null);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;

            var getActual = CDN.GetGuildIconUrl(guild, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var guildID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Icon).Returns(imageHash);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;
            yield return CDN.GetGuildIconUrl(guild, imageFormat, imageSize);
            yield return CDN.GetGuildIconUrl(guildID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetGuildSplashUrl(IGuild, Optional{CDNImageFormat}, Optional{ushort})"/> method and
    /// its overloads.
    /// </summary>
    public class GetGuildSplashUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildSplashUrl"/> class.
        /// </summary>
        public GetGuildSplashUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/splashes/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var guildID = DiscordSnowflake.New(0);

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Splash).Returns((IImageHash?)null);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;

            var getActual = CDN.GetGuildSplashUrl(guild, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var guildID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Splash).Returns(imageHash);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;
            yield return CDN.GetGuildSplashUrl(guild, imageFormat, imageSize);
            yield return CDN.GetGuildSplashUrl(guildID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetGuildDiscoverySplashUrl(IGuild, Optional{CDNImageFormat}, Optional{ushort})"/>
    /// method and its overloads.
    /// </summary>
    public class GetGuildDiscoverySplashUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildDiscoverySplashUrl"/> class.
        /// </summary>
        public GetGuildDiscoverySplashUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/discovery-splashes/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var guildID = DiscordSnowflake.New(0);

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.DiscoverySplash).Returns((IImageHash?)null);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;

            var getActual = CDN.GetGuildDiscoverySplashUrl(guild, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var guildID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.DiscoverySplash).Returns(imageHash);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;
            yield return CDN.GetGuildDiscoverySplashUrl(guild, imageFormat, imageSize);
            yield return CDN.GetGuildDiscoverySplashUrl(guildID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetGuildBannerUrl(IGuild, Optional{CDNImageFormat}, Optional{ushort})"/> method and
    /// its overloads.
    /// </summary>
    public class GetGuildBannerUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildBannerUrl"/> class.
        /// </summary>
        public GetGuildBannerUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/banners/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP, CDNImageFormat.GIF }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var guildID = DiscordSnowflake.New(0);

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Banner).Returns((IImageHash?)null);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;

            var getActual = CDN.GetGuildBannerUrl(guild, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var guildID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedGuild = new Mock<IGuild>();
            mockedGuild.SetupGet(g => g.Banner).Returns(imageHash);
            mockedGuild.SetupGet(g => g.ID).Returns(guildID);

            var guild = mockedGuild.Object;
            yield return CDN.GetGuildBannerUrl(guild, imageFormat, imageSize);
            yield return CDN.GetGuildBannerUrl(guildID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetUserBannerUrl(IUser, Optional{CDNImageFormat}, Optional{ushort})"/> method and
    /// its overloads.
    /// </summary>
    public class GetUserBannerUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserBannerUrl"/> class.
        /// </summary>
        public GetUserBannerUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/banners/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP, CDNImageFormat.GIF }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var userID = DiscordSnowflake.New(0);

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.Banner).Returns(default(Optional<IImageHash?>));
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var user = mockedUser.Object;

            var getActual = CDN.GetUserBannerUrl(user, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNullImage()
        {
            var userID = DiscordSnowflake.New(0);

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.Banner).Returns(new Optional<IImageHash?>(null));
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var user = mockedUser.Object;

            var getActual = CDN.GetUserBannerUrl(user, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var userID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.Banner).Returns(imageHash);
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var user = mockedUser.Object;
            yield return CDN.GetUserBannerUrl(user, imageFormat, imageSize);
            yield return CDN.GetUserBannerUrl(userID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetDefaultUserAvatarUrl(IUser, Optional{CDNImageFormat}, Optional{ushort})"/>
    /// method and its overloads.
    /// </summary>
    public class GetDefaultUserAvatarUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetDefaultUserAvatarUrl"/> class.
        /// </summary>
        public GetDefaultUserAvatarUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/embed/avatars/0"),
                new[] { CDNImageFormat.PNG }
            )
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            ushort discriminator = 0;

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(u => u.Discriminator).Returns(discriminator);

            var user = mockedUser.Object;
            yield return CDN.GetDefaultUserAvatarUrl(user, imageFormat, imageSize);
            yield return CDN.GetDefaultUserAvatarUrl(discriminator, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetUserAvatarUrl(IUser, Optional{CDNImageFormat}, Optional{ushort})"/> method and
    /// its overloads.
    /// </summary>
    public class GetUserAvatarUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserAvatarUrl"/> class.
        /// </summary>
        public GetUserAvatarUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/avatars/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP, CDNImageFormat.GIF }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has an animated image.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedImage()
        {
            var expected = new Uri("https://cdn.discordapp.com/avatars/0/a_1.gif");

            var userID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("a_1");

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.Avatar).Returns(imageHash);
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var user = mockedUser.Object;

            var getActual = CDN.GetUserAvatarUrl(user);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has an animated image, but a custom
        /// format is explicitly requested.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedImageAndCustomFormat()
        {
            var expected = new Uri("https://cdn.discordapp.com/avatars/0/a_1.png");

            var userID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("a_1");

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.Avatar).Returns(imageHash);
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var user = mockedUser.Object;

            var getActual = CDN.GetUserAvatarUrl(user, CDNImageFormat.PNG);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var userID = DiscordSnowflake.New(0);

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.Avatar).Returns((IImageHash?)null);
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var user = mockedUser.Object;

            var getActual = CDN.GetUserAvatarUrl(user, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var userID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.Avatar).Returns(imageHash);
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var user = mockedUser.Object;
            yield return CDN.GetUserAvatarUrl(user, imageFormat, imageSize);
            yield return CDN.GetUserAvatarUrl(userID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetGuildMemberAvatarUrl(Snowflake, IGuildMember, Optional{CDNImageFormat}, Optional{ushort})"/> method and
    /// its overloads.
    /// </summary>
    public class GetGuildMemberAvatarUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildMemberAvatarUrl"/> class.
        /// </summary>
        public GetGuildMemberAvatarUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/guilds/0/users/1/avatars/2"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP, CDNImageFormat.GIF }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has an animated image.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedImage()
        {
            var expected = new Uri("https://cdn.discordapp.com/guilds/0/users/1/avatars/a_2.gif");

            var guildID = DiscordSnowflake.New(0);
            var userID = DiscordSnowflake.New(1);
            var imageHash = new ImageHash("a_2");

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var mockedMember = new Mock<IGuildMember>();
            mockedMember.SetupGet(g => g.Avatar).Returns(imageHash);
            mockedMember.SetupGet(g => g.User).Returns(new Optional<IUser>(mockedUser.Object));

            var member = mockedMember.Object;

            var getActual = CDN.GetGuildMemberAvatarUrl(guildID, member);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has an animated image, but a custom
        /// format is explicitly requested.
        /// </summary>
        [Fact]
        public void ReturnsCorrectAddressWithAnimatedImageAndCustomFormat()
        {
            var expected = new Uri("https://cdn.discordapp.com/guilds/0/users/1/avatars/a_2.png");

            var guildID = DiscordSnowflake.New(0);
            var userID = DiscordSnowflake.New(1);
            var imageHash = new ImageHash("a_2");

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var mockedMember = new Mock<IGuildMember>();
            mockedMember.SetupGet(g => g.Avatar).Returns(imageHash);
            mockedMember.SetupGet(g => g.User).Returns(new Optional<IUser>(mockedUser.Object));

            var member = mockedMember.Object;

            var getActual = CDN.GetGuildMemberAvatarUrl(guildID, member, CDNImageFormat.PNG);

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var guildID = DiscordSnowflake.New(0);
            var userID = DiscordSnowflake.New(1);

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var mockedMember = new Mock<IGuildMember>();
            mockedMember.SetupGet(g => g.Avatar).Returns(new Optional<IImageHash?>(null));
            mockedMember.SetupGet(g => g.User).Returns(new Optional<IUser>(mockedUser.Object));

            var member = mockedMember.Object;

            var getActual = CDN.GetGuildMemberAvatarUrl(guildID, member, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var guildID = DiscordSnowflake.New(0);
            var userID = DiscordSnowflake.New(1);
            var imageHash = new ImageHash("2");

            var mockedUser = new Mock<IUser>();
            mockedUser.SetupGet(g => g.ID).Returns(userID);

            var mockedMember = new Mock<IGuildMember>();
            mockedMember.SetupGet(g => g.Avatar).Returns(imageHash);
            mockedMember.SetupGet(g => g.User).Returns(new Optional<IUser>(mockedUser.Object));

            var member = mockedMember.Object;
            yield return CDN.GetGuildMemberAvatarUrl(guildID, member, imageFormat, imageSize);
            yield return CDN.GetGuildMemberAvatarUrl(guildID, userID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetApplicationIconUrl(IApplication, Optional{CDNImageFormat}, Optional{ushort})"/>
    /// method and its overloads.
    /// </summary>
    public class GetApplicationIconUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetApplicationIconUrl"/> class.
        /// </summary>
        public GetApplicationIconUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/app-icons/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var applicationID = DiscordSnowflake.New(0);

            var mockedApplication = new Mock<IApplication>();
            mockedApplication.SetupGet(g => g.Icon).Returns((IImageHash?)null);
            mockedApplication.SetupGet(g => g.ID).Returns(applicationID);

            var application = mockedApplication.Object;

            var getActual = CDN.GetApplicationIconUrl(application, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var applicationID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedApplication = new Mock<IApplication>();
            mockedApplication.SetupGet(g => g.Icon).Returns(imageHash);
            mockedApplication.SetupGet(g => g.ID).Returns(applicationID);

            var application = mockedApplication.Object;
            yield return CDN.GetApplicationIconUrl(application, imageFormat, imageSize);
            yield return CDN.GetApplicationIconUrl(applicationID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetApplicationCoverUrl(IApplication, Optional{CDNImageFormat}, Optional{ushort})"/>
    /// method and its overloads.
    /// </summary>
    public class GetApplicationCoverUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetApplicationCoverUrl"/> class.
        /// </summary>
        public GetApplicationCoverUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/app-icons/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var applicationID = DiscordSnowflake.New(0);

            var mockedApplication = new Mock<IApplication>();
            mockedApplication.SetupGet(g => g.CoverImage).Returns(default(Optional<IImageHash>));
            mockedApplication.SetupGet(g => g.ID).Returns(applicationID);

            var application = mockedApplication.Object;

            var getActual = CDN.GetApplicationCoverUrl(application, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var applicationID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedApplication = new Mock<IApplication>();
            mockedApplication.SetupGet(g => g.CoverImage).Returns(imageHash);
            mockedApplication.SetupGet(g => g.ID).Returns(applicationID);

            var application = mockedApplication.Object;
            yield return CDN.GetApplicationCoverUrl(application, imageFormat, imageSize);
            yield return CDN.GetApplicationCoverUrl(applicationID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="CDN.GetApplicationAssetUrl(IApplication, string, Optional{CDNImageFormat}, Optional{ushort})"/>
    /// method and its overloads.
    /// </summary>
    public class GetApplicationAssetUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetApplicationAssetUrl"/> class.
        /// </summary>
        public GetApplicationAssetUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/app-assets/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var applicationID = DiscordSnowflake.New(0);

            var mockedApplication = new Mock<IApplication>();
            mockedApplication.SetupGet(g => g.ID).Returns(applicationID);

            var application = mockedApplication.Object;
            yield return CDN.GetApplicationAssetUrl(application, "1", imageFormat, imageSize);
            yield return CDN.GetApplicationAssetUrl(applicationID, "1", imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="CDN.GetAchievementIconUrl(IApplication, Snowflake, IImageHash, Optional{CDNImageFormat}, Optional{ushort})"/>
    /// method and its overloads.
    /// </summary>
    public class GetAchievementIconUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAchievementIconUrl"/> class.
        /// </summary>
        public GetAchievementIconUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/app-assets/0/achievements/1/icons/2"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var applicationID = DiscordSnowflake.New(0);
            var achievementID = DiscordSnowflake.New(1);
            var imageHash = new ImageHash("2");

            var mockedApplication = new Mock<IApplication>();
            mockedApplication.SetupGet(g => g.ID).Returns(applicationID);

            var application = mockedApplication.Object;
            yield return CDN.GetAchievementIconUrl(application, achievementID, imageHash, imageFormat, imageSize);
            yield return CDN.GetAchievementIconUrl(applicationID, achievementID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetTeamIconUrl(ITeam, Optional{CDNImageFormat}, Optional{ushort})"/> method and its
    /// overloads.
    /// </summary>
    public class GetTeamIconUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTeamIconUrl"/> class.
        /// </summary>
        public GetTeamIconUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/team-icons/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var teamID = DiscordSnowflake.New(0);

            var mockedTeam = new Mock<ITeam>();
            mockedTeam.SetupGet(g => g.Icon).Returns((IImageHash?)null);
            mockedTeam.SetupGet(g => g.ID).Returns(teamID);

            var team = mockedTeam.Object;

            var getActual = CDN.GetTeamIconUrl(team, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var teamID = DiscordSnowflake.New(0);
            var imageHash = new ImageHash("1");

            var mockedTeam = new Mock<ITeam>();
            mockedTeam.SetupGet(g => g.Icon).Returns(imageHash);
            mockedTeam.SetupGet(g => g.ID).Returns(teamID);

            var team = mockedTeam.Object;
            yield return CDN.GetTeamIconUrl(team, imageFormat, imageSize);
            yield return CDN.GetTeamIconUrl(teamID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetRoleIconUrl(IRole, Optional{CDNImageFormat}, Optional{ushort})"/> method and its
    /// overloads.
    /// </summary>
    public class GetRoleIconUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetRoleIconUrl"/> class.
        /// </summary>
        public GetRoleIconUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/role-icons/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var mockedRole = new Mock<IRole>();
            mockedRole.SetupGet(g => g.Icon).Returns(default(Optional<IImageHash?>));

            var role = mockedRole.Object;

            var getActual = CDN.GetRoleIconUrl(role, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var imageHash = new ImageHash("1");

            var mockedRole = new Mock<IRole>();
            mockedRole.SetupGet(g => g.ID).Returns(new Snowflake(0));
            mockedRole.SetupGet(g => g.Icon).Returns(imageHash);

            var role = mockedRole.Object;
            yield return CDN.GetRoleIconUrl(role, imageFormat, imageSize);
            yield return CDN.GetRoleIconUrl(role.ID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetGuildScheduledEventCoverUrl(IGuildScheduledEvent, Optional{CDNImageFormat}, Optional{ushort})"/>
    /// method and its overloads.
    /// </summary>
    public class GetGuildScheduledEventCoverUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildScheduledEventCoverUrl"/> class.
        /// </summary>
        public GetGuildScheduledEventCoverUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/guild-events/0/1"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP }
            )
        {
        }

        /// <summary>
        /// Tests whether the correct address is returned when the instance has no image set.
        /// </summary>
        [Fact]
        public void ReturnsUnsuccessfulResultIfInstanceHasNoImage()
        {
            var mockedEvent = new Mock<IGuildScheduledEvent>();
            mockedEvent.SetupGet(g => g.Image).Returns(default(IImageHash?));

            var scheduledEvent = mockedEvent.Object;

            var getActual = CDN.GetGuildScheduledEventCoverUrl(scheduledEvent, CDNImageFormat.PNG);

            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageNotFoundError>(getActual.Error);
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var imageHash = new ImageHash("1");

            var mockedEvent = new Mock<IGuildScheduledEvent>();
            mockedEvent.SetupGet(g => g.ID).Returns(new Snowflake(0));
            mockedEvent.SetupGet(g => g.Image).Returns(imageHash);

            var scheduledEvent = mockedEvent.Object;
            yield return CDN.GetGuildScheduledEventCoverUrl(scheduledEvent, imageFormat, imageSize);
            yield return CDN.GetGuildScheduledEventCoverUrl(scheduledEvent.ID, imageHash, imageFormat, imageSize);
        }
    }

    /// <summary>
    /// Tests the <see cref="CDN.GetGuildMemberBannerUrl"/> method and its overloads.
    /// </summary>
    public class GetGuildMemberBannerUrl : CDNTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildMemberBannerUrl"/> class.
        /// </summary>
        public GetGuildMemberBannerUrl()
            : base
            (
                new Uri("https://cdn.discordapp.com/guilds/0/users/1/banners/2"),
                new[] { CDNImageFormat.PNG, CDNImageFormat.JPEG, CDNImageFormat.WebP, CDNImageFormat.GIF }
            )
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Result<Uri>> GetImageUris
        (
            Optional<CDNImageFormat> imageFormat = default,
            Optional<ushort> imageSize = default
        )
        {
            var guild = new Snowflake(0);
            var user = new Snowflake(1);
            var imageHash = new ImageHash("2");

            yield return CDN.GetGuildMemberBannerUrl(guild, user, imageHash, imageFormat, imageSize);
        }
    }
}
