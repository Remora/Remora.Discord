//
//  DiscordRestMonetizationAPITests.cs
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Monetization;

/// <summary>
/// Tests the <see cref="DiscordRestMonetizationAPI"/> class.
/// </summary>
public class DiscordRestMonetizationAPITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestMonetizationAPI.ListEntitlementsAsync"/> method.
    /// </summary>
    public class ListEntitlementsAsync : RestAPITestBase<IDiscordRestMonetizationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListEntitlementsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ListEntitlementsAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(1);
            var userID = DiscordSnowflake.New(2);
            var skuIDs = new[] { DiscordSnowflake.New(3), DiscordSnowflake.New(4) };
            var before = DiscordSnowflake.New(5);
            var after = DiscordSnowflake.New(6);
            var limit = 1;
            var guildID = DiscordSnowflake.New(7);
            var excludeEnded = true;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}applications/{applicationID}/entitlements")
                    .WithExactQueryString
                    (
                        new KeyValuePair<string, string>[]
                        {
                            new("user_id", userID.ToString()),
                            new("sku_ids", string.Join(',', skuIDs.Select(id => id.ToString()))),
                            new("before", before.ToString()),
                            new("after", after.ToString()),
                            new("limit", limit.ToString()),
                            new("guild_id", guildID.ToString()),
                            new("exclude_ended", excludeEnded.ToString())
                        }
                    )
                    .Respond("application/json", "[ ]")
            );

            var result = await api.ListEntitlementsAsync
            (
                applicationID,
                userID,
                skuIDs,
                before,
                after,
                limit,
                guildID,
                excludeEnded
            );

            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestMonetizationAPI.CreateTestEntitlementAsync"/> method.
    /// </summary>
    public class CreateTestEntitlementAsync : RestAPITestBase<IDiscordRestMonetizationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTestEntitlementAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public CreateTestEntitlementAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(1);
            var skuID = DiscordSnowflake.New(2);
            var ownerID = DiscordSnowflake.New(3);
            var ownerType = EntitlementOwnerType.Guild;

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}applications/{applicationID}/entitlements")
                    .WithJson
                    (
                        j => j.IsObject
                        (
                            o => o
                                .WithProperty("sku_id", p => p.Is(skuID))
                                .WithProperty("owner_id", p => p.Is(ownerID))
                                .WithProperty("owner_type", p => p.Is((int)ownerType))
                        )
                    )
                    .Respond("application/json", SampleRepository.Samples[typeof(IEntitlement)])
            );

            var result = await api.CreateTestEntitlementAsync(applicationID, skuID, ownerID, ownerType);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestMonetizationAPI.DeleteTestEntitlementAsync"/> method.
    /// </summary>
    public class DeleteTestEntitlementAsync : RestAPITestBase<IDiscordRestMonetizationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTestEntitlementAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public DeleteTestEntitlementAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(1);
            var entitlementID = DiscordSnowflake.New(2);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Delete, $"{Constants.BaseURL}applications/{applicationID}/entitlements/{entitlementID}")
                    .Respond(HttpStatusCode.NoContent)
            );

            var result = await api.DeleteTestEntitlementAsync(applicationID, entitlementID);
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestMonetizationAPI.ListSKUsAsync"/> method.
    /// </summary>
    public class ListSKUsAsync : RestAPITestBase<IDiscordRestMonetizationAPI>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListSKUsAsync"/> class.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        public ListSKUsAsync(RestAPITestFixture fixture)
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
            var applicationID = DiscordSnowflake.New(1);

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}applications/{applicationID}/skus")
                    .Respond("application/json", "[ ]")
            );

            var result = await api.ListSKUsAsync(applicationID);
            ResultAssert.Successful(result);
        }
    }
}
