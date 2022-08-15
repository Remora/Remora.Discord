//
//  DiscordRestUserAPITests.cs
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
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Users;

/// <summary>
/// Tests the <see cref="DiscordRestUserAPI"/> class.
/// </summary>
public class DiscordRestUserAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.GetUserAsync"/> method.
    /// </summary>
    public class GetCurrentUserAsync : RestAPITestBase<IDiscordRestUserAPI>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/@me")
                    .Respond("application/json", SampleRepository.Samples[typeof(IUser)])
            );

            var result = await api.GetCurrentUserAsync();
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.GetUserAsync"/> method.
    /// </summary>
    public class GetUserAsync : RestAPITestBase<IDiscordRestUserAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var userId = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/{userId}")
                    .Respond("application/json", SampleRepository.Samples[typeof(IUser)])
            );

            var result = await api.GetUserAsync(userId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.ModifyCurrentUserAsync"/> method.
    /// </summary>
    public class ModifyCurrentUserAsync : RestAPITestBase<IDiscordRestUserAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var username = "aa";

            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            avatar.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}users/@me")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("username", p => p.Is(username))
                                .WithProperty("avatar", p => p.IsString())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IUser)])
            );

            var result = await api.ModifyCurrentUserAsync(username, avatar);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsNullableRequestCorrectly()
        {
            var username = "aa";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}users/@me")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("username", p => p.Is(username))
                                .WithProperty("avatar", p => p.IsNull())
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IUser)])
            );

            var result = await api.ModifyCurrentUserAsync(username, null);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfAvatarIsUnknownFormat()
        {
            var username = "aa";

            // Create a dummy PNG image
            await using var avatar = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(avatar);
            binaryWriter.Write(0x000000);
            avatar.Position = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}users/@me")
                    .Respond("application/json", SampleRepository.Samples[typeof(IUser)])
            );

            var result = await api.ModifyCurrentUserAsync(username, avatar);
            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.GetCurrentUserGuildsAsync"/> method.
    /// </summary>
    public class GetCurrentUserGuildsAsync : RestAPITestBase<IDiscordRestUserAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var before = DiscordSnowflake.New(0);
            var after = DiscordSnowflake.New(1);
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/@me/guilds")
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("before", before.ToString()),
                            new KeyValuePair<string, string>("after", after.ToString()),
                            new KeyValuePair<string, string>("limit", limit.ToString())
                        }
                    )
                    .Respond("application/json", "[]")
            );

            var result = await api.GetCurrentUserGuildsAsync(before, after, limit);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfLimitIsTooLow()
        {
            var limit = 0;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/@me/guilds")
                    .Respond("application/json", "[]")
            );

            var result = await api.GetCurrentUserGuildsAsync(limit: limit);
            ResultAssert.Unsuccessful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfLimitIsTooHigh()
        {
            var limit = 201;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/@me/guilds")
                    .Respond("application/json", "[]")
            );

            var result = await api.GetCurrentUserGuildsAsync(limit: limit);
            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.GetCurrentUserGuildMemberAsync"/> method.
    /// </summary>
    public class GetCurrentUserGuildMemberAsync : RestAPITestBase<IDiscordRestUserAPI>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/@me/guilds/{guildId}/member")
                    .Respond("application/json", SampleRepository.Samples[typeof(IGuildMember)])
            );

            var result = await api.GetCurrentUserGuildMemberAsync(guildId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.LeaveGuildAsync"/> method.
    /// </summary>
    public class LeaveGuildAsync : RestAPITestBase<IDiscordRestUserAPI>
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
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}users/@me/guilds/{guildId}")
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.LeaveGuildAsync(guildId);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.GetUserDMsAsync"/> method.
    /// </summary>
    public class GetUserDMsAsync : RestAPITestBase<IDiscordRestUserAPI>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/@me/channels")
                    .Respond("application/json", "[]")
            );

            var result = await api.GetUserDMsAsync();
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.CreateDMAsync"/> method.
    /// </summary>
    public class CreateDMAsync : RestAPITestBase<IDiscordRestUserAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var recipientID = DiscordSnowflake.New(0);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}users/@me/channels")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("recipient_id", p => p.Is(recipientID.ToString()))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IChannel)])
            );

            var result = await api.CreateDMAsync(recipientID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestUserAPI.GetUserConnectionsAsync"/> method.
    /// </summary>
    public class GetUserConnectionsAsync : RestAPITestBase<IDiscordRestUserAPI>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}users/@me/connections")
                    .Respond("application/json", "[]")
            );

            var result = await api.GetUserConnectionsAsync();
            ResultAssert.Successful(result);
        }
    }
}
