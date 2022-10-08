//
//  IDiscordRestUserAPI.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord User API.
/// </summary>
[PublicAPI]
public interface IDiscordRestUserAPI
{
    /// <summary>
    /// Gets the user object of the requester's account.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IUser>> GetCurrentUserAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets the user with the given ID.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IUser>> GetUserAsync(Snowflake userID, CancellationToken ct = default);

    /// <summary>
    /// Modifies the current user.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="username">The new username.</param>
    /// <param name="avatar">The new avatar.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IUser>> ModifyCurrentUserAsync
    (
        Optional<string> username,
        Optional<Stream?> avatar = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the guilds the user is in.
    /// </summary>
    /// <param name="before">Get guilds before this guild ID.</param>
    /// <param name="after">Get guilds after this guild ID.</param>
    /// <param name="limit">The maximum number of guilds to get (1-200). Defaults to 200.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IPartialGuild>>> GetCurrentUserGuildsAsync
    (
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a guild member object for the current user.
    /// </summary>
    /// <remarks>
    /// Requires the "guild.members.read" OAuth" scope.
    /// </remarks>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IGuildMember>> GetCurrentUserGuildMemberAsync(Snowflake guildID, CancellationToken ct = default);

    /// <summary>
    /// Leaves the given guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> LeaveGuildAsync(Snowflake guildID, CancellationToken ct = default);

    /// <summary>
    /// Gets a list of DM channels the user has. This always returns an empty array for bots.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IChannel>>> GetUserDMsAsync
    (
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new DM channel with the given user.
    /// </summary>
    /// <param name="recipientID">The ID of the recipient.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IChannel>> CreateDMAsync
    (
        Snowflake recipientID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a list of connection objects.
    /// </summary>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IConnection>>> GetUserConnectionsAsync
    (
        CancellationToken ct = default
    );
}
