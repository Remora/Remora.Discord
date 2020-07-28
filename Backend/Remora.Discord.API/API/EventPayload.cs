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

using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Events;

namespace Remora.Discord.API
{
    /// <summary>
    /// Represents a Discord event payload.
    /// </summary>
    /// <typeparam name="TEventData">The event data.</typeparam>
    public class EventPayload<TEventData> : Payload<TEventData>, IEventPayload
        where TEventData : IGatewayEvent
    {
        /// <inheritdoc />
        public int SequenceNumber { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPayload{TEventData}"/> class.
        /// </summary>
        /// <param name="data">The event data.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        public EventPayload(TEventData data, int sequenceNumber)
            : base(data)
        {
            this.SequenceNumber = sequenceNumber;
        }
    }
}
