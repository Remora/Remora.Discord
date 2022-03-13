//
//  RedisCacheService.cs
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
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Results;

namespace Remora.Discord.Caching.Services;

/// <summary>
/// Handles cache insert/evict operations for various types, using Redis as a backing-store.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly CacheSettings _cacheSettings;
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
    /// </summary>
    /// <param name="settings">The settings for the cache.</param>
    /// <param name="cache">The redis cache.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    public RedisCacheService(IOptions<CacheSettings> settings, IDistributedCache cache, IOptionsMonitor<JsonSerializerOptions> jsonOptions)
    {
        _cacheSettings = settings.Value;
        _cache = cache;
        jsonOptions.Get("Discord");
    }

    /// <inheritdoc />
    public async ValueTask CacheAsync<TInstance>(string key, TInstance instance) where TInstance : class
    {
        if (_cacheSettings.GetAbsoluteExpirationOrDefault(typeof(TInstance)) is var absoluteExpiration)
        {
            if (absoluteExpiration == TimeSpan.Zero)
            {
                return;
            }
        }

        Task cacheTask = instance switch
        {
            IWebhook webhook => CacheWebhook(key, webhook),
            ITemplate template => CacheTemplate(key, template),
            IIntegration integration => CacheIntegration(key, integration),
            IBan ban => CacheBan(key, ban),
            IGuildMember member => CacheGuildMember(key, member),
            IGuildPreview preview => CacheGuildPreview(key, preview),
            IGuild guild => CacheGuild(key, guild),
            IEmoji emoji => CacheEmoji(key, emoji),
            IInvite invite => CacheInvite(key, invite),
            IMessage message => CacheMessage(key, message),
            IChannel channel => CacheChannel(key, channel),
            _ => CacheInstanceAsync(key, instance)
        };

        await cacheTask;
    }

    /// <inheritdoc />
    public async ValueTask<Result<TInstance>> TryGetValueAsync<TInstance>(string key) where TInstance : class
    {
        var cacheResult = await _cache.GetAsync(key);

        if (cacheResult is null)
        {
            return Result<TInstance>.FromError(new NotFoundError($"The key \"{key}\" was not found in the cache."));
        }

        var deserialized = JsonSerializer.Deserialize<TInstance>(cacheResult, _jsonOptions);

        await _cache.RefreshAsync(key);

        return Result<TInstance>.FromSuccess(deserialized);
    }

    /// <inheritdoc />
    public async ValueTask<Result<TInstance>> TryGetPreviousValueAsync<TInstance>(string key) where TInstance : class
    {
        var evictionKey = $"Evicted:{key}";

        var cacheResult = await _cache.GetAsync(evictionKey);

        if (cacheResult is null)
        {
            return Result<TInstance>.FromError(new NotFoundError($"The key \"{key}\" was not found in the eviction cache."));
        }

        var deserialized = JsonSerializer.Deserialize<TInstance>(cacheResult, _jsonOptions);

        await _cache.RefreshAsync(key);

        return Result<TInstance>.FromSuccess(deserialized);
    }

    /// <inheritdoc />
    public async ValueTask EvictAsync<TInstance>(string key) where TInstance : class
    {
        var value = await _cache.GetAsync(key);

        if (value is null)
        {
            return;
        }

        var evictionKey = $"Evicted:{key}";
        var settings = _cacheSettings.GetRedisEntryOptions<TInstance>();

        await _cache.SetAsync(evictionKey, value, settings);
    }

    private async Task CacheWebhook(string key, IWebhook webhook)
    {
        CacheInstanceAsync(key, webhook);

        if (!webhook.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async Task CacheTemplate(string key, ITemplate template)
    {
        CacheInstanceAsync(key, template);

        var creatorKey = KeyHelpers.CreateUserCacheKey(template.Creator.ID);
        await CacheAsync(creatorKey, template.Creator);
    }

    private async Task CacheIntegration(string key, IIntegration integration)
    {
        CacheInstanceAsync(key, integration);

        if (!integration.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async Task CacheBan(string key, IBan ban)
    {
        CacheInstanceAsync(key, ban);

        var userKey = KeyHelpers.CreateUserCacheKey(ban.User.ID);
        await CacheAsync(userKey, ban.User);
    }

    private async Task CacheGuildMember(string key, IGuildMember member)
    {
        CacheInstanceAsync(key, member);

        if (!member.User.IsDefined(out var user))
        {
            return;
        }

        var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
        await CacheAsync(userKey, user);
    }

    private async Task CacheGuildPreview(string key, IGuildPreview preview)
    {
        CacheInstanceAsync(key, preview);

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

    private async Task CacheGuild(string key, IGuild guild)
    {
        CacheInstanceAsync(key, guild);

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

    private async Task CacheEmoji(string key, IEmoji emoji)
    {
        CacheInstanceAsync(key, emoji);

        if (!emoji.User.IsDefined(out var creator))
        {
            return;
        }

        var creatorKey = KeyHelpers.CreateUserCacheKey(creator.ID);
        await CacheAsync(creatorKey, creator);
    }

    private async Task CacheInvite(string key, IInvite invite)
    {
        CacheInstanceAsync(key, invite);

        if (!invite.Inviter.IsDefined(out var inviter))
        {
            return;
        }

        var inviterKey = KeyHelpers.CreateUserCacheKey(inviter.ID);
        await CacheAsync(inviterKey, inviter);
    }

    private async Task CacheMessage(string key, IMessage message)
    {
        CacheInstanceAsync(key, message);

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

    private async Task CacheChannel(string key, IChannel channel)
    {
        CacheInstanceAsync(key, channel);
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

    private async Task CacheInstanceAsync<TInstance>(string key, TInstance instance) where TInstance : class
    {
        var settings = _cacheSettings.GetRedisEntryOptions<TInstance>();

        var existingValue = await _cache.GetAsync(key);

        if (existingValue is not null)
        {
            var evictionKey = $"Evicted:{key}";
            await _cache.RemoveAsync(key);
            await _cache.SetAsync(evictionKey, existingValue, settings);
        }

        var json = JsonSerializer.Serialize(instance);

        await _cache.SetStringAsync(key, json, settings);
    }
}
