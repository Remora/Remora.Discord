//
//  DiscordRestWebhookAPITests.cs
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
using Microsoft.Extensions.DependencyInjection;
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

namespace Remora.Discord.Rest.Tests.API.Webhooks
{
    /// <summary>
    /// Tests the <see cref="DiscordRestWebhookAPI"/> class.
    /// </summary>
    public class DiscordRestWebhookAPITests
    {
        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.CreateWebhookAsync"/> method.
        /// </summary>
        public class CreateWebhookAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var channelId = new Snowflake(0);
                var name = "aaa";

                // Create a dummy PNG image
                await using var avatar = new MemoryStream();
                await using var binaryWriter = new BinaryWriter(avatar);
                binaryWriter.Write(9894494448401390090);
                avatar.Position = 0;

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/webhooks")
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("avatar", p => p.IsString())
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
                );

                var result = await api.CreateWebhookAsync
                (
                    channelId,
                    name,
                    avatar
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.GetChannelWebhooksAsync"/> method.
        /// </summary>
        public class GetChannelWebhooksAsync : RestAPITestBase<IDiscordRestWebhookAPI>
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
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelId}/webhooks")
                        .Respond("application/json", "[]")
                );

                var result = await api.GetChannelWebhooksAsync
                (
                    channelId
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.GetGuildWebhooksAsync"/> method.
        /// </summary>
        public class GetGuildWebhooksAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var guildId = new Snowflake(0);
                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/webhooks")
                        .Respond("application/json", "[]")
                );

                var result = await api.GetGuildWebhooksAsync
                (
                    guildId
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.GetWebhookAsync"/> method.
        /// </summary>
        public class GetWebhookAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var webhookID = new Snowflake(0);
                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}webhooks/{webhookID}")
                        .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
                );

                var result = await api.GetWebhookAsync
                (
                    webhookID
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.GetWebhookWithTokenAsync"/> method.
        /// </summary>
        public class GetWebhookWithTokenAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var webhookID = new Snowflake(0);
                var token = "aa";

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}webhooks/{webhookID}/{token}")
                        .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
                );

                var result = await api.GetWebhookWithTokenAsync
                (
                    webhookID,
                    token
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.ModifyWebhookAsync"/> method.
        /// </summary>
        public class ModifyWebhookAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var webhookId = new Snowflake(0);
                var name = "aaa";

                // Create a dummy PNG image
                await using var avatar = new MemoryStream();
                await using var binaryWriter = new BinaryWriter(avatar);
                binaryWriter.Write(9894494448401390090);
                avatar.Position = 0;

                var channelId = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Patch, $"{Constants.BaseURL}webhooks/{webhookId}")
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("avatar", p => p.IsString())
                                    .WithProperty("channel_id", p => p.Is(channelId.ToString()))
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
                );

                var result = await api.ModifyWebhookAsync
                (
                    webhookId,
                    name,
                    avatar,
                    channelId
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.ModifyWebhookWithTokenAsync"/> method.
        /// </summary>
        public class ModifyWebhookWithTokenAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var webhookId = new Snowflake(0);
                var token = "aasdasdaa";
                var name = "aaa";

                // Create a dummy PNG image
                await using var avatar = new MemoryStream();
                await using var binaryWriter = new BinaryWriter(avatar);
                binaryWriter.Write(9894494448401390090);
                avatar.Position = 0;

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Patch, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o
                                    .WithProperty("name", p => p.Is(name))
                                    .WithProperty("avatar", p => p.IsString())
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
                );

                var result = await api.ModifyWebhookWithTokenAsync
                (
                    webhookId,
                    token,
                    name,
                    avatar
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.DeleteWebhookAsync"/> method.
        /// </summary>
        public class DeleteWebhookAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var webhookID = new Snowflake(0);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Delete, $"{Constants.BaseURL}webhooks/{webhookID}")
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.DeleteWebhookAsync
                (
                    webhookID
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.DeleteWebhookWithTokenAsync"/> method.
        /// </summary>
        public class DeleteWebhookWithTokenAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var webhookID = new Snowflake(0);
                var token = "aa";

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Delete, $"{Constants.BaseURL}webhooks/{webhookID}/{token}")
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.DeleteWebhookWithTokenAsync
                (
                    webhookID,
                    token
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPI.ExecuteWebhookAsync"/> method.
        /// </summary>
        public class ExecuteWebhookAsync : RestAPITestBase<IDiscordRestWebhookAPI>
        {
             /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsNormalRequestCorrectly()
            {
                var webhookId = new Snowflake(0);
                var token = "aa";

                var shouldWait = true;
                var content = "brr";
                var username = "aaaag";
                var avatarUrl = "http://aaaa";
                var tts = false;
                var allowedMentions = new AllowedMentions(default, default, default);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                        .WithQueryString("wait", shouldWait.ToString())
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o
                                    .WithProperty("content", p => p.Is(content))
                                    .WithProperty("username", p => p.Is(username))
                                    .WithProperty("avatar_url", p => p.Is(avatarUrl))
                                    .WithProperty("tts", p => p.Is(tts))
                                    .WithProperty("allowed_mentions", p => p.IsObject())
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.ExecuteWebhookAsync
                (
                    webhookId,
                    token,
                    shouldWait,
                    content,
                    username,
                    avatarUrl,
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
                var webhookId = new Snowflake(0);
                var token = "aa";

                var shouldWait = true;
                var embeds = new List<Embed>();
                var username = "aaaag";
                var avatarUrl = "http://aaaa";
                var tts = false;
                var allowedMentions = new AllowedMentions(default, default, default);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                        .WithQueryString("wait", shouldWait.ToString())
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o
                                    .WithProperty("embeds", p => p.IsArray())
                                    .WithProperty("username", p => p.Is(username))
                                    .WithProperty("avatar_url", p => p.Is(avatarUrl))
                                    .WithProperty("tts", p => p.Is(tts))
                                    .WithProperty("allowed_mentions", p => p.IsObject())
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.ExecuteWebhookAsync
                (
                    webhookId,
                    token,
                    shouldWait,
                    username: username,
                    avatarUrl: avatarUrl,
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
            public async Task PerformsFileUploadRequestCorrectly()
            {
                var webhookId = new Snowflake(0);
                var token = "aa";

                var shouldWait = true;
                var tts = false;

                await using var file = new MemoryStream();

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                        .WithQueryString("wait", shouldWait.ToString())
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
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.ExecuteWebhookAsync
                (
                    webhookId,
                    token,
                    shouldWait,
                    isTTS: tts,
                    file: file
                );

                ResultAssert.Successful(result);
            }
        }
    }
}
