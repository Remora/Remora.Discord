//
//  ServiceCollectionExtensions.cs
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
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Extensions;
using Remora.Discord.API.Objects;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.API;
using Remora.Discord.Rest.API.AutoModeration;
using Remora.Discord.Rest.Caching;
using Remora.Discord.Rest.Polly;
using Remora.Rest;
using Remora.Rest.Extensions;

namespace Remora.Discord.Rest.Extensions;

/// <summary>
/// Defines various extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required for Discord's REST API.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="tokenFactory">A function that creates or retrieves the authorization token.</param>
    /// <param name="buildClient">Extra client building operations.</param>
    /// <returns>The service collection, with the services added.</returns>
    public static IServiceCollection AddDiscordRest
    (
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, string> tokenFactory,
        Action<IHttpClientBuilder>? buildClient = null
    )
    {
        serviceCollection.AddMemoryCache();

        serviceCollection.TryAddSingleton<MemoryCacheProvider>();
        serviceCollection.AddSingleton<ICacheProvider>(s => s.GetRequiredService<MemoryCacheProvider>());

        serviceCollection.ConfigureDiscordJsonConverters();

        serviceCollection
            .AddSingleton<ITokenStore>(serviceProvider => new TokenStore(tokenFactory(serviceProvider)));

        serviceCollection.TryAddTransient<IDiscordRestAuditLogAPI>(s => new DiscordRestAuditLogAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestAutoModerationAPI>(s => new DiscordRestAutoModerationAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestChannelAPI>(s => new DiscordRestChannelAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestEmojiAPI>(s => new DiscordRestEmojiAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestGatewayAPI>(s => new DiscordRestGatewayAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestGuildAPI>(s => new DiscordRestGuildAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestGuildScheduledEventAPI>(s => new DiscordRestGuildScheduledEventAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestInviteAPI>(s => new DiscordRestInviteAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestUserAPI>(s => new DiscordRestUserAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestVoiceAPI>(s => new DiscordRestVoiceAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestWebhookAPI>(s => new DiscordRestWebhookAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestTemplateAPI>(s => new DiscordRestTemplateAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestInteractionAPI>(s => new DiscordRestInteractionAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestApplicationAPI>(s => new DiscordRestApplicationAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestOAuth2API>(s => new DiscordRestOAuth2API
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestStageInstanceAPI>(s => new DiscordRestStageInstanceAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        serviceCollection.TryAddTransient<IDiscordRestStickerAPI>(s => new DiscordRestStickerAPI
        (
            s.GetRequiredService<IRestHttpClient>(),
            s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get("Discord"),
            s.GetRequiredService<ICacheProvider>()
        ));

        var rateLimitPolicy = DiscordRateLimitPolicy.Create();
        var retryDelay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5);
        var clientBuilder = serviceCollection
            .AddRestHttpClient<RestError>("Discord")
            .ConfigureHttpClient((services, client) =>
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                var name = assemblyName.Name ?? "Remora.Discord";
                var version = assemblyName.Version ?? new Version(1, 0, 0);

                var tokenStore = services.GetRequiredService<ITokenStore>();

                client.BaseAddress = Constants.BaseURL;
                client.DefaultRequestHeaders.UserAgent.Add
                (
                    new ProductInfoHeaderValue(name, version.ToString())
                );

                var token = tokenStore.Token;
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("The authentication token has to contain something.");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue
                (
                    "Bot",
                    token
                );
            })
            .AddTransientHttpErrorPolicy
            (
                b => b
                    .WaitAndRetryAsync(retryDelay)
                    .WrapAsync(rateLimitPolicy)
            )
            .AddPolicyHandler
            (
                Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync
                    (
                        1,
                        (_, response, _) =>
                        {
                            if (response.Result == default)
                            {
                                return TimeSpan.FromSeconds(1);
                            }

                            return response.Result.Headers.RetryAfter is null or { Delta: null }
                                ? TimeSpan.FromSeconds(1)
                                : response.Result.Headers.RetryAfter.Delta.Value;
                        },
                        (_, _, _, _) => Task.CompletedTask
                    )
            );

        // Run extra user-provided client building operations.
        buildClient?.Invoke(clientBuilder);

        return serviceCollection;
    }
}
