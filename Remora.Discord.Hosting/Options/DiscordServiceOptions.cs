//
//  DiscordServiceOptions.cs
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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using OneOf;
using Remora.Discord.Rest;

using ApiConstants = Remora.Discord.API.Constants;
using RestConstants = Remora.Discord.Rest.Constants;

namespace Remora.Discord.Hosting.Options;

/// <summary>
/// Defines a set of options used by the background gateway service.
/// </summary>
/// <param name="APIBasePath">The base path to the API.</param>
/// <param name="CDNBasePath">The base path to the CDN.</param>
/// <param name="TokenType">The type of the token to use.
/// <param name="TerminateApplicationOnCriticalGatewayErrors">
/// Whether the service should stop the application if a critical gateway error is encountered.
/// </param>
[PublicAPI]
public sealed record DiscordServiceOptions
(
    Uri APIBasePath,
    Uri CDNBasePath,
    DiscordTokenType TokenType,
    bool TerminateApplicationOnCriticalGatewayErrors = true
)
{
    /// <summary>
    /// Gets a <see cref="DiscordServiceOptions"/> intended for use with the official Discord API.
    /// </summary>
    public static DiscordServiceOptions Discord => new(RestConstants.DiscordBaseURL, ApiConstants.CDNBaseURL, DiscordTokenType.Bot, true);

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordServiceOptions"/> class.
    /// </summary>
    /// <param name="apiBasePath">The api base path, as either a <see cref="string"/> or a <see cref="Uri"/>.</param>
    /// <param name="cdnBasePath">The cdn base path, as either a <see cref="string"/> or a <see cref="Uri"/>.</param>
    public DiscordServiceOptions(OneOf<string, Uri> apiBasePath, OneOf<string, Uri> cdnBasePath)
        : this(apiBasePath, DiscordTokenType.Bot, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordServiceOptions"/> class.
    /// </summary>
    /// <param name="apiBasePath">The base path, either as a <see cref="string"/> or a <see cref="Uri"/>.</param>
    /// <param name="cdnBasePath">The cdn base path, as either a <see cref="string"/> or a <see cref="Uri"/>.</param>
    /// <param name="tokenType">The type of the token to use.</param>
    /// <param name="terminateApplicationOnCriticalGatewayErrors">Whether the service should stop the application if a critical gateway error is encountered.</param>
    public DiscordServiceOptions(OneOf<string, Uri> apiBasePath, OneOf<string, Uri> cdnBasePath, DiscordTokenType tokenType, bool terminateApplicationOnCriticalGatewayErrors)
        : this(apiBasePath.Match(path => new Uri(path), uri => uri), cdnBasePath.Match(path => new Uri(path), uri => uri), tokenType, true)
    {
    }

    /// <summary>
    /// Verifies this instance to ensure a user is not trying to connect to Discord via a user token.
    /// </summary>
    /// <exception cref="InvalidOperationException">The API path is pointing to official Discord servers
    /// and a connection attempt was made with a <see cref="DiscordTokenType.User"/> token.</exception>
    public void Verify()
    {
        if (!TryVerify())
        {
            throw GetUserTokenException();
        }
    }

    /// <summary>
    /// Verifies this instance to ensure a user is not trying to connect to Discord via a user token.
    /// </summary>
    /// <returns><see langword="true"/> if the instance passes verification; otherwise, <see langword="false"/>.</returns>
    public bool TryVerify()
        => DiscordServiceOptions.TryVerify(this);

    /// <summary>
    /// Verifies the provided <paramref name="serviceOptions"/> to ensure a user is not trying to connect to Discord via a user token.
    /// </summary>
    /// <param name="serviceOptions">The <see cref="DiscordServiceOptions"/> instance to verify.</param>
    /// <exception cref="InvalidOperationException">The API path is pointing to official Discord servers
    /// and a connection attempt was made with a <see cref="DiscordTokenType.User"/> token.</exception>
    public static void Verify(DiscordServiceOptions serviceOptions)
    {
        if (!DiscordServiceOptions.TryVerify(serviceOptions))
        {
            throw GetUserTokenException();
        }
    }

    /// <summary>
    /// Verifies the provided <paramref name="serviceOptions"/> to ensure a user is not trying to connect to Discord via a user token.
    /// </summary>
    /// <param name="serviceOptions">The <see cref="DiscordServiceOptions"/> instance to verify.</param>
    /// <returns><see langword="true"/> if the instance passes verification; otherwise, <see langword="false"/>.</returns>
    public static bool TryVerify(DiscordServiceOptions serviceOptions)
    {
        if (serviceOptions.APIBasePath == RestConstants.DiscordBaseURL)
        {
            return serviceOptions.TokenType != DiscordTokenType.User;
        }

        // If we're not using the Discord API, allow any token type.
        return true;
    }

    private static InvalidOperationException GetUserTokenException()
        => new InvalidOperationException("You MUST NOT use a user token with the official Discord API.");
}
