//
//  DiscordRestTemplateAPITests.cs
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
/// Tests the <see cref="DiscordRestTemplateAPI"/> class.
/// </summary>
public class DiscordRestTemplateAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestTemplateAPI.GetTemplateAsync"/> method.
    /// </summary>
    public class GetTemplateAsync : RestAPITestBase<IDiscordRestTemplateAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var templateCode = "aaa";
            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/templates/{templateCode}")
                    .Respond("application/json", SampleRepository.Samples[typeof(ITemplate)])
            );

            var result = await api.GetTemplateAsync(templateCode);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestTemplateAPI.CreateGuildFromTemplateAsync"/> method.
    /// </summary>
    public class CreateGuildFromTemplateAsync : RestAPITestBase<IDiscordRestTemplateAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var templateCode = "aaa";
            var name = "name";

            // Create a dummy PNG image
            await using var icon = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(icon);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            icon.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/templates/{templateCode}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("icon", p => p.IsString())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuild)])
            );

            var result = await api.CreateGuildFromTemplateAsync(templateCode, name, icon);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestTemplateAPI.GetGuildTemplatesAsync"/> method.
    /// </summary>
    public class GetGuildTemplatesAsync : RestAPITestBase<IDiscordRestTemplateAPI>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/{guildId}/templates")
                    .Respond("application/json", "[ ]")
            );

            var result = await api.GetGuildTemplatesAsync(guildId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestTemplateAPI.CreateGuildTemplateAsync"/> method.
    /// </summary>
    public class CreateGuildTemplateAsync : RestAPITestBase<IDiscordRestTemplateAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildId = DiscordSnowflake.New(0);
            var name = "name";
            var description = "description";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}guilds/{guildId}/templates")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(ITemplate)])
            );

            var result = await api.CreateGuildTemplateAsync(guildId, name, description);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestTemplateAPI.SyncGuildTemplateAsync"/> method.
    /// </summary>
    public class SyncGuildTemplateAsync : RestAPITestBase<IDiscordRestTemplateAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var templateCode = "aaa";
            var guildId = DiscordSnowflake.New(0);
            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Put, $"{Constants.BaseURL}guilds/{guildId}/templates/{templateCode}")
                    .Respond("application/json", SampleRepository.Samples[typeof(ITemplate)])
            );

            var result = await api.SyncGuildTemplateAsync(guildId, templateCode);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestTemplateAPI.ModifyGuildTemplateAsync"/> method.
    /// </summary>
    public class ModifyGuildTemplateAsync : RestAPITestBase<IDiscordRestTemplateAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var templateCode = "aaa";
            var guildId = DiscordSnowflake.New(0);
            var name = "name";
            var description = "description";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}guilds/{guildId}/templates/{templateCode}")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("description", p => p.Is(description))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(ITemplate)])
            );

            var result = await api.ModifyGuildTemplateAsync(guildId, templateCode, name, description);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestTemplateAPI.DeleteGuildTemplateAsync"/> method.
    /// </summary>
    public class DeleteGuildTemplateAsync : RestAPITestBase<IDiscordRestTemplateAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var templateCode = "aaa";
            var guildId = DiscordSnowflake.New(0);
            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}guilds/{guildId}/templates/{templateCode}")
                    .Respond("application/json", SampleRepository.Samples[typeof(ITemplate)])
            );

            var result = await api.DeleteGuildTemplateAsync(guildId, templateCode);
            ResultAssert.Successful(result);
        }
    }
}
