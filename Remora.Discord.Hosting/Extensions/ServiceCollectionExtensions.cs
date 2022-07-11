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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Hosting.Options;
using Remora.Discord.Hosting.Services;
using Remora.Extensions.Options.Immutable;

namespace Remora.Discord.Hosting.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the required services for Remora Discord and a <see cref="IHostedService"/> implementation
    /// using the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="tokenFactory">A function that retrieves the bot token.</param>
    /// <returns>The service collection, with the services added.</returns>
    public static IServiceCollection AddDiscordService
    (
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, string> tokenFactory
    )
    {
        _ = serviceCollection.Configure(() => new DiscordServiceOptions());

        _ = serviceCollection
            .AddDiscordGateway(tokenFactory);

        serviceCollection
            .TryAddSingleton<DiscordService>();

        _ = serviceCollection
            .AddSingleton<IHostedService, DiscordService>(serviceProvider =>
                serviceProvider.GetRequiredService<DiscordService>());
        return serviceCollection;
    }
}
