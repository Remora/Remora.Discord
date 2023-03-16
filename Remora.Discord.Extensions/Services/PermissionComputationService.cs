//
//  PermissionComputationService.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Extensions.Services;

/// <inheritdoc />
public class PermissionComputationService : IPermissionComputationService
{
    private readonly ContextInjectionService _contextInjection;
    private readonly IDiscordRestGuildAPI _guildAPI;
    private readonly IDiscordRestChannelAPI _channelAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionComputationService"/> class.
    /// </summary>
    /// <param name="contextInjection">The context injection service.</param>
    /// <param name="guildAPI">The guild API.</param>
    /// <param name="channelAPI">The channel API.</param>
    public PermissionComputationService
    (
        ContextInjectionService contextInjection,
        IDiscordRestGuildAPI guildAPI,
        IDiscordRestChannelAPI channelAPI
    )
    {
        _contextInjection = contextInjection;
        _guildAPI = guildAPI;
        _channelAPI = channelAPI;
    }

    /// <inheritdoc/>
    public async Task<Result<IDiscordPermissionSet>> ComputeMemberPermissions
    (
        Snowflake userID,
        Snowflake guildID,
        Snowflake? channelID = null,
        CancellationToken ct = default
    )
    {
        var getMember = await _guildAPI.GetGuildMemberAsync(guildID, userID, ct);
        if (!getMember.IsDefined(out var member))
        {
            return Result<IDiscordPermissionSet>.FromError(getMember);
        }

        var getGuildRoles = await _guildAPI.GetGuildRolesAsync(guildID, ct);
        if (!getGuildRoles.IsDefined(out var guildRoles))
        {
            return Result<IDiscordPermissionSet>.FromError(getGuildRoles);
        }

        var everyoneRole = guildRoles.First(r => r.ID == guildID);
        var memberRoles = guildRoles.Where(r => member.Roles.Contains(r.ID)).ToList();
        IReadOnlyList<IPermissionOverwrite>? overwrites = null;

        // ReSharper disable once InvertIf
        if (channelID is not null)
        {
            var getChannel = await _channelAPI.GetChannelAsync(channelID.Value, ct);
            if (!getChannel.IsDefined(out var channel))
            {
                return Result<IDiscordPermissionSet>.FromError(getChannel);
            }

            overwrites = channel.PermissionOverwrites.OrDefault();
        }

        return Result<IDiscordPermissionSet>.FromSuccess
        (
            DiscordPermissionSet.ComputePermissions(userID, everyoneRole, memberRoles, overwrites)
        );
    }

    /// <inheritdoc />
    public Task<Result<IDiscordPermissionSet>> ComputeContextualMemberPermissions
    (
        Snowflake userID,
        bool inChannel = true,
        CancellationToken ct = default
    )
    {
        if (_contextInjection.Context is null)
        {
            throw new InvalidOperationException("Contextual permission computation requires an available context.");
        }

        var context = _contextInjection.Context;

        if (!context.TryGetGuildID(out var guildID))
        {
            throw new InvalidOperationException("Contextual permission computation requires an available guild ID.");
        }

        if (!context.TryGetChannelID(out var channelID) && inChannel)
        {
            throw new InvalidOperationException("Contextual permission computation requires an available channel ID.");
        }

        return ComputeMemberPermissions(userID, guildID.Value, channelID, ct);
    }

    /// <inheritdoc/>
    public async Task<Result<IDiscordPermissionSet>> ComputeRolePermissions
    (
        Snowflake roleID,
        Snowflake guildID,
        Snowflake? channelID = null,
        CancellationToken ct = default
    )
    {
        var getGuildRoles = await _guildAPI.GetGuildRolesAsync(guildID, ct);
        if (!getGuildRoles.IsDefined(out var guildRoles))
        {
            return Result<IDiscordPermissionSet>.FromError(getGuildRoles);
        }

        var everyoneRole = guildRoles.First(r => r.ID == guildID);
        var role = guildRoles.Single(r => r.ID == roleID);
        IReadOnlyList<IPermissionOverwrite>? overwrites = null;

        // ReSharper disable once InvertIf
        if (channelID is not null)
        {
            var getChannel = await _channelAPI.GetChannelAsync(channelID.Value, ct);
            if (!getChannel.IsDefined(out var channel))
            {
                return Result<IDiscordPermissionSet>.FromError(getChannel);
            }

            overwrites = channel.PermissionOverwrites.OrDefault();
        }

        return Result<IDiscordPermissionSet>.FromSuccess
        (
            DiscordPermissionSet.ComputePermissions(role, everyoneRole, overwrites)
        );
    }

    /// <inheritdoc />
    public Task<Result<IDiscordPermissionSet>> ComputeContextualRolePermissions
    (
        Snowflake roleID,
        bool inChannel = true,
        CancellationToken ct = default
    )
    {
        if (_contextInjection.Context is null)
        {
            throw new InvalidOperationException("Contextual permission computation requires an available context.");
        }

        var context = _contextInjection.Context;

        if (!context.TryGetGuildID(out var guildID))
        {
            throw new InvalidOperationException("Contextual permission computation requires an available guild ID.");
        }

        if (!context.TryGetChannelID(out var channelID) && inChannel)
        {
            throw new InvalidOperationException("Contextual permission computation requires an available channel ID.");
        }

        return ComputeRolePermissions(roleID, guildID.Value, channelID, ct);
    }
}
