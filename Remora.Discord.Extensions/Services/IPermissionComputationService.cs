//
//  IPermissionComputationService.cs
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

using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Extensions.Services;

/// <summary>
/// Computes permission sets based on available data.
/// </summary>
[PublicAPI]
public interface IPermissionComputationService
{
    /// <summary>
    /// Computes the effective permissions for the user with the given ID.
    /// </summary>
    /// <remarks>
    /// <paramref name="channelID"/> controls whether permission overwrite in any particular channel are taken into
    /// account. If it is omitted, the computed permissions are valid anywhere in the server where a permission
    /// overwrite does not otherwise modify the permissions.
    ///
    /// If you want to compute effective permissions for some kind of user action, it's likely that you want to set this
    /// parameter to an appropriate value.
    /// </remarks>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="guildID">The ID of the guild the user is in.</param>
    /// <param name="channelID">The ID of the channel the user is in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The computed permissions.</returns>
    Task<Result<IDiscordPermissionSet>> ComputeMemberPermissions
    (
        Snowflake userID,
        Snowflake guildID,
        Snowflake? channelID = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Computes the effective permissions for the user with the given ID in the context of the current operation.
    /// </summary>
    /// <remarks>
    /// This method uses the guild and channel ID from an available <see cref="IOperationContext"/>.
    /// </remarks>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="inChannel">Whether the computation should use the channel ID from the context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no context is available or it does not have the required information.
    /// </exception>
    /// <returns>The computed permissions.</returns>
    Task<Result<IDiscordPermissionSet>> ComputeContextualMemberPermissions
    (
        Snowflake userID,
        bool inChannel = true,
        CancellationToken ct = default
    );

    /// <summary>
    /// Computes the effective permissions for the role with the given ID.
    /// </summary>
    /// <remarks>
    /// <paramref name="channelID"/> controls whether permission overwrite in any particular channel are taken into
    /// account. If it is omitted, the computed permissions are valid anywhere in the server where a permission
    /// overwrite does not otherwise modify the permissions.
    ///
    /// If you want to compute effective permissions for some kind of user action, it's likely that you want to set this
    /// parameter to an appropriate value.
    /// </remarks>
    /// <param name="roleID">The ID of the role.</param>
    /// <param name="guildID">The ID of the guild the user is in.</param>
    /// <param name="channelID">The ID of the channel the user is in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The computed permissions.</returns>
    Task<Result<IDiscordPermissionSet>> ComputeRolePermissions
    (
        Snowflake roleID,
        Snowflake guildID,
        Snowflake? channelID = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Computes the effective permissions for the role with the given ID in the context of the current operation.
    /// </summary>
    /// <remarks>
    /// This method uses the guild and channel ID from an available <see cref="IOperationContext"/>.
    /// </remarks>
    /// <param name="roleID">The ID of the role.</param>
    /// <param name="inChannel">Whether the computation should use the channel ID from the context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no context is available or it does not have the required information.
    /// </exception>
    /// <returns>The computed permissions.</returns>
    Task<Result<IDiscordPermissionSet>> ComputeContextualRolePermissions
    (
        Snowflake roleID,
        bool inChannel = true,
        CancellationToken ct = default
    );
}
