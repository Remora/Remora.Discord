//
//  DiscordRestChannelAPITests.cs
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Channels
{
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
                var channelID = new Snowflake(0);

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
            public async Task PerformsTextChannelRequestCorrectly()
            {
                var channelId = new Snowflake(0);
                var name = "brr";
                var type = ChannelType.GuildNews;
                var position = 1;
                var topic = "aa";
                var nsfw = true;
                var rateLimitPerUser = 10;
                var permissionOverwrites = new List<PermissionOverwrite>();
                var parentId = new Snowflake(1);

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
                                        .WithProperty("type", p => p.Is((int)type))
                                        .WithProperty("position", p => p.Is(position))
                                        .WithProperty("topic", p => p.Is(topic))
                                        .WithProperty("nsfw", p => p.Is(nsfw))
                                        .WithProperty("rate_limit_per_user", p => p.Is(rateLimitPerUser))
                                        .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                        .WithProperty("parent_id", p => p.Is(parentId.Value.ToString()))
                                )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
                );

                var result = await api.ModifyChannelAsync
                (
                    channelId,
                    name,
                    type,
                    position,
                    topic,
                    nsfw,
                    rateLimitPerUser,
                    default,
                    default,
                    permissionOverwrites,
                    parentId
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
                var channelId = new Snowflake(0);
                var name = "brr";
                var position = 1;
                var bitrate = 8000;
                var userLimit = 10;
                var permissionOverwrites = new List<PermissionOverwrite>();
                var parentId = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Patch, $"{Constants.BaseURL}channels/{channelId}")
                        .WithJson
                        (
                            j => j
                                .IsObject
                                (
                                    o => o
                                        .WithProperty("name", p => p.Is(name))
                                        .WithProperty("position", p => p.Is(position))
                                        .WithProperty("bitrate", p => p.Is(bitrate))
                                        .WithProperty("user_limit", p => p.Is(userLimit))
                                        .WithProperty("permission_overwrites", p => p.IsArray(a => a.WithCount(0)))
                                        .WithProperty("parent_id", p => p.Is(parentId.Value.ToString()))
                                )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
                );

                var result = await api.ModifyChannelAsync
                (
                    channelId,
                    name,
                    default,
                    position,
                    default,
                    default,
                    default,
                    bitrate,
                    userLimit,
                    permissionOverwrites,
                    parentId
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
                var channelId = new Snowflake(0);
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
                                )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
                );

                var result = await api.ModifyChannelAsync
                (
                    channelId,
                    default,
                    default,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
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
                var channelId = new Snowflake(0);
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
                var channelId = new Snowflake(0);
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
                var channelId = new Snowflake(0);
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
                var channelId = new Snowflake(0);
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
                var channelId = new Snowflake(0);
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
                var channelId = new Snowflake(0);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Delete, $"{Constants.BaseURL}channels/{channelId.ToString()}")
                        .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
                );

                var result = await api.DeleteChannelAsync(channelId);
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
                var channelId = new Snowflake(0);
                var before = new Snowflake(1);
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
                var channelId = new Snowflake(0);
                var after = new Snowflake(1);
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
                var channelId = new Snowflake(0);
                var around = new Snowflake(1);
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
                                ? new Snowflake(1)
                                : default(Optional<Snowflake>);

                            var before = (b & 0b010) > 0
                                ? new Snowflake(1)
                                : default(Optional<Snowflake>);

                            var after = (b & 0b001) > 0
                                ? new Snowflake(1)
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
                var channelId = new Snowflake(0);
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

                var channelId = new Snowflake(0);

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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

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
                var channelId = new Snowflake(0);
                var content = "brr";
                var nonce = "aasda";
                var tts = false;
                var allowedMentions = new AllowedMentions(default, default, default, default);

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
                    allowedMentions: allowedMentions
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
                var channelId = new Snowflake(0);

                var embed = new Embed();
                var nonce = "aasda";
                var tts = false;
                var allowedMentions = new AllowedMentions(default, default, default, default);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/messages")
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o
                                    .WithProperty("embed", p => p.IsObject())
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
                    embed: embed,
                    allowedMentions: allowedMentions
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
                var channelId = new Snowflake(0);

                await using var file = new MemoryStream();
                var fileName = "file.bin";
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
                                if (!(m.Content is MultipartFormDataContent multipart))
                                {
                                    return false;
                                }

                                var streamContent = multipart.FirstOrDefault(x => x is StreamContent);
                                if (streamContent?.Headers.ContentDisposition is null)
                                {
                                    return false;
                                }

                                if (streamContent.Headers.ContentDisposition.FileName != fileName)
                                {
                                    return false;
                                }

                                if (!multipart.Any(c => c is StringContent))
                                {
                                    return false;
                                }

                                return true;
                            }
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.CreateMessageAsync
                (
                    channelId,
                    nonce: nonce,
                    isTTS: tts,
                    file: new FileData(fileName, file)
                );

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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);
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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);
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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);
                var userId = new Snowflake(2);

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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);
                var after = new Snowflake(3);
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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);
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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);
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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

                var content = "drr";
                var embed = new Embed();
                var flags = MessageFlags.SuppressEmbeds;

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
                                .WithProperty("embed", p => p.IsObject())
                                .WithProperty("flags", p => p.Is((int)flags))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.EditMessageAsync(channelId, messageId, content, embed, flags);
                ResultAssert.Successful(result);
            }

            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsNullableRequestCorrectly()
            {
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

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
                                    .WithProperty("embed", p => p.IsNull())
                                    .WithProperty("flags", p => p.IsNull())
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.EditMessageAsync(channelId, messageId, null, null, null);
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
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

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
                var channelId = new Snowflake(0);
                var messageIds = new[] { new Snowflake(1), new Snowflake(2) };

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Post,
                            $"{Constants.BaseURL}channels/{channelId}/messages/bulk-delete"
                        )
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

                var result = await api.BulkDeleteMessagesAsync(channelId, messageIds);
                ResultAssert.Successful(result);
            }

            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task ReturnsErrorIfMessageCountIsTooSmall()
            {
                var channelId = new Snowflake(0);
                var messageIds = new[] { new Snowflake(1) };

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
                var channelId = new Snowflake(0);
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
                var channelId = new Snowflake(0);
                var overwriteId = new Snowflake(1);

                var allow = new DiscordPermissionSet(DiscordPermission.Administrator);
                var deny = new DiscordPermissionSet(DiscordPermission.Administrator);
                var type = PermissionOverwriteType.Member;

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
                                    .WithProperty("allow", p => p.Is(allow.Value.ToString()))
                                    .WithProperty("deny", p => p.Is(deny.Value.ToString()))
                                    .WithProperty("type", p => p.Is((int)type))
                            )
                        )
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.EditChannelPermissionsAsync(channelId, overwriteId, allow, deny, type);
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
                var channelId = new Snowflake(0);

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
                var channelId = new Snowflake(0);
                var maxAge = TimeSpan.FromSeconds(10);
                var maxUses = 12;
                var temporary = false;
                var unique = true;
                var targetUser = new Snowflake(1);
                var targetUserType = TargetUserType.Stream;

                var api = CreateAPI
                (
                    b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelId}/invites"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("max_age", p => p.Is(maxAge.TotalSeconds))
                                .WithProperty("max_uses", p => p.Is(maxUses))
                                .WithProperty("temporary", p => p.Is(temporary))
                                .WithProperty("unique", p => p.Is(unique))
                                .WithProperty("target_user_id", p => p.Is(targetUser.Value.ToString()))
                                .WithProperty("target_user_type", p => p.Is((int)targetUserType))
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
                    targetUser,
                    targetUserType
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
                var channelId = new Snowflake(0);
                var overwriteId = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Delete,
                            $"{Constants.BaseURL}channels/{channelId}/permissions/{overwriteId}"
                        )
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.DeleteChannelPermissionAsync(channelId, overwriteId);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestChannelAPI.FollowNewsChannelAsync"/> method.
        /// </summary>
        public class FollowNewsChannelAsync : RestAPITestBase<IDiscordRestChannelAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var channelId = new Snowflake(0);
                var webhookChannelId = new Snowflake(1);

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

                var result = await api.FollowNewsChannelAsync(channelId, webhookChannelId);
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
                var channelId = new Snowflake(0);

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
                var channelId = new Snowflake(0);

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
        /// Tests the <see cref="DiscordRestChannelAPI.AddPinnedChannelMessageAsync"/> method.
        /// </summary>
        public class AddPinnedChannelMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                    .Expect
                    (
                        HttpMethod.Put,
                        $"{Constants.BaseURL}channels/{channelId}/pins/{messageId}"
                    )
                    .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.AddPinnedChannelMessageAsync(channelId, messageId);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestChannelAPI.DeletePinnedChannelMessageAsync"/> method.
        /// </summary>
        public class DeletePinnedChannelMessageAsync : RestAPITestBase<IDiscordRestChannelAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var channelId = new Snowflake(0);
                var messageId = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Delete,
                            $"{Constants.BaseURL}channels/{channelId}/pins/{messageId}"
                        )
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.DeletePinnedChannelMessageAsync(channelId, messageId);
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
                var channelId = new Snowflake(0);
                var userId = new Snowflake(1);
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
                    var channelId = new Snowflake(0);
                    var userId = new Snowflake(1);

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
        }
    }
}
