//
//  PartySize.cs
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
using Remora.Discord.Gateway.API.Json.Converters;

namespace Remora.Discord.Gateway.API.Objects
{
    /// <summary>
    /// Represents a party size object.
    /// </summary>
    [JsonConverter(typeof(PartySizeConverter))]
    internal sealed class PartySize
    {
        /// <summary>
        /// Gets the current number of people in the party.
        /// </summary>
        public int CurrentSize { get; }

        /// <summary>
        /// Gets the maximum size of the party.
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartySize"/> class.
        /// </summary>
        /// <param name="currentSize">The current number of people in the party.</param>
        /// <param name="maxSize">The maximum size of the party.</param>
        public PartySize(int currentSize, int maxSize)
        {
            this.CurrentSize = currentSize;
            this.MaxSize = maxSize;
        }
    }
}
