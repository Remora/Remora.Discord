//
//  DiscordRestEmojiAPITests.cs
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Core;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Emoji
{
    /// <summary>
    /// Tests the <see cref="DiscordRestEmojiAPI"/> class.
    /// </summary>
    public class DiscordRestEmojiAPITests
    {
        /// <summary>
        /// Tests the <see cref="ListGuildEmojis"/> method.
        /// </summary>
        public class ListGuildEmojis : RestAPITestBase<IDiscordRestEmojiAPI>
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
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/emojis")
                        .WithNoContent()
                        .Respond("application/json", "[ ]")
                );

                var result = await api.ListGuildEmojisAsync(guildId);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="ListGuildEmojis"/> method.
        /// </summary>
        public class GetGuildEmoji : RestAPITestBase<IDiscordRestEmojiAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var guildId = new Snowflake(0);
                var emojiId = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/emojis/{emojiId}")
                        .WithNoContent()
                        .Respond("application/json", "{\n    \"id\": \"748969255210450984\",\n    \"name\": \"dss\",\n    \"roles\": [ ],\n    \"user\": {\n        \"id\": \"54351326674432000\",\n        \"username\": \"Azazial\",\n        \"discriminator\": \"4061\",\n        \"avatar\": \"3cd6d2c10a8e927e2e096aebadece22a\",\n        \"bot\": false,\n        \"system\": false,\n        \"mfa_enabled\": false,\n        \"locale\": \"en-US\",\n        \"verified\": true,\n        \"email\": \"example@test.org\",\n        \"flags\": 0,\n        \"premium_type\": 0,\n        \"public_flags\": 0\n    },\n    \"require_colons\": true,\n    \"managed\": false,\n    \"animated\": true,\n    \"available\": true\n}\n")
                );

                var result = await api.GetGuildEmojiAsync(guildId, emojiId);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="ListGuildEmojis"/> method.
        /// </summary>
        public class CreateGuildEmoji : RestAPITestBase<IDiscordRestEmojiAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var guildId = new Snowflake(0);
                var name = "ff";

                // Create a dummy PNG image
                await using var image = new MemoryStream();
                await using var binaryWriter = new BinaryWriter(image);
                binaryWriter.Write(9894494448401390090);
                image.Position = 0;

                var roles = new List<Snowflake>();

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
                        .Respond("application/json", "{\n    \"id\": \"748969255210450984\",\n    \"name\": \"dss\",\n    \"roles\": [ ],\n    \"user\": {\n        \"id\": \"54351326674432000\",\n        \"username\": \"Azazial\",\n        \"discriminator\": \"4061\",\n        \"avatar\": \"3cd6d2c10a8e927e2e096aebadece22a\",\n        \"bot\": false,\n        \"system\": false,\n        \"mfa_enabled\": false,\n        \"locale\": \"en-US\",\n        \"verified\": true,\n        \"email\": \"example@test.org\",\n        \"flags\": 0,\n        \"premium_type\": 0,\n        \"public_flags\": 0\n    },\n    \"require_colons\": true,\n    \"managed\": false,\n    \"animated\": true,\n    \"available\": true\n}\n")
                );

                var result = await api.CreateGuildEmojiAsync(guildId, name, image, roles);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="ListGuildEmojis"/> method.
        /// </summary>
        public class ModifyGuildEmoji : RestAPITestBase<IDiscordRestEmojiAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var guildId = new Snowflake(0);
                var emojiId = new Snowflake(1);
                var name = "ff";
                var roles = new List<Snowflake>();

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
                                    .WithProperty("roles", p => p.IsArray(a => a.WithCount(0)))
                            )
                        )
                        .Respond("application/json", "{\n    \"id\": \"748969255210450984\",\n    \"name\": \"dss\",\n    \"roles\": [ ],\n    \"user\": {\n        \"id\": \"54351326674432000\",\n        \"username\": \"Azazial\",\n        \"discriminator\": \"4061\",\n        \"avatar\": \"3cd6d2c10a8e927e2e096aebadece22a\",\n        \"bot\": false,\n        \"system\": false,\n        \"mfa_enabled\": false,\n        \"locale\": \"en-US\",\n        \"verified\": true,\n        \"email\": \"example@test.org\",\n        \"flags\": 0,\n        \"premium_type\": 0,\n        \"public_flags\": 0\n    },\n    \"require_colons\": true,\n    \"managed\": false,\n    \"animated\": true,\n    \"available\": true\n}\n")
                );

                var result = await api.ModifyGuildEmojiAsync(guildId, emojiId, name, roles);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="ListGuildEmojis"/> method.
        /// </summary>
        public class DeleteGuildEmoji : RestAPITestBase<IDiscordRestEmojiAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var guildId = new Snowflake(0);
                var emojiId = new Snowflake(1);

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}/emojis/{emojiId}")
                        .Respond(HttpStatusCode.NoContent)
                );

                var result = await api.DeleteGuildEmojiAsync(guildId, emojiId);
                ResultAssert.Successful(result);
            }
        }
    }
}
