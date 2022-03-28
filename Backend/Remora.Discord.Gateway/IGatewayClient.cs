//
//  IGatewayClient.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Results;

namespace Remora.Discord.Gateway;

/// <summary>
/// Represents a Discord gateway client.
/// </summary>
[PublicAPI]
public interface IGatewayClient
{
    /// <summary>
    /// Gets the status of the client.
    /// </summary>
    GatewayConnectionStatus Status { get; }

    /// <summary>
    /// Gets an estimate of the current round-trip latency of the gateway.
    /// This property will return <c>TimeSpan.Zero</c> until the gateway
    /// has started.
    /// </summary>
    TimeSpan Latency { get; }

    /// <summary>
    /// Runs the gateway client. This task will only complete when the
    /// gateway connection fatally disconnects, or it is cancelled.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A result representing the outcome of the operation.</returns>
    Task<Result> RunAsync(CancellationToken ct = default);

    /// <summary>
    /// Enqueues a command to be sent to the remote gateway.
    /// </summary>
    /// <typeparam name="TCommand">The type of the <paramref name="command"/>.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the potentially asynchronous operation.</returns>
    ValueTask EnqueueCommandAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : IGatewayCommand;

    /// <summary>
    /// Registers a command that will be sent immediately before a user-requested shutdown.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <returns>
    /// A unique identifier that can be used to deregister the
    /// command by calling <see cref="DeregisterPreShutdownCommand"/>.
    /// </returns>
    Guid RegisterPreShutdownCommand<TCommand>(TCommand command)
        where TCommand : IGatewayCommand;

    /// <summary>
    /// De-registers a a pre-shutdown command that was previous registered
    /// with <see cref="RegisterPreShutdownCommand{TCommand}"/>.
    /// </summary>
    /// <param name="id">The identifier of the command to de-register.</param>
    void DeregisterPreShutdownCommand(Guid id);
}
