//
//  IDiscordRestAuditLogAPI.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Audit Log API.
/// </summary>
[PublicAPI]
public interface IDiscordRestAuditLogAPI
{
    /// <summary>
    /// Gets an audit log page for the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="userID">The ID of the user to filter on.</param>
    /// <param name="actionType">The action type to filter on.</param>
    /// <param name="before">The ID of the audit log entry to limit searches before.</param>
    /// <param name="limit">The number of log entries to limit the request to.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IAuditLog>> GetAuditLogAsync
    (
        Snowflake guildID,
        Optional<Snowflake> userID = default,
        Optional<AuditLogEvent> actionType = default,
        Optional<Snowflake> before = default,
        Optional<byte> limit = default,
        CancellationToken ct = default
    );
}
