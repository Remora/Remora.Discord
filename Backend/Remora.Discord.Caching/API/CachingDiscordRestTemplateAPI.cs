//
//  CachingDiscordRestTemplateAPI.cs
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
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Services;
using Remora.Discord.Core;
using Remora.Discord.Rest;
using Remora.Discord.Rest.API;
using Remora.Results;

namespace Remora.Discord.Caching.API
{
    /// <inheritdoc />
    public class CachingDiscordRestTemplateAPI : DiscordRestTemplateAPI
    {
        private readonly CacheService _cacheService;

        /// <inheritdoc cref="DiscordRestTemplateAPI(DiscordHttpClient, IOptions{JsonSerializerOptions})" />
        public CachingDiscordRestTemplateAPI
        (
            DiscordHttpClient discordHttpClient,
            IOptions<JsonSerializerOptions> jsonOptions,
            CacheService cacheService
        )
            : base(discordHttpClient, jsonOptions)
        {
            _cacheService = cacheService;
        }

        /// <inheritdoc />
        public override async Task<Result<ITemplate>> GetTemplateAsync
        (
            string templateCode,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateTemplateCacheKey(templateCode);
            if (_cacheService.TryGetValue<ITemplate>(key, out var cachedInstance))
            {
                return Result<ITemplate>.FromSuccess(cachedInstance);
            }

            var getTemplate = await base.GetTemplateAsync(templateCode, ct);
            if (!getTemplate.IsSuccess)
            {
                return getTemplate;
            }

            var template = getTemplate.Entity;
            _cacheService.Cache(key, template);

            return getTemplate;
        }

        /// <inheritdoc />
        public override async Task<Result<ITemplate>> CreateGuildTemplateAsync
        (
            Snowflake guildID,
            string name,
            Optional<string?> description = default,
            CancellationToken ct = default
        )
        {
            var createTemplate = await base.CreateGuildTemplateAsync(guildID, name, description, ct);
            if (!createTemplate.IsSuccess)
            {
                return createTemplate;
            }

            var template = createTemplate.Entity;
            var key = KeyHelpers.CreateTemplateCacheKey(template.Code);

            _cacheService.Cache(key, template);

            return createTemplate;
        }

        /// <inheritdoc />
        public override async Task<Result<ITemplate>> DeleteGuildTemplateAsync
        (
            Snowflake guildID,
            string templateCode,
            CancellationToken ct = default
        )
        {
            var deleteTemplate = await base.DeleteGuildTemplateAsync(guildID, templateCode, ct);
            if (!deleteTemplate.IsSuccess)
            {
                return deleteTemplate;
            }

            var key = KeyHelpers.CreateTemplateCacheKey(templateCode);
            _cacheService.Evict(key);

            return deleteTemplate;
        }

        /// <inheritdoc />
        public override async Task<Result<IReadOnlyList<ITemplate>>> GetGuildTemplatesAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            var key = KeyHelpers.CreateGuildTemplatesCacheKey(guildID);
            if (_cacheService.TryGetValue<IReadOnlyList<ITemplate>>(key, out var cachedInstance))
            {
                return Result<IReadOnlyList<ITemplate>>.FromSuccess(cachedInstance);
            }

            var getTemplates = await base.GetGuildTemplatesAsync(guildID, ct);
            if (!getTemplates.IsSuccess)
            {
                return getTemplates;
            }

            var templates = getTemplates.Entity;
            _cacheService.Cache(key, templates);

            foreach (var template in templates)
            {
                var templateKey = KeyHelpers.CreateTemplateCacheKey(template.Code);
                _cacheService.Cache(templateKey, template);
            }

            return getTemplates;
        }

        /// <inheritdoc />
        public override async Task<Result<ITemplate>> ModifyGuildTemplateAsync
        (
            Snowflake guildID,
            string templateCode,
            string name,
            Optional<string> description,
            CancellationToken ct = default
        )
        {
            var modifyTemplate = await base.ModifyGuildTemplateAsync(guildID, templateCode, name, description, ct);
            if (!modifyTemplate.IsSuccess)
            {
                return modifyTemplate;
            }

            var template = modifyTemplate.Entity;
            var key = KeyHelpers.CreateTemplateCacheKey(templateCode);

            _cacheService.Cache(key, template);

            return modifyTemplate;
        }

        /// <inheritdoc />
        public override async Task<Result<ITemplate>> SyncGuildTemplateAsync
        (
            Snowflake guildID,
            string templateCode,
            CancellationToken ct = default
        )
        {
            var syncTemplate = await base.SyncGuildTemplateAsync(guildID, templateCode, ct);
            if (!syncTemplate.IsSuccess)
            {
                return syncTemplate;
            }

            var template = syncTemplate.Entity;
            var key = KeyHelpers.CreateTemplateCacheKey(templateCode);

            _cacheService.Cache(key, template);

            return syncTemplate;
        }

        /// <inheritdoc />
        public override async Task<Result<IGuild>> CreateGuildFromTemplateAsync
        (
            string templateCode,
            string name,
            Optional<Stream> icon = default,
            CancellationToken ct = default
        )
        {
            var createGuild = await base.CreateGuildFromTemplateAsync(templateCode, name, icon, ct);
            if (!createGuild.IsSuccess)
            {
                return createGuild;
            }

            var guild = createGuild.Entity;
            var key = KeyHelpers.CreateGuildCacheKey(guild.ID);

            _cacheService.Cache(key, guild);

            return createGuild;
        }
    }
}
