//
//  IResponder.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Results;

// ReSharper disable SA1402 - we allow two types in this file, because one is a marker interface of the other
#pragma warning disable SA1402

namespace Remora.Discord.Gateway.Responders;

/// <summary>
/// Represents a marker interface for event responders.
/// </summary>
[PublicAPI]
public interface IResponder
{
}

/// <summary>
/// Represents a type that can respond to certain gateway events.
/// </summary>
/// <typeparam name="TGatewayEvent">The gateway event.</typeparam>
[PublicAPI]
public interface IResponder<in TGatewayEvent> : IResponder
    where TGatewayEvent : IGatewayEvent
{
    /// <summary>
    /// Responds to the given gateway event, handling it asynchronously.
    /// </summary>
    /// <param name="gatewayEvent">The event to respond to.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A response result which may or may not have succeeded.</returns>
    Task<Result> RespondAsync(TGatewayEvent gatewayEvent, CancellationToken ct = default);
}
