//
//  RequestGuildMembers.cs
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
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Commands
{
    /// <summary>
    /// Represents a command used to request guild members.
    /// </summary>
    public class RequestGuildMembers : IRequestGuildMembers
    {
        /// <inheritdoc />
        public Snowflake GuildID { get; }

        /// <inheritdoc />
        public Optional<string> Query { get; }

        /// <inheritdoc />
        public int Limit { get; }

        /// <inheritdoc />
        public Optional<bool> Presences { get; }

        /// <inheritdoc />
        public Optional<IReadOnlyCollection<Snowflake>> UserIDs { get; }

        /// <inheritdoc />
        public Optional<string> Nonce { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestGuildMembers"/> class.
        /// </summary>
        /// <param name="guildID">The ID of the guild to request from.</param>
        /// <param name="limit">The maximum number of users to return.</param>
        /// <param name="presences">Whether the user presences should be included.</param>
        /// <param name="query">The query string.</param>
        /// <param name="userIDs">The query IDs.</param>
        /// <param name="nonce">The identifying nonce.</param>
        public RequestGuildMembers
        (
            Snowflake guildID,
            int limit,
            Optional<bool> presences = default,
            Optional<string> query = default,
            Optional<IReadOnlyCollection<Snowflake>> userIDs = default,
            Optional<string> nonce = default)
        {
            this.GuildID = guildID;
            this.Query = query;
            this.Limit = limit;
            this.Presences = presences;
            this.UserIDs = userIDs;
            this.Nonce = nonce;
        }
    }
}
