//
//  ServiceCollectionExtensions.cs
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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.API;
using Remora.Discord.Caching.Responders;
using Remora.Discord.Caching.Services;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Responders;
using Remora.Discord.Rest.Extensions;

namespace Remora.Discord.Caching.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds caching implementations of various API types, overriding the normally non-caching versions.
    /// </summary>
    /// <remarks>
    /// The cache uses a run-of-the-mill <see cref="IMemoryCache"/>. Cache entry options for any cached type can be
    /// configured using <see cref="IOptions{CacheSettings}"/>.
    ///
    /// When choosing a cache implementation, it should be noted that choosing this will override the backing store for
    /// caching REST clients and responders.
    /// </remarks>
    /// <param name="services">The services.</param>
    /// <returns>The services, with caching enabled.</returns>
    public static IServiceCollection AddDiscordCaching(this IServiceCollection services)
    {
        services.TryAddSingleton<CacheService>();
        services.AddOptions<CacheSettings>();

        services
            .Decorate<IDiscordRestChannelAPI, CachingDiscordRestChannelAPI>()
            .Decorate<IDiscordRestEmojiAPI, CachingDiscordRestEmojiAPI>()
            .Decorate<IDiscordRestGuildAPI, CachingDiscordRestGuildAPI>()
            .Decorate<IDiscordRestInteractionAPI, CachingDiscordRestInteractionAPI>()
            .Decorate<IDiscordRestInviteAPI, CachingDiscordRestInviteAPI>()
            .Decorate<IDiscordRestOAuth2API, CachingDiscordRestOAuth2API>()
            .Decorate<IDiscordRestTemplateAPI, CachingDiscordRestTemplateAPI>()
            .Decorate<IDiscordRestUserAPI, CachingDiscordRestUserAPI>()
            .Decorate<IDiscordRestVoiceAPI, CachingDiscordRestVoiceAPI>()
            .Decorate<IDiscordRestWebhookAPI, CachingDiscordRestWebhookAPI>();

        services
            .AddResponder<EarlyCacheResponder>(ResponderGroup.Early)
            .AddResponder<LateCacheResponder>(ResponderGroup.Late);

        return services;
    }
}
