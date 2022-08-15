//
//  IDiscordRestInteractionAPI.cs
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
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Interaction API.
/// </summary>
[PublicAPI]
public interface IDiscordRestInteractionAPI
{
    /// <summary>
    /// Creates a response to an interaction from the gateway.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="interactionID">The ID of the interaction.</param>
    /// <param name="interactionToken">The interaction token.</param>
    /// <param name="response">The response.</param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IAttachment"/>. If this request edits the original message, then any attachments not mentioned in
    /// this parameter will be deleted.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result> CreateInteractionResponseAsync
    (
        Snowflake interactionID,
        string interactionToken,
        IInteractionResponse response,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the message object of the original interaction response.
    /// </summary>
    /// <param name="applicationID">The ID of the application.</param>
    /// <param name="interactionToken">The interaction token.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> GetOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string interactionToken,
        CancellationToken ct = default
    );

    /// <summary>
    /// Edits the initial interaction response.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="token">The interaction token.</param>
    /// <param name="content">The new content, if any.</param>
    /// <param name="embeds">The new embeds, if any.</param>
    /// <param name="allowedMentions">The new allowed mentions, if any.</param>
    /// <param name="components">The components, if any.</param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IPartialAttachment"/>. If this request edits the original message, then any attachments not
    /// mentioned in this parameter will be deleted.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> EditOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string token,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the original interaction response.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="token">The interaction token.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteOriginalInteractionResponseAsync
    (
        Snowflake applicationID,
        string token,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a followup message.
    /// </summary>
    /// <remarks>
    /// At least one of <paramref name="content"/>, <paramref name="embeds"/>, or <paramref name="attachments"/> must be
    /// present.
    ///
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="token">The interaction token.</param>
    /// <param name="content">The content of the message.</param>
    /// <param name="isTTS">Whether this message is a TTS message.</param>
    /// <param name="embeds">The embeds in the message.</param>
    /// <param name="allowedMentions">The set of allowed mentions of the message.</param>
    /// <param name="components">The components that should be included with the message.</param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IPartialAttachment"/>. If this request edits the original message, then any attachments not
    /// mentioned in this parameter will be deleted.
    /// </param>
    /// <param name="flags">The message flags to use.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> CreateFollowupMessageAsync
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
    );

    /// <summary>
    /// Gets a followup message associated with the given interaction.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="token">The interaction token.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> GetFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Edits an interaction followup message.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="token">The interaction token.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="content">The new content, if any.</param>
    /// <param name="embeds">The new embeds, if any.</param>
    /// <param name="allowedMentions">The new allowed mentions, if any.</param>
    /// <param name="components">The components, if any.</param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IPartialAttachment"/>. If this request edits the original message, then any attachments not
    /// mentioned in this parameter will be deleted.
    /// </param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> EditFollowupMessageAsync
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
    );

    /// <summary>
    /// Deletes an interaction followup message.
    /// </summary>
    /// <param name="applicationID">The ID of the bot application.</param>
    /// <param name="token">The interaction token.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result> DeleteFollowupMessageAsync
    (
        Snowflake applicationID,
        string token,
        Snowflake messageID,
        CancellationToken ct = default
    );
}
