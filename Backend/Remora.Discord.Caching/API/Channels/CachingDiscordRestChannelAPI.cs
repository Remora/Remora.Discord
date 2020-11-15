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

using System.Collections.Generic;
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

namespace Remora.Discord.Caching.API.Channels
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
            var key = (nameof(GetChannelAsync), channelID);

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
        public override Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetChannelMessagesAsync
        (
            Snowflake channelID,
            Optional<Snowflake> around = default,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            var key = (nameof(GetChannelMessagesAsync), channelID, new { around, before, after, limit });

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IReadOnlyList<IMessage>>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IReadOnlyList<IMessage>>();

                    return base.GetChannelMessagesAsync(channelID, around, before, after, limit, ct);
                }
            );
        }

        /// <inheritdoc />
        public override Task<IRetrieveRestEntityResult<IMessage>> GetChannelMessageAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            CancellationToken ct = default
        )
        {
            var key = (nameof(GetChannelMessageAsync), channelID, messageID);

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
        public override Task<IRetrieveRestEntityResult<IReadOnlyList<IUser>>> GetReactionsAsync
        (
            Snowflake channelID,
            Snowflake messageID,
            string emoji,
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            var key = (nameof(GetReactionsAsync), channelID, messageID, emoji, new { before, after, limit });

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IReadOnlyList<IUser>>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IReadOnlyList<IUser>>();

                    return base.GetReactionsAsync(channelID, messageID, emoji, before, after, limit, ct);
                }
            );
        }

        /// <inheritdoc />
        public override Task<IRetrieveRestEntityResult<IReadOnlyList<IInvite>>> GetChannelInvitesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            var key = (nameof(GetChannelInvitesAsync), channelID);

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IReadOnlyList<IUser>>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IReadOnlyList<IUser>>();

                    return base.GetChannelInvitesAsync(channelID, ct);
                }
            );
        }

        /// <inheritdoc />
        public override Task<IRetrieveRestEntityResult<IReadOnlyList<IMessage>>> GetPinnedMessagesAsync
        (
            Snowflake channelID,
            CancellationToken ct = default
        )
        {
            var key = (nameof(GetPinnedMessagesAsync), channelID);

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IReadOnlyList<IMessage>>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IReadOnlyList<IMessage>>();

                    return base.GetPinnedMessagesAsync(channelID, ct);
                }
            );
        }
    }
}
