//
//  CachingDiscordRestOAuth2API.Delegations.cs
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
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest;
using Remora.Results;

namespace Remora.Discord.Caching.API;

public partial class CachingDiscordRestOAuth2API
{
    /// <inheritdoc/>
    public RestRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            // TODO: not ideal...
            throw new NotImplementedException("The decorated API type is not customizable.");
        }

        return customizable.WithCustomization(requestCustomizer);
    }

    /// <inheritdoc/>
    void IRestCustomizable.RemoveCustomization(RestRequestCustomization customization)
    {
        if (_actual is not IRestCustomizable customizable)
        {
            return;
        }

        customizable.RemoveCustomization(customization);
    }

    /// <inheritdoc/>
    public Task<Result<IAccessTokenInformation>> GetTokenByAuthorizationCodeAsync
    (
        string clientID,
        string clientSecret,
        string code,
        string redirectUri,
        CancellationToken ct = default
    )
    {
        return _actual.GetTokenByAuthorizationCodeAsync
        (
            clientID,
            clientSecret,
            code,
            redirectUri,
            ct
        );
    }

    /// <inheritdoc/>
    public Task<Result<IAccessTokenInformation>> GetTokenByRefreshTokenAsync
    (
        string clientID,
        string clientSecret,
        string refreshToken,
        CancellationToken ct = default
    )
    {
        return _actual.GetTokenByRefreshTokenAsync
        (
            clientID,
            clientSecret,
            refreshToken,
            ct
        );
    }

    /// <inheritdoc/>
    public Task<Result<IAccessTokenInformation>> GetTokenByClientCredentialsAsync
    (
        string clientID,
        string clientSecret,
        IReadOnlyList<string> scopes,
        CancellationToken ct = default
    )
    {
        return _actual.GetTokenByClientCredentialsAsync
        (
            clientID,
            clientSecret,
            scopes,
            ct
        );
    }

    /// <inheritdoc/>
    public Task<Result> RevokeAccessTokenAsync
    (
        string clientID,
        string clientSecret,
        string accessToken,
        CancellationToken ct = default
    )
    {
        return _actual.RevokeAccessTokenAsync
        (
            clientID,
            clientSecret,
            accessToken,
            ct
        );
    }

    /// <inheritdoc/>
    public Task<Result> RevokeRefreshTokenAsync
    (
        string clientID,
        string clientSecret,
        string refreshToken,
        CancellationToken ct = default
    )
    {
        return _actual.RevokeRefreshTokenAsync
        (
            clientID,
            clientSecret,
            refreshToken,
            ct
        );
    }
}
