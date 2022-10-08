//
//  CachingDiscordRestWebhookAPI.cs
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
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Services;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Decorates the registered webhook API with caching functionality.
/// </summary>
[PublicAPI]
public partial class CachingDiscordRestWebhookAPI : IDiscordRestWebhookAPI, IRestCustomizable
{
    private readonly IDiscordRestWebhookAPI _actual;
    private readonly CacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingDiscordRestWebhookAPI"/> class.
    /// </summary>
    /// <param name="actual">The decorated instance.</param>
    /// <param name="cacheService">The cache service.</param>
    public CachingDiscordRestWebhookAPI
    (
        IDiscordRestWebhookAPI actual,
        CacheService cacheService
    )
    {
        _actual = actual;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<Result<IWebhook>> CreateWebhookAsync
    (
        Snowflake channelID,
        string name,
        Optional<Stream?> avatar,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var createWebhook = await _actual.CreateWebhookAsync(channelID, name, avatar, reason, ct);
        if (!createWebhook.IsSuccess)
        {
            return createWebhook;
        }

        var webhook = createWebhook.Entity;
        var key = KeyHelpers.CreateWebhookCacheKey(webhook.ID);

        await _cacheService.CacheAsync(key, webhook, ct);

        return createWebhook;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteWebhookAsync
    (
        Snowflake webhookID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteWebhook = await _actual.DeleteWebhookAsync(webhookID, reason, ct);
        if (!deleteWebhook.IsSuccess)
        {
            return deleteWebhook;
        }

        var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
        await _cacheService.EvictAsync<IWebhook>(key, ct);

        return deleteWebhook;
    }

    /// <inheritdoc />
    public async Task<Result<IMessage?>> ExecuteWebhookAsync
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
    )
    {
        var execute = await _actual.ExecuteWebhookAsync
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
            flags,
            threadName,
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
        await _cacheService.CacheAsync(key, message, ct);

        return execute;
    }

    /// <inheritdoc />
    public async Task<Result<IWebhook>> GetWebhookAsync
    (
        Snowflake webhookID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
        var cacheResult = await _cacheService.TryGetValueAsync<IWebhook>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getWebhook = await _actual.GetWebhookAsync(webhookID, ct);
        if (!getWebhook.IsSuccess)
        {
            return getWebhook;
        }

        var webhook = getWebhook.Entity;
        await _cacheService.CacheAsync(key, webhook, ct);

        return getWebhook;
    }

    /// <inheritdoc />
    public async Task<Result<IWebhook>> ModifyWebhookAsync
    (
        Snowflake webhookID,
        Optional<string> name = default,
        Optional<Stream?> avatar = default,
        Optional<Snowflake> channelID = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var modifyWebhook = await _actual.ModifyWebhookAsync(webhookID, name, avatar, channelID, reason, ct);
        if (!modifyWebhook.IsSuccess)
        {
            return modifyWebhook;
        }

        var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
        var webhook = modifyWebhook.Entity;

        await _cacheService.CacheAsync(key, webhook, ct);

        return modifyWebhook;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IWebhook>>> GetChannelWebhooksAsync
    (
        Snowflake channelID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateChannelWebhooksCacheKey(channelID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IWebhook>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getWebhooks = await _actual.GetChannelWebhooksAsync(channelID, ct);
        if (!getWebhooks.IsSuccess)
        {
            return getWebhooks;
        }

        var webhooks = getWebhooks.Entity;
        await _cacheService.CacheAsync(key, webhooks, ct);

        foreach (var webhook in webhooks)
        {
            var webhookKey = KeyHelpers.CreateWebhookCacheKey(webhook.ID);
            await _cacheService.CacheAsync(webhookKey, webhook, ct);
        }

        return getWebhooks;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IWebhook>>> GetGuildWebhooksAsync
    (
        Snowflake guildID, CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildWebhooksCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IWebhook>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getWebhooks = await _actual.GetGuildWebhooksAsync(guildID, ct);
        if (!getWebhooks.IsSuccess)
        {
            return getWebhooks;
        }

        var webhooks = getWebhooks.Entity;
        await _cacheService.CacheAsync(key, webhooks, ct);

        foreach (var webhook in webhooks)
        {
            var webhookKey = KeyHelpers.CreateWebhookCacheKey(webhook.ID);
            await _cacheService.CacheAsync(webhookKey, webhook, ct);
        }

        return getWebhooks;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteWebhookWithTokenAsync
    (
        Snowflake webhookID,
        string token,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteWebhook = await _actual.DeleteWebhookWithTokenAsync(webhookID, token, reason, ct);
        if (!deleteWebhook.IsSuccess)
        {
            return deleteWebhook;
        }

        var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
        await _cacheService.EvictAsync<IWebhook>(key, ct);

        return deleteWebhook;
    }

    /// <inheritdoc />
    public async Task<Result<IWebhook>> GetWebhookWithTokenAsync
    (
        Snowflake webhookID,
        string token,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
        var cacheResult = await _cacheService.TryGetValueAsync<IWebhook>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var getWebhook = await _actual.GetWebhookWithTokenAsync(webhookID, token, ct);
        if (!getWebhook.IsSuccess)
        {
            return getWebhook;
        }

        var webhook = getWebhook.Entity;
        await _cacheService.CacheAsync(key, webhook, ct);

        return getWebhook;
    }

    /// <inheritdoc />
    public async Task<Result<IWebhook>> ModifyWebhookWithTokenAsync
    (
        Snowflake webhookID,
        string token,
        Optional<string> name = default,
        Optional<Stream?> avatar = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var modifyWebhook = await _actual.ModifyWebhookWithTokenAsync(webhookID, token, name, avatar, reason, ct);
        if (!modifyWebhook.IsSuccess)
        {
            return modifyWebhook;
        }

        var key = KeyHelpers.CreateWebhookCacheKey(webhookID);
        var webhook = modifyWebhook.Entity;

        await _cacheService.CacheAsync(key, webhook, ct);

        return modifyWebhook;
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> EditWebhookMessageAsync
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
    )
    {
        var result = await _actual.EditWebhookMessageAsync
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
        await _cacheService.CacheAsync(key, result.Entity, ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteWebhookMessageAsync
    (
        Snowflake webhookID,
        string token,
        Snowflake messageID,
        Optional<Snowflake> threadID = default,
        CancellationToken ct = default
    )
    {
        var result = await _actual.DeleteWebhookMessageAsync(webhookID, token, messageID, threadID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        var key = KeyHelpers.CreateWebhookMessageCacheKey(token, messageID);
        await _cacheService.EvictAsync<IMessage>(key, ct);

        return result;
    }

    /// <inheritdoc />
    public async Task<Result<IMessage>> GetWebhookMessageAsync
    (
        Snowflake webhookID,
        string webhookToken,
        Snowflake messageID,
        Optional<Snowflake> threadID = default,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateWebhookMessageCacheKey(webhookToken, messageID);
        var cacheResult = await _cacheService.TryGetValueAsync<IMessage>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return cacheResult;
        }

        var result = await _actual.GetWebhookMessageAsync(webhookID, webhookToken, messageID, threadID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        await _cacheService.CacheAsync(key, result.Entity, ct);

        return result;
    }
}
