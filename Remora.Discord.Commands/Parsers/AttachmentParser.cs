//
//  AttachmentParser.cs
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Parsers;
using Remora.Commands.Results;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="IAttachment"/> from an interaction.
/// </summary>
[PublicAPI]
public class AttachmentParser : AbstractTypeParser<IAttachment>, ITypeParser<IPartialAttachment>
{
    private readonly IOperationContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttachmentParser"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    public AttachmentParser(IOperationContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public override ValueTask<Result<IAttachment>> TryParseAsync(string value, CancellationToken ct = default)
    {
        if (!DiscordSnowflake.TryParse(value, out var attachmentID))
        {
            return new(new ParsingError<IAttachment>(value, "Invalid attachment ID."));
        }

        var resolvedAttachment = GetResolvedAttachmentOrDefault(attachmentID.Value);
        return resolvedAttachment is not null
            ? new(Result<IAttachment>.FromSuccess(resolvedAttachment))
            : new(new ParsingError<IAttachment>(value, "No matching attachment found."));
    }

    private IAttachment? GetResolvedAttachmentOrDefault(Snowflake attachmentID)
    {
        if (_context is not IInteractionContext interactionContext)
        {
            return null;
        }

        if (!interactionContext.Interaction.Data.TryGet(out var data))
        {
            return null;
        }

        var resolvedData = data.Match(a => a.Resolved, _ => default, _ => default);
        if (!resolvedData.TryGet(out var resolved) || !resolved.Attachments.TryGet(out var attachments))
        {
            return null;
        }

        _ = attachments.TryGetValue(attachmentID, out var attachment);
        return attachment;
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialAttachment>> ITypeParser<IPartialAttachment>.TryParseAsync
    (
        IReadOnlyList<string> tokens,
        CancellationToken ct
    )
    {
        return (await (this as ITypeParser<IAttachment>).TryParseAsync(tokens, ct)).Map(a => a as IPartialAttachment);
    }

    /// <inheritdoc/>
    async ValueTask<Result<IPartialAttachment>> ITypeParser<IPartialAttachment>.TryParseAsync
    (
        string token,
        CancellationToken ct
    )
    {
        return (await (this as ITypeParser<IAttachment>).TryParseAsync(token, ct)).Map(a => a as IPartialAttachment);
    }
}
