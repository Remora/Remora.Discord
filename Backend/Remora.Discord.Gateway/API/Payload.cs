//
//  Payload.cs
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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Remora.Discord.Gateway.API
{
    /// <summary>
    /// Represents a payload from the Discord gateway.
    /// </summary>
    internal sealed class Payload
    {
        /// <summary>
        /// Gets the operation code for this payload.
        /// </summary>
        [JsonProperty("op")]
        public OperationCode Operation { get; }

        /// <summary>
        /// Gets the data structure for the event.
        /// </summary>
        [JsonProperty("d")]
        public JObject Data { get; }

        /// <summary>
        /// Gets the sequence number of the event. This is always null when <see cref="Operation"/> is not
        /// <see cref="OperationCode.Dispatch"/>.
        /// </summary>
        [JsonProperty("s")]
        public int? SequenceNumber { get; }

        /// <summary>
        /// Gets the name of the event. This is always null when <see cref="Operation"/> is not
        /// <see cref="OperationCode.Dispatch"/>.
        /// </summary>
        [JsonProperty("t")]
        public string? EventName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payload"/> class.
        /// </summary>
        /// <param name="operation">The operation code.</param>
        /// <param name="data">The JSON data.</param>
        /// <param name="sequenceNumber">The sequence number, if the payload is an event.</param>
        /// <param name="eventName">The name of the event.</param>
        public Payload(OperationCode operation, JObject data, int? sequenceNumber = null, string? eventName = null)
        {
            this.Operation = operation;
            this.Data = data;
            this.SequenceNumber = sequenceNumber;
            this.EventName = eventName;
        }
    }
}
