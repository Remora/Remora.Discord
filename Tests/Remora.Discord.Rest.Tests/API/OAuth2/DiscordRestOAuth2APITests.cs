//
//  DiscordRestOAuth2APITests.cs
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

using System.Net.Http;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.Tests.TestBases;
using Remora.Discord.Tests;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Discord.Rest.Tests.API.OAuth2;

/// <summary>
/// Tests the <see cref="DiscordRestOAuth2API"/> class.
/// </summary>
public class DiscordRestOAuth2APITests
{
    /// <summary>
    /// Tests the <see cref="DiscordRestOAuth2API.GetCurrentBotApplicationInformationAsync"/> method.
    /// </summary>
    public class GetCurrentApplicationInformationAsync : RestAPITestBase<IDiscordRestOAuth2API>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}oauth2/applications/@me")
                    .WithNoContent()
                    .Respond("application/json", SampleRepository.Samples[typeof(IApplication)])
            );

            var result = await api.GetCurrentBotApplicationInformationAsync();
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestOAuth2API.GetCurrentAuthorizationInformationAsync"/> method.
    /// </summary>
    public class GetCurrentAuthorizationInformationAsync : RestAPITestBase<IDiscordRestOAuth2API>
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
                    .Expect(HttpMethod.Get, $"{Constants.BaseURL}oauth2/@me")
                    .WithNoContent()
                    .Respond("Authorization/json", SampleRepository.Samples[typeof(IAuthorizationInformation)])
            );

            var result = await api.GetCurrentAuthorizationInformationAsync();
            ResultAssert.Successful(result);
        }
    }
}
