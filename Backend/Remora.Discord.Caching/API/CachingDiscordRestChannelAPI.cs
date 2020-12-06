//
//  CachingDiscordRestChannelAPI.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Caching.Services;
using Remora.Discord.Core;
using Remora.Discord.Rest;
using Remora.Discord.Rest.API;

namespace Remora.Discord.Caching.API
{
    /// <summary>
    /// Implements a caching version of the channel API.
    /// </summary>
    public class CachingDiscordRestChannelAPI : DiscordRestChannelAPI
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheSettings _cacheSettings;

        /// <inheritdoc cref="DiscordRestChannelAPI" />
        public CachingDiscordRestChannelAPI
        (
            DiscordHttpClient discordHttpClient,
            IOptions<JsonSerializerOptions> jsonOptions,
            IMemoryCache memoryCache,
            CacheSettings cacheSettings
        )
            : base(discordHttpClient, jsonOptions)
        {
            _memoryCache = memoryCache;
            _cacheSettings = cacheSettings;
        }

        /// <inheritdoc />
        public override Task<IRetrieveRestEntityResult<IChannel>> GetChannelAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateChannelCacheKey(channelID);

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IChannel>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IChannel>();

                    return base.GetChannelAsync(channelID, ct);
                }
            );
        }

        /// <inheritdoc />
        public override async Task<IModifyRestEntityResult<IChannel>> ModifyChannelAsync
        (
            Snowflake channelID,
            Optional<string> name = default,
            Optional<ChannelType> type = default,
            Optional<int?> position = default,
            Optional<string?> topic = default,
            Optional<bool?> isNsfw = default,
            Optional<int?> rateLimitPerUser = default,
            Optional<int?> bitrate = default,
            Optional<int?> userLimit = default,
            Optional<IReadOnlyList<IPermissionOverwrite>?> permissionOverwrites = default,
            Optional<Snowflake?> parentId = default,
            CancellationToken ct = default
        )
        {
            var modificationResult = await base.ModifyChannelAsync
            (
                channelID,
                name,
                type,
                position,
                topic,
                isNsfw,
                rateLimitPerUser,
                bitrate,
                userLimit,
                permissionOverwrites,
                parentId,
                ct
            );

            if (!modificationResult.IsSuccess)
            {
                return modificationResult;
            }

            var channel = modificationResult.Entity;
            var key = KeyHelpers.CreateChannelCacheKey(channelID);
            _memoryCache.Set(key, channel);

            return modificationResult;
        }

        /// <inheritdoc />
        public override async Task<IDeleteRestEntityResult> DeleteChannelAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.DeleteChannelAsync(channelID, ct);

            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            var key = KeyHelpers.CreateChannelCacheKey(channelID);
            _memoryCache.Remove(key);

            return deleteResult;
        }

        /// <inheritdoc />
        public override Task<IRetrieveRestEntityResult<IMessage>> GetChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IMessage>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IMessage>();

                    return base.GetChannelMessageAsync(channelID, messageID, ct);
                }
            );
        }

        /// <inheritdoc />
        public override async Task<ICreateRestEntityResult<IMessage>> CreateMessageAsync
        (
            Snowflake channelID,
            Optional<string> content = default,
            Optional<string> nonce = default,
            Optional<bool> isTTS = default,
            Optional<Stream> file = default,
            Optional<IEmbed> embed = default,
            Optional<IAllowedMentions> allowedMentions = default,
            CancellationToken ct = default
        )
        {
            var createResult = await base.CreateMessageAsync
            (
                channelID,
                content,
                nonce,
                isTTS,
                file,
                embed,
                allowedMentions,
                ct
            );

            if (!createResult.IsSuccess)
            {
                return createResult;
            }

            var message = createResult.Entity;
            var key = KeyHelpers.CreateMessageCacheKey(channelID, message.ID);
            _memoryCache.Set(key, message);

            return createResult;
        }

        /// <inheritdoc />
        public override async Task<IModifyRestEntityResult<IMessage>> EditMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            Optional<string?> content = default,
            Optional<IEmbed?> embed = default,
            Optional<MessageFlags?> flags = default,
            CancellationToken ct = default
        )
        {
            var editResult = await base.EditMessageAsync(channelID, messageID, content, embed, flags, ct);

            if (!editResult.IsSuccess)
            {
                return editResult;
            }

            var message = editResult.Entity;
            var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
            _memoryCache.Set(key, message);

            return editResult;
        }

        /// <inheritdoc />
        public override async Task<IDeleteRestEntityResult> DeleteMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.DeleteMessageAsync(channelID, messageID, ct);

            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
            _memoryCache.Remove(key);

            return deleteResult;
        }

        /// <inheritdoc />
        public override async Task<IDeleteRestEntityResult> BulkDeleteMessagesAsync
        (
            Snowflake channelID,
            IReadOnlyList<Snowflake> messageIDs,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.BulkDeleteMessagesAsync(channelID, messageIDs, ct);

            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            foreach (var messageID in messageIDs)
            {
                var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
                _memoryCache.Remove(key);
            }

            return deleteResult;
        }

        /// <inheritdoc />
        public override async Task<ICreateRestEntityResult<IInvite>> CreateChannelInviteAsync
        (
            Snowflake channelID,
            Optional<TimeSpan> maxAge = default,
            Optional<int> maxUses = default,
            Optional<bool> isTemporary = default,
            Optional<bool> isUnique = default,
            Optional<Snowflake> targetUser = default,
            Optional<TargetUserType> targetUserType = default,
            CancellationToken ct = default
        )
        {
            var createResult = await base.CreateChannelInviteAsync
            (
                channelID,
                maxAge,
                maxUses,
                isTemporary,
                isUnique,
                targetUser,
                targetUserType,
                ct
            );

            if (!createResult.IsSuccess)
            {
                return createResult;
            }

            var invite = createResult.Entity;
            var key = KeyHelpers.CreateInviteCacheKey(channelID, invite.Code);
            _memoryCache.Set(key, invite);

            return createResult;
        }

        /// <inheritdoc />
        public override async Task<IDeleteRestEntityResult> DeleteChannelPermissionAsync
        (
            Snowflake channelID,
            Snowflake overwriteID,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.DeleteChannelPermissionAsync(channelID, overwriteID, ct);

            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            var key = KeyHelpers.CreateChannelPermissionCacheKey(channelID, overwriteID);
            _memoryCache.Remove(key);

            return deleteResult;
        }

        /// <inheritdoc />
        public override async Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            var getResult = await base.GetPinnedMessagesAsync(channelID, ct);

            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var messages = getResult.Entity;
            foreach (var message in messages)
            {
                var key = KeyHelpers.CreateMessageCacheKey(channelID, message.ID);
                _memoryCache.Set(key, message);
            }

            return getResult;
        }

        /// <inheritdoc />
        public override async Task<IDeleteRestEntityResult> DeletePinnedChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            var deleteResult = await base.DeletePinnedChannelMessageAsync(channelID, messageID, ct);

            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            var key = KeyHelpers.CreateMessageCacheKey(channelID, messageID);
            _memoryCache.Remove(key);

            return deleteResult;
        }
    }
}
