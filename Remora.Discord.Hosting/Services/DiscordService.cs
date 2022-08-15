//
//  DiscordService.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Discord.Hosting.Options;
using Remora.Results;

namespace Remora.Discord.Hosting.Services;

/// <summary>
/// The <see cref="IHostedService"/> that will run discord in the background.
/// </summary>
[PublicAPI]
public class DiscordService : BackgroundService
{
    private readonly DiscordGatewayClient _gatewayClient;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly DiscordServiceOptions _options;
    private readonly ILogger<DiscordService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordService"/> class.
    /// </summary>
    /// <param name="gatewayClient">The gateway client.</param>
    /// <param name="lifetime">The application lifetime.</param>
    /// <param name="options">The service options.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public DiscordService
    (
        DiscordGatewayClient gatewayClient,
        IHostApplicationLifetime lifetime,
        IOptions<DiscordServiceOptions> options,
        ILogger<DiscordService> logger
    )
    {
        _gatewayClient = gatewayClient;
        _lifetime = lifetime;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var runResult = await _gatewayClient.RunAsync(stoppingToken);

        if (!runResult.IsSuccess)
        {
            switch (runResult.Error)
            {
                case ExceptionError exe:
                {
                    if (exe.Exception is OperationCanceledException)
                    {
                        // No need for further cleanup
                        return;
                    }

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
                case GatewayError:
                {
                    _logger.LogError("Gateway error: {Message}", runResult.Error.Message);
                    break;
                }
                default:
                {
                    _logger.LogError("Unknown error: {Message}", runResult.Error?.Message);
                    break;
                }
            }

            if (_options.TerminateApplicationOnCriticalGatewayErrors)
            {
                _lifetime.StopApplication();
            }
        }
    }
}
