//
//  IResponderDispatchService.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Results;

namespace Remora.Discord.Gateway.Services;

/// <summary>
/// Represents a service for dispatching gateway events.
/// </summary>
public interface IResponderDispatchService
{
    /// <summary>
    /// Gets a value indicating whether or not this <see cref="IResponderDispatchService"/> is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Enqueues an event for dispatch to all appropriate responders.
    /// </summary>
    /// <typeparam name="TGatewayEvent">The type of the event.</typeparam>
    /// <param name="gatewayEvent">The gateway event to dispatch.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the potentially asynchronous operation.</returns>
    ValueTask EnqueueEventAsync<TGatewayEvent>(TGatewayEvent gatewayEvent, CancellationToken ct = default)
        where TGatewayEvent : IGatewayEvent;

    /// <summary>
    /// Runs the dispatch service, allowing payloads to be enqueued and dispatched.
    /// This method will not return until it is cancelled.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A result representing the outcome of the operation.</returns>
    Task<Result> RunAsync(CancellationToken ct = default);
}
