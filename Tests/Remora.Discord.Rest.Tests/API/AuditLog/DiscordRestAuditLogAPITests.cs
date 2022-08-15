//
//  DiscordRestAuditLogAPITests.cs
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
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

#pragma warning disable CS1591, SA1600

namespace Remora.Discord.Rest.Tests.API.AuditLog;

/// <summary>
/// Tests the <see cref="DiscordRestAuditLogAPI"/> class.
/// </summary>
public class DiscordRestAuditLogAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestAuditLogAPI.GetAuditLogAsync"/> method.
    /// </summary>
    public class GetAuditLogAsync : RestAPITestBase<IDiscordRestAuditLogAPI>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var guildID = DiscordSnowflake.New(0);
            var userID = DiscordSnowflake.New(1);
            var actionType = AuditLogEvent.BotAdd;
            var before = DiscordSnowflake.New(2);
            byte limit = 45;

            var api = CreateAPI
            (
                b =>
                    b.Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/*/audit-logs")
                        .WithAuthentication()
                        .WithQueryString
                        (
                            new[]
                            {
                                new KeyValuePair<string, string>("user_id", userID.ToString()),
                                new KeyValuePair<string, string>("action_type", ((int)actionType).ToString()),
                                new KeyValuePair<string, string>("before", before.ToString()),
                                new KeyValuePair<string, string>("limit", limit.ToString())
                            }
                        )
                        .Respond("application/json", SampleRepository.Samples[typeof(IAuditLog)])
            );

            var result = await api.GetAuditLogAsync
            (
                guildID,
                userID,
                actionType,
                before,
                limit
            );

            ResultAssert.Successful(result);
        }

        [Fact]
        public async Task ReturnsErrorIfLimitIsOutsideValidRange()
        {
            var services = CreateConfiguredAPIServices(_ => { });
            var api = services.GetRequiredService<IDiscordRestAuditLogAPI>();

            var guildID = DiscordSnowflake.New(0);
            var userID = DiscordSnowflake.New(1);
            var actionType = AuditLogEvent.BotAdd;
            var before = DiscordSnowflake.New(2);

            var result = await api.GetAuditLogAsync
            (
                guildID,
                userID,
                actionType,
                before,
                0
            );

            ResultAssert.Unsuccessful(result);

            result = await api.GetAuditLogAsync
            (
                guildID,
                userID,
                actionType,
                before,
                101
            );

            ResultAssert.Unsuccessful(result);
        }
    }
}
