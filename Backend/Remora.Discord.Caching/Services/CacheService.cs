//
//  CacheService.cs
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Results;

namespace Remora.Discord.Caching.Services;

/// <summary>
/// Handles cache insert/evict operations for various types.
/// </summary>
[PublicAPI]
public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IMemoryCache _evictionCache;
    private readonly CacheSettings _cacheSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheService"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    /// <param name="cacheSettings">The cache settings.</param>
    /// <param name="memoryCacheOptions">The memory cache options.</param>
    public CacheService
    (
        IMemoryCache memoryCache,
        IOptions<CacheSettings> cacheSettings,
        IOptions<MemoryCacheOptions> memoryCacheOptions
    )
    {
        _memoryCache = memoryCache;
        _cacheSettings = cacheSettings.Value;
        _evictionCache = new MemoryCache(memoryCacheOptions);
    }

    /// <inheritdoc/>
    public ValueTask CacheAsync<TInstance>(string key, TInstance instance)
        where TInstance : class
    {
        if (_cacheSettings.GetAbsoluteExpirationOrDefault(typeof(TInstance)) is var absoluteExpiration)
        {
            if (absoluteExpiration == TimeSpan.Zero)
            {
                return ValueTask.CompletedTask;
            }
        }

        Action cacheAction = instance switch
        {
            IWebhook webhook => () => CacheWebhook(key, webhook),
            ITemplate template => () => CacheTemplate(key, template),
            IIntegration integration => () => CacheIntegration(key, integration),
            IBan ban => () => CacheBan(key, ban),
            IGuildMember member => () => CacheGuildMember(key, member),
            IGuildPreview preview => () => CacheGuildPreview(key, preview),
            IGuild guild => () => CacheGuild(key, guild),
            IEmoji emoji => () => CacheEmoji(key, emoji),
            IInvite invite => () => CacheInvite(key, invite),
            IMessage message => () => CacheMessage(key, message),
            IChannel channel => () => CacheChannel(key, channel),
            _ => () => CacheInstance(key, instance)
        };

        cacheAction();

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<Result<TInstance>> TryGetValueAsync<TInstance>(string key)
        where TInstance : class
    {
        var hasExistingValue = _memoryCache.TryGetValue<TInstance>(key, out var cachedInstance);

        if (hasExistingValue)
        {
            return ValueTask.FromResult(Result<TInstance>.FromSuccess(cachedInstance));
        }
        else
        {
            return ValueTask.FromResult(Result<TInstance>.FromError(new NotFoundError($"The key \"{key}\" did not possess a value in cache.")));
        }
    }

    /// <inheritdoc/>
    public ValueTask<Result<TInstance>> TryGetPreviousValueAsync<TInstance>(string key)
        where TInstance : class
    {
        var hasExistingValue = _evictionCache.TryGetValue<TInstance>(key, out var cachedInstance);

        if (hasExistingValue)
        {
            return ValueTask.FromResult(Result<TInstance>.FromSuccess(cachedInstance));
        }
        else
        {
            return ValueTask.FromResult(Result<TInstance>.FromError(new NotFoundError($"The key \"{key}\" did not possess a value in cache.")));
        }
    }

    /// <inheritdoc/>
    public ValueTask EvictAsync<TInstance>(string key)
        where TInstance : class
    {
        if (_memoryCache.TryGetValue(key, out var existing))
        {
            _evictionCache.Set(key, existing, _cacheSettings.GetEntryOptions<TInstance>());
        }

        _memoryCache.Remove(key);

        return ValueTask.CompletedTask;
    }

    private async Task CacheWebhook(object key, IWebhook webhook)
    {
        CacheInstance(key, webhook);

        if (!webhook.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async Task CacheTemplate(object key, ITemplate template)
    {
        CacheInstance(key, template);

        var creatorKey = KeyHelpers.CreateUserCacheKey(template.Creator.ID);
        await CacheAsync(creatorKey, template.Creator);
    }

    private async Task CacheIntegration(object key, IIntegration integration)
    {
        CacheInstance(key, integration);

        if (!integration.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async Task CacheBan(object key, IBan ban)
    {
        CacheInstance(key, ban);

        var userKey = KeyHelpers.CreateUserCacheKey(ban.User.ID);
        await CacheAsync(userKey, ban.User);
    }

    private async Task CacheGuildMember(object key, IGuildMember member)
    {
        CacheInstance(key, member);

        if (!member.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async Task CacheGuildPreview(object key, IGuildPreview preview)
    {
        CacheInstance(key, preview);

        foreach (var emoji in preview.Emojis)
        {
            if (emoji.ID is null)
            {
                continue;
            }

            var emojiKey = KeyHelpers.CreateEmojiCacheKey(preview.ID, emoji.ID.Value);
            await CacheAsync(emojiKey, emoji);
        }
    }

    private async Task CacheGuild(object key, IGuild guild)
    {
        CacheInstance(key, guild);

        if (guild.Channels.IsDefined(out var channels))
        {
            foreach (var channel in channels)
            {
                var channelKey = KeyHelpers.CreateChannelCacheKey(channel.ID);

                if (!channel.GuildID.HasValue && channel.Type is not (ChannelType.DM or ChannelType.GroupDM))
                {
                    if (channel is Channel record)
                    {
                        // Polyfill the instance with contextual data - bit of a cheat, but it's okay in this
                        // instance
                        await CacheAsync(channelKey, record with { GuildID = guild.ID });
                    }
                }
                else
                {
                    await CacheAsync(channelKey, channel);
                }
            }
        }

        foreach (var emoji in guild.Emojis)
        {
            if (emoji.ID is null)
            {
                continue;
            }

            var emojiKey = KeyHelpers.CreateEmojiCacheKey(guild.ID, emoji.ID.Value);
            await CacheAsync(emojiKey, emoji);
        }

        if (guild.Members.IsDefined(out var members))
        {
            var membersKey = KeyHelpers.CreateGuildMembersKey(guild.ID, default, default);
            await CacheAsync(membersKey, members);

            foreach (var guildMember in members)
            {
                if (!guildMember.User.IsDefined(out var user))
                {
                    continue;
                }

                var memberKey = KeyHelpers.CreateGuildMemberKey(guild.ID, user.ID);
                await CacheAsync(memberKey, guildMember);
            }
        }

        var rolesKey = KeyHelpers.CreateGuildRolesCacheKey(guild.ID);
        await CacheAsync(rolesKey, guild.Roles);

        foreach (var role in guild.Roles)
        {
            var roleKey = KeyHelpers.CreateGuildRoleCacheKey(guild.ID, role.ID);
            await CacheAsync(roleKey, role);
        }
    }

    private async Task CacheEmoji(object key, IEmoji emoji)
    {
        CacheInstance(key, emoji);

        if (!emoji.User.IsDefined(out var creator))
        {
            return;
        }

        var creatorKey = KeyHelpers.CreateUserCacheKey(creator.ID);
        await CacheAsync(creatorKey, creator);
    }

    private async Task CacheInvite(object key, IInvite invite)
    {
        CacheInstance(key, invite);

        if (!invite.Inviter.IsDefined(out var inviter))
        {
            return;
        }

        var inviterKey = KeyHelpers.CreateUserCacheKey(inviter.ID);
        await CacheAsync(inviterKey, inviter);
    }

    private async Task CacheMessage(object key, IMessage message)
    {
        CacheInstance(key, message);

        var authorKey = KeyHelpers.CreateUserCacheKey(message.Author.ID);
        await CacheAsync(authorKey, message.Author);

        if (!message.ReferencedMessage.IsDefined(out var referencedMessage))
        {
            return;
        }

        var referencedMessageKey = KeyHelpers.CreateMessageCacheKey
        (
            referencedMessage.ChannelID,
            referencedMessage.ID
        );

        await CacheAsync(referencedMessageKey, referencedMessage);
    }

    private async Task CacheChannel(object key, IChannel channel)
    {
        CacheInstance(key, channel);
        if (!channel.Recipients.IsDefined(out var recipients))
        {
            return;
        }

        foreach (var recipient in recipients)
        {
            var recipientKey = KeyHelpers.CreateUserCacheKey(recipient.ID);
            await CacheAsync(recipientKey, recipient);
        }
    }

    /// <summary>
    /// Caches an instance using the default entry options for that instance type.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="instance">The instance.</param>
    /// <typeparam name="TInstance">The instance type.</typeparam>
    private async Task CacheInstance<TInstance>(object key, TInstance instance)
        where TInstance : class
    {
        var entryOptions = _cacheSettings.GetEntryOptions<TInstance>();
        if (_memoryCache.TryGetValue<TInstance>(key, out var existing))
        {
            _evictionCache.Set(key, existing, entryOptions);
        }

        _memoryCache.Set(key, instance, entryOptions);
    }
}
