//
//  MessageParser.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="IMessage"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class MessageParser : AbstractTypeParser<IMessage>
{
    private static readonly Regex _messageLinkRegex = new
    (
        @"^https://(canary\.|ptb\.)?discord\.com/channels/(?<guild_id>@me|[0-9]*)/(?<channel_id>[0-9]*)/(?<message_id>[0-9]*)/?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    private readonly IOperationContext _context;
    private readonly IDiscordRestChannelAPI _channelAPI;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageParser"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="channelAPI">The channel API.</param>
    public MessageParser(IOperationContext context, IDiscordRestChannelAPI channelAPI)
    {
        _channelAPI = channelAPI;
        _context = context;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<IMessage>> TryParseAsync
    (
        string value,
        CancellationToken ct = default
    )
    {
        if (DiscordSnowflake.TryParse(value.Unmention(), out var messageID))
        {
            if (!_context.TryGetChannelID(out var channelID))
            {
                return new ParsingError<IMessage>(value, "Messages can only be parsed by ID in channels.");
            }

            return await _channelAPI.GetChannelMessageAsync(channelID, messageID.Value, ct);
        }

        var messageLinkMatch = _messageLinkRegex.Match(value);
        if (!messageLinkMatch.Success)
        {
            return new ParsingError<IMessage>(value, "Unrecognized input format.");
        }

        var channelIdRaw = messageLinkMatch.Groups["channel_id"].Value;
        if (!DiscordSnowflake.TryParse(channelIdRaw, out var channelId))
        {
            return new ParsingError<IMessage>(value, "Unrecognized input format.");
        }

        var messageIdRaw = messageLinkMatch.Groups["message_id"].Value;
        if (!DiscordSnowflake.TryParse(messageIdRaw, out var messageId))
        {
            return new ParsingError<IMessage>(value, "Unrecognized input format.");
        }

        return await _channelAPI.GetChannelMessageAsync(channelId.Value, messageId.Value, ct);
    }
}
