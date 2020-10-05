//
//  DiscordRestInviteAPITests.cs
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
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.Extensions;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.Invites
{
    /// <summary>
    /// Tests the <see cref="DiscordRestInviteAPI"/> class.
    /// </summary>
    public class DiscordRestInviteAPITests
    {
        /// <summary>
        /// Tests the <see cref="DiscordRestInviteAPI.GetInviteAsync"/> method.
        /// </summary>
        public class GetInviteAsync : RestAPITestBase<IDiscordRestInviteAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var inviteCode = "brr";
                var withCounts = true;

                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Get, $"{Constants.BaseURL}invites/{inviteCode}")
                        .WithQueryString("with_counts", withCounts.ToString())
                        .Respond("application/json", SampleRepository.Samples[typeof(IInvite)])
                );

                var result = await api.GetInviteAsync(inviteCode, withCounts);
                ResultAssert.Successful(result);
            }
        }

        /// <summary>
        /// Tests the <see cref="DiscordRestInviteAPI.GetInviteAsync"/> method.
        /// </summary>
        public class DeleteInviteAsync : RestAPITestBase<IDiscordRestInviteAPI>
        {
            /// <summary>
            /// Tests whether the API method performs its request correctly.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
            [Fact]
            public async Task PerformsRequestCorrectly()
            {
                var inviteCode = "brr";
                var api = CreateAPI
                (
                    b => b
                        .Expect(HttpMethod.Delete, $"{Constants.BaseURL}invites/{inviteCode}")
                        .Respond("application/json", SampleRepository.Samples[typeof(IInvite)])
                );

                var result = await api.DeleteInviteAsync(inviteCode);
                ResultAssert.Successful(result);
            }
        }
    }
}
