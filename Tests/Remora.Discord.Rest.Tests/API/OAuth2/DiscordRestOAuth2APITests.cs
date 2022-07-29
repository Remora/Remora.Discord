//
//  DiscordRestOAuth2APITests.cs
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
using System.Net;
using System.Net.Http;
using System.Text;
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
                    .Respond("application/json", SampleRepository.Samples[typeof(IAuthorizationInformation)])
            );

            var result = await api.GetCurrentAuthorizationInformationAsync();
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestOAuth2API.GetTokenByAuthorizationCodeAsync"/> method.
    /// </summary>
    public class GetTokenByAuthorizationCodeAsync : RestAPITestBase<IDiscordRestOAuth2API>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var clientID = "123456789";
            var clientSecret = "abcdefg";
            var code = "hijklmno";
            var redirecUri = "https://localhost/receive-token";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}oauth2/token")
                    .With(m => !m.Headers.Contains("Authorization"))
                    .WithFormData("grant_type", "authorization_code")
                    .WithFormData("client_id", clientID)
                    .WithFormData("client_secret", clientSecret)
                    .WithFormData("code", code)
                    .WithFormData("redirect_uri", redirecUri)
                    .Respond("application/json", SampleRepository.Samples[typeof(IAccessTokenInformation)])
            );

            var result = await api.GetTokenByAuthorizationCodeAsync
            (
                clientID,
                clientSecret,
                code,
                redirecUri
            );
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestOAuth2API.GetTokenByRefreshTokenAsync"/> method.
    /// </summary>
    public class GetTokenByRefreshTokenAsync : RestAPITestBase<IDiscordRestOAuth2API>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var clientID = "123456789";
            var clientSecret = "abcdefg";
            var refreshToken = "hijklmno";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}oauth2/token")
                    .With(m => !m.Headers.Contains("Authorization"))
                    .WithFormData("grant_type", "refresh_token")
                    .WithFormData("client_id", clientID)
                    .WithFormData("client_secret", clientSecret)
                    .WithFormData("refresh_token", refreshToken)
                    .Respond("application/json", SampleRepository.Samples[typeof(IAccessTokenInformation)])
            );

            var result = await api.GetTokenByRefreshTokenAsync
            (
                clientID,
                clientSecret,
                refreshToken
            );
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestOAuth2API.GetTokenByRefreshTokenAsync"/> method.
    /// </summary>
    public class GetTokenByClientCredentialsAsync : RestAPITestBase<IDiscordRestOAuth2API>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var clientID = "123456789";
            var clientSecret = "abcdefg";
            var scopes = new[]
            {
                "identify",
                "email"
            };

            var encodedAuthorizationValue = Convert.ToBase64String
            (
                Encoding.UTF8.GetBytes($"{clientID}:{clientSecret}")
            );

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}oauth2/token")
                    .With(m => m.Headers.Authorization?.Parameter == encodedAuthorizationValue)
                    .WithFormData("grant_type", "client_credentials")
                    .WithFormData("scope", string.Join(" ", scopes))
                    .Respond("application/json", SampleRepository.Samples[typeof(IAccessTokenInformation)])
            );

            var result = await api.GetTokenByClientCredentialsAsync
            (
                clientID,
                clientSecret,
                scopes
            );
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestOAuth2API.RevokeAccessTokenAsync"/> method.
    /// </summary>
    public class RevokeAccessTokenAsync : RestAPITestBase<IDiscordRestOAuth2API>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var clientID = "123456789";
            var clientSecret = "abcdefg";
            var accessToken = "a1b2c3d4e5";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}oauth2/token/revoke")
                    .With(m => !m.Headers.Contains("Authorization"))
                    .WithFormData("client_id", clientID)
                    .WithFormData("client_secret", clientSecret)
                    .WithFormData("token", accessToken)
                    .WithFormData("token_type_hint", "access_token")
                    .Respond(HttpStatusCode.OK)
            );

            var result = await api.RevokeAccessTokenAsync
            (
                clientID,
                clientSecret,
                accessToken
            );
            ResultAssert.Successful(result);
        }
    }

    /// <summary>
    /// Tests the <see cref="DiscordRestOAuth2API.RevokeRefreshTokenAsync"/> method.
    /// </summary>
    public class RevokeRefreshTokenAsync : RestAPITestBase<IDiscordRestOAuth2API>
    {
        /// <summary>
        /// Tests whether the API method performs its request correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task PerformsRequestCorrectly()
        {
            var clientID = "123456789";
            var clientSecret = "abcdefg";
            var refreshToken = "a1b2c3d4e5";

            var api = CreateAPI
            (
                b => b
                    .Expect(HttpMethod.Post, $"{Constants.BaseURL}oauth2/token/revoke")
                    .With(m => !m.Headers.Contains("Authorization"))
                    .WithFormData("client_id", clientID)
                    .WithFormData("client_secret", clientSecret)
                    .WithFormData("token", refreshToken)
                    .WithFormData("token_type_hint", "refresh_token")
                    .Respond(HttpStatusCode.OK)
            );

            var result = await api.RevokeRefreshTokenAsync
            (
                clientID,
                clientSecret,
                refreshToken
            );
            ResultAssert.Successful(result);
        }
    }
}
