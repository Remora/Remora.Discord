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
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="IRole"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class RoleParser : AbstractTypeParser<IRole>
{
    private readonly ICommandContext _context;
    private readonly IDiscordRestGuildAPI _guildAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleParser"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="guildAPI">The guild API.</param>
    public RoleParser(ICommandContext context, IDiscordRestGuildAPI guildAPI)
    {
        _guildAPI = guildAPI;
        _context = context;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<IRole>> TryParseAsync(string value, CancellationToken ct = default)
    {
        if (!_context.GuildID.IsDefined(out var guildID))
        {
            return new InvalidOperationError("You're not in a guild channel, so I can't get any roles.");
        }

        var getRoles = await _guildAPI.GetGuildRolesAsync(guildID, ct);
        if (!getRoles.IsSuccess)
        {
            return Result<IRole>.FromError(getRoles);
        }

        var roles = getRoles.Entity;
        if (!DiscordSnowflake.TryParse(value.Unmention(), out var roleID))
        {
            // Try a name-based lookup
            var roleByName = roles.FirstOrDefault(r => r.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            return roleByName is not null
                ? Result<IRole>.FromSuccess(roleByName)
                : new ParsingError<IRole>(value.Unmention());
        }

        var role = roles.FirstOrDefault(r => r.ID.Equals(roleID));

        return role is not null
            ? Result<IRole>.FromSuccess(role)
            : new ParsingError<IRole>("No role with that ID could be found.");
    }
}
