//
//  ResponseTrackingInteractionAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Services;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Commands.API;

/// <summary>
/// Implements a tracking system for whether an interaction has been responded to.
/// </summary>
internal partial class ResponseTrackingInteractionAPI : IDiscordRestInteractionAPI, IRestCustomizable
{
    private readonly ContextInjectionService? _contextInjector;
    private readonly IDiscordRestInteractionAPI _actual;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseTrackingInteractionAPI"/> class.
    /// </summary>
    /// <param name="actual">The actual API instance.</param>
    /// <param name="provider">The service provider.</param>
    public ResponseTrackingInteractionAPI
    (
        IDiscordRestInteractionAPI actual,
        IServiceProvider provider
    )
    {
        _actual = actual;

        // TODO: Improve this with a proper scope check once I figure out how to do that
        try
        {
            _contextInjector = provider.GetRequiredService<ContextInjectionService>();
        }
        catch (InvalidOperationException)
        {
        }
    }

    /// <inheritdoc />
    public async Task<Result> CreateInteractionResponseAsync
    (
        Snowflake interactionID,
        string interactionToken,
        IInteractionResponse response,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        CancellationToken ct = default
    )
    {
        var createResponse = await _actual.CreateInteractionResponseAsync
        (
            interactionID,
            interactionToken,
            response,
            attachments,
            ct
        );

        if (_contextInjector?.Context is InteractionContext interactionContext)
        {
            // only flip this to true once, and never go back to false
            interactionContext.HasRespondedToInteraction |= createResponse.IsSuccess;
        }

        return createResponse;
    }

    /// <inheritdoc />
    public Task<Result<IMessage>> GetOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string interactionToken,
        CancellationToken ct = default
    ) =>
        _actual.GetOriginalInteractionResponseAsync(applicationID, interactionToken, ct);

    /// <inheritdoc />
    public Task<Result<IMessage>> EditOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string token,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        CancellationToken ct = default
    ) =>
        _actual.EditOriginalInteractionResponseAsync
        (
            applicationID,
            token,
            content,
            embeds,
            allowedMentions,
            components,
            attachments,
            ct
        );

    /// <inheritdoc />
    public Task<Result> DeleteOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string token,
        CancellationToken ct = default
    ) =>
        _actual.DeleteOriginalInteractionResponseAsync(applicationID, token, ct);

    /// <inheritdoc />
    public Task<Result<IMessage>> CreateFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Optional<string> content = default,
        Optional<bool> isTTS = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        Optional<IAllowedMentions> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        Optional<MessageFlags> flags = default,
        CancellationToken ct = default
    ) =>
        _actual.CreateFollowupMessageAsync
        (
            applicationID,
            token,
            content,
            isTTS,
            embeds,
            allowedMentions,
            components,
            attachments,
            flags,
            ct
        );

    /// <inheritdoc />
    public Task<Result<IMessage>> GetFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        CancellationToken ct = default
    ) =>
        _actual.GetFollowupMessageAsync(applicationID, token, messageID, ct);

    /// <inheritdoc />
    public Task<Result<IMessage>> EditFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        CancellationToken ct = default
    ) =>
        _actual.EditFollowupMessageAsync
        (
            applicationID,
            token,
            messageID,
            content,
            embeds,
            allowedMentions,
            components,
            attachments,
            ct
        );

    /// <inheritdoc />
    public Task<Result> DeleteFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        CancellationToken ct = default
    ) =>
        _actual.DeleteFollowupMessageAsync(applicationID, token, messageID, ct);
}
