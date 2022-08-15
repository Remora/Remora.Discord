//
//  DiscordRestStickerAPITests.cs
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
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API;

/// <summary>
/// Tests the <see cref="DiscordRestStickerAPI"/> class.
/// </summary>
public class DiscordRestStickerAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestStickerAPI.GetStickerAsync"/> method.
    /// </summary>
    public class GetStickerAsync : RestAPITestBase<IDiscordRestStickerAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var stickerId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}stickers/{stickerId}")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(ISticker)])
            );

            var result = await api.GetStickerAsync(stickerId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStickerAPI.ListNitroStickerPacksAsync"/> method.
    /// </summary>
    public class ListNitroStickerPacksAsync : RestAPITestBase<IDiscordRestStickerAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}sticker-packs")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(INitroStickerPacks)])
            );

            var result = await api.ListNitroStickerPacksAsync();
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStickerAPI.ListGuildStickersAsync"/> method.
    /// </summary>
    public class ListGuildStickersAsync : RestAPITestBase<IDiscordRestStickerAPI>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/stickers")
                    .WithNoContent()
                    .Respond("application/json", "[ ]")
            );

            var result = await api.ListGuildStickersAsync(guildId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStickerAPI.GetGuildStickerAsync"/> method.
    /// </summary>
    public class GetGuildStickerAsync : RestAPITestBase<IDiscordRestStickerAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var stickerId = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/stickers/{stickerId}")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(ISticker)])
            );

            var result = await api.GetGuildStickerAsync(guildId, stickerId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStickerAPI.CreateGuildStickerAsync"/> method.
    /// </summary>
    public class CreateGuildStickerAsync : RestAPITestBase<IDiscordRestStickerAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "aa";
            var description = "bb";
            var tags = "cc";

            var fileStream = new MemoryStream();
            var file = new FileData("dd", fileStream);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/stickers")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithMultipartFormData("name", name)
                    .WithMultipartFormData("description", description)
                    .WithMultipartFormData("tags", tags)
                    .WithMultipartFormData("file", file.Name, fileStream)
                    .Respond("application/json", SampleRepository.Samples[typeof(ISticker)])
            );

            var result = await api.CreateGuildStickerAsync(guildId, name, description, tags, file, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStickerAPI.ModifyGuildStickerAsync"/> method.
    /// </summary>
    public class ModifyGuildStickerAsync : RestAPITestBase<IDiscordRestStickerAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var stickerId = DiscordSnowflake.New(1);
            var name = "aa";
            var description = "bb";
            var tags = "cc";
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/stickers/{stickerId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                                .WithProperty("tags", p => p.Is(tags))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(ISticker)])
            );

            var result = await api.ModifyGuildStickerAsync(guildId, stickerId, name, description, tags, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStickerAPI.DeleteGuildStickerAsync"/> method.
    /// </summary>
    public class DeleteGuildStickerAsync : RestAPITestBase<IDiscordRestStickerAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var stickerId = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}/stickers/{stickerId}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildStickerAsync(guildId, stickerId, reason);
            ResultAssert.Successful(result);
        }
    }
}
