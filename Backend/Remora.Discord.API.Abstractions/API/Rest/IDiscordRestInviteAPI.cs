//
//  IDiscordRestInviteAPI.cs
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
/// Represents the Discord Invite API.
/// </summary>
[PublicAPI]
public interface IDiscordRestInviteAPI
{
    /// <summary>
    /// Gets an invite object for the given code.
    /// </summary>
    /// <param name="inviteCode">The invite code.</param>
    /// <param name="withCounts">Whether the invite should contain approximate member counts.</param>
    /// <param name="withExpiration">Whether the invite should contain the expiration date.</param>
    /// <param name="guildScheduledEventID">The scheduled event to include with the invite.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IInvite>> GetInviteAsync
    (
        string inviteCode,
        Optional<bool> withCounts = default,
        Optional<bool> withExpiration = default,
        Optional<Snowflake> guildScheduledEventID = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given invite code.
    /// </summary>
    /// <param name="inviteCode">The invite code.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result<IInvite>> DeleteInviteAsync
    (
        string inviteCode,
        Optional<string> reason = default,
        CancellationToken ct = default
    );
}
