//
//  SPDX-FileName: Program.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: MIT
//

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway;
using Remora.Discord.Hosting.Extensions;
using Remora.Discord.Interactivity.Extensions;
using Remora.Discord.Pagination.Extensions;
using Remora.Discord.Samples.Interactivity.Commands;
using Remora.Discord.Samples.Interactivity.Interactions;
using Remora.Rest.Core;

namespace Remora.Discord.Samples.Interactivity;

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
        var updateSlash = await slashService.UpdateSlashCommandsAsync(debugServer);
        if (!updateSlash.IsSuccess)
        {
            log.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
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
                services.Configure<DiscordGatewayClientOptions>(g => g.Intents |= GatewayIntents.MessageContents);

                services
                    .AddDiscordCommands(true)
                    .AddCommandTree()
                        .WithCommandGroup<InteractiveCommands>()
                        .Finish()
                    .AddPagination()
                    .AddInteractionGroup<ColourDropdownInteractions>()
                    .AddInteractionGroup<ModalInteractions>()
                    .AddInteractionGroup<StatefulInteractions>();
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
