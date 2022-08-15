//
//  AbstractDiscordRestAPI.cs
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
using System.Text.Json;
using JetBrains.Annotations;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Rest;

namespace Remora.Discord.Rest.API;

/// <summary>
/// Acts as an abstract base for REST API instances.
/// </summary>
[PublicAPI]
public abstract class AbstractDiscordRestAPI : IRestCustomizable
{
    /// <summary>
    /// Gets the <see cref="RestHttpClient{TError}"/> available to the API instance.
    /// </summary>
    protected IRestHttpClient RestHttpClient { get; }

    /// <summary>
    /// Gets the <see cref="System.Text.Json.JsonSerializerOptions"/> available to the API instance.
    /// </summary>
    protected JsonSerializerOptions JsonOptions { get; }

    /// <summary>
    /// Gets the rate limit memory cache.
    /// </summary>
    protected ICacheProvider RateLimitCache { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractDiscordRestAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord-specialized Http client.</param>
    /// <param name="jsonOptions">The Remora-specialized JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    protected AbstractDiscordRestAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
    {
        this.RestHttpClient = restHttpClient;
        this.JsonOptions = jsonOptions;
        this.RateLimitCache = rateLimitCache;
    }

    /// <inheritdoc cref="RestHttpClient{TError}.WithCustomization"/>
    public RestRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer)
    {
        return this.RestHttpClient.WithCustomization(requestCustomizer);
    }

    /// <inheritdoc cref="RestHttpClient{TError}.WithCustomization"/>
    void IRestCustomizable.RemoveCustomization(RestRequestCustomization customization)
    {
        this.RestHttpClient.RemoveCustomization(customization);
    }
}
