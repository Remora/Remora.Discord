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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Samples.DiceRoller.Commands;

namespace Remora.Discord.Samples.DiceRoller
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
                .AddDiscordGateway(_ => botToken)
                .AddDiscordCommands()
                .AddCommandGroup<DiceRollCommands>();

            serviceCollection.AddHttpClient();

            var services = serviceCollection.BuildServiceProvider(true);

            var log = services.GetRequiredService<ILogger<Program>>();

            var gatewayClient = services.GetRequiredService<DiscordGatewayClient>();

            var runResult = await gatewayClient.RunAsync(cancellationSource.Token);
            if (!runResult.IsSuccess)
            {
                if (runResult.Exception is not null)
                {
                    log.LogError
                    (
                        runResult.Exception,
                        "Exception during gateway connection: {Exception}",
                        runResult.ErrorReason
                    );
                }

                if (runResult.GatewayCloseStatus.HasValue)
                {
                    log.LogError
                    (
                        "Gateway close status: {GatewayCloseStatus}",
                        runResult.GatewayCloseStatus.Value
                    );
                }

                if (runResult.WebSocketCloseStatus.HasValue)
                {
                    log.LogError
                    (
                        "Websocket close status: {WebsocketCloseStatus}",
                        runResult.WebSocketCloseStatus.Value
                    );
                }
            }

            log.LogInformation("Bye bye");
        }
    }
}
