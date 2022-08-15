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
/// Parses instances of <see cref="IGuildMember"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class GuildMemberParser : AbstractTypeParser<IGuildMember>
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
            return new ParsingError<IGuildMember>(value);
        }

        if (!_context.GuildID.IsDefined(out var guildID))
        {
            return new InvalidOperationError("You're not in a guild channel, so I can't get any guild members.");
        }

        return await _guildAPI.GetGuildMemberAsync(guildID, guildMemberID.Value, ct);
    }
}
