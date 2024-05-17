//
//  DiscordRestPollsAPITests.cs
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

namespace Remora.Discord.Rest.Tests.API.Polls;

/// <summary>
/// Tests the <see cref="DiscordRestPollsAPITests"/> class.
/// </summary>
public class DiscordRestPollsAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestPollAPI.GetAnswerVotersAsync"/> method.
    /// </summary>
    public class GetAnswerVotersAsync : RestAPITestBase<IDiscordRestPollAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAnswerVotersAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public GetAnswerVotersAsync(RestAPITestFixture fixture)
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
            var channelID = DiscordSnowflake.New(1);
            var messageID = DiscordSnowflake.New(2);
            var answerID = 3;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelID}/polls/{messageID}/answers/{answerID}")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IPollAnswerVoters)])
            );

            var result = await api.GetAnswerVotersAsync(channelID, messageID, answerID);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsAfterRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(1);
            var messageID = DiscordSnowflake.New(2);
            var answerID = 3;
            var after = DiscordSnowflake.New(3);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelID}/polls/{messageID}/answers/{answerID}")
                    .WithExactQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("after", after.ToString())
                        }
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IPollAnswerVoters)])
            );

            var result = await api.GetAnswerVotersAsync(channelID, messageID, answerID, after: after);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsLimitRequestCorrectly()
        {
            var channelID = DiscordSnowflake.New(1);
            var messageID = DiscordSnowflake.New(2);
            var answerID = 3;
            var limit = 10;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}channels/{channelID}/polls/{messageID}/answers/{answerID}")
                    .WithExactQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("limit", limit.ToString())
                        }
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IPollAnswerVoters)])
            );

            var result = await api.GetAnswerVotersAsync(channelID, messageID, answerID, limit: limit);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether the API endpoint is correctly limited.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ReturnsErrorIfLimitIsOutsideValidRange()
        {
            var api = CreateAPI(_ => { });

            var channelID = DiscordSnowflake.New(1);
            var messageID = DiscordSnowflake.New(2);
            var answerID = 3;

            var result = await api.GetAnswerVotersAsync(channelID, messageID, answerID, default, 0);

            ResultAssert.Unsuccessful(result);

            result = await api.GetAnswerVotersAsync(channelID, messageID, answerID, default, 101);

            ResultAssert.Unsuccessful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestPollAPI.EndPollAsync"/> method.
    /// </summary>
    public class EndPollAsync : RestAPITestBase<IDiscordRestPollAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndPollAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public EndPollAsync(RestAPITestFixture fixture)
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
            var channelID = DiscordSnowflake.New(1);
            var messageID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}channels/{channelID}/polls/{messageID}/expire")
                    .WithNoContent()
                    .Respond("Authorization/json", SampleRepository.Samples[typeof(IMessage)])
            );

            var result = await api.EndPollAsync(channelID, messageID);
            ResultAssert.Successful(result);
        }
    }
}
