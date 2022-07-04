//
//  AuthorizationHandler.cs
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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Remora.Discord.Rest.Handlers;

/// <summary>
/// Represents a delegating handler for adding the Authorization header.
/// </summary>
internal class AuthorizationHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationHandler"/> class.
    /// </summary>
    /// <param name="tokenStore">The token store.</param>
    public AuthorizationHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Properties.ContainsKey(Constants.SkipAuthorizationPropertyName))
        {
            AddAuthorizationHeader(request);
        }

        return base.SendAsync(request, cancellationToken);
    }

    private void AddAuthorizationHeader(HttpRequestMessage request)
    {
        var token = _tokenStore.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("The authentication token has to contain something.");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue
        (
            "Bot",
            token
        );
    }
}
