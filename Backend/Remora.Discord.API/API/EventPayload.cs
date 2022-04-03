//
//  EventPayload.cs
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

using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Events;

namespace Remora.Discord.API;

/// <summary>
/// Represents a Discord event payload.
/// Initializes a new instance of the <see cref="EventPayload{TEventData}"/> class.
/// </summary>
/// <typeparam name="TEventData">The type of the event data.</typeparam>
/// <param name="EventName">The name of the event.</param>
/// <param name="SequenceNumber">The sequence number.</param>
/// <param name="OperationCode">The operation code of the event.</param>
/// <param name="Data">The event data.</param>
[PublicAPI]
public record EventPayload<TEventData>
(
    string? EventName,
    int SequenceNumber,
    OperationCode OperationCode,
    TEventData Data
) : Payload<TEventData>(Data), IEventPayload where TEventData : IGatewayEvent;
