//
//  DiscordRestStickerAPI.cs
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
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;
using Remora.Discord.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestStickerAPI" />
    [PublicAPI]
    public class DiscordRestStickerAPI : AbstractDiscordRestAPI, IDiscordRestStickerAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestStickerAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The JSON options.</param>
        public DiscordRestStickerAPI
        (
            DiscordHttpClient discordHttpClient,
            IOptions<JsonSerializerOptions> jsonOptions
        )
            : base(discordHttpClient, jsonOptions)
        {
        }

        /// <inheritdoc />
        public virtual Task<Result<ISticker>> GetStickerAsync(Snowflake id, CancellationToken ct = default)
        {
            return this.DiscordHttpClient.GetAsync<ISticker>($"stickers/{id}", ct: ct);
        }

        /// <inheritdoc />
        public virtual Task<Result<INitroStickerPacks>> ListNitroStickerPacksAsync(CancellationToken ct = default)
        {
            return this.DiscordHttpClient.GetAsync<INitroStickerPacks>("sticker-packs", ct: ct);
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<ISticker>>> ListGuildStickersAsync
        (
            Snowflake guildId,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<ISticker>>($"guilds/{guildId}/stickers", ct: ct);
        }

        /// <inheritdoc />
        public virtual Task<Result<ISticker>> GetGuildStickerAsync
        (
            Snowflake guildId,
            Snowflake stickerId,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<ISticker>($"guilds/{guildId}/stickers/{stickerId}", ct: ct);
        }

        /// <inheritdoc />
        public virtual async Task<Result<ISticker>> CreateGuildStickerAsync
        (
            Snowflake guildId,
            string name,
            string description,
            string tags,
            FileData file,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (name.Length is < 2 or > 30)
            {
                return new ArgumentOutOfRangeError(nameof(name), "The name must be between 2 and 30 characters.");
            }

            if (description.Length is not 0 and (< 2 or > 30))
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(description), "The description must be either empty, or between 2 and 30 characters."
                );
            }

            if (tags.Length is < 2 or > 200)
            {
                return new ArgumentOutOfRangeError(nameof(name), "The tags must be between 2 and 200 characters.");
            }

            return await this.DiscordHttpClient.PostAsync<ISticker>
            (
                $"guilds/{guildId}/stickers",
                b => b
                    .AddAuditLogReason(reason)
                    .AddContent(new StringContent(name), nameof(name))
                    .AddContent(new StringContent(description), nameof(description))
                    .AddContent(new StringContent(tags), nameof(tags))
                    .AddContent(new StreamContent(file.Content), nameof(file), file.Name),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<ISticker>> ModifyGuildStickerAsync
        (
            Snowflake guildId,
            Snowflake stickerId,
            Optional<string> name = default,
            Optional<string?> description = default,
            Optional<string> tags = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (name.HasValue && name.Value.Length is < 2 or > 30)
            {
                return new ArgumentOutOfRangeError(nameof(name), "The name must be between 2 and 30 characters.");
            }

            if (description.HasValue)
            {
                if (description.Value is not null && description.Value.Length is not 0 and (< 2 or > 30))
                {
                    return new ArgumentOutOfRangeError
                    (
                        nameof(description), "The description must be either empty, or between 2 and 30 characters."
                    );
                }
            }

            if (tags.HasValue && tags.Value.Length is < 2 or > 200)
            {
                return new ArgumentOutOfRangeError(nameof(name), "The tags must be between 2 and 200 characters.");
            }

            return await this.DiscordHttpClient.PatchAsync<ISticker>
            (
                $"guilds/{guildId}/stickers/{stickerId}",
                b => b
                    .AddAuditLogReason(reason)
                    .WithJson
                    (
                        json =>
                        {
                            json.Write("name", name, this.JsonOptions);
                            json.Write("description", description, this.JsonOptions);
                            json.Write("tags", tags, this.JsonOptions);
                        }
                    ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteGuildStickerAsync
        (
            Snowflake guildId,
            Snowflake stickerId,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"guilds/{guildId}/stickers/{stickerId}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }
    }
}
