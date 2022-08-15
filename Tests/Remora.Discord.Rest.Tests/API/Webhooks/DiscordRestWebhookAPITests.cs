//
//  DiscordRestWebhookAPITests.cs
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

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OneOf;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Webhooks;

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
            var channelId = DiscordSnowflake.New(0);
            var name = "aaa";
            var reason = "agag";

            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
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
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.CreateWebhookAsync
            (
                channelId,
                name,
                avatar,
                reason
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
            var channelId = DiscordSnowflake.New(0);
            var name = string.Empty;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/webhooks")
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.CreateWebhookAsync
            (
                channelId,
                name,
                null
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
            var channelId = DiscordSnowflake.New(0);
            var name = new string('a', 81);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/webhooks")
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.CreateWebhookAsync
            (
                channelId,
                name,
                null
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfNameIsClyde()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "clyde";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/webhooks")
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.CreateWebhookAsync
            (
                channelId,
                name,
                null
            );

            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfAvatarIsUnknownFormat()
        {
            var channelId = DiscordSnowflake.New(0);
            var name = "aaa";

            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(0x00000000);
            avatar.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelId}/webhooks")
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.CreateWebhookAsync
            (
                channelId,
                name,
                avatar
            );

            ResultAssert.Unsuccessful(result);
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
            var channelId = DiscordSnowflake.New(0);
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
            var guildId = DiscordSnowflake.New(0);
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
            var webhookID = DiscordSnowflake.New(0);
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
            var webhookID = DiscordSnowflake.New(0);
            var token = "aa";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}webhooks/{webhookID}/{token}")
                    .With(m => m.Headers.Authorization == null)
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
            var webhookId = DiscordSnowflake.New(0);
            var name = "aaa";
            var reason = "gagag";

            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            avatar.Position = 0;

            var channelId = DiscordSnowflake.New(1);

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
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.ModifyWebhookAsync
            (
                webhookId,
                name,
                avatar,
                channelId,
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
            var webhookId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}webhooks/{webhookId}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("avatar", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.ModifyWebhookAsync
            (
                webhookId,
                avatar: null
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfAvatarIsUnknownFormat()
        {
            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(0x000000);
            avatar.Position = 0;

            var webhookId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}webhooks/{webhookId}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("avatar", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.ModifyWebhookAsync
            (
                webhookId,
                avatar: avatar
            );

            ResultAssert.Unsuccessful(result);
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
            var webhookId = DiscordSnowflake.New(0);
            var token = "aasdasdaa";
            var name = "aaa";
            var reason = "agaga";

            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            avatar.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .With(m => m.Headers.Authorization == null)
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.ModifyWebhookWithTokenAsync
            (
                webhookId,
                token,
                name,
                avatar,
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
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("avatar", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.ModifyWebhookWithTokenAsync
            (
                webhookId,
                token,
                avatar: null
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfAvatarIsUnknownFormat()
        {
            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(0x000000);
            avatar.Position = 0;

            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IWebhook)])
            );

            var result = await api.ModifyWebhookWithTokenAsync
            (
                webhookId,
                token,
                avatar: avatar
            );

            ResultAssert.Unsuccessful(result);
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
            var webhookID = DiscordSnowflake.New(0);
            var reason = "aagag";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}webhooks/{webhookID}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteWebhookAsync
            (
                webhookID,
                reason
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
            var webhookID = DiscordSnowflake.New(0);
            var token = "aa";
            var reason = "agaga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}webhooks/{webhookID}/{token}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .With(m => m.Headers.Authorization == null)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteWebhookWithTokenAsync
            (
                webhookID,
                token,
                reason
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
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            var shouldWait = true;
            var content = "brr";
            var username = "aaaag";
            var avatarUrl = "http://aaaa";
            var tts = false;
            var allowedMentions = new AllowedMentions();
            var components = new List<IMessageComponent>();
            var flags = MessageFlags.SuppressEmbeds;

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
                                .WithProperty("components", p => p.IsArray())
                                .WithProperty("flags", p => p.Is((int)flags))
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
        public async Task PerformsInThreadRequestCorrectly()
        {
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            var shouldWait = true;
            var content = "brr";
            var username = "aaaag";
            var avatarUrl = "http://aaaa";
            var tts = false;
            var allowedMentions = new AllowedMentions();
            var threadID = DiscordSnowflake.New(1);
            var components = new List<IMessageComponent>();
            var flags = MessageFlags.SuppressEmbeds;

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
                                .WithProperty("thread_id", p => p.Is(threadID.ToString()))
                                .WithProperty("components", p => p.IsArray())
                                .WithProperty("flags", p => p.Is((int)flags))
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
                allowedMentions: allowedMentions,
                threadID: threadID,
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
        public async Task PerformsForumThreadRequestCorrectly()
        {
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            var shouldWait = true;
            var content = "brr";
            var username = "aaaag";
            var avatarUrl = "http://aaaa";
            var tts = false;
            var allowedMentions = new AllowedMentions();
            var components = new List<IMessageComponent>();
            var flags = MessageFlags.SuppressEmbeds;
            var threadName = "agaga";

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
                                .WithProperty("components", p => p.IsArray())
                                .WithProperty("flags", p => p.Is((int)flags))
                                .WithProperty("thread_name", p => p.Is(threadName))
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
                allowedMentions: allowedMentions,
                components: components,
                flags: flags,
                threadName: threadName
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
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            var shouldWait = true;
            var embeds = new List<Embed>();
            var username = "aaaag";
            var avatarUrl = "http://aaaa";
            var tts = false;
            var allowedMentions = new AllowedMentions();

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
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            await using var file = new MemoryStream();
            var fileName = "file.bin";
            var description = "wooga";

            var api = CreateAPI
            (
                b => b.Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
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

            var result = await api.ExecuteWebhookAsync
            (
                webhookId,
                token,
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
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            await using var file1 = new MemoryStream();
            await using var file2 = new MemoryStream();
            var fileName1 = "file1.bin";
            var fileName2 = "file2.bin";

            var description1 = "wooga";
            var description2 = "booga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
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

            var result = await api.ExecuteWebhookAsync
            (
                webhookId,
                token,
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
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";

            await using var file = new MemoryStream();
            var fileName = "file.bin";

            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
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

            var result = await api.ExecuteWebhookAsync
            (
                webhookId,
                token,
                attachments: new OneOf<FileData, IPartialAttachment>[]
                {
                    new FileData(fileName, file, description),
                    new PartialAttachment(DiscordSnowflake.New(999))
                }
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNullReturnRequestCorrectly()
        {
            var webhookId = DiscordSnowflake.New(0);
            var token = "aa";
            var content = "bb";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{webhookId}/{token}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("content", p => p.Is(content))
                        )
                    )
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.ExecuteWebhookAsync
            (
                webhookId,
                token,
                content: content
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestWebhookAPI.DeleteWebhookWithTokenAsync"/> method.
    /// </summary>
    public class EditWebhookMessageAsync : RestAPITestBase<IDiscordRestWebhookAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNormalRequestCorrectly()
        {
            var webhookID = DiscordSnowflake.New(0);
            var token = "aa";
            var messageID = DiscordSnowflake.New(1);
            var threadID = DiscordSnowflake.New(2);

            var content = "booga";
            var allowedMentions = new AllowedMentions();
            var components = new List<IMessageComponent>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{webhookID}/{token}/messages/{messageID}"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("content", p => p.Is(content))
                                .WithProperty("allowed_mentions", p => p.IsObject())
                                .WithProperty("components", p => p.IsArray())
                        )
                    )
                    .WithQueryString("thread_id", threadID.ToString())
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EditWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
                content,
                allowedMentions: allowedMentions,
                components: components,
                threadID: threadID
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
            var webhookID = DiscordSnowflake.New(0);
            var token = "aa";
            var messageID = DiscordSnowflake.New(1);

            var content = "booga";
            var embeds = new List<IEmbed>();

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{webhookID}/{token}/messages/{messageID}"
                    )
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("content", p => p.Is(content))
                                .WithProperty("embeds", p => p.IsArray(a => a.WithCount(0)))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EditWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
                content,
                embeds
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
            var webhookID = DiscordSnowflake.New(0);
            var token = "aa";
            var messageID = DiscordSnowflake.New(1);

            await using var file = new MemoryStream();
            var fileName = "file.bin";
            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{webhookID}/{token}/messages/{messageID}"
                    )
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

            var result = await api.EditWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
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
            var webhookID = DiscordSnowflake.New(0);
            var token = "aa";
            var messageID = DiscordSnowflake.New(1);

            await using var file1 = new MemoryStream();
            await using var file2 = new MemoryStream();
            var fileName1 = "file1.bin";
            var fileName2 = "file2.bin";

            var description1 = "wooga";
            var description2 = "booga";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{webhookID}/{token}/messages/{messageID}"
                    )
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

            var result = await api.EditWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
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
            var webhookID = DiscordSnowflake.New(0);
            var token = "aa";
            var messageID = DiscordSnowflake.New(1);

            await using var file = new MemoryStream();
            var fileName = "file.bin";

            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{webhookID}/{token}/messages/{messageID}"
                    )
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

            var result = await api.EditWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
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
    /// Tests the <see cref="DiscordRestWebhookAPI.DeleteWebhookMessageAsync"/> method.
    /// </summary>
    public class DeleteWebhookMessageAsync : RestAPITestBase<IDiscordRestWebhookAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var webhookID = DiscordSnowflake.New(0);
            var token = "token";
            var messageID = DiscordSnowflake.New(1);
            var threadID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}webhooks/{webhookID}/{token}/messages/{messageID}")
                    .WithQueryString("thread_id", threadID.ToString())
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
                threadID
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestWebhookAPI.GetWebhookMessageAsync"/> method.
    /// </summary>
    public class GetWebhookMessageAsync : RestAPITestBase<IDiscordRestWebhookAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var webhookID = DiscordSnowflake.New(0);
            var token = "aaa";
            var messageID = DiscordSnowflake.New(1);
            var threadID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}webhooks/{webhookID}/{token}/messages/{messageID}"
                    )
                    .WithNoContent()
                    .WithQueryString("thread_id", threadID.ToString())
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.GetWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
                threadID
            );

            ResultAssert.Successful(result);
        }
    }
}
