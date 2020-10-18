//
//  GuildMembersChunk.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc />
    [PublicAPI]
    public class GuildMembersChunk : IGuildMembersChunk
    {
        /// <inheritdoc/>
        public Snowflake GuildID { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IGuildMember> Members { get; }

        /// <inheritdoc/>
        public int ChunkIndex { get; }

        /// <inheritdoc/>
        public int ChunkCount { get; }

        /// <inheritdoc/>
        public Optional<IReadOnlyList<Snowflake>> NotFound { get; }

        /// <inheritdoc/>
        public Optional<IReadOnlyList<IPresence>> Presences { get; }

        /// <inheritdoc/>
        public Optional<string> Nonce { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildMembersChunk"/> class.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="members">The members in the chunk.</param>
        /// <param name="chunkIndex">The index of the chunk.</param>
        /// <param name="chunkCount">The total number of expected chunks.</param>
        /// <param name="notFound">Snowflakes that weren't found, or were otherwise invalid.</param>
        /// <param name="presences">The presences of the members in the chunk.</param>
        /// <param name="nonce">A unique request identifier.</param>
        public GuildMembersChunk
        (
            Snowflake guildID,
            IReadOnlyList<IGuildMember> members,
            int chunkIndex,
            int chunkCount,
            Optional<IReadOnlyList<Snowflake>> notFound,
            Optional<IReadOnlyList<IPresence>> presences,
            Optional<string> nonce
        )
        {
            this.GuildID = guildID;
            this.Members = members;
            this.ChunkIndex = chunkIndex;
            this.ChunkCount = chunkCount;
            this.NotFound = notFound;
            this.Presences = presences;
            this.Nonce = nonce;
        }
    }
}
