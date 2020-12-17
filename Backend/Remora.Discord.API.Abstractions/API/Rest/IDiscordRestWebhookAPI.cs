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
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Core;

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
        Task<ICreateRestEntityResult<IWebhook>> CreateWebhookAsync
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
        Task<IRetrieveRestEntityResult<IReadOnlyList<IWebhook>>> GetChannelWebhooksAsync
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
        Task<IRetrieveRestEntityResult<IReadOnlyList<IWebhook>>> GetGuildWebhooksAsync
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
        Task<IRetrieveRestEntityResult<IWebhook>> GetWebhookAsync
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
        Task<IRetrieveRestEntityResult<IWebhook>> GetWebhookWithTokenAsync
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
        Task<IModifyRestEntityResult<IWebhook>> ModifyWebhookAsync
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
        Task<IModifyRestEntityResult<IWebhook>> ModifyWebhookWithTokenAsync
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
        Task<IDeleteRestEntityResult> DeleteWebhookAsync(Snowflake webhookID, CancellationToken ct = default);

        /// <summary>
        /// Deletes the given webhook.
        /// </summary>
        /// <param name="webhookID">The ID of the webhook.</param>
        /// <param name="token">The token for the webhook.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A deletion result which may or may not have succeeded.</returns>
        Task<IDeleteRestEntityResult> DeleteWebhookWithTokenAsync
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IMessage>> ExecuteWebhookAsync
        (
            Snowflake webhookID,
            string token,
            Optional<bool> shouldWait = default,
            Optional<string> content = default,
            Optional<string> username = default,
            Optional<string> avatarUrl = default,
            Optional<bool> isTTS = default,
            Optional<Stream> file = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<IAllowedMentions> allowedMentions = default,
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<IModifyRestEntityResult<IMessage>> EditWebhookMessageAsync
        (
            Snowflake webhookID,
            string token,
            Snowflake messageID,
            Optional<string?> content = default,
            Optional<IReadOnlyList<IEmbed>?> embeds = default,
            Optional<IAllowedMentions?> allowedMentions = default,
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
        Task<IModifyRestEntityResult<IMessage>> EditOriginalInteractionResponseAsync
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
        Task<IDeleteRestEntityResult> DeleteOriginalInteractionResponseAsync
        (
            Snowflake applicationID,
            string token,
            CancellationToken ct
        );

        /// <summary>
        /// TODO: Is the interaction ID not involved here?
        /// Creates a followup message.
        /// </summary>
        /// <param name="applicationID">The ID of the bot application.</param>
        /// <param name="token">The interaction token.</param>
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
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IMessage>> CreateFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Optional<bool> shouldWait = default,
            Optional<string> content = default,
            Optional<string> username = default,
            Optional<string> avatarUrl = default,
            Optional<bool> isTTS = default,
            Optional<Stream> file = default,
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
        Task<IModifyRestEntityResult<IMessage>> EditFollowupMessageAsync
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
        Task<IDeleteRestEntityResult> DeleteFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Snowflake messageID,
            CancellationToken ct = default
        );
    }
}
