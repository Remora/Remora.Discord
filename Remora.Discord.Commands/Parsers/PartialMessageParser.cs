//
//  PartialMessageParser.cs
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
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="IPartialMessage"/> from command-line inputs.
/// </summary>
[PublicAPI]
public class PartialMessageParser : AbstractTypeParser<IPartialMessage>
{
    private readonly IInteractionContext? _context;
    private readonly ITypeParser<IMessage> _messageParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartialMessageParser"/> class.
    /// </summary>
    /// <param name="context">The interaction context, if available. </param>
    /// <param name="messageParser">The message parser to delegate to, if necessary.</param>
    public PartialMessageParser(IInteractionContext? context, ITypeParser<IMessage> messageParser)
    {
        _context = context;
        _messageParser = messageParser;
    }

    /// <inheritdoc />
    public override async ValueTask<Result<IPartialMessage>> TryParseAsync
    (
        string value,
        CancellationToken ct = default
    )
    {
        var resolvedValue = ResolveMessageFromContext(value);

        return resolvedValue is not null ? Result<IPartialMessage>.FromSuccess(resolvedValue) : (await _messageParser.TryParseAsync(value, ct)).Map<IPartialMessage>(m => m);
    }

    private IPartialMessage? ResolveMessageFromContext(string value)
    {
        if (_context is null)
        {
            return null;
        }

        if (!DiscordSnowflake.TryParse(value.Unmention(), out var messageID))
        {
            return null;
        }

        if (!_context.Interaction.Data.TryGet(out var interactionData) || interactionData.TryPickT0(out var strongData, out _))
        {
            return null;
        }

        if (!strongData.Resolved.TryGet(out var resolved) || !resolved.Messages.TryGet(out var messages))
        {
            return null;
        }

        _ = messages.TryGetValue(messageID.Value, out var message);
        return message;
    }
}
