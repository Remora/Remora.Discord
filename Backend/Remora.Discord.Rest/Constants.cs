//
//  Constants.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions;

namespace Remora.Discord.Rest;

/// <summary>
/// Holds various constants.
/// </summary>
[PublicAPI]
public static class Constants
{
    /// <summary>
    /// Gets the base API URL.
    /// </summary>
    public static Uri BaseURL { get; } = new($"https://discord.com/api/v{(int)DiscordAPIVersion.V10}/");

    /// <summary>
    /// Gets the name of the audit log reason header.
    /// </summary>
    public static string AuditLogHeaderName { get; } = "X-Audit-Log-Reason";

    /// <summary>
    /// Gets the name of the rate limit header.
    /// </summary>
    public static string RateLimitHeaderName { get; } = "X-RateLimit-Limit";

    /// <summary>
    /// Gets the name of the remaining rate limit token header.
    /// </summary>
    public static string RateLimitRemainingHeaderName { get; } = "X-RateLimit-Remaining";

    /// <summary>
    /// Gets the name of the rate limit reset time header.
    /// </summary>
    public static string RateLimitResetHeaderName { get; } = "X-RateLimit-Reset";

    /// <summary>
    /// Gets the name of the rate limit bucket header.
    /// </summary>
    public static string RateLimitBucketHeaderName { get; } = "X-RateLimit-Bucket";

    /// <summary>
    /// Gets the name of the locale header, used when requesting localized objects.
    /// </summary>
    public static string LocaleHeaderName { get; } = "X-Discord-Locale";

    /// <summary>
    /// Gets the global rate limit count.
    /// </summary>
    internal static int GlobalRateLimit { get; } = 50;

    /// <summary>
    /// Gets the name of the property, used when adding of Authorization should be skipped.
    /// </summary>
    internal static string SkipAuthorizationPropertyName { get; } = "remora::skip-authorization";

#if NET5_0_OR_GREATER
    /// <summary>
    /// Gets the HTTP request option, used when adding of Authorization should be skipped.
    /// </summary>
    internal static System.Net.Http.HttpRequestOptionsKey<bool> SkipAuthorizationOption { get; } = new(SkipAuthorizationPropertyName);
#endif
}
