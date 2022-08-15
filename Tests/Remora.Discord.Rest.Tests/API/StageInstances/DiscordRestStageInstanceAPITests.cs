//
//  DiscordRestStageInstanceAPITests.cs
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
/// Tests the <see cref="DiscordRestStageInstanceAPI"/> class.
/// </summary>
public class DiscordRestStageInstanceAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestStageInstanceAPI.CreateStageInstanceAsync"/> method.
    /// </summary>
    public class CreateStageInstanceAsync : RestAPITestBase<IDiscordRestStageInstanceAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(1);
            var topic = "aa";
            var privacyLevel = StagePrivacyLevel.GuildOnly;
            var sendNotification = true;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}stage-instances")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("channel_id", p => p.Is(channelID.ToString()))
                                .WithProperty("topic", p => p.Is(topic))
                                .WithProperty("privacy_level", p => p.Is((int)privacyLevel))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IStageInstance)])
            );

            var result = await api.CreateStageInstanceAsync(channelID, topic, privacyLevel, sendNotification, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStageInstanceAPI.GetStageInstanceAsync"/> method.
    /// </summary>
    public class GetStageInstanceAsync : RestAPITestBase<IDiscordRestStageInstanceAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}stage-instances/{channelID}")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IStageInstance)])
            );

            var result = await api.GetStageInstanceAsync(channelID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStageInstanceAPI.ModifyStageInstanceAsync"/> method.
    /// </summary>
    public class UpdateStageInstanceAsync : RestAPITestBase<IDiscordRestStageInstanceAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(1);
            var topic = "aa";
            var privacyLevel = StagePrivacyLevel.GuildOnly;
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Patch, $"{Constants.BaseURL}stage-instances/{channelID}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithJson
                    (
                        json => json.IsObject
                        (
                            o => o
                                .WithProperty("topic", p => p.Is(topic))
                                .WithProperty("privacy_level", p => p.Is((int)privacyLevel))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IStageInstance)])
            );

            var result = await api.ModifyStageInstanceAsync(channelID, topic, privacyLevel, reason);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestStageInstanceAPI.DeleteStageInstance"/> method.
    /// </summary>
    public class DeleteStageInstance : RestAPITestBase<IDiscordRestStageInstanceAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(1);
            var reason = "test";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}stage-instances/{channelID}")
                    .WithHeaders(Constants.AuditLogHeaderName, reason)
                    .WithNoContent()
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteStageInstance(channelID, reason);
            ResultAssert.Successful(result);
        }
    }
}
