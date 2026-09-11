//
//  HostBuilderExtensions.cs
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Hosting.Options;
using Remora.Discord.Hosting.Services;
using Remora.Discord.Rest;
using Remora.Extensions.Options.Immutable;

namespace Remora.Discord.Hosting.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IHostBuilder"/> interface.
/// </summary>
[PublicAPI]
public static class HostBuilderExtensions
{
    /// <summary>
    /// Adds the required services for Remora Discord and a <see cref="IHostedService"/> implementation to an
    /// <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="tokenFactory">A function that retrieves the bot token.</param>
    /// <param name="buildClient">Extra options to configure the rest client.</param>
    /// <returns>The host builder, with the services added.</returns>
    public static IHostBuilder AddDiscordService
    (
        this IHostBuilder hostBuilder,
        Func<IServiceProvider, string> tokenFactory,
        Action<IHttpClientBuilder>? buildClient = null
    )
        => AddDiscordService(hostBuilder, tokenFactory, DiscordServiceOptions.Discord, buildClient);

    /// <inheritdoc cref="AddDiscordService(IHostBuilder, Func{IServiceProvider, string}, Action{IHttpClientBuilder}?)"/>
    /// <param name="hostbuilder"/>
    /// <param name="tokenFactory"/>
    /// <param name="discordServiceOptions">The <paramref name="discordServiceOptions"/> used to configure this service.</param>
    /// <param name="buildClient"/>
    public static IHostBuilder AddDiscordService
    (
        this IHostBuilder hostbuilder,
        Func<IServiceProvider, string> tokenFactory,
        DiscordServiceOptions discordServiceOptions,
        Action<IHttpClientBuilder>? buildClient = null
    )
    {
        hostbuilder.ConfigureServices((_, serviceCollection) =>
            serviceCollection.AddDiscordService(tokenFactory, discordServiceOptions, buildClient));

        return hostbuilder;
    }

    /// <summary>
    /// Adds the required services for Remora Discord and a <see cref="IHostedService"/> implementation to an
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="tokenFactory">A function that retrieves the bot token.</param>
    /// <param name="discordServiceOptions">The <see cref="DiscordServiceOptions"/> used to configure this service.</param>
    /// <param name="buildClient">Extra options to configure the rest client.</param>
    /// <returns>The service collection, with the services added.</returns>
    public static IServiceCollection AddDiscordService
    (
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, string> tokenFactory,
        DiscordServiceOptions discordServiceOptions,
        Action<IHttpClientBuilder>? buildClient = null
    )
    {
        discordServiceOptions.Verify();

        serviceCollection.Configure(() => discordServiceOptions);

        serviceCollection
            .AddDiscordGateway(tokenFactory, discordServiceOptions.APIBasePath, discordServiceOptions.CDNBasePath, discordServiceOptions.TokenType, buildClient);

        serviceCollection
            .TryAddSingleton<DiscordService>();

        serviceCollection
            .AddSingleton<IHostedService, DiscordService>(serviceProvider =>
                serviceProvider.GetRequiredService<DiscordService>());

        return serviceCollection;
    }
}
