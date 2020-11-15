//
//  CachingDiscordRestEmojiAPI.cs
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

namespace Remora.Discord.Caching.API.Emoji
{
    /// <summary>
    /// Implements a caching version of the channel API.
    /// </summary>
    public class CachingDiscordRestEmojiAPI : DiscordRestEmojiAPI
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheSettings _cacheSettings;

        /// <inheritdoc cref="DiscordRestEmojiAPI" />
        public CachingDiscordRestEmojiAPI
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
        public override Task<IRetrieveRestEntityResult<IReadOnlyList<IEmoji>>> ListGuildEmojisAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var key = (nameof(ListGuildEmojisAsync), guildID);

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IReadOnlyList<IEmoji>>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IReadOnlyList<IEmoji>>();

                    return base.ListGuildEmojisAsync(guildID, ct);
                }
            );
        }

        /// <inheritdoc />
        public override Task<IRetrieveRestEntityResult<IEmoji>> GetGuildEmojiAsync
        (
            Snowflake guildID,
            Snowflake emojiID,
            CancellationToken ct = default
        )
        {
            var key = (nameof(GetGuildEmojiAsync), guildID, emojiID);

            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IEmoji>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IEmoji>();

                    return base.GetGuildEmojiAsync(guildID, emojiID, ct);
                }
            );
        }
    }
}
