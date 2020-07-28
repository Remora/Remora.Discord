//
//  GatewayEndpoint.cs
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

using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects.Gateway
{
    /// <summary>
    /// Represents a gateway endpoint.
    /// </summary>
    public class GatewayEndpoint : IGatewayEndpoint
    {
        /// <inheritdoc />
        public string Url { get; }

        /// <inheritdoc />
        public Optional<int> Shards { get; }

        /// <inheritdoc />
        public Optional<ISessionStartLimit> SessionStartLimit { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayEndpoint"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="shards">The suggested. shard count.</param>
        /// <param name="sessionStartLimit">The session start limit.</param>
        public GatewayEndpoint(string url, Optional<int> shards, Optional<ISessionStartLimit> sessionStartLimit)
        {
            this.Url = url;
            this.Shards = shards;
            this.SessionStartLimit = sessionStartLimit;
        }
    }
}
