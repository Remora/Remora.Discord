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

using System.Threading;
using System.Threading.Tasks;
using Remora.Commands.Parsers;
using Remora.Commands.Results;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Parsers;

/// <summary>
/// Parses instances of <see cref="IAttachment"/> from an interaction.
/// </summary>
public class AttachmentParser : AbstractTypeParser<IAttachment>
{
    private readonly ICommandContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttachmentParser"/> class.
    /// </summary>
    /// <param name="context">The command context.</param>
    public AttachmentParser(ICommandContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public override ValueTask<Result<IAttachment>> TryParseAsync(string token, CancellationToken ct = default)
    {
        if (_context is not InteractionContext interactionContext)
        {
            return new(new InvalidOperationError("Cannot parse attachments outside the context of an interaction."));
        }

        if (!Snowflake.TryParse(token, out var attachmentID))
        {
            return new(new ParsingError<IAttachment>(token, "Invalid attachment ID."));
        }

        if (!interactionContext.Data.TryPickT0(out var commandData, out _))
        {
            return new(new ParsingError<IAttachment>(token, "Cannot parse attachments without command data."));
        }

        if (!commandData.Resolved.IsDefined(out var resolvedData))
        {
            return new(new ParsingError<IAttachment>(token, "Cannot parse attachments without resolved data."));
        }

        if (!resolvedData.Attachments.IsDefined(out var resolvedAttachments))
        {
            return new(new ParsingError<IAttachment>(token, "Cannot parse attachments without resolved attachments."));
        }

        if (!resolvedAttachments.TryGetValue(attachmentID.Value, out var resolvedAttachment))
        {
            return new(new InvalidOperationError($"Attachment with ID {attachmentID} present in options, but not in resolved attachments."));
        }

        return new(Result<IAttachment>.FromSuccess(resolvedAttachment));
    }
}
