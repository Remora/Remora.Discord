//
//  DiscordRestInteractionAPITests.cs
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
using System.Diagnostics.CodeAnalysis;
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
using Remora.Discord.Rest.Tests.API.Webhooks;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Interactions;

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
        /// Initializes a new instance of the <see cref="CreateInteractionResponseAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateInteractionResponseAsync(RestAPITestFixture fixture)
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
            var interactionID = DiscordSnowflake.New(0);
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

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsFileUploadRequestCorrectly()
        {
            var interactionID = DiscordSnowflake.New(0);
            var token = "aa";
            var interactionResponse = new InteractionResponse
            (
                InteractionCallbackType.ChannelMessageWithSource,
                new(new InteractionMessageCallbackData())
            );

            await using var file = new MemoryStream();
            var fileName = "file.bin";
            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}interactions/{interactionID}/{token}/callback")
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o.WithProperty
                            (
                                "data",
                                dp => dp.IsObject(dd => dd.WithProperty
                                (
                                    "attachments",
                                    p => p.IsArray
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
                                    )
                                ))
                            )
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateInteractionResponseAsync
            (
                interactionID,
                token,
                interactionResponse,
                new OneOf<FileData, IPartialAttachment>[] { new FileData(fileName, file, description) }
            );

            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsMultiFileUploadRequestCorrectly()
        {
            var interactionID = DiscordSnowflake.New(0);
            var token = "aa";
            var interactionResponse = new InteractionResponse
            (
                InteractionCallbackType.ChannelMessageWithSource,
                new(new InteractionMessageCallbackData())
            );

            await using var file1 = new MemoryStream();
            await using var file2 = new MemoryStream();
            var fileName1 = "file1.bin";
            var fileName2 = "file2.bin";

            var description1 = "wooga";
            var description2 = "booga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}interactions/{interactionID}/{token}/callback")
                    .WithMultipartFormData("\"files[0]\"", fileName1, file1)
                    .WithMultipartFormData("\"files[1]\"", fileName2, file2)
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o.WithProperty
                            (
                                "data",
                                dp => dp.IsObject(dd => dd.WithProperty
                                (
                                    "attachments",
                                    p => p.IsArray
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
                                    )
                                ))
                            )
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateInteractionResponseAsync
            (
                interactionID,
                token,
                interactionResponse,
                new OneOf<FileData, IPartialAttachment>[]
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsRetainingFileUploadRequestCorrectly()
        {
            var interactionID = DiscordSnowflake.New(0);
            var token = "aa";
            var interactionResponse = new InteractionResponse
            (
                InteractionCallbackType.ChannelMessageWithSource,
                new(new InteractionMessageCallbackData())
            );

            await using var file = new MemoryStream();
            var fileName = "file.bin";

            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}interactions/{interactionID}/{token}/callback")
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
                    .WithMultipartJsonPayload
                    (
                        j => j.IsObject
                        (
                            o => o.WithProperty
                            (
                                "data",
                                dp => dp.IsObject(dd => dd.WithProperty
                                (
                                    "attachments",
                                    p => p.IsArray
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
                                    )
                                ))
                            )
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.CreateInteractionResponseAsync
            (
                interactionID,
                token,
                interactionResponse,
                new OneOf<FileData, IPartialAttachment>[]
                {
                    new FileData(fileName, file, description),
                    new PartialAttachment(DiscordSnowflake.New(999))
                }
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
        /// Initializes a new instance of the <see cref="EditOriginalInteractionResponseAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public EditOriginalInteractionResponseAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(0);
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

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";

            await using var file = new MemoryStream();
            var fileName = "file.bin";
            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/@original"
                    )
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
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

            var result = await api.EditOriginalInteractionResponseAsync
            (
                applicationID,
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsMultiFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
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
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/@original"
                    )
                    .WithMultipartFormData("\"files[0]\"", fileName1, file1)
                    .WithMultipartFormData("\"files[1]\"", fileName2, file2)
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

            var result = await api.EditOriginalInteractionResponseAsync
            (
                applicationID,
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsRetainingFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";

            await using var file = new MemoryStream();
            var fileName = "file.bin";

            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/@original"
                    )
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
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

            var result = await api.EditOriginalInteractionResponseAsync
            (
                applicationID,
                token,
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
    /// Tests the <see cref="DiscordRestInteractionAPI.DeleteOriginalInteractionResponseAsync"/> method.
    /// </summary>
    public class DeleteOriginalInteractionResponseAsync : RestAPITestBase<IDiscordRestInteractionAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteOriginalInteractionResponseAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteOriginalInteractionResponseAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(0);
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
        /// Initializes a new instance of the <see cref="CreateFollowupMessageAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateFollowupMessageAsync(RestAPITestFixture fixture)
            : base(fixture)
        {
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNormalRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";

            var content = "brr";
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
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";

            var embeds = new List<Embed>();
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";

            await using var file = new MemoryStream();
            var fileName = "file.bin";
            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{applicationID}/{token}")
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
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

            var result = await api.CreateFollowupMessageAsync
            (
                applicationID,
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsMultiFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
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
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{applicationID}/{token}")
                    .WithMultipartFormData("\"files[0]\"", fileName1, file1)
                    .WithMultipartFormData("\"files[1]\"", fileName2, file2)
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

            var result = await api.CreateFollowupMessageAsync
            (
                applicationID,
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsRetainingFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";

            await using var file = new MemoryStream();
            var fileName = "file.bin";

            var description = "wooga";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}webhooks/{applicationID}/{token}")
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
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

            var result = await api.CreateFollowupMessageAsync
            (
                applicationID,
                token,
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
    /// Tests the <see cref="DiscordRestInteractionAPI.GetFollowupMessageAsync"/> method.
    /// </summary>
    public class GetFollowupMessageAsync : RestAPITestBase<IDiscordRestInteractionAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetFollowupMessageAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetFollowupMessageAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";
            var messageID = DiscordSnowflake.New(1);

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
        /// Initializes a new instance of the <see cref="EditFollowupMessageAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public EditFollowupMessageAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(0);
            var token = "aa";
            var messageID = DiscordSnowflake.New(1);

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

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
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
                        $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/{messageID}"
                    )
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
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

            var result = await api.EditFollowupMessageAsync
            (
                applicationID,
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsMultiFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
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
                        $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/{messageID}"
                    )
                    .WithMultipartFormData("\"files[0]\"", fileName1, file1)
                    .WithMultipartFormData("\"files[1]\"", fileName2, file2)
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

            var result = await api.EditFollowupMessageAsync
            (
                applicationID,
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
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Inconsequential")]
        public async Task PerformsRetainingFileUploadRequestCorrectly()
        {
            var applicationID = DiscordSnowflake.New(0);
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
                        $"{Constants.BaseURL}webhooks/{applicationID}/{token}/messages/{messageID}"
                    )
                    .WithMultipartFormData("\"files[0]\"", fileName, file)
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

            var result = await api.EditFollowupMessageAsync
            (
                applicationID,
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
    /// Tests the <see cref="DiscordRestInteractionAPI.DeleteFollowupMessageAsync"/> method.
    /// </summary>
    public class DeleteFollowupMessageAsync : RestAPITestBase<IDiscordRestInteractionAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFollowupMessageAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteFollowupMessageAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(0);
            var token = "aaa";
            var messageID = DiscordSnowflake.New(1);

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
        /// Initializes a new instance of the <see cref="GetOriginalInteractionResponseAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetOriginalInteractionResponseAsync(RestAPITestFixture fixture)
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
            var interactionID = DiscordSnowflake.New(0);
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
