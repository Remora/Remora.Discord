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
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Extensions.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Hosting.Extensions;

namespace Remora.Discord.Samples.LoadRespondersFromAssembly;

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
                services.AddRespondersFromAssembly(typeof(Program).Assembly);
                services.Configure<DiscordGatewayClientOptions>
                (
                    g => g.Intents |= GatewayIntents.MessageContents
                                      | GatewayIntents.GuildMembers
                                      | GatewayIntents.GuildVoiceStates
                                      | GatewayIntents.GuildMessageReactions
                );
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
