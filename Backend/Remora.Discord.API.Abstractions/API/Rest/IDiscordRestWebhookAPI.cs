//
//  IDiscordRestWebhookAPI.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Webhook API.
/// </summary>
[PublicAPI]
public interface IDiscordRestWebhookAPI
{
    /// <summary>
    /// Creates a new webhook.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="channelID">The ID of the channel the webhook is for.</param>
    /// <param name="name">The name of the webhook.</param>
    /// <param name="avatar">The avatar of the webhook.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<IWebhook>> CreateWebhookAsync
    (
        Snowflake channelID,
        string name,
        Optional<Stream?> avatar,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the webhooks for a channel.
    /// </summary>
    /// <param name="channelID">The ID of the channel.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IWebhook>>> GetChannelWebhooksAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the webhooks for a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IWebhook>>> GetGuildWebhooksAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the webhook with the given ID.
    /// </summary>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IWebhook>> GetWebhookAsync
    (
        Snowflake webhookID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets the webhook with the given ID and token.
    /// </summary>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="token">The token.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IWebhook>> GetWebhookWithTokenAsync
    (
        Snowflake webhookID,
        string token,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given webhook.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="name">The new name of the webhook.</param>
    /// <param name="avatar">The new avatar of the webhook.</param>
    /// <param name="channelID">The new channel of the webhook.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IWebhook>> ModifyWebhookAsync
    (
        Snowflake webhookID,
        Optional<string> name = default,
        Optional<Stream?> avatar = default,
        Optional<Snowflake> channelID = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies the given webhook.
    /// </summary>
    /// <remarks>
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="token">The token for the webhook.</param>
    /// <param name="name">The new name of the webhook.</param>
    /// <param name="avatar">The new avatar of the webhook.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IWebhook>> ModifyWebhookWithTokenAsync
    (
        Snowflake webhookID,
        string token,
        Optional<string> name = default,
        Optional<Stream?> avatar = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given webhook.
    /// </summary>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteWebhookAsync
    (
        Snowflake webhookID,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes the given webhook.
    /// </summary>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="token">The token for the webhook.</param>
    /// <param name="reason">The reason to mark the action in the audit log with.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteWebhookWithTokenAsync
    (
        Snowflake webhookID,
        string token,
        Optional<string> reason = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Executes the given webhook.
    /// </summary>
    /// <remarks>
    /// At least one of <paramref name="content"/>, <paramref name="embeds"/>, or <paramref name="attachments"/> must be
    /// present.
    ///
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="token">The token for the webhook.</param>
    /// <param name="shouldWait">
    /// Whether the call should block until the server has confirmed that the message was sent.
    /// </param>
    /// <param name="content">The content of the message.</param>
    /// <param name="username">
    /// The username to use for this message. Note that Discord places some restrictions on valid usernames in order to
    /// prevent spam and abuse - check the Discord documentation for up-to-date information on allowed usernames.
    /// </param>
    /// <param name="avatarUrl">The avatar to use for this message.</param>
    /// <param name="isTTS">Whether this message is a TTS message.</param>
    /// <param name="embeds">The embeds in the message.</param>
    /// <param name="allowedMentions">The set of allowed mentions of the message.</param>
    /// <param name="threadID">Send a message to the specified thread within a webhook's channel.</param>
    /// <param name="components">
    /// The components that should be included with the message. The webhook must be application-owned to use this
    /// parameter.
    /// </param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IPartialAttachment"/>. If this request edits the original message, then any attachments not
    /// mentioned in this parameter will be deleted.
    /// </param>
    /// <param name="flags">The message flags.</param>
    /// <param name="threadName">The name of the forum thread to create.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>
    /// A result which may or may not have succeeded. The returned message is null if <paramref name="shouldWait"/>
    /// is false.
    /// </returns>
    Task<Result<IMessage?>> ExecuteWebhookAsync
    (
        Snowflake webhookID,
        string token,
        Optional<bool> shouldWait = default,
        Optional<string> content = default,
        Optional<string> username = default,
        Optional<string> avatarUrl = default,
        Optional<bool> isTTS = default,
        Optional<IReadOnlyList<IEmbed>> embeds = default,
        Optional<IAllowedMentions> allowedMentions = default,
        Optional<Snowflake> threadID = default,
        Optional<IReadOnlyList<IMessageComponent>> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
        Optional<MessageFlags> flags = default,
        Optional<string> threadName = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a previously-sent webhook message.
    /// </summary>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="webhookToken">The webhook token.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="threadID">The ID of the thread the message is in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> GetWebhookMessageAsync
    (
        Snowflake webhookID,
        string webhookToken,
        Snowflake messageID,
        Optional<Snowflake> threadID = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Edits a message posted by a webhook.
    /// </summary>
    /// <remarks>
    /// At least one of <paramref name="content"/>, <paramref name="embeds"/>, or <paramref name="attachments"/> must be
    /// present.
    ///
    /// Any streams passed to this method will be disposed of at the end of the call. If you want to reuse the streams
    /// afterwards, ensure that what you pass is a copy that the method can take ownership of.
    /// </remarks>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="token">The token for the webhook.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="content">The new content, if any.</param>
    /// <param name="embeds">The new embeds, if any.</param>
    /// <param name="allowedMentions">The new allowed mentions, if any.</param>
    /// <param name="components">
    /// The components that should be included with the message. The webhook must be application-owned to use this
    /// parameter.
    /// </param>
    /// <param name="attachments">
    /// The attachments to associate with the response. Each file may be a new file in the form of
    /// <see cref="FileData"/>, or an existing one that should be retained in the form of a
    /// <see cref="IPartialAttachment"/>. If this request edits the original message, then any attachments not
    /// mentioned in this parameter will be deleted.
    /// </param>
    /// <param name="threadID">The ID of the thread the message is in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<IMessage>> EditWebhookMessageAsync
    (
        Snowflake webhookID,
        string token,
        Snowflake messageID,
        Optional<string?> content = default,
        Optional<IReadOnlyList<IEmbed>?> embeds = default,
        Optional<IAllowedMentions?> allowedMentions = default,
        Optional<IReadOnlyList<IMessageComponent>?> components = default,
        Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>?> attachments = default,
        Optional<Snowflake> threadID = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes a message posted by a webhook.
    /// </summary>
    /// <param name="webhookID">The ID of the webhook.</param>
    /// <param name="token">The token for the webhook.</param>
    /// <param name="messageID">The ID of the message.</param>
    /// <param name="threadID">The ID of the thread the message is in.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> DeleteWebhookMessageAsync
    (
        Snowflake webhookID,
        string token,
        Snowflake messageID,
        Optional<Snowflake> threadID = default,
        CancellationToken ct = default
    );
}
