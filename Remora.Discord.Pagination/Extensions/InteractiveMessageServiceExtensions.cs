//
//  InteractiveMessageServiceExtensions.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Interactivity.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Pagination.Extensions;

/// <summary>
/// Defines extension methods to the <see cref="InteractiveMessageService"/> class.
/// </summary>
[PublicAPI]
public static class InteractiveMessageServiceExtensions
{
    /// <summary>
    /// Sends a paginated message to the given channel.
    /// </summary>
    /// <param name="interactiveMessages">The interactive message service.</param>
    /// <param name="channel">The channel.</param>
    /// <param name="sourceUser">
    /// The ID of the user that requested the paginated message. Only this user will be able to change page.
    /// </param>
    /// <param name="pages">The pages in the message.</param>
    /// <param name="appearance">The appearance options for the paginated message.</param>
    /// <param name="options">
    /// The message options to use. Any provided message components will be included in addition to the pagination's
    /// navigation buttons at the root of the component set.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<Result<IMessage>> SendPaginatedMessageAsync
    (
        this InteractiveMessageService interactiveMessages,
        Snowflake channel,
        Snowflake sourceUser,
        IReadOnlyList<Embed> pages,
        PaginatedAppearanceOptions? appearance = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        if (pages.Count == 0)
        {
            return new InvalidOperationError("No pages to send were provided.");
        }

        options ??= new FeedbackMessageOptions();
        appearance ??= PaginatedAppearanceOptions.Default;

        var data = new PaginatedMessageData(sourceUser, pages, appearance);
        var components = data.GetCurrentComponents();

        options = options with
        {
            MessageComponents = new Optional<IReadOnlyList<IMessageComponent>>
            (
                options.MessageComponents.IsDefined(out var existingComponents)
                    ? components.Concat(existingComponents).ToList()
                    : components
            )
        };

        return await interactiveMessages.SendInteractiveEmbedWithPersistentDataAsync
        (
            channel,
            pages[0] with { Footer = new EmbedFooter($"Page 1/{pages.Count}") },
            m => m.ID.ToString(),
            _ => data,
            options,
            ct
        );
    }

    /// <summary>
    /// Sends a paginated message to the current context.
    /// </summary>
    /// <param name="interactiveMessages">The interactive message service.</param>
    /// <param name="sourceUser">
    /// The ID of the user that requested the paginated message. Only this user will be able to change page.
    /// </param>
    /// <param name="pages">The pages in the message.</param>
    /// <param name="appearance">The appearance options for the paginated message.</param>
    /// <param name="options">
    /// The message options to use. Any provided message components will be included in addition to the pagination's
    /// navigation buttons at the root of the component set.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<Result<IMessage>> SendContextualPaginatedMessageAsync
    (
        this InteractiveMessageService interactiveMessages,
        Snowflake sourceUser,
        IReadOnlyList<Embed> pages,
        PaginatedAppearanceOptions? appearance = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        if (pages.Count == 0)
        {
            return new InvalidOperationError("No pages to send were provided.");
        }

        options ??= new FeedbackMessageOptions();
        appearance ??= PaginatedAppearanceOptions.Default;

        var data = new PaginatedMessageData(sourceUser, pages, appearance);
        var components = data.GetCurrentComponents();

        options = options with
        {
            MessageComponents = new Optional<IReadOnlyList<IMessageComponent>>
            (
                options.MessageComponents.IsDefined(out var existingComponents)
                    ? components.Concat(existingComponents).ToList()
                    : components
            )
        };

        return await interactiveMessages.SendInteractiveContextualEmbedWithPersistentDataAsync
        (
            pages[0] with { Footer = new EmbedFooter($"Page 1/{pages.Count}") },
            m => m.ID.ToString(),
            _ => data,
            options,
            ct
        );
    }

    /// <summary>
    /// Sends a paginated message to the given user in their private DN channel.
    /// </summary>
    /// <param name="interactiveMessages">The interactive message service.</param>
    /// <param name="user">The ID of the user to send the embed to.</param>
    /// <param name="pages">The pages in the message.</param>
    /// <param name="appearance">The appearance options for the paginated message.</param>
    /// <param name="options">
    /// The message options to use. Any provided message components will be included in addition to the pagination's
    /// navigation buttons at the root of the component set.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<Result<IMessage>> SendPrivatePaginatedMessageAsync
    (
        this InteractiveMessageService interactiveMessages,
        Snowflake user,
        IReadOnlyList<Embed> pages,
        PaginatedAppearanceOptions? appearance = null,
        FeedbackMessageOptions? options = null,
        CancellationToken ct = default
    )
    {
        if (pages.Count == 0)
        {
            return new InvalidOperationError("No pages to send were provided.");
        }

        options ??= new FeedbackMessageOptions();
        appearance ??= PaginatedAppearanceOptions.Default;

        var data = new PaginatedMessageData(user, pages, appearance);
        var components = data.GetCurrentComponents();

        options = options with
        {
            MessageComponents = new Optional<IReadOnlyList<IMessageComponent>>
            (
                options.MessageComponents.IsDefined(out var existingComponents)
                    ? components.Concat(existingComponents).ToList()
                    : components
            )
        };

        return await interactiveMessages.SendInteractiveContextualEmbedWithPersistentDataAsync
        (
            pages[0] with { Footer = new EmbedFooter($"Page 1/{pages.Count}") },
            m => m.ID.ToString(),
            _ => data,
            options,
            ct
        );
    }
}
