//
//  DiscordRestGuildAPITests.cs
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.API.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Guild;

/// <summary>
/// Tests the <see cref="DiscordRestGuildAPI"/> class.
/// </summary>
public class DiscordRestGuildAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.CreateGuildAsync"/> method.
    /// </summary>
    public class CreateGuildAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGuildAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateGuildAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var name = "brr";

            // Create a dummy PNG image
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            icon.Position = 0;

            var verificationLevel = VerificationLevel.High;
            var defaultMessageNotifications = MessageNotificationLevel.AllMessages;
            var explicitContentFilter = ExplicitContentFilterLevel.Disabled;
            var roles = new List<IRole>();
            var channels = new List<IPartialChannel>();
            var afkChannelID = DiscordSnowflake.New(0);
            var afkTimeout = TimeSpan.FromSeconds(10);
            var systemChannelID = DiscordSnowflake.New(1);
            var systemChannelFlags = SystemChannelFlags.SuppressJoinNotifications;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("icon", p => p.IsString())
                                .WithProperty("verification_level", p => p.Is((int)verificationLevel))
                                .WithProperty
                                (
                                    "default_message_notifications",
                                    p => p.Is((int)defaultMessageNotifications)
                                )
                                .WithProperty("explicit_content_filter", p => p.Is((int)explicitContentFilter))
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("channels", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("afk_channel_id", p => p.Is(afkChannelID.ToString()))
                                .WithProperty("afk_timeout", p => p.Is(afkTimeout.TotalSeconds))
                                .WithProperty("system_channel_id", p => p.Is(systemChannelID.ToString()))
                                .WithProperty("system_channel_flags", p => p.Is((int)systemChannelFlags))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.CreateGuildAsync
            (
                name,
                icon,
                verificationLevel,
                defaultMessageNotifications,
                explicitContentFilter,
                roles,
                channels,
                afkChannelID,
                afkTimeout,
                systemChannelID,
                systemChannelFlags
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfNameIsTooShort()
        {
            var name = new string('b', 1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.CreateGuildAsync
            (
                name
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfNameIsTooLong()
        {
            var name = new string('b', 101);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.CreateGuildAsync
            (
                name
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfImageIsUnknownType()
        {
            var name = "aaa";

            // Create a dummy PNG image
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(0x00000000);
            icon.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.CreateGuildAsync
            (
                name,
                icon
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildAsync"/> method.
    /// </summary>
    public class GetGuildAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var withCounts = true;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}")
                    .WithQueryString("with_counts", withCounts.ToString())
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.GetGuildAsync(guildId, withCounts);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildPreviewAsync"/> method.
    /// </summary>
    public class GetGuildPreviewAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildPreviewAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildPreviewAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/preview")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildPreview)])
            );

            var result = await api.GetGuildPreviewAsync(guildId);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildAsync"/> method.
    /// </summary>
    public class ModifyGuildAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "brr";

            // Create a dummy PNG image
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            icon.Position = 0;

            var verificationLevel = VerificationLevel.High;
            var defaultMessageNotifications = MessageNotificationLevel.AllMessages;
            var explicitContentFilter = ExplicitContentFilterLevel.Disabled;
            var afkChannelID = DiscordSnowflake.New(0);
            var afkTimeout = TimeSpan.FromSeconds(10);
            var systemChannelID = DiscordSnowflake.New(1);
            var systemChannelFlags = SystemChannelFlags.SuppressJoinNotifications;
            var ownerId = DiscordSnowflake.New(2);

            await using var splash = new MemoryStream();
            await using var splashBinaryWriter = new BinaryWriter(splash);
            splashBinaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            splash.Position = 0;

            await using var discoverySplash = new MemoryStream();
            await using var discoverySplashBinaryWriter = new BinaryWriter(discoverySplash);
            discoverySplashBinaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            discoverySplash.Position = 0;

            await using var banner = new MemoryStream();
            await using var bannerBinaryWriter = new BinaryWriter(banner);
            bannerBinaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            banner.Position = 0;

            var rulesChannelId = DiscordSnowflake.New(3);
            var publicUpdatesChannelID = DiscordSnowflake.New(4);
            var preferredLocale = "dd";
            var features = Array.Empty<GuildFeature>();
            var description = "aaa";
            var isPremiumProgressBarEnabled = true;
            var safetyAlertsChannel = DiscordSnowflake.New(5);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("verification_level", p => p.Is((int)verificationLevel))
                                .WithProperty
                                (
                                    "default_message_notifications",
                                    p => p.Is((int)defaultMessageNotifications)
                                )
                                .WithProperty("explicit_content_filter", p => p.Is((int)explicitContentFilter))
                                .WithProperty("afk_channel_id", p => p.Is(afkChannelID.ToString()))
                                .WithProperty("afk_timeout", p => p.Is(afkTimeout.TotalSeconds))
                                .WithProperty("icon", p => p.IsString())
                                .WithProperty("owner_id", p => p.Is(ownerId.ToString()))
                                .WithProperty("splash", p => p.IsString())
                                .WithProperty("discovery_splash", p => p.IsString())
                                .WithProperty("banner", p => p.IsString())
                                .WithProperty("system_channel_id", p => p.Is(systemChannelID.ToString()))
                                .WithProperty("system_channel_flags", p => p.Is((int)systemChannelFlags))
                                .WithProperty("rules_channel_id", p => p.Is(rulesChannelId.ToString()))
                                .WithProperty("public_updates_channel_id", p => p.Is(publicUpdatesChannelID.ToString()))
                                .WithProperty("preferred_locale", p => p.Is(preferredLocale))
                                .WithProperty("features", p => p.IsArray())
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("premium_progress_bar_enabled", p => p.Is(isPremiumProgressBarEnabled))
                                .WithProperty("safety_alerts_channel_id", p => p.Is(safetyAlertsChannel.ToString()))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.ModifyGuildAsync
            (
                guildId,
                name,
                verificationLevel,
                defaultMessageNotifications,
                explicitContentFilter,
                afkChannelID,
                afkTimeout,
                icon,
                ownerId,
                splash,
                discoverySplash,
                banner,
                systemChannelID,
                systemChannelFlags,
                rulesChannelId,
                publicUpdatesChannelID,
                preferredLocale,
                features,
                description,
                isPremiumProgressBarEnabled,
                safetyAlertsChannel,
                reason
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNullableRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "brr";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("icon", p => p.IsNull())
                                .WithProperty("splash", p => p.IsNull())
                                .WithProperty("banner", p => p.IsNull())
                                .WithProperty("safety_alerts_channel_id", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.ModifyGuildAsync
            (
                guildId,
                name,
                icon: null,
                banner: null,
                splash: null,
                safetyAlertsChannelID: null
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfIconIsUnknownFormat()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "brr";

            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(0x00000000);
            icon.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.ModifyGuildAsync
            (
                guildId,
                name,
                icon: icon
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfBannerIsUnknownFormat()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "brr";

            await using var banner = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(banner);
            binaryWriter.Write(0x00000000);
            banner.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.ModifyGuildAsync
            (
                guildId,
                name,
                banner: banner
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfSplashIsUnknownFormat()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "brr";

            await using var splash = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(splash);
            binaryWriter.Write(0x00000000);
            splash.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.ModifyGuildAsync
            (
                guildId,
                name,
                splash: splash
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.DeleteGuildAsync"/> method.
    /// </summary>
    public class DeleteGuildAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteGuildAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteGuildAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}")
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildAsync(guildId);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildChannelsAsync"/> method.
    /// </summary>
    public class GetGuildChannelsAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildChannelsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildChannelsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildChannelsAsync(guildId);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.CreateGuildChannelAsync"/> method.
    /// </summary>
    public class CreateGuildChannelAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGuildChannelAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateGuildChannelAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsTextRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "dd";
            var type = ChannelType.GuildText;
            var topic = "ggg";
            var rateLimitPerUser = 10;
            var position = 1;
            var permissionOverwrites = new List<IPermissionOverwrite>();
            var parentID = DiscordSnowflake.New(1);
            var isNsfw = true;
            var defaultAutoArchiveDuration = AutoArchiveDuration.Day;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("topic", p => p.Is(topic))
                                .WithProperty("rate_limit_per_user", p => p.Is(rateLimitPerUser))
                                .WithProperty("position", p => p.Is(position))
                                .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("parent_id", p => p.Is(parentID.ToString()))
                                .WithProperty("nsfw", p => p.Is(isNsfw))
                                .WithProperty
                                (
                                    "default_auto_archive_duration",
                                    p => p.Is((int)defaultAutoArchiveDuration)
                                )
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.CreateGuildTextChannelAsync
            (
                guildId,
                name,
                topic,
                rateLimitPerUser,
                position,
                permissionOverwrites,
                parentID,
                isNsfw,
                defaultAutoArchiveDuration,
                reason
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsAnnouncementRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "dd";
            var type = ChannelType.GuildAnnouncement;
            var bitrate = 2;
            var position = 1;
            var permissionOverwrites = new List<IPermissionOverwrite>();
            var parentID = DiscordSnowflake.New(1);
            var isNsfw = true;
            var defaultAutoArchiveDuration = AutoArchiveDuration.Day;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("bitrate", p => p.Is(bitrate))
                                .WithProperty("position", p => p.Is(position))
                                .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("parent_id", p => p.Is(parentID.ToString()))
                                .WithProperty("nsfw", p => p.Is(isNsfw))
                                .WithProperty
                                (
                                    "default_auto_archive_duration",
                                    p => p.Is((int)defaultAutoArchiveDuration)
                                )
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.CreateGuildAnnouncementChannelAsync
            (
                guildId,
                name,
                bitrate,
                position,
                permissionOverwrites,
                parentID,
                isNsfw,
                defaultAutoArchiveDuration,
                reason
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsForumRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "dd";
            var type = ChannelType.GuildForum;
            var topic = "aaa";
            var position = 1;
            var permissionOverwrites = new List<IPermissionOverwrite>();
            var parentID = DiscordSnowflake.New(1);
            var isNsfw = true;
            var defaultAutoArchiveDuration = AutoArchiveDuration.Day;
            var defaultReactionEmoji = new DefaultReaction(EmojiName: "booga");
            var availableTags = Array.Empty<IForumTag>();
            var defaultSortOrder = SortOrder.CreationDate;
            var defaultLayout = ForumLayout.GalleryView;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("topic", p => p.Is(topic))
                                .WithProperty("position", p => p.Is(position))
                                .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("parent_id", p => p.Is(parentID.ToString()))
                                .WithProperty("nsfw", p => p.Is(isNsfw))
                                .WithProperty
                                (
                                    "default_auto_archive_duration",
                                    p => p.Is((int)defaultAutoArchiveDuration)
                                )
                                .WithProperty("default_reaction_emoji", p => p.IsObject())
                                .WithProperty("available_tags", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("default_sort_order", p => p.Is((int)defaultSortOrder))
                                .WithProperty("default_forum_layout", p => p.Is((int)defaultLayout))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.CreateGuildForumChannelAsync
            (
                guildId,
                name,
                topic,
                position,
                permissionOverwrites,
                parentID,
                isNsfw,
                defaultAutoArchiveDuration,
                defaultReactionEmoji,
                availableTags,
                defaultSortOrder,
                defaultLayout,
                reason
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsVoiceRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "dd";
            var type = ChannelType.GuildVoice;
            var bitrate = 4600;
            var userLimit = 10;
            var rateLimitPerUser = 10;
            var position = 1;
            var permissionOverwrites = new List<IPermissionOverwrite>();
            var parentID = DiscordSnowflake.New(1);
            var isNsfw = true;
            var rtcRegion = "abcd";
            var videoQualityMode = VideoQualityMode.Full;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("bitrate", p => p.Is(bitrate))
                                .WithProperty("user_limit", p => p.Is(userLimit))
                                .WithProperty("rate_limit_per_user", p => p.Is(rateLimitPerUser))
                                .WithProperty("position", p => p.Is(position))
                                .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("parent_id", p => p.Is(parentID.ToString()))
                                .WithProperty("nsfw", p => p.Is(isNsfw))
                                .WithProperty("rtc_region", p => p.Is(rtcRegion))
                                .WithProperty("video_quality_mode", p => p.Is((int)videoQualityMode))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.CreateGuildVoiceChannelAsync
            (
                guildId,
                name,
                bitrate,
                userLimit,
                rateLimitPerUser,
                position,
                permissionOverwrites,
                parentID,
                isNsfw,
                rtcRegion,
                videoQualityMode,
                reason
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsStageRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "dd";
            var type = ChannelType.GuildStageVoice;
            var bitrate = 4600;
            var userLimit = 10;
            var rateLimitPerUser = 10;
            var position = 1;
            var permissionOverwrites = new List<IPermissionOverwrite>();
            var parentID = DiscordSnowflake.New(1);
            var isNsfw = true;
            var rtcRegion = "abcd";
            var videoQualityMode = VideoQualityMode.Full;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("bitrate", p => p.Is(bitrate))
                                .WithProperty("user_limit", p => p.Is(userLimit))
                                .WithProperty("rate_limit_per_user", p => p.Is(rateLimitPerUser))
                                .WithProperty("position", p => p.Is(position))
                                .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("parent_id", p => p.Is(parentID.ToString()))
                                .WithProperty("nsfw", p => p.Is(isNsfw))
                                .WithProperty("rtc_region", p => p.Is(rtcRegion))
                                .WithProperty("video_quality_mode", p => p.Is((int)videoQualityMode))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.CreateGuildStageChannelAsync
            (
                guildId,
                name,
                bitrate,
                userLimit,
                rateLimitPerUser,
                position,
                permissionOverwrites,
                parentID,
                isNsfw,
                rtcRegion,
                videoQualityMode,
                reason
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNullableRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "dd";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("type", p => p.IsNull())
                                .WithProperty("topic", p => p.IsNull())
                                .WithProperty("bitrate", p => p.IsNull())
                                .WithProperty("user_limit", p => p.IsNull())
                                .WithProperty("rate_limit_per_user", p => p.IsNull())
                                .WithProperty("position", p => p.IsNull())
                                .WithProperty("permission_overwrites", p => p.IsNull())
                                .WithProperty("parent_id", p => p.IsNull())
                                .WithProperty("nsfw", p => p.IsNull())
                                .WithProperty("rtc_region", p => p.IsNull())
                                .WithProperty("video_quality_mode", p => p.IsNull())
                                .WithProperty("default_auto_archive_duration", p => p.IsNull())
                                .WithProperty("default_reaction_emoji", p => p.IsNull())
                                .WithProperty("available_tags", p => p.IsNull())
                                .WithProperty("default_sort_order", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.CreateGuildChannelAsync
            (
                guildId,
                name,
                type: null,
                topic: null,
                bitrate: null,
                userLimit: null,
                rateLimitPerUser: null,
                position: null,
                permissionOverwrites: null,
                parentID: null,
                isNsfw: null,
                rtcRegion: null,
                videoQualityMode: null,
                defaultAutoArchiveDuration: null,
                defaultReactionEmoji: null,
                availableTags: null,
                defaultSortOrder: null,
                defaultForumLayout: null
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildChannelPositionsAsync"/> method.
    /// </summary>
    public class ModifyGuildChannelPositionsAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildChannelPositionsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildChannelPositionsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var swaps = new List<ChannelPositionModification>
            {
                new(DiscordSnowflake.New(1), 1, false, DiscordSnowflake.New(0)),
                new(DiscordSnowflake.New(2), 2, false, DiscordSnowflake.New(0)),
                new(DiscordSnowflake.New(3), 3, false, DiscordSnowflake.New(0)),
                new(DiscordSnowflake.New(4), 4, false, DiscordSnowflake.New(0))
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithJson
                    (
                        j => j.IsArray
                        (
                            a => a
                                .WithCount(swaps.Count)
                                .WithElement
                                (
                                    0,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(1.ToString()))
                                            .WithProperty("position", p => p.Is(1))
                                            .WithProperty("lock_permissions", p => p.Is(false))
                                            .WithProperty("parent_id", p => p.Is("0"))
                                    )
                                )
                                .WithElement
                                (
                                    1,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(2.ToString()))
                                            .WithProperty("position", p => p.Is(2))
                                            .WithProperty("lock_permissions", p => p.Is(false))
                                            .WithProperty("parent_id", p => p.Is("0"))
                                    )
                                )
                                .WithElement
                                (
                                    2,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(3.ToString()))
                                            .WithProperty("position", p => p.Is(3))
                                            .WithProperty("lock_permissions", p => p.Is(false))
                                            .WithProperty("parent_id", p => p.Is("0"))
                                    )
                                )
                                .WithElement
                                (
                                    3,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(4.ToString()))
                                            .WithProperty("position", p => p.Is(4))
                                            .WithProperty("lock_permissions", p => p.Is(false))
                                            .WithProperty("parent_id", p => p.Is("0"))
                                    )
                                )
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.ModifyGuildChannelPositionsAsync(guildId, swaps);

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNullableRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var swaps = new List<ChannelPositionModification>
            {
                new(DiscordSnowflake.New(1), null, null, null),
                new(DiscordSnowflake.New(2), null, null, null),
                new(DiscordSnowflake.New(3), null, null, null),
                new(DiscordSnowflake.New(4), null, null, null)
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithJson
                    (
                        j => j.IsArray
                        (
                            a => a
                                .WithCount(swaps.Count)
                                .WithElement
                                (
                                    0,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(1.ToString()))
                                            .WithProperty("position", p => p.IsNull())
                                            .WithProperty("lock_permissions", p => p.IsNull())
                                            .WithProperty("parent_id", p => p.IsNull())
                                    )
                                )
                                .WithElement
                                (
                                    1,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(2.ToString()))
                                            .WithProperty("position", p => p.IsNull())
                                            .WithProperty("lock_permissions", p => p.IsNull())
                                            .WithProperty("parent_id", p => p.IsNull())
                                    )
                                )
                                .WithElement
                                (
                                    2,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(3.ToString()))
                                            .WithProperty("position", p => p.IsNull())
                                            .WithProperty("lock_permissions", p => p.IsNull())
                                            .WithProperty("parent_id", p => p.IsNull())
                                    )
                                )
                                .WithElement
                                (
                                    3,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(4.ToString()))
                                            .WithProperty("position", p => p.IsNull())
                                            .WithProperty("lock_permissions", p => p.IsNull())
                                            .WithProperty("parent_id", p => p.IsNull())
                                    )
                                )
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.ModifyGuildChannelPositionsAsync(guildId, swaps);

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsEmptyRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var swaps = new List<ChannelPositionModification>
            {
                new(DiscordSnowflake.New(1)),
                new(DiscordSnowflake.New(2)),
                new(DiscordSnowflake.New(3)),
                new(DiscordSnowflake.New(4))
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/channels")
                    .WithJson
                    (
                        j => j.IsArray
                        (
                            a => a
                                .WithCount(swaps.Count)
                                .WithElement
                                (
                                    0,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(1.ToString()))
                                            .WithoutProperty("position")
                                            .WithoutProperty("lock_permissions")
                                            .WithoutProperty("parent_id")
                                    )
                                )
                                .WithElement
                                (
                                    1,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(2.ToString()))
                                            .WithoutProperty("position")
                                            .WithoutProperty("lock_permissions")
                                            .WithoutProperty("parent_id")
                                    )
                                )
                                .WithElement
                                (
                                    2,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(3.ToString()))
                                            .WithoutProperty("position")
                                            .WithoutProperty("lock_permissions")
                                            .WithoutProperty("parent_id")
                                    )
                                )
                                .WithElement
                                (
                                    3,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(4.ToString()))
                                            .WithoutProperty("position")
                                            .WithoutProperty("lock_permissions")
                                            .WithoutProperty("parent_id")
                                    )
                                )
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.ModifyGuildChannelPositionsAsync(guildId, swaps);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildMemberAsync"/> method.
    /// </summary>
    public class GetGuildMemberAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildMemberAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildMemberAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/members/{userId}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildMember)])
            );

            var result = await api.GetGuildMemberAsync(guildId, userId);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ListGuildMembersAsync"/> method.
    /// </summary>
    public class ListGuildMembersAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListGuildMembersAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ListGuildMembersAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var limit = 10;
            var after = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/members")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("after", after.ToString())
                        }
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.ListGuildMembersAsync(guildId, limit, after);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.SearchGuildMembersAsync"/> method.
    /// </summary>
    public class SearchGuildMembersAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchGuildMembersAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public SearchGuildMembersAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var query = "aaa";
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/members/search")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("query", query),
                            new KeyValuePair<string, string>("limit", limit.ToString())
                        }
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.SearchGuildMembersAsync(guildId, query, limit);

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.AddGuildMemberAsync"/> method.
    /// </summary>
    public class AddGuildMemberAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddGuildMemberAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public AddGuildMemberAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsExistingMemberRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var accessToken = "aa";
            var nick = "cdd";
            var roles = new List<Snowflake>();
            var mute = true;
            var deaf = true;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}guilds/{guildId}/members/{userId}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("access_token", p => p.Is(accessToken))
                                .WithProperty("nick", p => p.Is(nick))
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("mute", p => p.Is(mute))
                                .WithProperty("deaf", p => p.Is(deaf))
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.AddGuildMemberAsync
            (
                guildId,
                userId,
                accessToken,
                nick,
                roles,
                mute,
                deaf
            );

            ResultAssert.Successful(result);
            Assert.Null(result.Entity);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNewMemberRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var accessToken = "aa";
            var nick = "cdd";
            var roles = new List<Snowflake>();
            var mute = true;
            var deaf = true;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}guilds/{guildId}/members/{userId}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("access_token", p => p.Is(accessToken))
                                .WithProperty("nick", p => p.Is(nick))
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("mute", p => p.Is(mute))
                                .WithProperty("deaf", p => p.Is(deaf))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildMember)])
            );

            var result = await api.AddGuildMemberAsync
            (
                guildId,
                userId,
                accessToken,
                nick,
                roles,
                mute,
                deaf
            );

            ResultAssert.Successful(result);
            Assert.NotNull(result.Entity);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildMemberAsync"/> method.
    /// </summary>
    public class ModifyGuildMemberAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildMemberAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildMemberAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);

            var nick = "cdd";
            var roles = new List<Snowflake>();
            var mute = true;
            var flags = GuildMemberFlags.BypassesVerification;
            var deaf = true;
            var channelId = DiscordSnowflake.New(2);
            var communicationDisabledUntil = DateTimeOffset.Now.AddDays(2);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/members/{userId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("nick", p => p.Is(nick))
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("mute", p => p.Is(mute))
                                .WithProperty("flags", p => p.Is((int)flags))
                                .WithProperty("deaf", p => p.Is(deaf))
                                .WithProperty("channel_id", p => p.Is(channelId.ToString()))
                                .WithProperty("communication_disabled_until", p => p.IsString())
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.ModifyGuildMemberAsync
            (
                guildId,
                userId,
                nick,
                roles,
                mute,
                deaf,
                channelId,
                communicationDisabledUntil,
                flags,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyCurrentMemberAsync"/> method.
    /// </summary>
    public class ModifyCurrentMemberAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyCurrentMemberAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyCurrentMemberAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var nick = "cdd";
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/members/@me")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("nick", p => p.Is(nick))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildMember)])
            );

            var result = await api.ModifyCurrentMemberAsync
            (
                guildId,
                nick,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.AddGuildMemberRoleAsync"/> method.
    /// </summary>
    public class AddGuildMemberRoleAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddGuildMemberRoleAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public AddGuildMemberRoleAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var roleId = DiscordSnowflake.New(2);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}guilds/{guildId}/members/{userId}/roles/{roleId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.AddGuildMemberRoleAsync
            (
                guildId,
                userId,
                roleId,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.RemoveGuildMemberRoleAsync"/> method.
    /// </summary>
    public class RemoveGuildMemberRoleAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveGuildMemberRoleAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public RemoveGuildMemberRoleAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var roleId = DiscordSnowflake.New(2);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}guilds/{guildId}/members/{userId}/roles/{roleId}"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.RemoveGuildMemberRoleAsync
            (
                guildId,
                userId,
                roleId,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.RemoveGuildMemberAsync"/> method.
    /// </summary>
    public class RemoveGuildMemberAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveGuildMemberAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public RemoveGuildMemberAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}/members/{userId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.RemoveGuildMemberAsync
            (
                guildId,
                userId,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildBansAsync"/> method.
    /// </summary>
    public class GetGuildBansAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildBansAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildBansAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var limit = 1;
            var before = new Snowflake(2);
            var after = new Snowflake(3);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/bans")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("before", before.ToString()),
                            new KeyValuePair<string, string>("after", after.ToString())
                        }
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildBansAsync
            (
                guildId,
                limit,
                before,
                after
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildBanAsync"/> method.
    /// </summary>
    public class GetGuildBanAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildBanAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildBanAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/bans/{userId}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IBan)])
            );

            var result = await api.GetGuildBanAsync
            (
                guildId,
                userId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.CreateGuildBanAsync"/> method.
    /// </summary>
    public class CreateGuildBanAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGuildBanAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateGuildBanAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var deleteMessageSeconds = 864000;
            var reason = "ddd";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}guilds/{guildId}/bans/{userId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("delete_message_seconds", p => p.Is(deleteMessageSeconds))
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.CreateGuildBanAsync
            (
                guildId,
                userId,
                deleteMessageSeconds,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.RemoveGuildBanAsync"/> method.
    /// </summary>
    public class RemoveGuildBanAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveGuildBanAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public RemoveGuildBanAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}/bans/{userId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.RemoveGuildBanAsync
            (
                guildId,
                userId,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildRolesAsync"/> method.
    /// </summary>
    public class GetGuildRolesAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildRolesAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildRolesAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/roles")
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildRolesAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.CreateGuildRoleAsync"/> method.
    /// </summary>
    public class CreateGuildRoleAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGuildRoleAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateGuildRoleAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "brr";
            var permissions = new DiscordPermissionSet(DiscordPermission.Administrator);
            var color = Color.Aqua;
            var hoist = true;

            // Create a dummy PNG image
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            icon.Position = 0;

            var unicodeEmoji = "🦈";

            var mentionable = true;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/roles")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("permissions", p => p.Is(permissions.Value.ToString()))
                                .WithProperty("color", p => p.Is((uint)(color.ToArgb() & 0x00FFFFFF)))
                                .WithProperty("hoist", p => p.Is(hoist))
                                .WithProperty("icon", p => p.IsString())
                                .WithProperty("unicode_emoji", p => p.Is(unicodeEmoji))
                                .WithProperty("mentionable", p => p.Is(mentionable))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IRole)])
            );

            var result = await api.CreateGuildRoleAsync
            (
                guildId,
                name,
                permissions,
                color,
                hoist,
                icon,
                unicodeEmoji,
                mentionable,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildRolePositionsAsync"/> method.
    /// </summary>
    public class ModifyGuildRolePositionsAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildRolePositionsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildRolePositionsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var swaps = new List<(Snowflake ID, Optional<int?> Position)>
            {
                (DiscordSnowflake.New(1), 1),
                (DiscordSnowflake.New(2), 2),
                (DiscordSnowflake.New(3), 3),
                (DiscordSnowflake.New(4), 4)
            };
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/roles")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsArray
                        (
                            a => a
                                .WithCount(swaps.Count)
                                .WithElement
                                (
                                    0,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(1.ToString()))
                                            .WithProperty("position", p => p.Is(1))
                                    )
                                )
                                .WithElement
                                (
                                    1,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(2.ToString()))
                                            .WithProperty("position", p => p.Is(2))
                                    )
                                )
                                .WithElement
                                (
                                    2,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(3.ToString()))
                                            .WithProperty("position", p => p.Is(3))
                                    )
                                )
                                .WithElement
                                (
                                    3,
                                    e => e.IsObject
                                    (
                                        o => o
                                            .WithProperty("id", p => p.Is(4.ToString()))
                                            .WithProperty("position", p => p.Is(4))
                                    )
                                )
                        )
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.ModifyGuildRolePositionsAsync
            (
                guildId,
                swaps,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildRoleAsync"/> method.
    /// </summary>
    public class ModifyGuildRoleAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildRoleAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildRoleAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var roleId = DiscordSnowflake.New(1);
            var name = "ff";
            var permissions = new DiscordPermissionSet(DiscordPermission.Administrator);
            var color = Color.Aqua;
            var hoist = true;

            // Create a dummy PNG image
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            icon.Position = 0;

            var unicodeEmoji = "🦈";

            var mentionable = true;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/roles/{roleId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("permissions", p => p.Is(permissions.Value.ToString()))
                                .WithProperty("color", p => p.Is(color.ToArgb() & 0x00FFFFFF))
                                .WithProperty("hoist", p => p.Is(hoist))
                                .WithProperty("icon", p => p.IsString())
                                .WithProperty("unicode_emoji", p => p.Is(unicodeEmoji))
                                .WithProperty("mentionable", p => p.Is(mentionable))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IRole)])
            );

            var result = await api.ModifyGuildRoleAsync
            (
                guildId,
                roleId,
                name,
                permissions,
                color,
                hoist,
                icon,
                unicodeEmoji,
                mentionable,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildMFALevelAsync"/> method.
    /// </summary>
    public class ModifyGuildMFALevelAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildMFALevelAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildMFALevelAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var mfa = MultiFactorAuthenticationLevel.Elevated;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/mfa")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("level", p => p.Is((int)mfa))
                        )
                    )
                    .Respond("application/json", ((int)mfa).ToString())
            );

            var result = await api.ModifyGuildMFALevelAsync
            (
                guildId,
                mfa
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.DeleteGuildRoleAsync"/> method.
    /// </summary>
    public class DeleteGuildRoleAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteGuildRoleAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteGuildRoleAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var roleId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}/roles/{roleId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildRoleAsync
            (
                guildId,
                roleId,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildPruneCountAsync"/> method.
    /// </summary>
    public class GetGuildPruneCountAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildPruneCountAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildPruneCountAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var days = 2;
            var includeRoles = new List<Snowflake>
            {
                new(1),
                new(2),
                new(3)
            };

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/prune")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("days", days.ToString()),
                            new KeyValuePair<string, string>("include_roles", string.Join(',', includeRoles))
                        }
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IPruneCount)])
            );

            var result = await api.GetGuildPruneCountAsync
            (
                guildId,
                days,
                includeRoles
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.BeginGuildPruneAsync"/> method.
    /// </summary>
    public class BeginGuildPruneAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeginGuildPruneAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public BeginGuildPruneAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var days = 3;
            var computePruneCount = true;
            var includeRoles = new List<Snowflake>();
            var reason = "bab";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/prune")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("days", p => p.Is(days))
                                .WithProperty("compute_prune_count", p => p.Is(computePruneCount))
                                .WithProperty("include_roles", p => p.IsArray(a => a.WithCount(0)))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IPruneCount)])
            );

            var result = await api.BeginGuildPruneAsync
            (
                guildId,
                days,
                computePruneCount,
                includeRoles,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildVoiceRegionsAsync"/> method.
    /// </summary>
    public class GetGuildVoiceRegionsAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildVoiceRegionsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildVoiceRegionsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/regions")
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildVoiceRegionsAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildInvitesAsync"/> method.
    /// </summary>
    public class GetGuildInvitesAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildInvitesAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildInvitesAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/invites")
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildInvitesAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildIntegrationsAsync"/> method.
    /// </summary>
    public class GetGuildIntegrationsAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildIntegrationsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildIntegrationsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/integrations")
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildIntegrationsAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildWidgetSettingsAsync"/> method.
    /// </summary>
    public class GetGuildWidgetSettingsAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildWidgetSettingsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildWidgetSettingsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/widget")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildWidgetSettings)])
            );

            var result = await api.GetGuildWidgetSettingsAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildWidgetAsync"/> method.
    /// </summary>
    public class ModifyGuildWidgetAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildWidgetAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildWidgetAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var enabled = true;
            var channelId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/widget")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("enabled", p => p.Is(enabled))
                                .WithProperty("channel_id", p => p.Is(channelId.ToString()))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildWidgetSettings)])
            );

            var result = await api.ModifyGuildWidgetAsync
            (
                guildId,
                enabled,
                channelId,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildWidgetAsync"/> method.
    /// </summary>
    public class GetGuildWidgetAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildWidgetAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildWidgetAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/widget.json")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildWidget)])
            );

            var result = await api.GetGuildWidgetAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildVanityUrlAsync"/> method.
    /// </summary>
    public class GetGuildVanityUrlAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildVanityUrlAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildVanityUrlAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/vanity-url")
                    .Respond("application/json", SampleRepository.Samples[typeof(IInvite)])
            );

            var result = await api.GetGuildVanityUrlAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildWidgetImageAsync"/> method.
    /// </summary>
    public class GetGuildWidgetImageAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildWidgetImageAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildWidgetImageAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var widgetStyle = WidgetImageStyle.Banner1;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/widget.png")
                    .WithQueryString("style", widgetStyle.ToString().ToLowerInvariant())
                    .Respond("image/png", new MemoryStream())
            );

            var result = await api.GetGuildWidgetImageAsync
            (
                guildId,
                widgetStyle
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildWelcomeScreenAsync"/> method.
    /// </summary>
    public class GetGuildWelcomeScreenAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildWelcomeScreenAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildWelcomeScreenAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/welcome-screen")
                    .Respond("application/json", SampleRepository.Samples[typeof(IWelcomeScreen)])
            );

            var result = await api.GetGuildWelcomeScreenAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildWelcomeScreenAsync"/> method.
    /// </summary>
    public class ModifyGuildWelcomeScreenAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildWelcomeScreenAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildWelcomeScreenAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var isEnabled = true;
            var welcomeChannels = Array.Empty<IWelcomeScreenChannel>();
            var description = "aaa";
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/welcome-screen")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("enabled", p => p.Is(isEnabled))
                                .WithProperty("welcome_channels", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("description", p => p.Is(description))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IWelcomeScreen)])
            );

            var result = await api.ModifyGuildWelcomeScreenAsync
            (
                guildId,
                isEnabled,
                welcomeChannels,
                description,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.GetGuildOnboardingAsync"/> method.
    /// </summary>
    public class GetGuildOnboardingAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildOnboardingAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildOnboardingAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/onboarding")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildOnboarding)])
            );

            var result = await api.GetGuildOnboardingAsync
            (
                guildId
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyGuildOnboardingAsync"/> method.
    /// </summary>
    public class ModifyGuildOnboardingAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildOnboardingAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildOnboardingAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var prompts = Array.Empty<IOnboardingPrompt>();
            var defaultChannelIDs = Array.Empty<Snowflake>();
            var isEnabled = true;
            var mode = GuildOnboardingMode.Default;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}guilds/{guildId}/onboarding")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("prompts", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("default_channel_ids", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("enabled", p => p.Is(isEnabled))
                                .WithProperty("mode", p => p.Is((int)mode))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildOnboarding)])
            );

            var result = await api.ModifyGuildOnboardingAsync
            (
                guildId,
                prompts,
                defaultChannelIDs,
                isEnabled,
                mode,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyCurrentUserVoiceStateAsync"/> method.
    /// </summary>
    public class UpdateCurrentUserVoiceStateAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCurrentUserVoiceStateAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public UpdateCurrentUserVoiceStateAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var channelID = DiscordSnowflake.New(1);
            var suppress = true;
            var requestToSpeakTimestamp = DateTimeOffset.Parse("2020-08-28T18:17:25.377506\u002B00:00");

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/voice-states/@me")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("channel_id", p => p.Is(channelID.ToString()))
                                .WithProperty("suppress", p => p.Is(suppress))
                                .WithProperty
                                (
                                    "request_to_speak_timestamp",
                                    p => p.Is("2020-08-28T18:17:25.377506\u002B00:00")
                                )
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IVoiceState)])
            );

            var result = await api.ModifyCurrentUserVoiceStateAsync
            (
                guildId,
                channelID,
                suppress,
                requestToSpeakTimestamp
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ModifyCurrentUserVoiceStateAsync"/> method.
    /// </summary>
    public class UpdateUserVoiceStateAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserVoiceStateAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public UpdateUserVoiceStateAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var userID = DiscordSnowflake.New(1);
            var channelID = DiscordSnowflake.New(2);
            var suppress = true;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/voice-states/{userID}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("channel_id", p => p.Is(channelID.ToString()))
                                .WithProperty("suppress", p => p.Is(suppress))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IVoiceState)])
            );

            var result = await api.ModifyUserVoiceStateAsync
            (
                guildId,
                userID,
                channelID,
                suppress
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.ListActiveGuildThreadsAsync"/> method.
    /// </summary>
    public class ListActiveThreadsAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListActiveThreadsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ListActiveThreadsAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildID = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}guilds/{guildID}/threads/active"
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildThreadQueryResponse)])
            );

            var result = await api.ListActiveGuildThreadsAsync(guildID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestGuildAPI.DeleteGuildIntegrationAsync"/> method.
    /// </summary>
    public class DeleteGuildIntegrationAsync : RestAPITestBase<IDiscordRestGuildAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteGuildIntegrationAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteGuildIntegrationAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildID = DiscordSnowflake.New(0);
            var integrationID = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildID}/integrations/{integrationID}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildIntegrationAsync(guildID, integrationID, reason);

            ResultAssert.Successful(result);
        }
    }
}
