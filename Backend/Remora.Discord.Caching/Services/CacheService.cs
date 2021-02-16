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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace Remora.Discord.Caching.Services
{
    /// <summary>
    /// Handles cache insert/evict operations for various types.
    /// </summary>
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheSettings _cacheSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="memoryCache">The memory cache.</param>
        /// <param name="cacheSettings">The cache settings.</param>
        public CacheService(IMemoryCache memoryCache, IOptions<CacheSettings> cacheSettings)
        {
            _memoryCache = memoryCache;
            _cacheSettings = cacheSettings.Value;
        }

        /// <summary>
        /// Caches a value. Certain instance types may have specializations which cache more than one value from the
        /// instance.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="instance">The instance.</param>
        /// <typeparam name="TInstance">The instance type.</typeparam>
        public void Cache<TInstance>(object key, TInstance instance)
            where TInstance : class
        {
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
        }

        /// <summary>
        /// Attempts to retrieve a value from the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cachedInstance">The instance, if any.</param>
        /// <typeparam name="TInstance">The instance type.</typeparam>
        /// <returns>true if an instance was retrieved; otherwise, false.</returns>
        public bool TryGetValue<TInstance>(object key, [NotNullWhen(true)] out TInstance? cachedInstance)
            where TInstance : class
        {
            return _memoryCache.TryGetValue(key, out cachedInstance);
        }

        /// <summary>
        /// Evicts the instance with the given key from the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        public void Evict(object key)
        {
            _memoryCache.Remove(key);
        }

        private void CacheWebhook(object key, IWebhook webhook)
        {
            CacheInstance(key, webhook);

            if (!webhook.User.HasValue)
            {
                return;
            }

            var user = webhook.User.Value;
            var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
            Cache(userKey, user);
        }

        private void CacheTemplate(object key, ITemplate template)
        {
            CacheInstance(key, template);

            var creatorKey = KeyHelpers.CreateUserCacheKey(template.Creator.ID);
            Cache(creatorKey, template.Creator);
        }

        private void CacheIntegration(object key, IIntegration integration)
        {
            CacheInstance(key, integration);

            if (!integration.User.HasValue)
            {
                return;
            }

            var user = integration.User.Value;
            var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
            Cache(userKey, user);
        }

        private void CacheBan(object key, IBan ban)
        {
            CacheInstance(key, ban);

            var userKey = KeyHelpers.CreateUserCacheKey(ban.User.ID);
            Cache(userKey, ban.User);
        }

        private void CacheGuildMember(object key, IGuildMember member)
        {
            CacheInstance(key, member);

            if (!member.User.HasValue)
            {
                return;
            }

            var user = member.User.Value;
            var userKey = KeyHelpers.CreateUserCacheKey(user.ID);
            Cache(userKey, user);
        }

        private void CacheGuildPreview(object key, IGuildPreview preview)
        {
            CacheInstance(key, preview);

            foreach (var emoji in preview.Emojis)
            {
                if (emoji.ID is null)
                {
                    continue;
                }

                var emojiKey = KeyHelpers.CreateEmojiCacheKey(preview.ID, emoji.ID.Value);
                Cache(emojiKey, emoji);
            }
        }

        private void CacheGuild(object key, IGuild guild)
        {
            CacheInstance(key, guild);

            if (guild.Channels.HasValue)
            {
                foreach (var channel in guild.Channels.Value!)
                {
                    var channelKey = KeyHelpers.CreateChannelCacheKey(channel.ID);

                    if (!channel.GuildID.HasValue && channel.Type is not ChannelType.DM or ChannelType.GroupDM)
                    {
                        if (channel is Channel record)
                        {
                            // Polyfill the instance with contextual data - bit of a cheat, but it's okay in this
                            // instance
                            Cache(key, record with { GuildID = guild.ID });
                        }
                    }
                    else
                    {
                        Cache(channelKey, channel);
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
                Cache(emojiKey, emoji);
            }

            if (guild.Members.HasValue)
            {
                var membersKey = KeyHelpers.CreateGuildMembersKey(guild.ID, default, default);
                Cache(membersKey, guild.Members.Value!);

                foreach (var guildMember in guild.Members.Value!)
                {
                    if (!guildMember.User.HasValue)
                    {
                        continue;
                    }

                    var memberKey = KeyHelpers.CreateGuildMemberKey(guild.ID, guildMember.User.Value.ID);
                    Cache(memberKey, guildMember);
                }
            }

            var rolesKey = KeyHelpers.CreateGuildRolesCacheKey(guild.ID);
            Cache(rolesKey, guild.Roles);

            foreach (var role in guild.Roles)
            {
                var roleKey = KeyHelpers.CreateGuildRoleCacheKey(guild.ID, role.ID);
                Cache(roleKey, role);
            }
        }

        private void CacheEmoji(object key, IEmoji emoji)
        {
            CacheInstance(key, emoji);

            if (!emoji.User.HasValue)
            {
                return;
            }

            var creator = emoji.User.Value;
            var creatorKey = KeyHelpers.CreateUserCacheKey(creator.ID);
            Cache(creatorKey, creator);
        }

        private void CacheInvite(object key, IInvite invite)
        {
            CacheInstance(key, invite);

            if (!invite.Inviter.HasValue)
            {
                return;
            }

            var inviter = invite.Inviter.Value;
            var inviterKey = KeyHelpers.CreateUserCacheKey(inviter.ID);
            Cache(inviterKey, inviter);
        }

        private void CacheMessage(object key, IMessage message)
        {
            CacheInstance(key, message);

            var authorKey = KeyHelpers.CreateUserCacheKey(message.Author.ID);
            Cache(authorKey, message.Author);

            if (!message.ReferencedMessage.HasValue || message.ReferencedMessage.Value is null)
            {
                return;
            }

            var referencedMessage = message.ReferencedMessage.Value;
            var referencedMessageKey = KeyHelpers.CreateMessageCacheKey
            (
                referencedMessage.ChannelID,
                referencedMessage.ID
            );

            Cache(referencedMessageKey, referencedMessage);
        }

        private void CacheChannel(object key, IChannel channel)
        {
            CacheInstance(key, channel);
            if (!channel.Recipients.HasValue)
            {
                return;
            }

            foreach (var recipient in channel.Recipients.Value!)
            {
                var recipientKey = KeyHelpers.CreateUserCacheKey(recipient.ID);
                Cache(recipientKey, recipient);
            }
        }

        /// <summary>
        /// Caches an instance using the default entry options for that instance type.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="instance">The instance.</param>
        /// <typeparam name="TInstance">The instance type.</typeparam>
        private void CacheInstance<TInstance>(object key, TInstance instance)
            where TInstance : class
        {
            _memoryCache.Set(key, instance, _cacheSettings.GetEntryOptions<TInstance>());
        }
    }
}
