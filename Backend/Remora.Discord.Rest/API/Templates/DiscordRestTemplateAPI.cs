//
//  DiscordRestTemplateAPI.cs
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
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Core;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Rest.Results;
using Remora.Discord.Rest.Utility;

namespace Remora.Discord.Rest.API
{
    /// <inheritdoc />
    [PublicAPI]
    public class DiscordRestTemplateAPI : IDiscordRestTemplateAPI
    {
        private readonly DiscordHttpClient _discordHttpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestTemplateAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The Json options.</param>
        public DiscordRestTemplateAPI(DiscordHttpClient discordHttpClient, IOptions<JsonSerializerOptions> jsonOptions)
        {
            _discordHttpClient = discordHttpClient;
            _jsonOptions = jsonOptions.Value;
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<ITemplate>> GetTemplateAsync
        (
            string templateCode,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<ITemplate>
            (
                $"guilds/templates/{templateCode}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public async Task<ICreateRestEntityResult<IGuild>> CreateGuildFromTemplateAsync
        (
            string templateCode,
            string name,
            Optional<Stream> icon = default,
            CancellationToken ct = default
        )
        {
            string? iconDataString = null;
            if (icon.HasValue)
            {
                var packicon = await ImagePacker.PackImageAsync(icon.Value!, ct);
                if (!packicon.IsSuccess)
                {
                    return CreateRestEntityResult<IGuild>.FromError(packicon);
                }

                iconDataString = packicon.Entity;
            }

            return await _discordHttpClient.PostAsync<IGuild>
            (
                $"guilds/templates/{templateCode}",
                b => b.WithJson
                (
                    j =>
                    {
                        j.WriteString("name", name);

                        if (iconDataString is not null)
                        {
                            j.WriteString("icon", iconDataString);
                        }
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IRetrieveRestEntityResult<IReadOnlyList<ITemplate>>> GetGuildTemplatesAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IReadOnlyList<ITemplate>>
            (
                $"guilds/{guildID}/templates",
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<ICreateRestEntityResult<ITemplate>> CreateGuildTemplateAsync
        (
            Snowflake guildID,
            string name,
            Optional<string?> description = default,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PostAsync<ITemplate>
            (
                $"guilds/{guildID}/templates",
                b => b.WithJson
                (
                    j =>
                    {
                        j.WriteString("name", name);
                        j.Write("description", description, _jsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public async Task<IRetrieveRestEntityResult<ITemplate>> SyncGuildTemplateAsync
        (
            Snowflake guildID,
            string templateCode,
            CancellationToken ct = default
        )
        {
            var createResult = await _discordHttpClient.PutAsync<ITemplate>
            (
                $"guilds/{guildID}/templates/{templateCode}",
                ct: ct
            );

            return createResult.IsSuccess
                ? RetrieveRestEntityResult<ITemplate>.FromSuccess(createResult.Entity)
                : RetrieveRestEntityResult<ITemplate>.FromError(createResult);
        }

        /// <inheritdoc />
        public Task<IModifyRestEntityResult<ITemplate>> ModifyGuildTemplateAsync
        (
            Snowflake guildID,
            string templateCode,
            string name,
            Optional<string> description,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PatchAsync<ITemplate>
            (
                $"guilds/{guildID}/templates/{templateCode}",
                b => b.WithJson
                (
                    j =>
                    {
                        j.WriteString("name", name);
                        j.Write("description", description, _jsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public Task<IDeleteRestEntityResult<ITemplate>> DeleteGuildTemplateAsync
        (
            Snowflake guildID,
            string templateCode,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.DeleteAsync<ITemplate>
            (
                $"guilds/{guildID}/templates/{templateCode}",
                ct: ct
            );
        }
    }
}
