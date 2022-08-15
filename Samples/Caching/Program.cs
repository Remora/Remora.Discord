//
//  Program.cs
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
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Hosting.Extensions;

namespace Remora.Discord.Samples.Caching;

/// <summary>
/// Represents the main class of the program.
/// </summary>
public static class Program
{
    /// <summary>
    /// The main entrypoint of the program.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous program execution.</returns>
    public static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

    /// <summary>
    /// Creates a generic application host builder.
    /// </summary>
    /// <param name="args">The arguments passed to the application.</param>
    /// <returns>The host builder.</returns>
    private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
        .AddDiscordService
        (
            services =>
            {
                var configuration = services.GetRequiredService<IConfiguration>();

                return configuration.GetValue<string?>("REMORA_BOT_TOKEN") ??
                       throw new InvalidOperationException
                       (
                           "No bot token has been provided. Set the REMORA_BOT_TOKEN environment variable to a " +
                           "valid token."
                       );
            }
        )
        .ConfigureServices
        (
            (_, services) =>
            {
                services.Configure<CacheSettings>(settings =>
                {
                    // Set the total time for an object to remain in the cache by default.
                    settings.SetDefaultAbsoluteExpiration(TimeSpan.FromSeconds(60));

                    // Set the time for an object to last each time it is accessed.
                    settings.SetDefaultSlidingExpiration(TimeSpan.FromSeconds(20));

                    // Set the total time that objects implementing IMessage will last in the cache.
                    settings.SetAbsoluteExpiration<IMessage>(TimeSpan.FromMinutes(2));
                });
            }
        )
        .ConfigureLogging
        (
            c => c
                .AddConsole()
                .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning)
        );
}
