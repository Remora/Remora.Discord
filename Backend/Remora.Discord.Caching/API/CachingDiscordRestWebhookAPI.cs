//
//  CachingDiscordRestWebhookAPI.cs
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Discord.Core;
using Remora.Discord.Rest;
using Remora.Discord.Rest.API;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API
{
    /// <inheritdoc />
    [PublicAPI]
    public class CachingDiscordRestWebhookAPI : DiscordRestWebhookAPI
    {
        private readonly CacheService _cacheService;

        /// <inheritdoc cref="DiscordRestWebhookAPI(IRestHttpClient, JsonSerializerOptions)" />
        public CachingDiscordRestWebhookAPI
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
        public override async Task<Result<IWebhook>> CreateWebhookAsync
        (
            Snowflake channelID,
            string name,
            Optional<Stream?> avatar,
            CancellationToken ct = default
        )
        {
            var createWebhook = await base.CreateWebhookAsync(channelID, name, avatar, ct);
            if (!createWebhook.IsSuccess)
            {
                return createWebhook;
            }

            var webhook = createWebhook.Entity;
            var key = KeyHelpers.CreateWebhookCacheKey(webhook.ID);

            _cacheService.Cache(key, webhook);

            return createWebhook;
        }

        /// <inheritdoc />
        public override async Task<Result> DeleteWebhookAsync
        (
            Snowflake webhookID,
            CancellationToken ct = default
        )
        {
            var deleteWebhook = await base.DeleteWebhookAsync(webhookID, ct);
            if (!deleteWebhook.IsSuccess)
            {
                return deleteWebhook;
            }

            var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
            _cacheService.Evict(key);

            return deleteWebhook;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage?>> ExecuteWebhookAsync
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
            CancellationToken ct = default
        )
        {
            var execute = await base.ExecuteWebhookAsync
            (
                webhookID,
                token,
                shouldWait,
                content,
                username,
                avatarUrl,
                isTTS,
                embeds,
                allowedMentions,
                threadID,
                components,
                attachments,
                ct
            );

            if (!execute.IsSuccess)
            {
                return execute;
            }

            var message = execute.Entity;
            if (message is null)
            {
                return execute;
            }

            var key = KeyHelpers.CreateMessageCacheKey(message.ChannelID, message.ID);
            _cacheService.Cache(key, message);

            return execute;
        }

        /// <inheritdoc />
        public override async Task<Result<IWebhook>> GetWebhookAsync
        (
            Snowflake webhookID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
            if (_cacheService.TryGetValue<IWebhook>(key, out var cachedInstance))
            {
                return Result<IWebhook>.FromSuccess(cachedInstance);
            }

            var getWebhook = await base.GetWebhookAsync(webhookID, ct);
            if (!getWebhook.IsSuccess)
            {
                return getWebhook;
            }

            var webhook = getWebhook.Entity;
            _cacheService.Cache(key, webhook);

            return getWebhook;
        }

        /// <inheritdoc />
        public override async Task<Result<IWebhook>> ModifyWebhookAsync
        (
            Snowflake webhookID,
            Optional<string> name = default,
            Optional<Stream?> avatar = default,
            Optional<Snowflake> channelID = default,
            CancellationToken ct = default
        )
        {
            var modifyWebhook = await base.ModifyWebhookAsync(webhookID, name, avatar, channelID, ct);
            if (!modifyWebhook.IsSuccess)
            {
                return modifyWebhook;
            }

            var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
            var webhook = modifyWebhook.Entity;

            _cacheService.Cache(key, webhook);

            return modifyWebhook;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IWebhook>>> GetChannelWebhooksAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateChannelWebhooksCacheKey(channelID);
            if (_cacheService.TryGetValue<IReadOnlyList<IWebhook>>(key, out var cachedInstance))
            {
                return Result<IReadOnlyList<IWebhook>>.FromSuccess(cachedInstance);
            }

            var getWebhooks = await base.GetChannelWebhooksAsync(channelID, ct);
            if (!getWebhooks.IsSuccess)
            {
                return getWebhooks;
            }

            var webhooks = getWebhooks.Entity;
            _cacheService.Cache(key, webhooks);

            foreach (var webhook in webhooks)
            {
                var webhookKey = KeyHelpers.CreateWebhookCacheKey(webhook.ID);
                _cacheService.Cache(webhookKey, webhook);
            }

            return getWebhooks;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<IWebhook>>> GetGuildWebhooksAsync
        (
            Snowflake guildID, CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildWebhooksCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<IWebhook>>(key, out var cachedInstance))
            {
                return Result<IReadOnlyList<IWebhook>>.FromSuccess(cachedInstance);
            }

            var getWebhooks = await base.GetGuildWebhooksAsync(guildID, ct);
            if (!getWebhooks.IsSuccess)
            {
                return getWebhooks;
            }

            var webhooks = getWebhooks.Entity;
            _cacheService.Cache(key, webhooks);

            foreach (var webhook in webhooks)
            {
                var webhookKey = KeyHelpers.CreateWebhookCacheKey(webhook.ID);
                _cacheService.Cache(webhookKey, webhook);
            }

            return getWebhooks;
        }

        /// <inheritdoc />
        public override async Task<Result> DeleteWebhookWithTokenAsync
        (
            Snowflake webhookID,
            string token,
            CancellationToken ct = default
        )
        {
            var deleteWebhook = await base.DeleteWebhookWithTokenAsync(webhookID, token, ct);
            if (!deleteWebhook.IsSuccess)
            {
                return deleteWebhook;
            }

            var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
            _cacheService.Evict(key);

            return deleteWebhook;
        }

        /// <inheritdoc />
        public override async Task<Result<IWebhook>> GetWebhookWithTokenAsync
        (
            Snowflake webhookID,
            string token,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
            if (_cacheService.TryGetValue<IWebhook>(key, out var cachedInstance))
            {
                return Result<IWebhook>.FromSuccess(cachedInstance);
            }

            var getWebhook = await base.GetWebhookWithTokenAsync(webhookID, token, ct);
            if (!getWebhook.IsSuccess)
            {
                return getWebhook;
            }

            var webhook = getWebhook.Entity;
            _cacheService.Cache(key, webhook);

            return getWebhook;
        }

        /// <inheritdoc />
        public override async Task<Result<IWebhook>> ModifyWebhookWithTokenAsync
        (
            Snowflake webhookID,
            string token,
            Optional<string> name = default,
            Optional<Stream?> avatar = default,
            CancellationToken ct = default
        )
        {
            var modifyWebhook = await base.ModifyWebhookWithTokenAsync(webhookID, token, name, avatar, ct);
            if (!modifyWebhook.IsSuccess)
            {
                return modifyWebhook;
            }

            var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
            var webhook = modifyWebhook.Entity;

            _cacheService.Cache(key, webhook);

            return modifyWebhook;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage>> EditWebhookMessageAsync
        (
            Snowflake webhookID,
            string token,
            Snowflake messageID,
            Optional<string?> content = default,
            Optional<IReadOnlyList<IEmbed>?> embeds = default,
            Optional<IAllowedMentions?> allowedMentions = default,
            Optional<IReadOnlyList<IMessageComponent>> components = default,
            Optional<IReadOnlyList<OneOf<FileData, IPartialAttachment>>> attachments = default,
            Optional<Snowflake> threadID = default,
            CancellationToken ct = default
        )
        {
            var result = await base.EditWebhookMessageAsync
            (
                webhookID,
                token,
                messageID,
                content,
                embeds,
                allowedMentions,
                components,
                attachments,
                threadID,
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
        public override async Task<Result> DeleteWebhookMessageAsync
        (
            Snowflake webhookID,
            string token,
            Snowflake messageID,
            Optional<Snowflake> threadID = default,
            CancellationToken ct = default
        )
        {
            var result = await base.DeleteWebhookMessageAsync(webhookID, token, messageID, threadID, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            var key = KeyHelpers.CreateWebhookMessageCacheKey(token, messageID);
            _cacheService.Evict(key);

            return result;
        }

        /// <inheritdoc />
        public override async Task<Result<IMessage>> GetWebhookMessageAsync
        (
            Snowflake webhookID,
            string webhookToken,
            Snowflake messageID,
            Optional<Snowflake> threadID = default,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateWebhookMessageCacheKey(webhookToken, messageID);
            if (_cacheService.TryGetValue<IMessage>(key, out var cachedInstance))
            {
                return Result<IMessage>.FromSuccess(cachedInstance);
            }

            var result = await base.GetWebhookMessageAsync(webhookID, webhookToken, messageID, threadID, ct);
            if (!result.IsSuccess)
            {
                return result;
            }

            _cacheService.Cache(key, result.Entity);

            return result;
        }
    }
}
