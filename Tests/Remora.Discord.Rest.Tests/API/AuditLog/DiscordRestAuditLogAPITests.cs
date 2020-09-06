//
//  DiscordRestAuditLogAPITests.cs
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
using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.AuditLog
{
    /// <summary>
    /// Tests the <see cref="DiscordRestAuditLogAPI"/> class.
    /// </summary>
    public class DiscordRestAuditLogAPITests
    {
        /// <summary>
        /// Tests the <see cref="DiscordRestAuditLogAPI.GetAuditLogAsync"/> method.
        /// </summary>
        public class GetAuditLogBotAsync : RestAPITestBase<IDiscordRestAuditLogAPI>
        {
            private const string Response =
                "{\n    \"webhooks\": [],\n    \"users\": [],\n    \"audit_log_entries\": [],\n    \"integrations\": []\n}";

            private readonly Snowflake _guildID = new Snowflake(0);
            private readonly Snowflake _userID = new Snowflake(1);
            private readonly AuditLogEvent _actionType = AuditLogEvent.BotAdd;
            private readonly Snowflake _before = new Snowflake(2);
            private readonly byte _limit = 45;

            /// <inheritdoc />
            protected override Action<MockHttpMessageHandler> BuildMock => b =>
            {
                b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}guilds/*/audit-logs")
                    .WithAuthentication()
                    .WithQueryString
                    (
                        new[]
                        {
                            new KeyValuePair<string, string>("user_id", _userID.ToString()),
                            new KeyValuePair<string, string>("action_type", ((int)_actionType).ToString()),
                            new KeyValuePair<string, string>("before", _before.ToString()),
                            new KeyValuePair<string, string>("limit", _limit.ToString()),
                        }
                    )
                    .Respond("application/json", Response);
            };

            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var result = await this.API.GetAuditLogAsync
                (
                    _guildID,
                    _userID,
                    _actionType,
                    _before,
                    _limit
                );

                ResultAssert.Successful(result);
            }
        }
    }
}
