//
//  FeedbackService.cs
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
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Themes;
using Remora.Discord.Commands.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Feedback.Services;

/// <inheritdoc />
[PublicAPI]
public class FeedbackService : IFeedbackService
{
    private readonly ContextInjectionService _contextInjection;
    private readonly IDiscordRestChannelAPI _channelAPI;
    private readonly IDiscordRestUserAPI _userAPI;
    private readonly IDiscordRestInteractionAPI _interactionAPI;

    /// <inheritdoc />
    public IFeedbackTheme Theme { get; }

    /// <inheritdoc />
    public bool HasEditedOriginalMessage { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedbackService"/> class.
    /// </summary>
    /// <param name="channelAPI">The channel API.</param>
    /// <param name="userAPI">The user API.</param>
    /// <param name="contextInjection">The context injection service.</param>
    /// <param name="interactionAPI">The webhook API.</param>
    /// <param name="feedbackTheme">The feedback theme to use.</param>
    public FeedbackService
    (
        IDiscordRestChannelAPI channelAPI,
        IDiscordRestUserAPI userAPI,
        ContextInjectionService contextInjection,
        IDiscordRestInteractionAPI interactionAPI,
        IFeedbackTheme feedbackTheme
    )
    {
        _channelAPI = channelAPI;
        _userAPI = userAPI;
        _contextInjection = contextInjection;
        _interactionAPI = interactionAPI;

        this.Theme = feedbackTheme;
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendInfoAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendMessageAsync(channel, new FeedbackMessage(contents, this.Theme.Primary), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendContextualInfoAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContextualMessageAsync(new FeedbackMessage(contents, this.Theme.Primary), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendPrivateInfoAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendPrivateMessageAsync(user, new FeedbackMessage(contents, this.Theme.Primary), options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendSuccessAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendMessageAsync(channel, new FeedbackMessage(contents, this.Theme.Success), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendContextualSuccessAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContextualMessageAsync(new FeedbackMessage(contents, this.Theme.Success), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendPrivateSuccessAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendPrivateMessageAsync(user, new FeedbackMessage(contents, this.Theme.Success), options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendNeutralAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendMessageAsync(channel, new FeedbackMessage(contents, this.Theme.Secondary), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendContextualNeutralAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContextualMessageAsync(new FeedbackMessage(contents, this.Theme.Secondary), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendPrivateNeutralAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendPrivateMessageAsync(user, new FeedbackMessage(contents, this.Theme.Secondary), options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendWarningAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendMessageAsync(channel, new FeedbackMessage(contents, this.Theme.Warning), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendContextualWarningAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContextualMessageAsync(new FeedbackMessage(contents, this.Theme.Warning), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendPrivateWarningAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendPrivateMessageAsync(user, new FeedbackMessage(contents, this.Theme.Warning), options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendErrorAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendMessageAsync(channel, new FeedbackMessage(contents, this.Theme.FaultOrDanger), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendContextualErrorAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContextualMessageAsync(new FeedbackMessage(contents, this.Theme.FaultOrDanger), target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendPrivateErrorAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendPrivateMessageAsync(user, new FeedbackMessage(contents, this.Theme.FaultOrDanger), options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendMessageAsync
    (
        Snowflake channel,
        FeedbackMessage message,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContentAsync(channel, message.Message, message.Colour, target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendContextualMessageAsync
    (
        FeedbackMessage message,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContextualContentAsync(message.Message, message.Colour, target, options, ct);

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<IMessage>>> SendPrivateMessageAsync
    (
        Snowflake user,
        FeedbackMessage message,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendPrivateContentAsync(user, message.Message, message.Colour, options, ct);

    /// <inheritdoc />
    public Task<Result<IMessage>> SendEmbedAsync
    (
        Snowflake channel,
        Embed embed,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendAsync(channel, embeds: new[] { embed }, options: options, ct: ct);

    /// <inheritdoc />
    public Task<Result<IMessage>> SendContextualEmbedAsync
    (
        Embed embed,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendContextualAsync(embeds: new[] { embed }, options: options, ct: ct);

    /// <inheritdoc />
    public Task<Result<IMessage>> SendPrivateEmbedAsync
    (
        Snowflake user,
        Embed embed,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
        => SendPrivateAsync(user, embeds: new[] { embed }, options: options, ct: ct);

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IMessage>>> SendContentAsync
    (
        Snowflake channel,
        string contents,
        Color color,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        var sendResults = new List<IMessage>();
        foreach (var chunk in CreateContentChunks(target, color, contents))
        {
            var send = await SendEmbedAsync(channel, chunk, options, ct);
            if (!send.IsSuccess)
            {
                return Result<IReadOnlyList<IMessage>>.FromError(send);
            }

            sendResults.Add(send.Entity);
        }

        return sendResults;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IMessage>>> SendContextualContentAsync
    (
        string contents,
        Color color,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        var sendResults = new List<IMessage>();
        foreach (var chunk in CreateContentChunks(target, color, contents))
        {
            var send = await SendContextualEmbedAsync(chunk, options, ct);
            if (!send.IsSuccess)
            {
                return Result<IReadOnlyList<IMessage>>.FromError(send);
            }

            sendResults.Add(send.Entity);
        }

        return sendResults;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IMessage>>> SendPrivateContentAsync
    (
        Snowflake user,
        string contents,
        Color color,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        var getUserDM = await _userAPI.CreateDMAsync(user, ct);
        if (!getUserDM.IsSuccess)
        {
            return Result<IReadOnlyList<IMessage>>.FromError(getUserDM);
        }

        var dm = getUserDM.Entity;
        return await SendContentAsync(dm.ID, contents, color, null, options, ct);
    }

    /// <inheritdoc />
    public Task<Result<IMessage>> SendAsync
    (
        Snowflake channel,
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        return _channelAPI.CreateMessageAsync
        (
            channel,
            content: content,
            isTTS: options?.IsTTS ?? default,
            embeds: embeds,
            allowedMentions: options?.AllowedMentions ?? default,
            components: options?.MessageComponents ?? default,
            attachments: options?.Attachments ?? default,
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> SendContextualAsync
    (
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        if (_contextInjection.Context is null)
        {
            return new InvalidOperationError("Contextual sends require a context to be available.");
        }

        switch (_contextInjection.Context)
        {
            case MessageContext messageContext:
            {
                if (!messageContext.Message.ChannelID.TryGet(out var channelID))
                {
                    return new InvalidOperationError("Contextual sends require the channel ID to be available.");
                }

                return await SendAsync(channelID, content, embeds, options, ct);
            }
            case InteractionContext interactionContext:
            {
                var messageFlags = options?.MessageFlags ?? default;
                if (interactionContext.IsOriginalEphemeral)
                {
                    messageFlags = messageFlags.HasValue
                        ? messageFlags.Value | MessageFlags.Ephemeral
                        : MessageFlags.Ephemeral;
                }

                return interactionContext.HasRespondedToInteraction
                    ? await SendFollowupAsync(content, embeds, options, messageFlags, interactionContext, ct)
                    : await SendInteractionResponseAsync
                    (
                        content,
                        embeds,
                        options,
                        messageFlags,
                        interactionContext,
                        ct
                    );
            }
            default:
            {
                throw new InvalidOperationException();
            }
        }
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> SendPrivateAsync
    (
        Snowflake user,
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        var getUserDM = await _userAPI.CreateDMAsync(user, ct);
        if (!getUserDM.IsSuccess)
        {
            return Result<IMessage>.FromError(getUserDM);
        }

        var dm = getUserDM.Entity;

        return await SendAsync(dm.ID, content, embeds, options, ct);
    }

    /// <inheritdoc />
    public bool HasInteractionContext() => _contextInjection.Context is InteractionContext;

    /// <summary>
    /// Sends an interaction response.
    /// </summary>
    /// <param name="content">The contents of the message to send.</param>
    /// <param name="embeds">The embeds to send.</param>
    /// <param name="options">The feedback message options.</param>
    /// <param name="messageFlags">The message flags to use.</param>
    /// <param name="interactionContext">The interaction context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The created message.</returns>
    private async Task<Result<IMessage>> SendInteractionResponseAsync
    (
        Optional<string> content,
        Optional<IReadOnlyList<IEmbed>> embeds,
        FeedbackMessageOptions? options,
        Optional<MessageFlags> messageFlags,
        InteractionContext interactionContext,
        CancellationToken ct = default
    )
    {
        var callbackData = new InteractionMessageCallbackData
        (
            Content: content,
            IsTTS: options?.IsTTS ?? default,
            Embeds: embeds,
            AllowedMentions: options?.AllowedMentions ?? default,
            Components: options?.MessageComponents ?? default,
            Flags: messageFlags
        );

        var result = await _interactionAPI.CreateInteractionResponseAsync
        (
            interactionContext.Interaction.ID,
            interactionContext.Interaction.Token,
            new InteractionResponse(InteractionCallbackType.ChannelMessageWithSource, new(callbackData)),
            options?.Attachments ?? default,
            ct
        );

        if (!result.IsSuccess)
        {
            return Result<IMessage>.FromError(result);
        }

        var getOriginalMessage = await _interactionAPI.GetOriginalInteractionResponseAsync
        (
            interactionContext.Interaction.ApplicationID,
            interactionContext.Interaction.Token,
            ct
        );

        if (!getOriginalMessage.IsSuccess)
        {
            return getOriginalMessage;
        }

        var message = getOriginalMessage.Entity;
        if (this.HasEditedOriginalMessage)
        {
            return Result<IMessage>.FromSuccess(message);
        }

        this.HasEditedOriginalMessage = true;
        return Result<IMessage>.FromSuccess(message);
    }

    /// <summary>
    /// Sends a followup message.
    /// </summary>
    /// <param name="content">The contents of the message to send.</param>
    /// <param name="embeds">The embeds to send.</param>
    /// <param name="options">The feedback message options.</param>
    /// <param name="messageFlags">The message flags to use.</param>
    /// <param name="interactionContext">The interaction context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The created message.</returns>
    private async Task<Result<IMessage>> SendFollowupAsync
    (
        Optional<string> content,
        Optional<IReadOnlyList<IEmbed>> embeds,
        FeedbackMessageOptions? options,
        Optional<MessageFlags> messageFlags,
        InteractionContext interactionContext,
        CancellationToken ct = default
    )
    {
        var result = await _interactionAPI.CreateFollowupMessageAsync
        (
            interactionContext.Interaction.ApplicationID,
            interactionContext.Interaction.Token,
            content: content,
            isTTS: options?.IsTTS ?? default,
            embeds: embeds,
            allowedMentions: options?.AllowedMentions ?? default,
            components: options?.MessageComponents ?? default,
            flags: messageFlags,
            attachments: options?.Attachments ?? default,
            ct: ct
        );

        if (!result.IsSuccess)
        {
            return result;
        }

        if (this.HasEditedOriginalMessage)
        {
            return result;
        }

        this.HasEditedOriginalMessage = true;
        return result;
    }

    /// <summary>
    /// Creates a feedback embed.
    /// </summary>
    /// <param name="target">The invoking mentionable.</param>
    /// <param name="color">The colour of the embed.</param>
    /// <param name="contents">The contents of the embed.</param>
    /// <returns>A feedback embed.</returns>
    [Pure]
    private static Embed CreateFeedbackEmbed(Snowflake? target, Color color, string contents)
    {
        if (target is null)
        {
            return new Embed { Colour = color } with { Description = contents };
        }

        return new Embed { Colour = color } with { Description = $"<@{target}> | {contents}" };
    }

    /// <summary>
    /// Chunks an input string into one or more embeds. Discord places an internal limit on embed lengths of 2048
    /// characters, and we collapse that into 1024 for readability's sake.
    /// </summary>
    /// <param name="target">The target user, if any.</param>
    /// <param name="color">The color of the embed.</param>
    /// <param name="contents">The complete contents of the message.</param>
    /// <returns>The chunked embeds.</returns>
    [Pure]
    private IEnumerable<Embed> CreateContentChunks(Snowflake? target, Color color, string contents)
    {
        // Sometimes the content is > 2048 in length. We'll chunk it into embeds of 1024 here.
        if (contents.Length < 1024)
        {
            yield return CreateFeedbackEmbed(target, color, contents.Trim());
            yield break;
        }

        var words = contents.Split(' ');
        var messageBuilder = new StringBuilder();
        foreach (var word in words)
        {
            if (messageBuilder.Length >= 1024)
            {
                yield return CreateFeedbackEmbed(target, color, messageBuilder.ToString().Trim());
                messageBuilder.Clear();
            }

            messageBuilder.Append(word);
            messageBuilder.Append(' ');
        }

        if (messageBuilder.Length > 0)
        {
            yield return CreateFeedbackEmbed(target, color, messageBuilder.ToString().Trim());
        }
    }
}
