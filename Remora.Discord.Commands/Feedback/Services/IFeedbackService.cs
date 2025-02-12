//
//  IFeedbackService.cs
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
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Themes;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Feedback.Services;

/// <summary>
/// Handles sending formatted messages to the users.
/// </summary>
public interface IFeedbackService
{
    /// <summary>
    /// Gets the theme used by the feedback service.
    /// </summary>
    IFeedbackTheme Theme { get; }

    /// <summary>
    /// Gets a value indicating whether the service, in the context of an interaction, has edited the original
    /// message.
    /// </summary>
    /// <remarks>This method always returns false in a message context.</remarks>
    bool HasEditedOriginalMessage { get; }

    /// <summary>
    /// Send an informational message.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendInfoAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send an informational message wherever is most appropriate to the current context.
    /// </summary>
    /// <remarks>
    /// This method will either create a followup message (if the context is an interaction) or a normal channel
    /// message.
    /// </remarks>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualInfoAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send an informational message to the given user as a direct message.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateInfoAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a positive, successful message.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendSuccessAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a positive, successful message wherever is most appropriate to the current context.
    /// </summary>
    /// <remarks>
    /// This method will either create a followup message (if the context is an interaction) or a normal channel
    /// message.
    /// </remarks>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualSuccessAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a positive, successful message to the given user as a direct message.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateSuccessAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a neutral message.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendNeutralAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a neutral message wherever is most appropriate to the current context.
    /// </summary>
    /// <remarks>
    /// This method will either create a followup message (if the context is an interaction) or a normal channel
    /// message.
    /// </remarks>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualNeutralAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a neutral message to the given user as a direct message.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateNeutralAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a warning message.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendWarningAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a warning message wherever is most appropriate to the current context.
    /// </summary>
    /// <remarks>
    /// This method will either create a followup message (if the context is an interaction) or a normal channel
    /// message.
    /// </remarks>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualWarningAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a warning message to the given user as a direct message.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateWarningAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a negative error message.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendErrorAsync
    (
        Snowflake channel,
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a negative error message wherever is most appropriate to the current context.
    /// </summary>
    /// <remarks>
    /// This method will either create a followup message (if the context is an interaction) or a normal channel
    /// message.
    /// </remarks>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualErrorAsync
    (
        string contents,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a negative error message to the given user as a direct message.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="contents">The contents of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateErrorAsync
    (
        Snowflake user,
        string contents,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a message.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendMessageAsync
    (
        Snowflake channel,
        FeedbackMessage message,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a contextual message.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualMessageAsync
    (
        FeedbackMessage message,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Send a private message.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateMessageAsync
    (
        Snowflake user,
        FeedbackMessage message,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends the given embed to the given channel.
    /// </summary>
    /// <param name="channel">The channel to send the embed to.</param>
    /// <param name="embed">The embed.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IMessage>> SendEmbedAsync
    (
        Snowflake channel,
        Embed embed,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends the given embed to current context.
    /// </summary>
    /// <param name="embed">The embed.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IMessage>> SendContextualEmbedAsync
    (
        Embed embed,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends the given embed to the given user in their private DM channel.
    /// </summary>
    /// <param name="user">The ID of the user to send the embed to.</param>
    /// <param name="embed">The embed.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IMessage>> SendPrivateEmbedAsync
    (
        Snowflake user,
        Embed embed,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends the given string as one or more sequential embeds, chunked into sets of 1024 characters.
    /// </summary>
    /// <param name="channel">The channel to send the embed to.</param>
    /// <param name="contents">The contents to send.</param>
    /// <param name="color">The embed colour.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContentAsync
    (
        Snowflake channel,
        string contents,
        Color color,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends the given string as one or more sequential embeds, chunked into sets of 1024 characters.
    /// </summary>
    /// <param name="contents">The contents to send.</param>
    /// <param name="color">The embed colour.</param>
    /// <param name="target">The target user to mention, if any.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualContentAsync
    (
        string contents,
        Color color,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends the given string as one or more sequential embeds to the given user over DM, chunked into sets of 1024
    /// characters.
    /// </summary>
    /// <param name="user">The ID of the user to send the content to.</param>
    /// <param name="contents">The contents to send.</param>
    /// <param name="color">The embed colour.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateContentAsync
    (
        Snowflake user,
        string contents,
        Color color,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends an unformatted message.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="content">The content of the message.</param>
    /// <param name="embeds">The embeds of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The created message.</returns>
    Task<Result<IMessage>> SendAsync
    (
        Snowflake channel,
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends an unformatted message to the current context.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    /// <param name="embeds">The embeds of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The created message.</returns>
    Task<Result<IMessage>> SendContextualAsync
    (
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Sends an unformatted message to the given user in their private DM channel.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="content">The content of the message.</param>
    /// <param name="embeds">The embeds of the message.</param>
    /// <param name="options">The message options to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>The created message.</returns>
    Task<Result<IMessage>> SendPrivateAsync
    (
        Snowflake user,
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Determines whether the feedback service has access to an interaction context.
    /// </summary>
    /// <returns>true if the feedback service has access to an interaction context; otherwise, false.</returns>
    bool HasInteractionContext();
}
