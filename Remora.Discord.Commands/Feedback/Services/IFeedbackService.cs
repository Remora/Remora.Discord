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
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.Feedback.Services;

/// <summary>
///     Interface for a service that sends feedback messages to users.
/// </summary>
public interface IFeedbackService
{
    /// <summary>
    ///     Sends a feedback message with the given content and color in the current context, optionally mentioning a target
    ///     user.
    /// </summary>
    /// <param name="contents">The content of the feedback message.</param>
    /// <param name="color">The color of the feedback message embed.</param>
    /// <param name="target">The optional target user to mention in the feedback message.</param>
    /// <param name="options">Optional custom message options.</param>
    /// <param name="ct">Optional CancellationToken to cancel the operation.</param>
    /// <returns>A Result containing a list of sent messages, or an error if the operation failed.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendContextualContentAsync(
        string contents,
        Color color,
        Snowflake? target = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    ///     Sends a feedback message with the given content and color to the specified user via direct message.
    /// </summary>
    /// <param name="user">The target user to receive the private message.</param>
    /// <param name="contents">The content of the feedback message.</param>
    /// <param name="color">The color of the feedback message embed.</param>
    /// <param name="options">Optional custom message options.</param>
    /// <param name="ct">Optional CancellationToken to cancel the operation.</param>
    /// <returns>A Result containing a list of sent messages, or an error if the operation failed.</returns>
    Task<Result<IReadOnlyList<IMessage>>> SendPrivateContentAsync(
        Snowflake user,
        string contents,
        Color color,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    ///     Sends a message to the specified channel with optional content, embeds, and custom options.
    /// </summary>
    /// <param name="channel">The target channel to send the message.</param>
    /// <param name="content">Optional content of the message.</param>
    /// <param name="embeds">Optional list of embeds to include in the message.</param>
    /// <param name="options">Optional custom message options.</param>
    /// <param name="ct">Optional CancellationToken to cancel the operation.</param>
    /// <returns>A Result containing the sent message, or an error if the operation failed.</returns>
    Task<Result<IMessage>> SendAsync(
        Snowflake channel,
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    ///     Sends a contextual message with optional content, embeds, and custom options.
    /// </summary>
    /// <param name="content">Optional content of the message.</param>
    /// <param name="embeds">Optional list of embeds to include in the message.</param>
    /// <param name="options">Optional custom message options.</param>
    /// <param name="ct">Optional CancellationToken to cancel the operation.</param>
    /// <returns>A Result containing the sent message, or an error if the operation failed.</returns>
    Task<Result<IMessage>> SendContextualAsync(
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    ///     Sends a private message to the specified user with optional content, embeds, and custom options.
    /// </summary>
    /// <param name="user">The target user to receive the private message.</param>
    /// <param name="content">Optional content of the message.</param>
    /// <param name="embeds">Optional list of embeds to include in the message.</param>
    /// <param name="options">Optional custom message options.</param>
    /// <param name="ct">Optional CancellationToken to cancel the operation.</param>
    /// <returns>A Result containing the sent message, or an error if the operation failed.</returns>
    Task<Result<IMessage>> SendPrivateAsync(
        Snowflake user,
        Optional<string> content = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    );

    /// <summary>
    ///     Checks if the current context is an InteractionContext.
    /// </summary>
    /// <returns>True if the current context is an InteractionContext, false otherwise.</returns>
    bool HasInteractionContext();
}
