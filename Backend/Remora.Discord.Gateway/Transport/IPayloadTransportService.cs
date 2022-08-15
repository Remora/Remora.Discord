//
//  IPayloadTransportService.cs
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
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Results;

namespace Remora.Discord.Gateway.Transport;

/// <summary>
/// Represents an arbitrary transport service for <see cref="IPayload"/> instances.
/// </summary>
/// <remarks>
/// This interface defines the public API surface for a type that the gateway client can use to send and receive
/// payloads from the Discord gateway. It is not specifically concerned with the actual protocol used underneath the
/// hood, and instead only presents abstract I/O operations.
///
/// Some assumptions are made in regards to endpoints and availability of operations (one is expected to be able to
/// connect and disconnect separately from sending and receiving, for example), but generally, it is kept to a
/// minimum.
/// </remarks>
[PublicAPI]
public interface IPayloadTransportService
{
    /// <summary>
    /// Gets a value indicating whether the service has successfully connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the transport endpoint, enabling I/O operations.
    /// </summary>
    /// <param name="endpoint">The endpoint to connect to.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A connection result which may or may not have succeeded.</returns>
    Task<Result> ConnectAsync(Uri endpoint, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously sends a payload.
    /// </summary>
    /// <remarks>
    /// This method should be thread-safe in conjunction with <see cref="ReceivePayloadAsync"/>.
    /// </remarks>
    /// <param name="payload">The payload.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A send result which may or may not have succeeded.</returns>
    Task<Result> SendPayloadAsync(IPayload payload, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously receives a payload.
    /// </summary>
    /// <remarks>
    /// This method should be thread-safe in conjunction with <see cref="SendPayloadAsync"/>.
    /// </remarks>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A receive result which may or may not have succeeded.</returns>
    Task<Result<IPayload>> ReceivePayloadAsync(CancellationToken ct = default);

    /// <summary>
    /// Disconnects from the transport endpoint.
    /// </summary>
    /// <param name="reconnectionIntended">Whether reconnection is intended.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A connection result which may or may not have succeeded.</returns>
    Task<Result> DisconnectAsync(bool reconnectionIntended, CancellationToken ct = default);
}
