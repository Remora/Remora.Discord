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
using Remora.Commands.Extensions;
using Remora.Discord.API;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;
using Remora.Discord.Samples.SlashCommands.Commands;
using Remora.Rest.Core;

namespace Remora.Discord.Samples.SlashCommands;

/// <summary>
/// Represents the main class of the program.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entrypoint of the program.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous program execution.</returns>
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args)
            .UseConsoleLifetime()
            .Build();

        var services = host.Services;
        var log = services.GetRequiredService<ILogger<Program>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        Snowflake? debugServer = null;

        #if DEBUG
        var debugServerString = configuration.GetValue<string?>("REMORA_DEBUG_SERVER");
        if (debugServerString is not null)
        {
            if (!DiscordSnowflake.TryParse(debugServerString, out debugServer))
            {
                log.LogWarning("Failed to parse debug server from environment");
            }
        }
        #endif

        var slashService = services.GetRequiredService<SlashService>();

        var checkSlashSupport = slashService.SupportsSlashCommands();
        if (!checkSlashSupport.IsSuccess)
        {
            log.LogWarning
            (
                "The registered commands of the bot don't support slash commands: {Reason}",
                checkSlashSupport.Error.Message
            );
        }
        else
        {
            var updateSlash = await slashService.UpdateSlashCommandsAsync(debugServer);
            if (!updateSlash.IsSuccess)
            {
                log.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
            }
        }

        await host.RunAsync();
    }

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
                services
                    .AddDiscordCommands(true)
                    .AddCommandTree()
                        .WithCommandGroup<AutocompleteCommands>()
                        .Finish()
                    .AddAutocompleteProvider<DictionaryAutocompleteProvider>();
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
