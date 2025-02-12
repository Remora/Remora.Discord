//
//  DiscordRestSoundboardAPITests.cs
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects.Soundboard;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API;

/// <summary>
/// Tests the <see cref="DiscordRestSoundboardAPI"/> class.
/// </summary>
public class DiscordRestSoundboardAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestSoundboardAPI.SendSoundboardSoundAsync"/> method.
    /// </summary>
    public class SendSoundboardSoundAsync : RestAPITestBase<IDiscordRestSoundboardAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendSoundboardSoundAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public SendSoundboardSoundAsync(RestAPITestFixture fixture)
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
            var channelID = DiscordSnowflake.New(0);
            var soundID = DiscordSnowflake.New(1);
            var sourceGuildID = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}channels/{channelID}/send-soundboard-sound"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("sound_id", p => p.Is(soundID))
                                .WithProperty("source_guild_id", p => p.Is(sourceGuildID))
                        )
                    ).Respond(HttpStatusCode.OK)
            );

            var result = await api.SendSoundboardSoundAsync(channelID, soundID, sourceGuildID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestSoundboardAPI.ListDefaultSoundboardSoundsAsync"/> method.
    /// </summary>
    public class ListDefaultSoundboardSoundsAsync : RestAPITestBase<IDiscordRestSoundboardAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListDefaultSoundboardSoundsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ListDefaultSoundboardSoundsAsync(RestAPITestFixture fixture)
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
            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}soundboard-default-sounds"
                    )
                    .WithNoContent()
                    .Respond("application/json", "[" + SampleRepository.Samples[typeof(ISoundboardSound)] + "]")
            );

            var result = await api.ListDefaultSoundboardSoundsAsync();
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestSoundboardAPI.ListGuildSoundboardSoundsAsync"/> method.
    /// </summary>
    public class ListGuildSoundboardSoundsAsync : RestAPITestBase<IDiscordRestSoundboardAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListGuildSoundboardSoundsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ListGuildSoundboardSoundsAsync(RestAPITestFixture fixture)
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
            var guildID = DiscordSnowflake.New(0);
            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}guilds/{guildID}/soundboard-sounds"
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IListGuildSoundboardSoundsResponse)])
            );

            var result = await api.ListGuildSoundboardSoundsAsync(guildID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestSoundboardAPI.GetGuildSoundboardSoundAsync"/> method.
    /// </summary>
    public class GetGuildSoundboardSoundAsync : RestAPITestBase<IDiscordRestSoundboardAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetGuildSoundboardSoundAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetGuildSoundboardSoundAsync(RestAPITestFixture fixture)
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
            var guildID = DiscordSnowflake.New(0);
            var soundID = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Get,
                        $"{Constants.BaseURL}guilds/{guildID}/soundboard-sounds/{soundID}"
                    )
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(ISoundboardSound)])
            );

            var result = await api.GetGuildSoundboardSoundAsync(guildID, soundID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestSoundboardAPI.CreateGuildSoundboardSoundAsync"/> method.
    /// </summary>
    public class CreateGuildSoundboardSoundAsync : RestAPITestBase<IDiscordRestSoundboardAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGuildSoundboardSoundAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateGuildSoundboardSoundAsync(RestAPITestFixture fixture)
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
            var guildID = DiscordSnowflake.New(0);
            var name = "ooga";
            var sound = new byte[] { 1, 2, 3, 4 };
            var encodedSound = Convert.ToBase64String(sound);
            var volume = 0.5;
            var emojiID = DiscordSnowflake.New(1);
            var emojiName = "booga";
            var reason = "aaaa";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Post,
                        $"{Constants.BaseURL}guilds/{guildID}/soundboard-sounds"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("sound", p => p.Is(encodedSound))
                                .WithProperty("volume", p => p.Is(volume))
                                .WithProperty("emoji_id", p => p.Is(emojiID))
                                .WithProperty("emoji_name", p => p.Is(emojiName))
                        )
                    )
                    .WithHeaders("X-Audit-Log-Reason", reason)
                    .Respond("application/json", SampleRepository.Samples[typeof(ISoundboardSound)])
            );

            var result = await api.CreateGuildSoundboardSoundAsync
            (
                guildID,
                name,
                sound,
                volume,
                emojiID,
                emojiName,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestSoundboardAPI.ModifyGuildSoundboardSoundAsync"/> method.
    /// </summary>
    public class ModifyGuildSoundboardSoundAsync : RestAPITestBase<IDiscordRestSoundboardAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyGuildSoundboardSoundAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ModifyGuildSoundboardSoundAsync(RestAPITestFixture fixture)
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
            var guildID = DiscordSnowflake.New(0);
            var soundID = DiscordSnowflake.New(1);
            var name = "ooga";
            var volume = 0.5;
            var emojiID = DiscordSnowflake.New(2);
            var emojiName = "booga";
            var reason = "aaaa";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Patch,
                        $"{Constants.BaseURL}guilds/{guildID}/soundboard-sounds/{soundID}"
                    )
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("name", p => p.Is(name))
                                .WithProperty("volume", p => p.Is(volume))
                                .WithProperty("emoji_id", p => p.Is(emojiID))
                                .WithProperty("emoji_name", p => p.Is(emojiName))
                        )
                    )
                    .WithHeaders("X-Audit-Log-Reason", reason)
                    .Respond("application/json", SampleRepository.Samples[typeof(ISoundboardSound)])
            );

            var result = await api.ModifyGuildSoundboardSoundAsync
            (
                guildID,
                soundID,
                name,
                volume,
                emojiID,
                emojiName,
                reason
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestSoundboardAPI.DeleteGuildSoundboardSoundAsync"/> method.
    /// </summary>
    public class DeleteGuildSoundboardSoundAsync : RestAPITestBase<IDiscordRestSoundboardAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteGuildSoundboardSoundAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteGuildSoundboardSoundAsync(RestAPITestFixture fixture)
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
            var guildID = DiscordSnowflake.New(0);
            var soundID = DiscordSnowflake.New(1);
            var reason = "aaaa";

            var api = CreateAPI
            (
                b => b
                    .Expect
                    (
                        HttpMethod.Delete,
                        $"{Constants.BaseURL}guilds/{guildID}/soundboard-sounds/{soundID}"
                    )
                    .WithNoContent()
                    .WithHeaders("X-Audit-Log-Reason", reason)
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteGuildSoundboardSoundAsync
            (
                guildID,
                soundID,
                reason
            );

            ResultAssert.Successful(result);
        }
    }
}
