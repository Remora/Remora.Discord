//
//  IDiscordRestWebhookAPI.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest
{
    /// <summary>
    /// Represents the Discord Webhook API.
    /// </summary>
    [PublicAPI]
    public interface IDiscordRestWebhookAPI
    {
        /// <summary>
        /// Creates a new webhook.
        /// </summary>
        /// <param name="channelID">The ID of the channel the webhook is for.</param>
        /// <param name="name">The name of the webhook.</param>
        /// <param name="avatar">The avatar of the webhook.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<Result<IWebhook>> CreateWebhookAsync
        (
            Snowflake channelID,
            string name,
            Stream? avatar,
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
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="name">The new name of the webhook.</param>
        /// <param name="avatar">The new avatar of the webhook.</param>
        /// <param name="channelID">The new channel of the webhook.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<Result<IWebhook>> ModifyWebhookAsync
        (
            Snowflake webhookID,
            Optional<string> name = default,
            Optional<Stream?> avatar = default,
            Optional<Snowflake> channelID = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Modifies the given webhook.
        /// </summary>
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="token">The token for the webhook.</param>
        /// <param name="name">The new name of the webhook.</param>
        /// <param name="avatar">The new avatar of the webhook.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<Result<IWebhook>> ModifyWebhookWithTokenAsync
        (
            Snowflake webhookID,
            string token,
            Optional<string> name = default,
            Optional<Stream?> avatar = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Deletes the given webhook.
        /// </summary>
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<Result> DeleteWebhookAsync(Snowflake webhookID, CancellationToken ct = default);

        /// <summary>
        /// Deletes the given webhook.
        /// </summary>
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="token">The token for the webhook.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<Result> DeleteWebhookWithTokenAsync
        (
            Snowflake webhookID,
            string token,
            CancellationToken ct = default
        );

        /// <summary>
        /// Executes the given webhook.
        /// </summary>
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="token">The token for the webhook.</param>
        /// <param name="shouldWait">
        /// Whether the call should block until the server has confirmed that the message was sent.
        /// </param>
        /// <param name="content">
        /// The content of the message. At least one of <paramref name="content"/>, <paramref name="file"/>, or
        /// <paramref name="embeds"/> is required.
        /// </param>
        /// <param name="username">The username to use for this message.</param>
        /// <param name="avatarUrl">The avatar to use for this message.</param>
        /// <param name="isTTS">Whether this message is a TTS message.</param>
        /// <param name="file">
        /// The file attached to message. At least one of <paramref name="content"/>, <paramref name="file"/>, or
        /// <paramref name="embeds"/> is required.
        /// </param>
        /// <param name="embeds">
        /// The embeds in the message. At least one of <paramref name="content"/>, <paramref name="file"/>, or
        /// <paramref name="embeds"/> is required.
        /// </param>
        /// <param name="allowedMentions">The set of allowed mentions of the message.</param>
        /// <param name="threadID">Send a message to the specified thread within a webhook's channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<Result<IMessage>> ExecuteWebhookAsync
        (
            Snowflake webhookID,
            string token,
            Optional<bool> shouldWait = default,
            Optional<string> content = default,
            Optional<string> username = default,
            Optional<string> avatarUrl = default,
            Optional<bool> isTTS = default,
            Optional<FileData> file = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<IAllowedMentions> allowedMentions = default,
            Optional<Snowflake> threadID = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// Gets a previously-sent webhook message.
        /// </summary>
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="webhookToken">The webhook token.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<Result<IMessage>> GetWebhookMessageAsync
        (
            Snowflake webhookID,
            string webhookToken,
            Snowflake messageID,
            CancellationToken ct = default
        );

        /// <summary>
        /// Edits a messages posted by a webhook.
        /// </summary>
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="token">The token for the webhook.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="content">The new content, if any.</param>
        /// <param name="embeds">The new embeds, if any.</param>
        /// <param name="allowedMentions">The new allowed mentions, if any.</param>
        /// <param name="file">The new file, if any.</param>
        /// <param name="attachments">The attachments to keep, if any.</param>
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
            Optional<FileData?> file = default,
            Optional<IReadOnlyList<IAttachment>> attachments = default,
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
        /// TODO: Is the interaction ID not involved here?
        /// Edits the initial interaction response.
        /// </summary>
        /// <param name="applicationID">The ID of the bot application.</param>
        /// <param name="token">The interaction token.</param>
        /// <param name="content">The new content, if any.</param>
        /// <param name="embeds">The new embeds, if any.</param>
        /// <param name="allowedMentions">The new allowed mentions, if any.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A modification result which may or may not have succeeded.</returns>
        Task<Result<IMessage>> EditOriginalInteractionResponseAsync
        (
            Snowflake applicationID,
            string token,
            Optional<string?> content = default,
            Optional<IReadOnlyList<IEmbed>?> embeds = default,
            Optional<IAllowedMentions?> allowedMentions = default,
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
        /// <param name="applicationID">The ID of the bot application.</param>
        /// <param name="token">The interaction token.</param>
        /// <param name="content">
        /// The content of the message. At least one of <paramref name="content"/>, <paramref name="file"/>, or
        /// <paramref name="embeds"/> is required.
        /// </param>
        /// <param name="username">The username to use for this message.</param>
        /// <param name="avatarUrl">The avatar to use for this message.</param>
        /// <param name="isTTS">Whether this message is a TTS message.</param>
        /// <param name="file">
        /// The file attached to message. At least one of <paramref name="content"/>, <paramref name="file"/>, or
        /// <paramref name="embeds"/> is required.
        /// </param>
        /// <param name="embeds">
        /// The embeds in the message. At least one of <paramref name="content"/>, <paramref name="file"/>, or
        /// <paramref name="embeds"/> is required.
        /// </param>
        /// <param name="allowedMentions">The set of allowed mentions of the message.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<Result<IMessage>> CreateFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Optional<string> content = default,
            Optional<string> username = default,
            Optional<string> avatarUrl = default,
            Optional<bool> isTTS = default,
            Optional<FileData> file = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<IAllowedMentions> allowedMentions = default,
            CancellationToken ct = default
        );

        /// <summary>
        /// TODO: Is the interaction ID not involved here?
        /// Edits an interaction followup message.
        /// </summary>
        /// <param name="applicationID">The ID of the bot application.</param>
        /// <param name="token">The interaction token.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <param name="content">The new content, if any.</param>
        /// <param name="embeds">The new embeds, if any.</param>
        /// <param name="allowedMentions">The new allowed mentions, if any.</param>
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
            CancellationToken ct = default
        );

        /// <summary>
        /// TODO: Is the interaction ID not involved here?
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
}
