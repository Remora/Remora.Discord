//
//  DiscordRestEmojiAPITests.cs
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Emoji;

/// <summary>
/// Tests the <see cref="DiscordRestEmojiAPI"/> class.
/// </summary>
public class DiscordRestEmojiAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestEmojiAPI.ListGuildEmojisAsync"/> method.
    /// </summary>
    public class ListGuildEmojisAsync : RestAPITestBase<IDiscordRestEmojiAPI>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/emojis")
                    .WithNoContent()
                    .Respond("application/json", "[ ]")
            );

            var result = await api.ListGuildEmojisAsync(guildId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestEmojiAPI.ListGuildEmojisAsync"/> method.
    /// </summary>
    public class GetGuildEmojiAsync : RestAPITestBase<IDiscordRestEmojiAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var emojiId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/emojis/{emojiId}")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IEmoji)])
            );

            var result = await api.GetGuildEmojiAsync(guildId, emojiId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestEmojiAPI.ListGuildEmojisAsync"/> method.
    /// </summary>
    public class CreateGuildEmojiAsync : RestAPITestBase<IDiscordRestEmojiAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "ff";
            var reason = "test";

            // Create a dummy PNG image
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            image.Position = 0;

            var roles = Array.Empty<Snowflake>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/emojis")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("image", p => p.IsString())
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IEmoji)])
            );

            var result = await api.CreateGuildEmojiAsync(guildId, name, image, roles, reason);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorForUnknownImageFormat()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "ff";

            // Create a dummy PNG image
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(0x00000000);
            image.Position = 0;

            var roles = Array.Empty<Snowflake>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/emojis")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("image", p => p.IsString())
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IEmoji)])
            );

            var result = await api.CreateGuildEmojiAsync(guildId, name, image, roles);
            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorForTooLargeImage()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "ff";

            // Create a dummy PNG image
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(new byte[256001]);
            image.Position = 0;

            var roles = Array.Empty<Snowflake>();

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/emojis")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("image", p => p.IsString())
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IEmoji)])
            );

            var result = await api.CreateGuildEmojiAsync(guildId, name, image, roles);
            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestEmojiAPI.ListGuildEmojisAsync"/> method.
    /// </summary>
    public class ModifyGuildEmojiAsync : RestAPITestBase<IDiscordRestEmojiAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var emojiId = DiscordSnowflake.New(1);
            var name = "ff";
            var roles = new List<Snowflake>();
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/emojis/{emojiId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IEmoji)])
            );

            var result = await api.ModifyGuildEmojiAsync(guildId, emojiId, name, roles, reason);
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
            var emojiId = DiscordSnowflake.New(1);
            var name = "ff";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/emojis/{emojiId}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("roles", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IEmoji)])
            );

            var result = await api.ModifyGuildEmojiAsync(guildId, emojiId, name, null);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestEmojiAPI.ListGuildEmojisAsync"/> method.
    /// </summary>
    public class DeleteGuildEmojiAsync : RestAPITestBase<IDiscordRestEmojiAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var emojiId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}/emojis/{emojiId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildEmojiAsync(guildId, emojiId, reason);
            ResultAssert.Successful(result);
        }
    }
}
