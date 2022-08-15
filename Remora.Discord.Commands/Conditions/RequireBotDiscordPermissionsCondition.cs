//
//  RequireBotDiscordPermissionsCondition.cs
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
using JetBrains.Annotations;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Results;
using Remora.Results;

namespace Remora.Discord.Commands.Conditions;

/// <summary>
/// Determines whether the bot fulfills a set of requirements related to Discord permissions.
/// </summary>
[PublicAPI]
public class RequireBotDiscordPermissionsCondition :
    ICondition<RequireBotDiscordPermissionsAttribute>,
    ICondition<RequireBotDiscordPermissionsAttribute, IChannel>
{
    private readonly IDiscordRestUserAPI _userAPI;
    private readonly IDiscordRestGuildAPI _guildAPI;
    private readonly IDiscordRestChannelAPI _channelAPI;
    private readonly ICommandContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBotDiscordPermissionsCondition"/> class.
    /// </summary>
    /// <param name="userAPI">The user API.</param>
    /// <param name="guildAPI">The guild API.</param>
    /// <param name="channelAPI">The channel API.</param>
    /// <param name="context">The command context.</param>
    public RequireBotDiscordPermissionsCondition
    (
        IDiscordRestUserAPI userAPI,
        IDiscordRestGuildAPI guildAPI,
        IDiscordRestChannelAPI channelAPI,
        ICommandContext context
    )
    {
        _userAPI = userAPI;
        _guildAPI = guildAPI;
        _channelAPI = channelAPI;
        _context = context;
    }

    /// <inheritdoc />
    public async ValueTask<Result> CheckAsync
    (
        RequireBotDiscordPermissionsAttribute attribute,
        CancellationToken ct = default
    )
    {
        if (!_context.GuildID.IsDefined())
        {
            return new PermissionDeniedError
            (
                "Commands executed outside of guilds may not require any permissions."
            );
        }

        var getChannel = await _channelAPI.GetChannelAsync(_context.ChannelID, ct);

        if (!getChannel.IsSuccess)
        {
            return (Result)getChannel;
        }

        var channel = getChannel.Entity;

        return await CheckAsync(attribute, channel, ct);
    }

    /// <inheritdoc />
    public async ValueTask<Result> CheckAsync
    (
        RequireBotDiscordPermissionsAttribute attribute,
        IChannel data,
        CancellationToken ct = default
    )
    {
        if (!_context.GuildID.IsDefined(out var guildID))
        {
            return new PermissionDeniedError
            (
                "Commands executed outside of guilds may not require any permissions."
            );
        }

        var getUser = await _userAPI.GetCurrentUserAsync(ct);
        if (!getUser.IsSuccess)
        {
            return (Result)getUser;
        }

        var user = getUser.Entity;

        var getGuild = await _guildAPI.GetGuildAsync(guildID, ct: ct);
        if (!getGuild.IsSuccess)
        {
            return (Result)getGuild;
        }

        var guild = getGuild.Entity;
        if (guild.OwnerID == user.ID)
        {
            // guild owner is always allowed
            return Result.FromSuccess();
        }

        var getRoles = await _guildAPI.GetGuildRolesAsync(guildID, ct);
        if (!getRoles.IsSuccess)
        {
            return (Result)getRoles;
        }

        var guildRoles = getRoles.Entity;

        var getMember = await _guildAPI.GetGuildMemberAsync(guildID, user.ID, ct);
        if (!getMember.IsSuccess)
        {
            return (Result)getMember;
        }

        var everyoneRole = guildRoles.First(x => x.ID == guildID);

        var member = getMember.Entity;
        var memberRoles = guildRoles.Where(r => member.Roles.Contains(r.ID)).ToArray();

        var channelOverwrites = data.PermissionOverwrites.IsDefined(out var overwrites)
            ? overwrites
            : Array.Empty<IPermissionOverwrite>();

        var computedPermissions = DiscordPermissionSet.ComputePermissions
        (
            user.ID,
            everyoneRole,
            memberRoles,
            channelOverwrites
        );

        if (computedPermissions.HasPermission(DiscordPermission.Administrator))
        {
            // Always allowed.
            return Result.FromSuccess();
        }

        var permissionInformation = attribute.Permissions
            .Distinct()
            .ToDictionary
            (
                p => p,
                p => computedPermissions.HasPermission(p)
            );

        var result = CheckRequirements(permissionInformation, attribute.Operator);
        if (result.IsSuccess)
        {
            return result;
        }

        if (result.Error is PermissionDeniedError permissionDeniedError)
        {
            return permissionDeniedError with
            {
                Message = "The given channel does not fulfill the permission requirements " +
                          $"({Explain(permissionInformation, attribute.Operator)})."
            };
        }

        return result;
    }

    private string Explain
    (
        IReadOnlyDictionary<DiscordPermission, bool> permissionInformation,
        LogicalOperator logicalOperator
    )
    {
        return logicalOperator switch
        {
            LogicalOperator.Not =>
                $"had disallowed permissions {string.Join(", ", permissionInformation.Where(kvp => kvp.Value).Select(kvp => kvp.Key.ToString()))}",
            LogicalOperator.And =>
                $"missing permissions {string.Join(", ", permissionInformation.Where(kvp => !kvp.Value).Select(kvp => kvp.Key.ToString()))}",
            LogicalOperator.Or =>
                $"missing one of {string.Join(", ", permissionInformation.Keys.Select(k => k.ToString()))}",
            LogicalOperator.Xor =>
                $"had {string.Join(", ", permissionInformation.Where(kvp => !kvp.Value).Select(kvp => kvp.Key.ToString()))}; only one is allowed",
            _ => throw new ArgumentOutOfRangeException(nameof(logicalOperator), logicalOperator, null)
        };
    }

    private Result CheckRequirements
    (
        IReadOnlyDictionary<DiscordPermission, bool> permissionInformation,
        LogicalOperator logicalOperator
    )
    {
        var passesCheck = logicalOperator switch
        {
            LogicalOperator.Not => permissionInformation.Values.All(v => !v),
            LogicalOperator.And => permissionInformation.Values.All(v => v),
            LogicalOperator.Or => permissionInformation.Values.Any(v => v),
            LogicalOperator.Xor => permissionInformation.Values.Aggregate
            (
                false,
                (current, value) => current ^ value
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(logicalOperator))
        };

        return passesCheck
            ? Result.FromSuccess()
            : new PermissionDeniedError(Permissions: permissionInformation.Keys.ToArray());
    }
}
