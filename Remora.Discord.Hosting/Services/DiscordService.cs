//
//  DiscordService.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace Remora.Discord.Hosting.Services
{
    /// <summary>
    /// The <see cref="IHostedService"/> that will run discord in the background.
    /// </summary>
    [PublicAPI]
    public class DiscordService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DiscordService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordService"/> class.
        /// </summary>
        /// <param name="scopeFactory">The scope factory.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public DiscordService(IServiceScopeFactory scopeFactory, ILogger<DiscordService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var gatewayClient = scope.ServiceProvider.GetRequiredService<DiscordGatewayClient>();
            var runResult = await gatewayClient.RunAsync(stoppingToken);

            if (!runResult.IsSuccess)
            {
                switch (runResult.Error)
                {
                    case ExceptionError exe:
                    {
                        _logger.LogError
                        (
                            exe.Exception,
                            "Exception during gateway connection: {ExceptionMessage}",
                            exe.Message
                        );

                        break;
                    }
                    case GatewayWebSocketError:
                    case GatewayDiscordError:
                    {
                        _logger.LogError("Gateway error: {Message}", runResult.Error.Message);
                        break;
                    }
                    default:
                    {
                        _logger.LogError("Unknown error: {Message}", runResult.Error.Message);
                        break;
                    }
                }
            }
        }
    }
}
