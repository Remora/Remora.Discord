//
//  DiscordRestOAuth2API.cs
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
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Json.Policies;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestOAuth2API" />
[PublicAPI]
public class DiscordRestOAuth2API : AbstractDiscordRestAPI, IDiscordRestOAuth2API
{
    private readonly SnakeCaseNamingPolicy _snakeCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestOAuth2API"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    /// <param name="snakeCase">The snake case naming policy.</param>
    public DiscordRestOAuth2API
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache,
        SnakeCaseNamingPolicy snakeCase
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
        _snakeCase = snakeCase;
    }

    /// <inheritdoc />
    public virtual Task<Result<IApplication>> GetCurrentBotApplicationInformationAsync
    (
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IApplication>
        (
            "oauth2/applications/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IAuthorizationInformation>> GetCurrentAuthorizationInformationAsync
    (
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IAuthorizationInformation>
        (
            "oauth2/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IAccessTokenInformation>> GetTokenByAuthorizationCodeAsync
    (
        string clientID,
        string clientSecret,
        string code,
        string redirectUri,
        CancellationToken ct = default
    )
    {
        return await GetTokenAsync
        (
            GrantType.AuthorizationCode,
            clientID,
            clientSecret,
            code,
            redirectUri,
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IAccessTokenInformation>> GetTokenByRefreshTokenAsync
    (
        string clientID,
        string clientSecret,
        string refreshToken,
        CancellationToken ct = default
    )
    {
        return await GetTokenAsync
        (
            GrantType.RefreshToken,
            clientID,
            clientSecret,
            refreshToken: refreshToken,
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IAccessTokenInformation>> GetTokenByClientCredentialsAsync
    (
        string clientID,
        string clientSecret,
        IReadOnlyList<string> scopes,
        CancellationToken ct = default
    )
    {
        return await GetTokenAsync
        (
            GrantType.ClientCredentials,
            clientID,
            clientSecret,
            scopes: new Optional<IReadOnlyList<string>>(scopes),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result> RevokeAccessTokenAsync
    (
        string clientID,
        string clientSecret,
        string accessToken,
        CancellationToken ct = default
    )
    {
        return RevokeTokenAsync
        (
            clientID,
            clientSecret,
            accessToken,
            "access_token",
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> RevokeRefreshTokenAsync
    (
        string clientID,
        string clientSecret,
        string refreshToken,
        CancellationToken ct = default
    )
    {
        return RevokeTokenAsync
        (
            clientID,
            clientSecret,
            refreshToken,
            "refresh_token",
            ct
        );
    }

    private Task<Result<IAccessTokenInformation>> GetTokenAsync
    (
        GrantType grantType,
        string clientID,
        string clientSecret,
        Optional<string> code = default,
        Optional<string> redirectUri = default,
        Optional<string> refreshToken = default,
        Optional<IReadOnlyList<string>> scopes = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<IAccessTokenInformation>
        (
            "oauth2/token",
            b =>
            {
                var requestData = new Dictionary<string, string>
                {
                    { "grant_type", _snakeCase.ConvertName(grantType.ToString()) }
                };

                if (grantType == GrantType.ClientCredentials)
                {
                    var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientID}:{clientSecret}"));
                    b.AddHeader("Authorization", $"Basic {encodedCredentials}");
                }
                else
                {
                    requestData["client_id"] = clientID;
                    requestData["client_secret"] = clientSecret;
                }

                if (code.IsDefined(out var _))
                {
                    requestData["code"] = code.Value;
                }

                if (redirectUri.IsDefined(out var _))
                {
                    requestData["redirect_uri"] = redirectUri.Value;
                }

                if (refreshToken.IsDefined(out var _))
                {
                    requestData["refresh_token"] = refreshToken.Value;
                }

                if (scopes.IsDefined(out var s) && s.Count > 0)
                {
                    requestData["scope"] = string.Join(" ", s);
                }

                b.With(m => m.Content = new FormUrlEncodedContent(requestData));

                b.WithRateLimitContext(this.RateLimitCache, true);
                b.SkipAuthorization();
            },
            ct: ct
        );
    }

    private Task<Result> RevokeTokenAsync
    (
        string clientID,
        string clientSecret,
        string token,
        string tokenTypeHint,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync
        (
            "oauth2/token/revoke",
            b =>
            {
                var requestData = new Dictionary<string, string>
                {
                    { "client_id", clientID },
                    { "client_secret", clientSecret },
                    { "token", token },
                    { "token_type_hint", tokenTypeHint }
                };

                b.With(m => m.Content = new FormUrlEncodedContent(requestData));

                b.WithRateLimitContext(this.RateLimitCache, true);
                b.SkipAuthorization();
            },
            ct: ct
        );
    }
}
