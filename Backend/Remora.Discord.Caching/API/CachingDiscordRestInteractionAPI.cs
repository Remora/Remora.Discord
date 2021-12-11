//
//  CachingDiscordRestInteractionAPI.cs
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest;
using Remora.Discord.Rest.API;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API
{
    /// <inheritdoc />
    [PublicAPI]
    public class CachingDiscordRestInteractionAPI : DiscordRestInteractionAPI
    {
        private readonly CacheService _cacheService;

        /// <inheritdoc cref="DiscordRestInteractionAPI(IRestHttpClient, JsonSerializerOptions)" />
        public CachingDiscordRestInteractionAPI
        (
            IRestHttpClient restHttpClient,
            JsonSerializerOptions jsonOptions,
            CacheService cacheService
        )
            : base(restHttpClient, jsonOptions)
        {
            _cacheService = cacheService;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage>> CreateFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Optional<string> content = default,
            Optional<bool> isTTS = default,
            Optional<IReadOnlyList<IEmbed>> embeds = default,
            Optional<IAllowedMentions> allowedMentions = default,
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            Optional<MessageFlags> flags = default,
            Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
            CancellationToken ct = default
        )
        {
            var result = await base.CreateFollowupMessageAsync
            (
                applicationID,
                token,
                content,
                isTTS,
                embeds,
                allowedMentions,
                components,
                flags,
                attachments,
                ct
            );

            if (!result.IsSuccess)
            {
                return result;
            }

            var message = result.Entity;

            var messageKey = KeyHelpers.CreateMessageCacheKey(message.ChannelID, message.ID);
            var followupKey = KeyHelpers.CreateFollowupMessageCacheKey(token, message.ID);

            _cacheService.Cache(messageKey, message);
            _cacheService.Cache(followupKey, message);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result> DeleteFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            var result = await base.DeleteFollowupMessageAsync(applicationID, token, messageID, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            var key = KeyHelpers.CreateFollowupMessageCacheKey(token, messageID);
            _cacheService.Evict<IMessage>(key);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage>> GetFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateFollowupMessageCacheKey(token, messageID);
            if (_cacheService.TryGetValue<IMessage>(key, out var cachedInstance))
            {
                return Result<IMessage>.FromSuccess(cachedInstance);
            }

            var result = await base.GetFollowupMessageAsync(applicationID, token, messageID, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            _cacheService.Cache(key, result.Entity);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage>> EditFollowupMessageAsync
        (
            Snowflake applicationID,
            string token,
            Snowflake messageID,
            Optional<string?> content = default,
            Optional<IReadOnlyList<IEmbed>?> embeds = default,
            Optional<IAllowedMentions?> allowedMentions = default,
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
            CancellationToken ct = default
        )
        {
            var result = await base.EditFollowupMessageAsync
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

            if (!result.IsSuccess)
            {
                return result;
            }

            var key = KeyHelpers.CreateWebhookMessageCacheKey(token, messageID);
            _cacheService.Cache(key, result.Entity);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage>> GetOriginalInteractionResponseAsync
        (
            Snowflake applicationID,
            string interactionToken,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateOriginalInteractionMessageCacheKey(interactionToken);
            if (_cacheService.TryGetValue<IMessage>(key, out var cachedInstance))
            {
                return Result<IMessage>.FromSuccess(cachedInstance);
            }

            var result = await base.GetOriginalInteractionResponseAsync(applicationID, interactionToken, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            var message = result.Entity;

            var messageKey = KeyHelpers.CreateMessageCacheKey(message.ChannelID, message.ID);

            _cacheService.Cache(key, message);
            _cacheService.Cache(messageKey, message);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage>> EditOriginalInteractionResponseAsync
        (
            Snowflake applicationID,
            string token,
            Optional<string?> content = default,
            Optional<IReadOnlyList<IEmbed>?> embeds = default,
            Optional<IAllowedMentions?> allowedMentions = default,
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
            CancellationToken ct = default
        )
        {
            var result = await base.EditOriginalInteractionResponseAsync
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

            if (!result.IsSuccess)
            {
                return result;
            }

            var message = result.Entity;

            var key = KeyHelpers.CreateOriginalInteractionMessageCacheKey(token);
            var messageKey = KeyHelpers.CreateMessageCacheKey(message.ChannelID, message.ID);

            _cacheService.Cache(key, message);
            _cacheService.Cache(messageKey, message);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result> DeleteOriginalInteractionResponseAsync
        (
            Snowflake applicationID,
            string token,
            CancellationToken ct = default
        )
        {
            var result = await base.DeleteOriginalInteractionResponseAsync(applicationID, token, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            var key = KeyHelpers.CreateOriginalInteractionMessageCacheKey(token);
            _cacheService.Evict<IMessage>(key);

            return result;
        }
    }
}
