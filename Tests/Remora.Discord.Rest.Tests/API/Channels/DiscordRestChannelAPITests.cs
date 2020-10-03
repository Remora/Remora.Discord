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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        /// Tests the <see cref="GetChannel"/> method.
        /// </summary>
        public class GetChannel : RestAPITestBase<IDiscordRestChannelAPI>
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
                        .Respond("application/json", "{ \"id\": 0, \"type\": 0 }")
                );

                var result = await api.GetChannelAsync(channelID);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="ModifyChannel"/> method.
        /// </summary>
        public class ModifyChannel : RestAPITestBase<IDiscordRestChannelAPI>
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
                        .Respond("application/json", "{ \"id\": 0, \"type\": 0 }")
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
                        .Respond("application/json", "{ \"id\": 0, \"type\": 0 }")
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
            /// Tests whether the API checks parameter lengths correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task ReturnsErrorIfNameIsTooLong()
            {
                var channelId = new Snowflake(0);
                var name = new string('b', 101);

                var api = CreateAPI(b => { });

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

                var api = CreateAPI(b => { });

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

                var api = CreateAPI(b => { });

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

                var api = CreateAPI(b => { });

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

                var api = CreateAPI(b => { });

                var result = await api.ModifyChannelAsync
                (
                    channelId,
                    userLimit: userLimit
                );

                ResultAssert.Unsuccessful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DeleteChannel"/> method.
        /// </summary>
        public class DeleteChannel : RestAPITestBase<IDiscordRestChannelAPI>
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
                        .Respond("application/json", "{ \"id\": 0, \"type\": 0 }")
                );

                var result = await api.DeleteChannelAsync(channelId);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="GetChannelMessages"/> method.
        /// </summary>
        public class GetChannelMessages : RestAPITestBase<IDiscordRestChannelAPI>
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
                                new KeyValuePair<string, string>("before", before.ToString()),
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
                                new KeyValuePair<string, string>("after", after.ToString()),
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
                                new KeyValuePair<string, string>("around", around.ToString()),
                            }
                        )
                        .Respond("application/json", "[ ]")
                );

                var result = await api.GetChannelMessagesAsync(channelId, around: around, limit: limit);
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
                    new KeyValuePair<string, string>("limit", limit.ToString())
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
                var api = CreateAPI(b => { });

                var channelId = new Snowflake(0);

                var result = await api.GetChannelMessagesAsync(channelId, default, default, default, 0);

                ResultAssert.Unsuccessful(result);

                result = await api.GetChannelMessagesAsync(channelId, default, default, default, 101);

                ResultAssert.Unsuccessful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="GetChannelMessage"/> method.
        /// </summary>
        public class GetChannelMessage : RestAPITestBase<IDiscordRestChannelAPI>
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
                        .Respond("application/json", "{\n    \"id\": 1,\n    \"channel_id\": 0,\n    \"author\": {\n        \"id\": 3,\n        \"username\": \"b\",\n        \"discriminator\": \"0000\",\n        \"avatar\": null\n    },\n    \"content\": \"brr\",\n    \"timestamp\": \"2020-08-28T18:17:25.377506\\u002B00:00\",\n    \"edited_timestamp\": null,\n    \"tts\": false,\n    \"mention_everyone\": false,\n    \"mentions\": [],\n    \"mention_roles\": [],\n    \"mention_channels\": [],\n    \"attachments\": [],\n    \"embeds\": [],\n    \"pinned\": false,\n    \"type\": 0\n}")
                );

                var result = await api.GetChannelMessageAsync(channelId, messageId);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="CreateMessage"/> method.
        /// </summary>
        public class CreateMessage : RestAPITestBase<IDiscordRestChannelAPI>
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
                var allowedMentions = new AllowedMentions(default, default, default);

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
                        .Respond("application/json", "{\n    \"id\": 1,\n    \"channel_id\": 0,\n    \"author\": {\n        \"id\": 3,\n        \"username\": \"b\",\n        \"discriminator\": \"0000\",\n        \"avatar\": null\n    },\n    \"content\": \"brr\",\n    \"timestamp\": \"2020-08-28T18:17:25.377506\\u002B00:00\",\n    \"edited_timestamp\": null,\n    \"tts\": false,\n    \"mention_everyone\": false,\n    \"mentions\": [],\n    \"mention_roles\": [],\n    \"mention_channels\": [],\n    \"attachments\": [],\n    \"embeds\": [],\n    \"pinned\": false,\n    \"type\": 0\n}")
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
                var allowedMentions = new AllowedMentions(default, default, default);

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
                        .Respond("application/json", "{\n    \"id\": 1,\n    \"channel_id\": 0,\n    \"author\": {\n        \"id\": 3,\n        \"username\": \"b\",\n        \"discriminator\": \"0000\",\n        \"avatar\": null\n    },\n    \"content\": \"brr\",\n    \"timestamp\": \"2020-08-28T18:17:25.377506\\u002B00:00\",\n    \"edited_timestamp\": null,\n    \"tts\": false,\n    \"mention_everyone\": false,\n    \"mentions\": [],\n    \"mention_roles\": [],\n    \"mention_channels\": [],\n    \"attachments\": [],\n    \"embeds\": [],\n    \"pinned\": false,\n    \"type\": 0\n}")
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

                                if (!multipart.Any(c => c is StreamContent))
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
                        .Respond("application/json", "{\n    \"id\": 1,\n    \"channel_id\": 0,\n    \"author\": {\n        \"id\": 3,\n        \"username\": \"b\",\n        \"discriminator\": \"0000\",\n        \"avatar\": null\n    },\n    \"content\": \"brr\",\n    \"timestamp\": \"2020-08-28T18:17:25.377506\\u002B00:00\",\n    \"edited_timestamp\": null,\n    \"tts\": false,\n    \"mention_everyone\": false,\n    \"mentions\": [],\n    \"mention_roles\": [],\n    \"mention_channels\": [],\n    \"attachments\": [],\n    \"embeds\": [],\n    \"pinned\": false,\n    \"type\": 0\n}")
                );

                var result = await api.CreateMessageAsync
                (
                    channelId,
                    nonce: nonce,
                    isTTS: tts,
                    file: file
                );

                ResultAssert.Successful(result);
            }
        }
    }
}
