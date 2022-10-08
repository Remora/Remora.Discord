//
//  RequestGuildMembers.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Gateway.Commands;

/// <summary>
/// Represents a command used to request guild members.
/// </summary>
/// <remarks>
/// This command has some special requirements related to the presence or absence of certain members in the data
/// payload. Please read <see cref="!:https://discord.com/developers/docs/topics/gateway#request-guild-members"/> for
/// more information before use, as misuse may cause Discord to terminate the gateway connection.
/// </remarks>
[PublicAPI]
public record RequestGuildMembers
(
    Snowflake GuildID,
    Optional<int> Limit,
    Optional<bool> Presences,
    Optional<string> Query,
    Optional<IReadOnlyList<Snowflake>> UserIDs,
    Optional<string> Nonce
) : IRequestGuildMembers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestGuildMembers"/> class.
    /// </summary>
    /// <param name="guildID">The ID of the guild to query.</param>
    /// <param name="query">
    /// The string to filter usernames on (that is, usernames should start with this string). Defaults to the empty
    /// string, which matches all members.
    /// </param>
    /// <param name="limit">
    /// The maximum number of members to return. Defaults to 0, which signifies a request for all members.
    /// </param>
    /// <param name="withPresences">true if member presence data should be included; otherwise, false.</param>
    /// <param name="nonce">A nonce to identify response chunks by.</param>
    public RequestGuildMembers
    (
        Snowflake guildID,
        string query = "",
        int limit = 0,
        Optional<bool> withPresences = default,
        Optional<string> nonce = default
    )
        : this(guildID, limit, withPresences, query, default, nonce)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestGuildMembers"/> class.
    /// </summary>
    /// <param name="guildID">The ID of the guild to query.</param>
    /// <param name="userIDs">The IDs of the users to retrieve.</param>
    /// <param name="limit">
    /// The maximum number of members to return. Defaults to 0, which signifies a request for all members.
    /// </param>
    /// <param name="withPresences">true if member presence data should be included; otherwise, false.</param>
    /// <param name="nonce">A nonce to identify response chunks by.</param>
    public RequestGuildMembers
    (
        Snowflake guildID,
        IReadOnlyList<Snowflake> userIDs,
        Optional<int> limit = default,
        Optional<bool> withPresences = default,
        Optional<string> nonce = default
    )
        : this(guildID, limit, withPresences, default, new Optional<IReadOnlyList<Snowflake>>(userIDs), nonce)
    {
    }
}
