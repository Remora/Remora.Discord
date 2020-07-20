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
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Remora.Discord.API.Abstractions;
using Remora.Discord.Core;
using Remora.Discord.Rest.API.Gateway;
using Remora.Discord.Rest.Json;
using Remora.Discord.Rest.Polly;

namespace Remora.Discord.Rest.Extensions
{
    /// <summary>
    /// Defines various extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the services required for Discord's REST API.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="token">A function that creates or retrieves the authorization token.</param>
        /// <returns>The service collection, with the services added.</returns>
        public static IServiceCollection AddDiscordRest(this IServiceCollection serviceCollection, Func<string> token)
        {
            var retryDelay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5);
            serviceCollection
                .AddHttpClient<DiscordHttpClient>
                (
                    "Discord",
                    (services, client) =>
                    {
                        var assemblyName = Assembly.GetExecutingAssembly().GetName();
                        var name = assemblyName.Name;
                        var version = assemblyName.Version ?? new Version(1, 0, 0);

                        var tokenStore = services.GetRequiredService<ITokenStore>();

                        client.BaseAddress = Constants.BaseURL;
                        client.DefaultRequestHeaders.UserAgent.Add
                        (
                            new ProductInfoHeaderValue(name, version.ToString())
                        );

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue
                        (
                            "Bot",
                            tokenStore.Token
                        );
                    }
                )
                .AddTransientHttpErrorPolicy
                (
                    b => b
                        .WaitAndRetryAsync(retryDelay)
                        .WrapAsync(DiscordRateLimitPolicy.Create())
                )
                .AddPolicyHandler
                (
                    Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                        .WaitAndRetryAsync
                        (
                            1,
                            (retryCount, response, context) =>
                            {
                                if (response.Result == default)
                                {
                                    return TimeSpan.FromSeconds(1);
                                }

                                if (!response.Result.Headers.RetryAfter.Delta.HasValue)
                                {
                                    return TimeSpan.FromSeconds(1);
                                }

                                return response.Result.Headers.RetryAfter.Delta.Value;
                            },
                            (x, y, z, w) => Task.CompletedTask
                        )
                );

            serviceCollection
                .AddSingleton<Random>()
                .AddSingleton
                (
                    s => new JsonSerializerOptions
                        {
                            Converters = { new GatewayEndpointConverter() }
                        }
                )
                .AddSingleton<ITokenStore>(s => new TokenStore(token()));

            serviceCollection
                .AddScoped<IDiscordRestGatewayAPI, DiscordRestGatewayAPI>();

            return serviceCollection;
        }
    }
}
