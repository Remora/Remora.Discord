//
//  CacheService.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Results;

namespace Remora.Discord.Caching.Services;

/// <summary>
/// Handles cache insert/evict operations for various types.
/// </summary>
[PublicAPI]
public class CacheService
{
    private readonly ICacheProvider _cacheProvider;
    private readonly CacheSettings _cacheSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheService"/> class.
    /// </summary>
    /// <param name="cacheProvider">The cache provider.</param>
    /// <param name="cacheSettings">The cache settings.</param>
    public CacheService(ICacheProvider cacheProvider, IOptions<CacheSettings> cacheSettings)
    {
        _cacheProvider = cacheProvider;
        _cacheSettings = cacheSettings.Value;
    }

    /// <summary>
    /// Caches a value. Certain instance types may have specializations which cache more than one value from the
    /// instance.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="TInstance">The instance type.</typeparam>
    /// <returns>A <see cref="ValueTask"/> representing the potentially asynchronous operation.</returns>
    public async ValueTask CacheAsync<TInstance>(CacheKey key, TInstance instance, CancellationToken ct = default)
        where TInstance : class
    {
        if (_cacheSettings.GetAbsoluteExpirationOrDefault(typeof(TInstance)) is var absoluteExpiration)
        {
            if (absoluteExpiration == TimeSpan.Zero)
            {
                return;
            }
        }

        Func<ValueTask> cacheAction = instance switch
        {
            IWebhook webhook => () => CacheWebhookAsync(key, webhook),
            ITemplate template => () => CacheTemplateAsync(key, template),
            IIntegration integration => () => CacheIntegrationAsync(key, integration),
            IBan ban => () => CacheBanAsync(key, ban),
            IGuildMember member => () => CacheGuildMemberAsync(key, member),
            IGuildPreview preview => () => CacheGuildPreviewAsync(key, preview),
            IGuild guild => () => CacheGuildAsync(key, guild),
            IEmoji emoji => () => CacheEmojiAsync(key, emoji),
            IInvite invite => () => CacheInviteAsync(key, invite),
            IMessage message => () => CacheMessageAsync(key, message),
            IChannel channel => () => CacheChannelAsync(key, channel),
            _ => () => CacheInstanceAsync(key, instance)
        };

        await cacheAction();
    }

    /// <summary>
    /// Attempts to retrieve a value from the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <typeparam name="TInstance">The instance type.</typeparam>
    /// <returns>A <see cref="Result"/> that may or not have succeeded.</returns>
    public ValueTask<Result<TInstance>> TryGetValueAsync<TInstance>(CacheKey key, CancellationToken ct = default)
        where TInstance : class => _cacheProvider.RetrieveAsync<TInstance>(key, ct);

    /// <summary>
    /// Attempts to retrieve the previous value of the given key from the eviction cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <typeparam name="TInstance">The instance type.</typeparam>
    /// <returns>A <see cref="Result"/> that may or not have succeeded.</returns>
    public ValueTask<Result<TInstance>> TryGetPreviousValueAsync<TInstance>(CacheKey key)
        where TInstance : class => _cacheProvider.RetrieveAsync<TInstance>(new KeyHelpers.EvictionCacheKey(key));

    /// <summary>
    /// Evicts the instance with the given key from the cache.
    /// </summary>
    /// <typeparam name="TInstance">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the potentially asynchronous operation.</returns>
    public async ValueTask<Result<TInstance>> EvictAsync<TInstance>(CacheKey key, CancellationToken ct = default)
        where TInstance : class
    {
        var evictionResult = await _cacheProvider.EvictAsync<TInstance>(key, ct);

        if (!evictionResult.IsSuccess)
        {
            return evictionResult;
        }

        var options = _cacheSettings.GetEvictionEntryOptions<TInstance>();
        await _cacheProvider.CacheAsync
        (
            new KeyHelpers.EvictionCacheKey(key),
            evictionResult.Entity,
            options.AbsoluteExpiration,
            options.SlidingExpiration,
            ct
        );

        return evictionResult.Entity;
    }

    private async ValueTask CacheWebhookAsync(CacheKey key, IWebhook webhook)
    {
        await CacheInstanceAsync(key, webhook);

        if (!webhook.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = new KeyHelpers.UserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async ValueTask CacheTemplateAsync(CacheKey key, ITemplate template)
    {
        await CacheInstanceAsync(key, template);

        var creatorKey = new KeyHelpers.UserCacheKey(template.Creator.ID);
        await CacheAsync(creatorKey, template.Creator);
    }

    private async ValueTask CacheIntegrationAsync(CacheKey key, IIntegration integration)
    {
        await CacheInstanceAsync(key, integration);

        if (!integration.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = new KeyHelpers.UserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async ValueTask CacheBanAsync(CacheKey key, IBan ban)
    {
        await CacheInstanceAsync(key, ban);

        var userKey = new KeyHelpers.UserCacheKey(ban.User.ID);
        await CacheAsync(userKey, ban.User);
    }

    private async ValueTask CacheGuildMemberAsync(CacheKey key, IGuildMember member)
    {
        await CacheInstanceAsync(key, member);

        if (!member.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = new KeyHelpers.UserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async ValueTask CacheGuildPreviewAsync(CacheKey key, IGuildPreview preview)
    {
        await CacheInstanceAsync(key, preview);

        foreach (var emoji in preview.Emojis)
        {
            if (emoji.ID is null)
            {
                continue;
            }

            var emojiKey = new KeyHelpers.EmojiCacheKey(preview.ID, emoji.ID.Value);
            await CacheAsync(emojiKey, emoji);
        }
    }

    private async ValueTask CacheGuildAsync(CacheKey key, IGuild guild)
    {
        await CacheInstanceAsync(key, guild);

        foreach (var emoji in guild.Emojis)
        {
            if (emoji.ID is null)
            {
                continue;
            }

            var emojiKey = new KeyHelpers.EmojiCacheKey(guild.ID, emoji.ID.Value);
            await CacheAsync(emojiKey, emoji);
        }

        var rolesKey = new KeyHelpers.GuildRolesCacheKey(guild.ID);
        await CacheAsync(rolesKey, guild.Roles);

        foreach (var role in guild.Roles)
        {
            var roleKey = new KeyHelpers.GuildRoleCacheKey(guild.ID, role.ID);
            await CacheAsync(roleKey, role);
        }
    }

    private async ValueTask CacheEmojiAsync(CacheKey key, IEmoji emoji)
    {
        await CacheInstanceAsync(key, emoji);

        if (!emoji.User.IsDefined(out var creator))
        {
            return;
        }

        var creatorKey = new KeyHelpers.UserCacheKey(creator.ID);
        await CacheAsync(creatorKey, creator);
    }

    private async ValueTask CacheInviteAsync(CacheKey key, IInvite invite)
    {
        await CacheInstanceAsync(key, invite);

        if (!invite.Inviter.IsDefined(out var inviter))
        {
            return;
        }

        var inviterKey = new KeyHelpers.UserCacheKey(inviter.ID);
        await CacheAsync(inviterKey, inviter);
    }

    private async ValueTask CacheMessageAsync(CacheKey key, IMessage message)
    {
        await CacheInstanceAsync(key, message);

        var authorKey = new KeyHelpers.UserCacheKey(message.Author.ID);
        await CacheAsync(authorKey, message.Author);

        if (!message.ReferencedMessage.IsDefined(out var referencedMessage))
        {
            return;
        }

        var referencedMessageKey = new KeyHelpers.MessageCacheKey
        (
            referencedMessage.ChannelID,
            referencedMessage.ID
        );

        await CacheAsync(referencedMessageKey, referencedMessage);
    }

    private async ValueTask CacheChannelAsync(CacheKey key, IChannel channel)
    {
        await CacheInstanceAsync(key, channel);
        if (!channel.Recipients.IsDefined(out var recipients))
        {
            return;
        }

        foreach (var recipient in recipients)
        {
            var recipientKey = new KeyHelpers.UserCacheKey(recipient.ID);
            await CacheAsync(recipientKey, recipient);
        }
    }

    /// <summary>
    /// Caches an instance using the default entry options for that instance type.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="instance">The instance.</param>
    /// <typeparam name="TInstance">The instance type.</typeparam>
    private ValueTask CacheInstanceAsync<TInstance>(CacheKey key, TInstance instance)
        where TInstance : class
    {
        var options = _cacheSettings.GetEntryOptions<TInstance>();
        return _cacheProvider.CacheAsync(key, instance, options.AbsoluteExpiration, options.SlidingExpiration);
    }
}
