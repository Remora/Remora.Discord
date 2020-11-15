//
//  CachingDiscordRestAuditLogAPI.cs
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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Caching.Services;
using Remora.Discord.Core;
using Remora.Discord.Rest;
using Remora.Discord.Rest.API;

namespace Remora.Discord.Caching.API.AuditLog
{
    /// <summary>
    /// Implements a caching version of the audit log API.
    /// </summary>
    public class CachingDiscordRestAuditLogAPI : DiscordRestAuditLogAPI
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheSettings _cacheSettings;

        /// <inheritdoc cref="DiscordRestAuditLogAPI"/>
        public CachingDiscordRestAuditLogAPI
        (
            DiscordHttpClient discordHttpClient,
            IMemoryCache memoryCache,
            CacheSettings cacheSettings
        )
            : base(discordHttpClient)
        {
            _memoryCache = memoryCache;
            _cacheSettings = cacheSettings;
        }

        /// <inheritdoc />
        public override Task<IRetrieveRestEntityResult<IAuditLog>> GetAuditLogAsync
        (
            Snowflake guildID,
            Optional<Snowflake> userID = default,
            Optional<AuditLogEvent> actionType = default,
            Optional<Snowflake> before = default,
            Optional<byte> limit = default,
            CancellationToken ct = default
        )
        {
            var key = (nameof(GetAuditLogAsync), guildID, new { userID, actionType, before, limit });
            return _memoryCache
            .GetOrCreateAsync
            (
                key,
                entry =>
                {
                    entry.SlidingExpiration = _cacheSettings
                        .GetSlidingExpirationOrDefault<IAuditLog>();

                    entry.AbsoluteExpirationRelativeToNow = _cacheSettings
                        .GetAbsoluteExpirationOrDefault<IAuditLog>();

                    return base.GetAuditLogAsync(guildID, userID, actionType, before, limit, ct);
                }
            );
        }
    }
}
