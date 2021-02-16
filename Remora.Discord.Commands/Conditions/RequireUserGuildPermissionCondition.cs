//
//  RequireUserGuildPermissionCondition.cs
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Results;
using Remora.Results;

namespace Remora.Discord.Commands.Conditions
{
    /// <summary>
    /// Checks required Guild permissions before allowing execution.
    ///
    /// <remarks>Fails if the command is executed outside of a Guild. It should be used together with <see cref=""./></remarks>
    /// </summary>
    public class RequireUserGuildPermissionCondition : ICondition<RequireUserGuildPermissionAttribute>
    {
        private readonly ICommandContext _context;
        private readonly IDiscordRestChannelAPI _channelApi;
        private readonly IDiscordRestGuildAPI _guildApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireUserGuildPermissionCondition"/> class.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="guildApi">The guild API.</param>
        /// <param name="channelApi">The channel API.</param>
        public RequireUserGuildPermissionCondition(ICommandContext context, IDiscordRestGuildAPI guildApi, IDiscordRestChannelAPI channelApi)
        {
            _context = context;
            _guildApi = guildApi;
            _channelApi = channelApi;
        }

        /// <inheritdoc />
        public async ValueTask<Result> CheckAsync(RequireUserGuildPermissionAttribute attribute, CancellationToken ct)
        {
            var getChannel = await _channelApi.GetChannelAsync(_context.ChannelID, ct);
            if (!getChannel.IsSuccess)
            {
                return Result.FromError(getChannel);
            }
            var channel = getChannel.Entity;
            if (!channel.GuildID.HasValue)
            {
                return new ConditionNotSatisfiedError("Command requires a guild permission but was executed outside of a guild.");
            }

            var guildId = channel.GuildID.Value;

            var getGuildMember = await _guildApi.GetGuildMemberAsync(guildId, _context.User.ID, ct);
            if (!getGuildMember.IsSuccess)
            {
                return Result.FromError(getGuildMember);
            }

            var getGuildRoles = await _guildApi.GetGuildRolesAsync(guildId, ct);
            if (!getGuildRoles.IsSuccess)
            {
                return Result.FromError(getGuildRoles);
            }

            var guildRoles = getGuildRoles.Entity;
            var everyoneRole = guildRoles.FirstOrDefault(r => r.Name.Equals("@everyone"));
            if (everyoneRole is null)
            {
                return new GenericError("No @everyone role found.");
            }

            var user = getGuildMember.Entity;
            if (user is null)
            {
                return new GenericError("Executing user not found");
            }

            var getGuild = await _guildApi.GetGuildAsync(guildId, ct: ct);
            if (!getGuild.IsSuccess)
            {
                return Result.FromError(getGuild);
            }
            var guildOwnerId = getGuild.Entity.OwnerID;
            if (guildOwnerId.Equals(_context.User.ID))
            {
                return Result.FromSuccess();
            }

            var memberRoles = guildRoles.Where(r => user.Roles.Contains(r.ID)).ToList();
            IDiscordPermissionSet computedPermissions;
            if (channel.PermissionOverwrites.HasValue)
            {
                computedPermissions = DiscordPermissionSet.ComputePermissions(
                    _context.User.ID, everyoneRole, memberRoles, channel.PermissionOverwrites.Value
                );
            }
            else
            {
                computedPermissions = DiscordPermissionSet.ComputePermissions(
                    _context.User.ID, everyoneRole, memberRoles
                );
            }

            if (computedPermissions.HasPermission(DiscordPermission.Administrator))
            {
                return Result.FromSuccess();
            }

            var hasPermission = computedPermissions.HasPermission(attribute.Permission);
            return !hasPermission
                ? new ConditionNotSatisfiedError($"Guild User requesting the command does not have the required {attribute.Permission.ToString()} permission")
                : Result.FromSuccess();
        }
    }
}
