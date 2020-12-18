//
//  Program.cs
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Samples.SlashCommands.Responders;
using Remora.Results;

namespace Remora.Discord.Samples.SlashCommands
{
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
            var cancellationSource = new CancellationTokenSource();

            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationSource.Cancel();
            };

            var botToken =
                Environment.GetEnvironmentVariable("REMORA_BOT_TOKEN")
                ?? throw new InvalidOperationException
                (
                    "No bot token has been provided. Set the REMORA_BOT_TOKEN environment variable to a valid token."
                );

            var serviceCollection = new ServiceCollection()
                .AddLogging
                (
                    c => c
                        .AddConsole()
                        .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                        .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning)
                )
                .AddDiscordGateway(() => botToken)
                .AddResponder<HttpCatResponder>();

            serviceCollection.AddHttpClient();

            var services = serviceCollection.BuildServiceProvider();
            var log = services.GetRequiredService<ILogger<Program>>();

            var oauth2 = services.GetRequiredService<IDiscordRestOAuth2API>();
            var applications = services.GetRequiredService<IDiscordRestApplicationAPI>();

            var configureCommands = await ConfigureSlashCommands(oauth2, applications, cancellationSource.Token);
            if (!configureCommands.IsSuccess)
            {
                log.LogError("Failed to initialize slash commands.");
                return;
            }

            var gatewayClient = services.GetRequiredService<DiscordGatewayClient>();

            var runResult = await gatewayClient.RunAsync(cancellationSource.Token);
            if (!runResult.IsSuccess)
            {
                log.LogError(runResult.Exception, runResult.ErrorReason);

                if (runResult.GatewayCloseStatus.HasValue)
                {
                    log.LogError($"Gateway close status: {runResult.GatewayCloseStatus}");
                }

                if (runResult.WebSocketCloseStatus.HasValue)
                {
                    log.LogError($"Websocket close status: {runResult.WebSocketCloseStatus}");
                }
            }

            log.LogInformation("Bye bye");
        }

        private static async Task<OperationResult> ConfigureSlashCommands
        (
            IDiscordRestOAuth2API oauth2,
            IDiscordRestApplicationAPI applications,
            CancellationToken ct = default
        )
        {
            var getApplication = await oauth2.GetCurrentApplicationInformationAsync(ct);
            if (!getApplication.IsSuccess)
            {
                return OperationResult.FromError(getApplication);
            }

            var application = getApplication.Entity;

            var getCommands = await applications.GetGlobalApplicationCommandsAsync(application.ID, ct);
            if (!getCommands.IsSuccess)
            {
                return OperationResult.FromError(getCommands);
            }

            var commands = getCommands.Entity;
            var catCommand = commands.FirstOrDefault(c => c.Name == "cat");
            if (catCommand is not null)
            {
                return OperationResult.FromSuccess();
            }

            var commandOption = new ApplicationCommandOption
            (
                ApplicationCommandOptionType.Integer,
                "code",
                "The HTTP error code",
                false,
                true,
                default,
                default
            );

            var createCommand = await applications.CreateGlobalApplicationCommandAsync
            (
                application.ID,
                "cat",
                "Posts a cat image with an HTTP error code",
                new[] { commandOption },
                ct
            );

            return !createCommand.IsSuccess ? OperationResult.FromError(createCommand) : OperationResult.FromSuccess();
        }
    }
}
