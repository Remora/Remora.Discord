//
//  IRequestGuildMembers.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Commands;

/// <summary>
/// Represents a command used to request guild members.
/// </summary>
[PublicAPI]
public interface IRequestGuildMembers : IGatewayCommand
{
    /// <summary>
    /// Gets the ID of the guild that members should be requested from.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets a query string that the requested usernames should start with.
    /// </summary>
    Optional<string> Query { get; }

    /// <summary>
    /// Gets a limiting number of users to fetch.
    /// </summary>
    Optional<int> Limit { get; }

    /// <summary>
    /// Gets a value indicating whether we want to fetch the presences of the users.
    /// </summary>
    Optional<bool> Presences { get; }

    /// <summary>
    /// Gets a collection of user IDs that should be fetched.
    /// </summary>
    Optional<IReadOnlyList<Snowflake>> UserIDs { get; }

    /// <summary>
    /// Gets a nonce (unique string) to identify the incoming guild member chunks after the request has been accepted.
    /// </summary>
    Optional<string> Nonce { get; }
}
