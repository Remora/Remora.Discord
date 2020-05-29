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

using Newtonsoft.Json;

namespace Remora.Discord.Gateway.API
{
    /// <summary>
    /// Represents a Discord event payload.
    /// </summary>
    /// <typeparam name="TEventData">The event data.</typeparam>
    internal sealed class EventPayload<TEventData> : Payload<TEventData>
    {
        /// <summary>
        /// Gets the sequence number of the event.
        /// </summary>
        [JsonProperty("s")]
        public int SequenceNumber { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPayload{TEventData}"/> class.
        /// </summary>
        /// <param name="data">The event data.</param>
        public EventPayload(TEventData data)
            : base(data)
        {
        }
    }
}
