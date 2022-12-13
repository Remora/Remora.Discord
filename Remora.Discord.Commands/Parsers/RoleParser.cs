//
//  RoleParser.cs
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
using Remora.Commands.Parsers;
using Remora.Commands.Results;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="IRole"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class RoleParser : AbstractTypeParser<IRole>, ITypeParser<IPartialRole>
{
    private readonly IOperationContext _context;
    private readonly IDiscordRestGuildAPI _guildAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleParser"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="guildAPI">The guild API.</param>
    public RoleParser(IOperationContext context, IDiscordRestGuildAPI guildAPI)
    {
        _guildAPI = guildAPI;
        _context = context;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<IRole>> TryParseAsync(string value, CancellationToken ct = default)
    {
        // Check for locally resolved data first
        _ = DiscordSnowflake.TryParse(value.Unmention(), out var roleID);
        if (roleID is not null)
        {
            var resolvedRole = GetResolvedRoleOrDefault(roleID.Value);
            if (resolvedRole is not null)
            {
                return Result<IRole>.FromSuccess(resolvedRole);
            }
        }

        var guildID = _context switch
        {
            IInteractionContext ix => ix.Interaction.GuildID,
            IMessageContext tx => tx.GuildID,
            _ => throw new NotSupportedException()
        };

        // If there's nothing available, query the system
        if (!guildID.HasValue)
        {
            return new InvalidOperationError("Roles cannot be parsed outside of guild channels.");
        }

        var getRoles = await _guildAPI.GetGuildRolesAsync(guildID.Value, ct);
        if (!getRoles.IsSuccess)
        {
            return Result<IRole>.FromError(getRoles);
        }

        var roles = getRoles.Entity;

        var role = roleID is null
                ? roles.FirstOrDefault(r => r.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
                : roles.FirstOrDefault(r => r.ID.Equals(roleID));

        return role is not null
            ? Result<IRole>.FromSuccess(role)
            : new ParsingError<IRole>(value, "No matching role found.");
    }

    private IRole? GetResolvedRoleOrDefault(Snowflake roleID)
    {
        if (_context is not IInteractionContext interactionContext)
        {
            return null;
        }

        if (!interactionContext.Interaction.Data.IsDefined(out var data))
        {
            return null;
        }

        var resolvedData = data.Match(a => a.Resolved, _ => default, _ => default);
        if (!resolvedData.IsDefined(out var resolved) || !resolved.Roles.IsDefined(out var roles))
        {
            return null;
        }

        _ = roles.TryGetValue(roleID, out var role);
        return role;
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialRole>> ITypeParser<IPartialRole>.TryParseAsync
    (
        IReadOnlyList<string> tokens,
        CancellationToken ct
    )
    {
        return (await (this as ITypeParser<IRole>).TryParseAsync(tokens, ct)).Map(a => a as IPartialRole);
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialRole>> ITypeParser<IPartialRole>.TryParseAsync(string token, CancellationToken ct)
    {
        return (await (this as ITypeParser<IRole>).TryParseAsync(token, ct)).Map(a => a as IPartialRole);
    }
}
