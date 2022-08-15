//
//  RestRequestBuilderExtensions.cs
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

using JetBrains.Annotations;
using Polly;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;

namespace Remora.Discord.Rest.Extensions;

/// <summary>
/// Defines extensions to the <see cref="RestRequestBuilder"/> class.
/// </summary>
[PublicAPI]
public static class RestRequestBuilderExtensions
{
    /// <summary>
    /// Adds an audit log reason header to the request, provided the value is defined.
    /// </summary>
    /// <param name="builder">The request builder.</param>
    /// <param name="value">The value of the header.</param>
    /// <returns>The builder, potentially with the header.</returns>
    public static RestRequestBuilder AddAuditLogReason(this RestRequestBuilder builder, Optional<string> value)
    {
        return builder.AddHeader(Constants.AuditLogHeaderName, value);
    }

    /// <summary>
    /// Sets up a Polly context with an endpoint for rate limiting purposes.
    /// </summary>
    /// <param name="builder">The request builder.</param>
    /// <param name="cache">The memory cache in use.</param>
    /// <param name="isExemptFromGlobalLimits">
    /// Whether this request is exempt from global rate limits, and doesn't need to consider them.
    /// </param>
    /// <returns>The builder, with the context.</returns>
    public static RestRequestBuilder WithRateLimitContext
    (
        this RestRequestBuilder builder,
        ICacheProvider cache,
        bool isExemptFromGlobalLimits = false
    )
    {
        void ModifyContext(Context context)
        {
            context.Add("endpoint", builder.Endpoint);
            context.Add("cache", cache);
            context.Add("exempt-from-global-rate-limits", isExemptFromGlobalLimits);
        }

        builder.With(r => r.ModifyPolicyExecutionContext(ModifyContext));

        return builder;
    }

    /// <summary>
    /// Adds an property to the request, used for skipping the addition of the Authorization header.
    /// </summary>
    /// <param name="builder">The request builder.</param>
    /// <returns>The builder, with the property.</returns>
    public static RestRequestBuilder SkipAuthorization
    (
        this RestRequestBuilder builder
    )
    {
        #if NET5_0_OR_GREATER
        builder.With(r => r.Options.Set(Constants.SkipAuthorizationOption, true));
        #else
        builder.With(r => r.Properties.Add(Constants.SkipAuthorizationPropertyName, true));
        #endif

        return builder;
    }
}
