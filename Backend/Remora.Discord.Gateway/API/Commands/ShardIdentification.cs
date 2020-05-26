//
//  ShardIdentification.cs
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

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents an identification set for a sharded connection.
    /// </summary>
    [JsonConverter(typeof(ShardIdentificationConverter))]
    internal sealed class ShardIdentification
    {
        /// <summary>
        /// Gets the ID of this shard.
        /// </summary>
        public int ShardID { get; }

        /// <summary>
        /// Gets the total number of shards.
        /// </summary>
        public int ShardCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShardIdentification"/> class.
        /// </summary>
        /// <param name="shardID">The shard ID.</param>
        /// <param name="shardCount">The shard count.</param>
        public ShardIdentification(int shardID, int shardCount)
        {
            this.ShardID = shardID;
            this.ShardCount = shardCount;
        }
    }
}
