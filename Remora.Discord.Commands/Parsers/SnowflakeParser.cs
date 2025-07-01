//
//  SnowflakeParser.cs
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

using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Parsers;
using Remora.Commands.Results;
using Remora.Discord.API;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="Snowflake"/>s.
/// </summary>
[PublicAPI]
public class SnowflakeParser : AbstractTypeParser<Snowflake>
{
    private static readonly Regex _channelLinkRegex = new
    (
        @"^https://(canary\.|ptb\.)?discord\.com/channels/(?<guild_id>@me|[0-9]*)/(?<channel_id>[0-9]*)(/(?<message_id>[0-9]*))?/?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    /// <inheritdoc />
    public override ValueTask<Result<Snowflake>> TryParseAsync(string value, CancellationToken ct = default)
    {
        var channelLinkMatch = _channelLinkRegex.Match(value);
        if (channelLinkMatch.Success)
        {
            value = channelLinkMatch.Groups["message_id"].Value;

            if (string.IsNullOrEmpty(value))
            {
                value = channelLinkMatch.Groups["channel_id"].Value;
            }
        }

        return new
        (
            !DiscordSnowflake.TryParse(value.Unmention(), out var snowflake)
                ? new ParsingError<Snowflake>(value)
                : snowflake.Value
        );
    }
}
