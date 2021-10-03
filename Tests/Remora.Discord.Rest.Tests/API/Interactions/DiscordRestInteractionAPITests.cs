//
//  DiscordRestInteractionAPITests.cs
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.API.Webhooks;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Interactions
{
    /// <summary>
    /// Tests the <see cref="DiscordRestInteractionAPI"/> class.
    /// </summary>
    public class DiscordRestInteractionAPITests
    {
        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.CreateInteractionResponseAsync"/> method.
        /// </summary>
        public class CreateInteractionResponseAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var interactionID = new Snowflake(0);
                var token = "aaaa";
                var interactionResponse = new InteractionResponse(InteractionCallbackType.DeferredChannelMessageWithSource);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}interactions/{interactionID}/{token}/callback")
                        .WithJson
                        (
                            j => j.IsObject
                            (
                                o => o.WithProperty("type", p => p.Is(5))
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IInteractionResponse)])
                );

                var result = await api.CreateInteractionResponseAsync
                (
                    interactionID,
                    token,
                    interactionResponse
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.EditOriginalInteractionResponseAsync"/> method.
        /// </summary>
        public class EditOriginalInteractionResponseAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var token = "aa";

                var content = "booga";
                var embeds = new List<IEmbed>();
                var allowedMentions = new AllowedMentions();
                var components = new List<IMessageComponent>();

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Patch,
                            $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/@original"
                        )
                        .WithJson
                        (
                            json => json.IsObject
                            (
                                o => o
                                    .WithProperty("content", p => p.Is(content))
                                    .WithProperty("embeds", p => p.IsArray(a => a.WithCount(0)))
                                    .WithProperty("allowed_mentions", p => p.IsObject())
                                    .WithProperty("components", p => p.IsArray())
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.EditOriginalInteractionResponseAsync
                (
                    applicationID,
                    token,
                    content,
                    embeds,
                    allowedMentions,
                    components
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.DeleteOriginalInteractionResponseAsync"/> method.
        /// </summary>
        public class DeleteOriginalInteractionResponseAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var token = "aaa";

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Delete,
                            $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/@original"
                        )
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.DeleteOriginalInteractionResponseAsync
                (
                    applicationID,
                    token
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.CreateFollowupMessageAsync"/> method.
        /// </summary>
        public class CreateFollowupMessageAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
             /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsNormalRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var token = "aa";

                var content = "brr";
                var username = "aaaag";
                var avatarUrl = "http://aaaa";
                var tts = false;
                var allowedMentions = new AllowedMentions();
                var components = new List<IMessageComponent>();
                var flags = MessageFlags.Ephemeral;

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{applicationID}/{token}")
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
                                    .WithProperty("components", p => p.IsArray())
                                    .WithProperty("flags", p => p.Is((int)flags))
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.CreateFollowupMessageAsync
                (
                    applicationID,
                    token,
                    content,
                    username,
                    avatarUrl,
                    tts,
                    allowedMentions: allowedMentions,
                    components: components,
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
                var applicationID = new Snowflake(0);
                var token = "aa";

                var embeds = new List<Embed>();
                var username = "aaaag";
                var avatarUrl = "http://aaaa";
                var tts = false;
                var allowedMentions = new AllowedMentions();

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{applicationID}/{token}")
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

                var result = await api.CreateFollowupMessageAsync
                (
                    applicationID,
                    token,
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
                var applicationID = new Snowflake(0);
                var token = "aa";

                var tts = false;

                await using var file = new MemoryStream();
                var fileName = "file.bin";

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{applicationID}/{token}")
                        .With
                        (
                            m =>
                            {
                                if (m.Content is not MultipartFormDataContent multipart)
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

                var result = await api.CreateFollowupMessageAsync
                (
                    applicationID,
                    token,
                    isTTS: tts,
                    file: new FileData(fileName, file)
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.GetFollowupMessageAsync"/> method.
        /// </summary>
        public class GetFollowupMessageAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var token = "aa";
                var messageID = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Get,
                            $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/{messageID}"
                        )
                        .WithNoContent()
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.GetFollowupMessageAsync
                (
                    applicationID,
                    token,
                    messageID
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestWebhookAPITests.DeleteWebhookWithTokenAsync"/> method.
        /// </summary>
        public class EditFollowupMessageAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var token = "aa";
                var messageID = new Snowflake(1);

                var content = "booga";
                var embeds = new List<IEmbed>();
                var allowedMentions = new AllowedMentions();
                var components = new List<IMessageComponent>();

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Patch,
                            $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/{messageID}"
                        )
                        .WithJson
                        (
                            json => json.IsObject
                            (
                                o => o
                                    .WithProperty("content", p => p.Is(content))
                                    .WithProperty("embeds", p => p.IsArray(a => a.WithCount(0)))
                                    .WithProperty("allowed_mentions", p => p.IsObject())
                                    .WithProperty("components", p => p.IsArray())
                            )
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.EditFollowupMessageAsync
                (
                    applicationID,
                    token,
                    messageID,
                    content,
                    embeds,
                    allowedMentions,
                    components
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.DeleteFollowupMessageAsync"/> method.
        /// </summary>
        public class DeleteFollowupMessageAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var applicationID = new Snowflake(0);
                var token = "aaa";
                var messageID = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Delete,
                            $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/{messageID}"
                        )
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.DeleteFollowupMessageAsync
                (
                    applicationID,
                    token,
                    messageID
                );

                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestInteractionAPI.GetOriginalInteractionResponseAsync"/> method.
        /// </summary>
        public class GetOriginalInteractionResponseAsync : RestAPITestBase<IDiscordRestInteractionAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var interactionID = new Snowflake(0);
                var token = "aaa";

                var api = CreateAPI
                (
                    b => b
                        .Expect
                        (
                            HttpMethod.Get,
                            $"{Constants.BaseURL}webhooks/{interactionID}/{token}/messages/@original"
                        )
                        .WithNoContent()
                        .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
                );

                var result = await api.GetOriginalInteractionResponseAsync
                (
                    interactionID,
                    token
                );

                ResultAssert.Successful(result);
            }
        }
    }
}
