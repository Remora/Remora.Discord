//
//  DiscordRestChannelAPITests.cs
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using OneOf;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Channels;

/// <summary>
/// Tests the <see cref="DiscordRestChannelAPI"/> class.
/// </summary>
public class DiscordRestChannelAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GetChannelAsync"/> method.
    /// </summary>
    public class GetChannelAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelID}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.GetChannelAsync(channelID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.ModifyChannelAsync"/> method.
    /// </summary>
    public class ModifyChannelAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsGroupDMRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "brr";
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            icon.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId.ToString()}")
                    .WithJson
                    (
                        j => j
                            .IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("icon", p => p.Is("data:image/png;base64,iVBORw0KGgo="))
                            )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.ModifyGroupDMChannelAsync
            (
                channelId,
                name,
                icon
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsTextChannelRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "brr";
            var type = ChannelType.GuildAnnouncement;
            var position = 1;
            var topic = "aa";
            var nsfw = true;
            var rateLimitPerUser = 10;
            var permissionOverwrites = new List<PermissionOverwrite>();
            var parentId = DiscordSnowflake.New(1);
            var defaultAutoArchiveDuration = AutoArchiveDuration.Hour;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId.ToString()}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j
                            .IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("type", p => p.Is((int)type))
                                    .WithProperty("position", p => p.Is(position))
                                    .WithProperty("topic", p => p.Is(topic))
                                    .WithProperty("nsfw", p => p.Is(nsfw))
                                    .WithProperty("rate_limit_per_user", p => p.Is(rateLimitPerUser))
                                    .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                    .WithProperty("parent_id", p => p.Is(parentId.Value.ToString()))
                                    .WithProperty("default_auto_archive_duration", p => p.Is((int)defaultAutoArchiveDuration))
                            )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.ModifyGuildTextChannelAsync
            (
                channelId,
                name,
                type,
                position,
                topic,
                nsfw,
                rateLimitPerUser,
                permissionOverwrites,
                parentId,
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
        public async Task PerformsAnnouncementChannelRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "brr";
            var type = ChannelType.GuildAnnouncement;
            var position = 1;
            var topic = "aa";
            var nsfw = true;
            var permissionOverwrites = new List<PermissionOverwrite>();
            var parentId = DiscordSnowflake.New(1);
            var defaultAutoArchiveDuration = AutoArchiveDuration.Hour;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId.ToString()}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j
                            .IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("type", p => p.Is((int)type))
                                    .WithProperty("position", p => p.Is(position))
                                    .WithProperty("topic", p => p.Is(topic))
                                    .WithProperty("nsfw", p => p.Is(nsfw))
                                    .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                    .WithProperty("parent_id", p => p.Is(parentId.Value.ToString()))
                                    .WithProperty("default_auto_archive_duration", p => p.Is((int)defaultAutoArchiveDuration))
                            )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.ModifyGuildAnnouncementChannelAsync
            (
                channelId,
                name,
                type,
                position,
                topic,
                nsfw,
                permissionOverwrites,
                parentId,
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
        public async Task PerformsVoiceChannelRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "brr";
            var position = 1;
            var isNsfw = true;
            var bitrate = 8000;
            var userLimit = 10;
            var permissionOverwrites = new List<PermissionOverwrite>();
            var parentId = DiscordSnowflake.New(1);
            var rtcRegion = "somewhere";
            var videoQualityMode = VideoQualityMode.Auto;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j
                            .IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("position", p => p.Is(position))
                                    .WithProperty("nsfw", p => p.Is(isNsfw))
                                    .WithProperty("bitrate", p => p.Is(bitrate))
                                    .WithProperty("user_limit", p => p.Is(userLimit))
                                    .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                    .WithProperty("parent_id", p => p.Is(parentId.Value.ToString()))
                                    .WithProperty("rtc_region", p => p.Is(rtcRegion))
                                    .WithProperty("video_quality_mode", p => p.Is((int)videoQualityMode))
                            )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.ModifyGuildVoiceChannelAsync
            (
                channelId,
                name,
                position,
                isNsfw,
                bitrate,
                userLimit,
                permissionOverwrites,
                parentId,
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
        public async Task PerformsStageChannelRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "brr";
            var position = 1;
            var bitrate = 8000;
            var permissionOverwrites = new List<PermissionOverwrite>();
            var rtcRegion = "somewhere";
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j
                            .IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("position", p => p.Is(position))
                                    .WithProperty("bitrate", p => p.Is(bitrate))
                                    .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                    .WithProperty("rtc_region", p => p.Is(rtcRegion))
                            )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.ModifyGuildStageChannelAsync
            (
                channelId,
                name,
                position,
                bitrate,
                permissionOverwrites,
                rtcRegion,
                reason
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsThreadChannelRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "brr";
            var isArchived = true;
            var autoArchiveDuration = AutoArchiveDuration.Hour;
            var isLocked = false;
            var isInvitable = true;
            var rateLimitPerUser = 10;
            var reason = "test";
            var flags = ChannelFlags.Pinned;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId.ToString()}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j
                            .IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("archived", p => p.Is(isArchived))
                                    .WithProperty("auto_archive_duration", p => p.Is((int)autoArchiveDuration))
                                    .WithProperty("locked", p => p.Is(isLocked))
                                    .WithProperty("invitable", p => p.Is(isInvitable))
                                    .WithProperty("rate_limit_per_user", p => p.Is(rateLimitPerUser))
                                    .WithProperty("flags", p => p.Is((int)flags))
                            )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.ModifyThreadChannelAsync
            (
                channelId,
                name,
                isArchived,
                autoArchiveDuration,
                isLocked,
                isInvitable,
                rateLimitPerUser,
                flags,
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
            var channelId = DiscordSnowflake.New(0);
            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId.ToString()}")
                    .WithJson
                    (
                        j => j
                            .IsObject
                            (
                                o => o
                                    .WithProperty("position", p => p.IsNull())
                                    .WithProperty("topic", p => p.IsNull())
                                    .WithProperty("nsfw", p => p.IsNull())
                                    .WithProperty("rate_limit_per_user", p => p.IsNull())
                                    .WithProperty("bitrate", p => p.IsNull())
                                    .WithProperty("user_limit", p => p.IsNull())
                                    .WithProperty("permission_overwrites", p => p.IsNull())
                                    .WithProperty("parent_id", p => p.IsNull())
                                    .WithProperty("video_quality_mode", p => p.IsNull())
                                    .WithProperty("rtc_region", p => p.IsNull())
                                    .WithProperty("default_auto_archive_duration", p => p.IsNull())
                            )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.ModifyChannelAsync
            (
                channelId,
                position: null,
                topic: null,
                isNsfw: null,
                rateLimitPerUser: null,
                bitrate: null,
                userLimit: null,
                permissionOverwrites: null,
                parentID: null,
                videoQualityMode: null,
                rtcRegion: null,
                defaultAutoArchiveDuration: null
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API checks parameter lengths correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfNameIsTooLong()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = new string('b', 101);

            var api = CreateAPI(_ => { });

            var result = await api.ModifyChannelAsync
            (
                channelId,
                name
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API checks parameter lengths correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfNameIsTooShort()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = new string('b', 1);

            var api = CreateAPI(_ => { });

            var result = await api.ModifyChannelAsync
            (
                channelId,
                name
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API checks parameter lengths correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfTopicIsTooLong()
        {
            var channelId = DiscordSnowflake.New(0);
            var topic = new string('b', 1025);

            var api = CreateAPI(_ => { });

            var result = await api.ModifyChannelAsync
            (
                channelId,
                topic: topic
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API checks parameter lengths correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfUserLimitIsTooSmall()
        {
            var channelId = DiscordSnowflake.New(0);
            var userLimit = -1;

            var api = CreateAPI(_ => { });

            var result = await api.ModifyChannelAsync
            (
                channelId,
                userLimit: userLimit
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API checks parameter lengths correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfUserLimitIsTooLarge()
        {
            var channelId = DiscordSnowflake.New(0);
            var userLimit = 100;

            var api = CreateAPI(_ => { });

            var result = await api.ModifyChannelAsync
            (
                channelId,
                userLimit: userLimit
            );

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteChannelAsync"/> method.
    /// </summary>
    public class DeleteChannelAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}channels/{channelId.ToString()}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.DeleteChannelAsync(channelId, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GetChannelMessagesAsync"/> method.
    /// </summary>
    public class GetChannelMessagesAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsBeforeRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var before = DiscordSnowflake.New(1);
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("before", before.ToString())
                        }
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetChannelMessagesAsync(channelId, before: before, limit: limit);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsAfterRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var after = DiscordSnowflake.New(1);
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelId}/messages")
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

            var result = await api.GetChannelMessagesAsync(channelId, after: after, limit: limit);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsAroundRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var around = DiscordSnowflake.New(1);
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("around", around.ToString())
                        }
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetChannelMessagesAsync(channelId, around, limit: limit);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Gets the data set for <see cref="AroundBeforeAndAfterAreMutuallyExclusive"/>.
        /// </summary>
        public static IEnumerable<object[]> MutuallyExclusivePermutations
        {
            get
            {
                // Get all permutations where more than one bit is set
                var bitPatterns = Enumerable.Range(0, 7).Where(i => (i & (i - 1)) != 0 && i != 0);
                var parameterPermutations = bitPatterns.Select
                (
                    b =>
                    {
                        var around = (b & 0b100) > 0
                            ? DiscordSnowflake.New(1)
                            : default(Optional<Snowflake>);

                        var before = (b & 0b010) > 0
                            ? DiscordSnowflake.New(1)
                            : default(Optional<Snowflake>);

                        var after = (b & 0b001) > 0
                            ? DiscordSnowflake.New(1)
                            : default(Optional<Snowflake>);

                        return new object[] { around, before, after };
                    }
                );

                return parameterPermutations;
            }
        }

        /// <summary>
        /// Tests whether the around, before, and after parameters are mutually exclusive.
        /// </summary>
        /// <param name="around">The message to search around.</param>
        /// <param name="before">The message to search before.</param>
        /// <param name="after">The message to search after.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Theory]
        [MemberData(nameof(MutuallyExclusivePermutations))]
        public async Task AroundBeforeAndAfterAreMutuallyExclusive
        (
            Optional<Snowflake> around,
            Optional<Snowflake> before,
            Optional<Snowflake> after
        )
        {
            var channelId = DiscordSnowflake.New(0);
            var limit = 10;

            var expectedQueryStringParameters = new List<KeyValuePair<string, string>>
            {
                new("limit", limit.ToString())
            };

            if (around.HasValue)
            {
                expectedQueryStringParameters.Add
                (
                    new KeyValuePair<string, string>("around", around.Value.ToString())
                );
            }

            if (before.HasValue)
            {
                expectedQueryStringParameters.Add
                (
                    new KeyValuePair<string, string>("before", before.Value.ToString())
                );
            }

            if (after.HasValue)
            {
                expectedQueryStringParameters.Add
                (
                    new KeyValuePair<string, string>("after", after.Value.ToString())
                );
            }

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .WithQueryString(expectedQueryStringParameters)
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetChannelMessagesAsync(channelId, around, before, after, limit);
            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API endpoint is correctly limited.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfLimitIsOutsideValidRange()
        {
            var api = CreateAPI(_ => { });

            var channelId = DiscordSnowflake.New(0);

            var result = await api.GetChannelMessagesAsync(channelId, default, default, default, 0);

            ResultAssert.Unsuccessful(result);

            result = await api.GetChannelMessagesAsync(channelId, default, default, default, 101);

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GetChannelMessageAsync"/> method.
    /// </summary>
    public class GetChannelMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.GetChannelMessageAsync(channelId, messageId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.CreateMessageAsync"/> method.
    /// </summary>
    public class CreateMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNormalRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var content = "brr";
            var nonce = "aasda";
            var tts = false;
            var allowedMentions = new AllowedMentions();
            var flags = MessageFlags.SuppressEmbeds;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("content", p => p.Is(content))
                                .WithProperty("nonce", p => p.Is(nonce))
                                .WithProperty("tts", p => p.Is(tts))
                                .WithProperty("allowed_mentions", p => p.IsObject())
                                .WithProperty("flags", p => p.Is((int)flags))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateMessageAsync
            (
                channelId,
                content,
                nonce,
                tts,
                allowedMentions: allowedMentions,
                flags: flags
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsEmbedRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var embeds = new List<Embed>();
            var nonce = "aasda";
            var tts = false;
            var allowedMentions = new AllowedMentions();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("embeds", p => p.IsArray())
                                .WithProperty("nonce", p => p.Is(nonce))
                                .WithProperty("tts", p => p.Is(tts))
                                .WithProperty("allowed_mentions", p => p.IsObject())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateMessageAsync
            (
                channelId,
                nonce: nonce,
                isTTS: tts,
                embeds: embeds,
                allowedMentions: allowedMentions
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsComponentRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var embeds = new List<Embed>();
            var nonce = "aasda";
            var tts = false;
            var allowedMentions = new AllowedMentions();
            var components = new List<IMessageComponent>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("embeds", p => p.IsArray())
                                .WithProperty("nonce", p => p.Is(nonce))
                                .WithProperty("tts", p => p.Is(tts))
                                .WithProperty("allowed_mentions", p => p.IsObject())
                                .WithProperty("components", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateMessageAsync
            (
                channelId,
                nonce: nonce,
                isTTS: tts,
                embeds: embeds,
                allowedMentions: allowedMentions,
                components: components
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsFileUploadRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            await using var file = new MemoryStream();
            var fileName = "file.bin";
            var description = "wooga";

            var nonce = "aasda";
            var tts = false;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .With
                    (
                        m =>
                        {
                            if (m.Content is not MultipartFormDataContent multipart)
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[0]", fileName))
                            {
                                return false;
                            }

                            return multipart.ContainsContent<StringContent>("payload_json");
                        }
                    )
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("nonce", p => p.Is(nonce))
                                .WithProperty("tts", p => p.Is(tts))
                                .WithProperty("attachments", p => p.IsArray
                                (
                                    a => a
                                        .WithElement
                                        (
                                            0,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(0.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName))
                                                    .WithProperty("description", ep => ep.Is(description))
                                            )
                                        )
                                ))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateMessageAsync
            (
                channelId,
                nonce: nonce,
                isTTS: tts,
                attachments: new OneOf<FileData, IPartialAttachment>[] { new FileData(fileName, file, description) }
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsMultiFileUploadRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            await using var file1 = new MemoryStream();
            await using var file2 = new MemoryStream();
            var fileName1 = "file1.bin";
            var fileName2 = "file2.bin";

            var description1 = "wooga";
            var description2 = "booga";

            var nonce = "aasda";
            var tts = false;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .With
                    (
                        m =>
                        {
                            if (m.Content is not MultipartFormDataContent multipart)
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[0]", fileName1))
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[1]", fileName2))
                            {
                                return false;
                            }

                            return multipart.ContainsContent<StringContent>("payload_json");
                        }
                    )
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("nonce", p => p.Is(nonce))
                                .WithProperty("tts", p => p.Is(tts))
                                .WithProperty("attachments", p => p.IsArray
                                (
                                    a => a
                                        .WithElement
                                        (
                                            0,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(0.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName1))
                                                    .WithProperty("description", ep => ep.Is(description1))
                                            )
                                        )
                                        .WithElement
                                        (
                                            1,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(1.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName2))
                                                    .WithProperty("description", ep => ep.Is(description2))
                                            )
                                        )
                                ))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateMessageAsync
            (
                channelId,
                nonce: nonce,
                isTTS: tts,
                attachments: new OneOf<FileData, IPartialAttachment>[]
                {
                    new FileData(fileName1, file1, description1),
                    new FileData(fileName2, file2, description2)
                }
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRetainingFileUploadRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            await using var file = new MemoryStream();
            var fileName = "file.bin";

            var description = "wooga";

            var nonce = "aasda";
            var tts = false;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/messages")
                    .With
                    (
                        m =>
                        {
                            if (m.Content is not MultipartFormDataContent multipart)
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[0]", fileName))
                            {
                                return false;
                            }

                            return multipart.ContainsContent<StringContent>("payload_json");
                        }
                    )
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("nonce", p => p.Is(nonce))
                                .WithProperty("tts", p => p.Is(tts))
                                .WithProperty("attachments", p => p.IsArray
                                (
                                    a => a
                                        .WithElement
                                        (
                                            0,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(0.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName))
                                                    .WithProperty("description", ep => ep.Is(description))
                                            )
                                        )
                                        .WithElement
                                        (
                                            1,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(999.ToString()))
                                            )
                                        )
                                ))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateMessageAsync
            (
                channelId,
                nonce: nonce,
                isTTS: tts,
                attachments: new OneOf<FileData, IPartialAttachment>[]
                {
                    new FileData(fileName, file, description),
                    new PartialAttachment(DiscordSnowflake.New(999))
                }
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.CrosspostMessageAsync"/> method.
    /// </summary>
    public class CrosspostMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/crosspost"
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CrosspostMessageAsync(channelId, messageId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.CreateReactionAsync"/> method.
    /// </summary>
    public class CreateReactionAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var urlEncodedEmoji = HttpUtility.UrlEncode("🔥");

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions/{urlEncodedEmoji}/@me"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.CreateReactionAsync(channelId, messageId, "🔥");
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteOwnReactionAsync"/> method.
    /// </summary>
    public class DeleteOwnReactionAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var urlEncodedEmoji = HttpUtility.UrlEncode("🔥");

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions/{urlEncodedEmoji}/@me"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteOwnReactionAsync(channelId, messageId, "🔥");
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteUserReactionAsync"/> method.
    /// </summary>
    public class DeleteUserReactionAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var userId = DiscordSnowflake.New(2);

            var urlEncodedEmoji = HttpUtility.UrlEncode("🔥");

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions/{urlEncodedEmoji}/{userId}"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteUserReactionAsync(channelId, messageId, "🔥", userId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GetReactionsAsync"/> method.
    /// </summary>
    public class GetReactionsAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var after = DiscordSnowflake.New(3);
            var limit = 10;
            var urlEncodedEmoji = HttpUtility.UrlEncode("🔥");

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions/{urlEncodedEmoji}"
                    )
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("after", after.ToString()),
                            new KeyValuePair<string, string>("limit", limit.ToString())
                        }
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.GetReactionsAsync(channelId, messageId, "🔥", after, limit);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfLimitIsTooLow()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var limit = 0;
            var urlEncodedEmoji = HttpUtility.UrlEncode("🔥");

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions/{urlEncodedEmoji}"
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.GetReactionsAsync(channelId, messageId, "🔥", limit: limit);
            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfLimitIsTooHigh()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var limit = 101;
            var urlEncodedEmoji = HttpUtility.UrlEncode("🔥");

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions/{urlEncodedEmoji}"
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.GetReactionsAsync(channelId, messageId, "🔥", limit: limit);
            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteAllReactionsAsync"/> method.
    /// </summary>
    public class DeleteAllReactionsAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteAllReactionsAsync(channelId, messageId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteAllReactionsForEmojiAsync"/> method.
    /// </summary>
    public class DeleteAllReactionsForEmojiAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            var urlEncodedEmoji = HttpUtility.UrlEncode("🔥");

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/reactions/{urlEncodedEmoji}"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteAllReactionsForEmojiAsync(channelId, messageId, "🔥");
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.EditMessageAsync"/> method.
    /// </summary>
    public class EditMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            var content = "drr";
            var embeds = new List<Embed>();
            var flags = MessageFlags.SuppressEmbeds;
            var attachments = new List<OneOf<FileData, IPartialAttachment>>();
            var components = new List<IMessageComponent>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("content", p => p.Is(content))
                                .WithProperty("embeds", p => p.IsArray())
                                .WithProperty("flags", p => p.Is((int)flags))
                                .WithProperty("attachments", p => p.IsArray(a => a.WithCount(0)))
                                .WithProperty("components", p => p.IsArray())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EditMessageAsync
            (
                channelId,
                messageId,
                content,
                embeds,
                flags,
                attachments: attachments,
                components: components
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
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("content", p => p.IsNull())
                                .WithProperty("embeds", p => p.IsNull())
                                .WithProperty("flags", p => p.IsNull())
                                .WithProperty("allowed_mentions", p => p.IsNull())
                                .WithProperty("components", p => p.IsNull())
                                .WithProperty("flags", p => p.IsNull())
                                .WithProperty("attachments", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EditMessageAsync
            (
                channelId,
                messageId,
                content: null,
                embeds: null,
                flags: null,
                allowedMentions: null,
                components: null,
                attachments: null
            );
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsFileUploadRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            await using var file = new MemoryStream();
            var fileName = "file.bin";
            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}")
                    .With
                    (
                        m =>
                        {
                            if (m.Content is not MultipartFormDataContent multipart)
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[0]", fileName))
                            {
                                return false;
                            }

                            return multipart.ContainsContent<StringContent>("payload_json");
                        }
                    )
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("attachments", p => p.IsArray
                                (
                                    a => a
                                        .WithElement
                                        (
                                            0,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(0.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName))
                                                    .WithProperty("description", ep => ep.Is(description))
                                            )
                                        )
                                ))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EditMessageAsync
            (
                channelId,
                messageId,
                attachments: new OneOf<FileData, IPartialAttachment>[] { new FileData(fileName, file, description) }
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsMultiFileUploadRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            await using var file1 = new MemoryStream();
            await using var file2 = new MemoryStream();
            var fileName1 = "file1.bin";
            var fileName2 = "file2.bin";

            var description1 = "wooga";
            var description2 = "booga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}")
                    .With
                    (
                        m =>
                        {
                            if (m.Content is not MultipartFormDataContent multipart)
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[0]", fileName1))
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[1]", fileName2))
                            {
                                return false;
                            }

                            return multipart.ContainsContent<StringContent>("payload_json");
                        }
                    )
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("attachments", p => p.IsArray
                                (
                                    a => a
                                        .WithElement
                                        (
                                            0,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(0.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName1))
                                                    .WithProperty("description", ep => ep.Is(description1))
                                            )
                                        )
                                        .WithElement
                                        (
                                            1,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(1.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName2))
                                                    .WithProperty("description", ep => ep.Is(description2))
                                            )
                                        )
                                ))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EditMessageAsync
            (
                channelId,
                messageId,
                attachments: new OneOf<FileData, IPartialAttachment>[]
                {
                    new FileData(fileName1, file1, description1),
                    new FileData(fileName2, file2, description2)
                }
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRetainingFileUploadRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            await using var file = new MemoryStream();
            var fileName = "file.bin";

            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}")
                    .With
                    (
                        m =>
                        {
                            if (m.Content is not MultipartFormDataContent multipart)
                            {
                                return false;
                            }

                            if (!multipart.ContainsContent("files[0]", fileName))
                            {
                                return false;
                            }

                            return multipart.ContainsContent<StringContent>("payload_json");
                        }
                    )
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("attachments", p => p.IsArray
                                (
                                    a => a
                                        .WithElement
                                        (
                                            0,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(0.ToString()))
                                                    .WithProperty("filename", ep => ep.Is(fileName))
                                                    .WithProperty("description", ep => ep.Is(description))
                                            )
                                        )
                                        .WithElement
                                        (
                                            1,
                                            e => e.IsObject
                                            (
                                                eo => eo
                                                    .WithProperty("id", ep => ep.Is(999.ToString()))
                                            )
                                        )
                                ))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EditMessageAsync
            (
                channelId,
                messageId,
                attachments: new OneOf<FileData, IPartialAttachment>[]
                {
                    new FileData(fileName, file, description),
                    new PartialAttachment(DiscordSnowflake.New(999))
                }
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteMessageAsync"/> method.
    /// </summary>
    public class DeleteMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteMessageAsync(channelId, messageId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteAllReactionsForEmojiAsync"/> method.
    /// </summary>
    public class BulkDeleteMessagesAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageIds = new[] { DiscordSnowflake.New(1), DiscordSnowflake.New(2) };
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/messages/bulk-delete"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty
                                (
                                    "messages",
                                    p => p.IsArray
                                    (
                                        a => a
                                            .WithCount(messageIds.Length)
                                    )
                                )
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.BulkDeleteMessagesAsync(channelId, messageIds, reason);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfMessageCountIsTooSmall()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageIds = new[] { DiscordSnowflake.New(1) };

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/messages/bulk-delete"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.BulkDeleteMessagesAsync(channelId, messageIds);
            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfMessageCountIsTooLarge()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageIds = new Snowflake[101];

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/messages/bulk-delete"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.BulkDeleteMessagesAsync(channelId, messageIds);
            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.EditChannelPermissionsAsync"/> method.
    /// </summary>
    public class EditChannelPermissionsAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var overwriteId = DiscordSnowflake.New(1);

            var allow = new DiscordPermissionSet(DiscordPermission.Administrator);
            var deny = new DiscordPermissionSet(DiscordPermission.Administrator);
            var type = PermissionOverwriteType.Member;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/permissions/{overwriteId}"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("allow", p => p.Is(allow.Value.ToString()))
                                .WithProperty("deny", p => p.Is(deny.Value.ToString()))
                                .WithProperty("type", p => p.Is((int)type))
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.EditChannelPermissionsAsync(channelId, overwriteId, allow, deny, type, reason);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNullableRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var overwriteId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/permissions/{overwriteId}"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("allow", p => p.IsNull())
                                .WithProperty("deny", p => p.IsNull())
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.EditChannelPermissionsAsync(channelId, overwriteId, null, null);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GetChannelInvitesAsync"/> method.
    /// </summary>
    public class GetChannelInvitesAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelId}/invites"
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.GetChannelInvitesAsync(channelId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.CreateChannelInviteAsync"/> method.
    /// </summary>
    public class CreateChannelInviteAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var maxAge = TimeSpan.FromSeconds(10);
            var maxUses = 12;
            var temporary = false;
            var unique = true;
            var targetUser = DiscordSnowflake.New(1);
            var targetType = InviteTarget.Stream;
            var targetApplication = DiscordSnowflake.New(2);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/invites"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("max_age", p => p.Is(maxAge.TotalSeconds))
                                .WithProperty("max_uses", p => p.Is(maxUses))
                                .WithProperty("temporary", p => p.Is(temporary))
                                .WithProperty("unique", p => p.Is(unique))
                                .WithProperty("target_type", p => p.Is((int)targetType))
                                .WithProperty("target_user_id", p => p.Is(targetUser.Value.ToString()))
                                .WithProperty("target_application_id", p => p.Is(targetApplication.Value.ToString()))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IInvite)])
            );

            var result = await api.CreateChannelInviteAsync
            (
                channelId,
                maxAge,
                maxUses,
                temporary,
                unique,
                targetType,
                targetUser,
                targetApplication,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.DeleteChannelPermissionAsync"/> method.
    /// </summary>
    public class DeleteChannelPermissionAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var overwriteId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/permissions/{overwriteId}"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteChannelPermissionAsync(channelId, overwriteId, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.FollowAnnouncementChannelAsync"/> method.
    /// </summary>
    public class FollowAnnouncementChannelAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var webhookChannelId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/followers"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o.WithProperty("webhook_channel_id", p => p.Is(webhookChannelId.ToString()))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IFollowedChannel)])
            );

            var result = await api.FollowAnnouncementChannelAsync(channelId, webhookChannelId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.TriggerTypingIndicatorAsync"/> method.
    /// </summary>
    public class TriggerTypingIndicatorAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/typing"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.TriggerTypingIndicatorAsync(channelId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GetPinnedMessagesAsync"/> method.
    /// </summary>
    public class GetPinnedMessagesAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelId}/pins"
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.GetPinnedMessagesAsync(channelId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.PinMessageAsync"/> method.
    /// </summary>
    public class PinMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/pins/{messageId}"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.PinMessageAsync(channelId, messageId, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.UnpinMessageAsync"/> method.
    /// </summary>
    public class UnpinMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/pins/{messageId}"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.UnpinMessageAsync(channelId, messageId, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GroupDMAddRecipientAsync"/> method.
    /// </summary>
    public class GroupDMAddRecipientAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);
            var accessToken = "fbb";
            var nick = "bb";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/recipients/{userId}"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("access_token", p => p.Is(accessToken))
                                .WithProperty("nick", p => p.Is(nick))
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.GroupDMAddRecipientAsync(channelId, userId, accessToken, nick);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GroupDMRemoveRecipientAsync"/> method.
    /// </summary>
    public class GroupDMRemoveRecipientAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/recipients/{userId}"
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.GroupDMRemoveRecipientAsync(channelId, userId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.StartThreadWithMessageAsync"/> method.
    /// </summary>
    public class StartThreadWithMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var messageId = DiscordSnowflake.New(1);
            var name = "abba";
            var duration = AutoArchiveDuration.Hour;
            var reason = "test";
            var rateLimit = 1;

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/messages/{messageId}/threads"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("auto_archive_duration", p => p.Is((int)duration))
                                .WithProperty("rate_limit_per_user", p => p.Is(rateLimit))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.StartThreadWithMessageAsync(channelId, messageId, name, duration, rateLimit, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="StartThreadWithoutMessageAsync"/> method.
    /// </summary>
    public class StartThreadWithoutMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "abba";
            var duration = AutoArchiveDuration.Hour;
            var type = ChannelType.PrivateThread;
            var isInvitable = true;
            var rateLimit = 1;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/threads"
                    )
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("auto_archive_duration", p => p.Is((int)duration))
                                .WithProperty("type", p => p.Is((int)type))
                                .WithProperty("invitable", p => p.Is(isInvitable))
                                .WithProperty("rate_limit_per_user", p => p.Is(rateLimit))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.StartThreadWithoutMessageAsync
            (
                channelId,
                name,
                type,
                duration,
                isInvitable,
                rateLimit,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.JoinThreadAsync"/> method.
    /// </summary>
    public class JoinThreadAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/thread-members/@me"
                    )
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.JoinThreadAsync(channelId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.AddThreadMemberAsync"/> method.
    /// </summary>
    public class AddThreadMemberAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/thread-members/{userId}"
                    )
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.AddThreadMemberAsync(channelId, userId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.LeaveThreadAsync"/> method.
    /// </summary>
    public class LeaveThreadAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/thread-members/@me"
                    )
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.LeaveThreadAsync(channelId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.RemoveThreadMemberAsync"/> method.
    /// </summary>
    public class RemoveThreadMemberAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}channels/{channelId}/thread-members/{userId}"
                    )
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.RemoveThreadMemberAsync(channelId, userId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.GetThreadMemberAsync"/> method.
    /// </summary>
    public class GetThreadMemberAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);
            var userId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelId}/thread-members/{userId}"
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IThreadMember)])
            );

            var result = await api.GetThreadMemberAsync(channelId, userId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.ListThreadMembersAsync"/> method.
    /// </summary>
    public class ListThreadMembersAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelId}/thread-members"
                    )
                    .WithNoContent()
                    .Respond("application/json", "[]")
            );

            var result = await api.ListThreadMembersAsync(channelId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.ListPublicArchivedThreadsAsync"/> method.
    /// </summary>
    public class ListPublicArchivedThreadsAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(0);
            var before = DateTimeOffset.UtcNow;
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelID}/threads/archived/public"
                    )
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("before", before.ToISO8601String())
                        }
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannelThreadQueryResponse)])
            );

            var result = await api.ListPublicArchivedThreadsAsync(channelID, before, limit);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.ListPrivateArchivedThreadsAsync"/> method.
    /// </summary>
    public class ListPrivateArchivedThreadsAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(0);
            var before = DateTimeOffset.UtcNow;
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelID}/threads/archived/private"
                    )
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("before", before.ToISO8601String())
                        }
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannelThreadQueryResponse)])
            );

            var result = await api.ListPrivateArchivedThreadsAsync(channelID, before, limit);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestChannelAPI.ListJoinedPrivateArchivedThreadsAsync"/> method.
    /// </summary>
    public class ListJoinedPrivateArchivedThreadsAsync : RestAPITestBase<IDiscordRestChannelAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(0);
            var before = DiscordSnowflake.New(1);
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}channels/{channelID}/users/@me/threads/archived/private"
                    )
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString()),
                            new KeyValuePair<string, string>("before", before.ToString())
                        }
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannelThreadQueryResponse)])
            );

            var result = await api.ListJoinedPrivateArchivedThreadsAsync(channelID, before, limit);
            ResultAssert.Successful(result);
        }
    }
}
