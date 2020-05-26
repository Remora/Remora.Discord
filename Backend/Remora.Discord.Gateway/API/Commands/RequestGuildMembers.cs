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
using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents a command used to request guild members.
    /// </summary>
    public class RequestGuildMembers
    {
        /// <summary>
        /// Gets the ID of the guild that members should be requested from.
        /// </summary>
        public Snowflake GuildID { get; }

        /// <summary>
        /// Gets a query string that the requested usernames should start with.
        /// </summary>
        public Optional<string> Query { get; }

        /// <summary>
        /// Gets a limiting number of users to fetch.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets a value indicating whether we want to fetch the presences of the users.
        /// </summary>
        public Optional<bool> Presences { get; }

        /// <summary>
        /// Gets a collection of user IDs that should be fetched.
        /// </summary>
        public Optional<IReadOnlyCollection<Snowflake>> UserIDs { get; }

        /// <summary>
        /// Gets a nonce (unique string) to identify the incoming guild member chunks after the request has been accepted.
        /// </summary>
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
