//
//  GuildMemberParser.cs
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
/// Parses instances of <see cref="IGuildMember"/> and <see cref="IPartialGuildMember"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class GuildMemberParser : AbstractTypeParser<IGuildMember>, ITypeParser<IPartialGuildMember>
{
    private readonly ICommandContext _context;
    private readonly IDiscordRestGuildAPI _guildAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildMemberParser"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="guildAPI">The guild API.</param>
    public GuildMemberParser(ICommandContext context, IDiscordRestGuildAPI guildAPI)
    {
        _guildAPI = guildAPI;
        _context = context;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<IGuildMember>> TryParseAsync
    (
        string value,
        CancellationToken ct = default
    )
    {
        if (!DiscordSnowflake.TryParse(value.Unmention(), out var guildMemberID))
        {
            return new ParsingError<IGuildMember>(value, "Unrecognized input format.");
        }

        var guildID = _context switch
        {
            IInteractionCommandContext ix => ix.Interaction.GuildID,
            ITextCommandContext tx => tx.GuildID,
            _ => throw new NotSupportedException()
        };

        if (!guildID.HasValue)
        {
            return new InvalidOperationError("Guild members cannot be parsed outside of guild channels.");
        }

        return await _guildAPI.GetGuildMemberAsync(guildID.Value, guildMemberID.Value, ct);
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialGuildMember>> ITypeParser<IPartialGuildMember>.TryParseAsync(IReadOnlyList<string> tokens, CancellationToken ct)
    {
        return (await (this as ITypeParser<IGuildMember>).TryParseAsync(tokens, ct)).Map(a => a as IPartialGuildMember);
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialGuildMember>> ITypeParser<IPartialGuildMember>.TryParseAsync(string token, CancellationToken ct)
    {
        _ = DiscordSnowflake.TryParse(token.Unmention(), out var guildMemberID);
        if (guildMemberID is null)
        {
            return new ParsingError<IPartialGuildMember>(token, "Unrecognized input format.");
        }

        var resolvedGuildMember = GetResolvedGuildMemberOrDefault(guildMemberID.Value);
        return resolvedGuildMember is null
            ? (await (this as ITypeParser<IGuildMember>).TryParseAsync(token, ct)).Map(a => a as IPartialGuildMember)
            : Result<IPartialGuildMember>.FromSuccess(resolvedGuildMember);
    }

    private IPartialGuildMember? GetResolvedGuildMemberOrDefault(Snowflake guildMemberID)
    {
        if (_context is not InteractionContext injectionContext)
        {
            return null;
        }

        if (!injectionContext.Interaction.Data.IsDefined(out var data))
        {
            return null;
        }

        var resolvedData = data.Match(a => a.Resolved, _ => default, _ => default);
        if (!resolvedData.IsDefined(out var resolved) || !resolved.Members.IsDefined(out var guildMembers))
        {
            return null;
        }

        _ = guildMembers.TryGetValue(guildMemberID, out var guildMember);
        return guildMember;
    }
}
