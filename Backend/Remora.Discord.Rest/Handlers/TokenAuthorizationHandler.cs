//
//  TokenAuthorizationHandler.cs
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

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Remora.Discord.Rest.Extensions;

namespace Remora.Discord.Rest.Handlers;

/// <summary>
/// Represents a delegating handler for adding the Authorization header.
/// </summary>
internal class TokenAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenAuthorizationHandler"/> class.
    /// </summary>
    /// <param name="tokenStore">The token store.</param>
    public TokenAuthorizationHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenStore.Token;
        var tokenType = _tokenStore.TokenType;

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("The authentication token has to contain something.");
        }

        #if NET5_0_OR_GREATER
        if (request.Options.TryGetValue(Constants.SkipAuthorizationOption, out _))
        #else
        if (request.Properties.ContainsKey(Constants.SkipAuthorizationPropertyName))
        #endif
        {
            return base.SendAsync(request, cancellationToken);
        }

        AddTokenToPollyContext(request, token);
        AddAuthorizationHeader(request, token, tokenType);

        return base.SendAsync(request, cancellationToken);
    }

    private void AddAuthorizationHeader
    (
        HttpRequestMessage request,
        string token,
        DiscordTokenType tokenType
    )
    {
        request.Headers.Authorization = new AuthenticationHeaderValue
        (
            tokenType.ToString(),
            token
        );
    }

    private void AddTokenToPollyContext(HttpRequestMessage request, string token)
    {
        void ModifyContext(Context context)
        {
            context.Add("token", token);
        }

        request.ModifyPolicyExecutionContext(ModifyContext);
    }
}
