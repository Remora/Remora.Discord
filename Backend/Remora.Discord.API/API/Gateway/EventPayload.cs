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

namespace Remora.Discord.API.Gateway
{
    /// <summary>
    /// Represents a Discord event payload.
    /// </summary>
    /// <typeparam name="TEventData">The event data.</typeparam>
    [PublicAPI]
    public record EventPayload<TEventData> : Payload<TEventData>, IEventPayload
        where TEventData : IGatewayEvent
    {
        /// <inheritdoc />
        public string? EventName { get; }

        /// <inheritdoc />
        public int SequenceNumber { get; }

        /// <inheritdoc />
        public OperationCode OperationCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPayload{TEventData}"/> class.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="operationCode">The operation code for the event.</param>
        /// <param name="data">The event data.</param>
        public EventPayload(string? eventName, int sequenceNumber, OperationCode operationCode, TEventData data)
            : base(data)
        {
            this.EventName = eventName;
            this.SequenceNumber = sequenceNumber;
            this.OperationCode = operationCode;
            this.SequenceNumber = sequenceNumber;
        }
    }
}
