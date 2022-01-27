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

    /// <inheritdoc/>
    public override async ValueTask<Result<IAttachment>> TryParseAsync(string token, CancellationToken ct = default)
    {
        if (_context is not InteractionContext interactionContext)
        {
            return Result<IAttachment>.FromError(new InvalidOperationError("Cannot parse attachments outside the context of an interaction."));
        }

        if (!Snowflake.TryParse(token, out var attachmentID))
        {
            return Result<IAttachment>.FromError(new ParsingError<IAttachment>(token, "Invalid attachment ID."));
        }

        if (!interactionContext.Data.Resolved.IsDefined(out var resolvedData))
        {
            return Result<IAttachment>.FromError(new ParsingError<IAttachment>(token, "Cannot parse attachments without resolved data."));
        }

        if (!resolvedData.Attachments.IsDefined(out var resolvedAttachments))
        {
            return Result<IAttachment>.FromError(new ParsingError<IAttachment>(token, "Cannot parse attachments without resolved attachments."));
        }

        if (!resolvedAttachments.TryGetValue(attachmentID.Value, out var resolvedAttachment))
        {
            return Result<IAttachment>.FromError(new InvalidOperationError($"Attachment with ID {attachmentID} present in options, but not in resolved attachments."));
        }

        return Result<IAttachment>.FromSuccess(resolvedAttachment);
    }
}
