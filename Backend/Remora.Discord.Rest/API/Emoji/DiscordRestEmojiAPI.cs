//
//  DiscordRestEmojiAPI.cs
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
using Remora.Discord.Core;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Rest.Utility;
using Remora.Results;

namespace Remora.Discord.Rest.API
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestEmojiAPI" />
    [PublicAPI]
    public class DiscordRestEmojiAPI : AbstractDiscordRestAPI, IDiscordRestEmojiAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestEmojiAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The JSON options.</param>
        public DiscordRestEmojiAPI(DiscordHttpClient discordHttpClient, IOptions<JsonSerializerOptions> jsonOptions)
            : base(discordHttpClient, jsonOptions)
        {
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<IEmoji>>> ListGuildEmojisAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IEmoji>>
            (
                $"guilds/{guildID}/emojis",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IEmoji>> GetGuildEmojiAsync
        (
            Snowflake guildID,
            Snowflake emojiID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IEmoji>
            (
                $"guilds/{guildID}/emojis/{emojiID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<Result<IEmoji>> CreateGuildEmojiAsync
        (
            Snowflake guildID,
            string name,
            Stream image,
            IReadOnlyList<Snowflake> roles,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            if (image.Length > 256000)
            {
                return new NotSupportedError("Image too large (max 256k).");
            }

            var packImage = await ImagePacker.PackImageAsync(image, ct);
            if (!packImage.IsSuccess)
            {
                return Result<IEmoji>.FromError(packImage);
            }

            var emojiData = packImage.Entity;

            return await this.DiscordHttpClient.PostAsync<IEmoji>
            (
                $"guilds/{guildID}/emojis",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.WriteString("image", emojiData);

                        json.WritePropertyName("roles");
                        JsonSerializer.Serialize(json, roles, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IEmoji>> ModifyGuildEmojiAsync
        (
            Snowflake guildID,
            Snowflake emojiID,
            Optional<string> name = default,
            Optional<IReadOnlyList<Snowflake>?> roles = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync<IEmoji>
            (
                $"guilds/{guildID}/emojis/{emojiID}",
                b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("roles", roles, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteGuildEmojiAsync
        (
            Snowflake guildID,
            Snowflake emojiID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"guilds/{guildID}/emojis/{emojiID}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }
    }
}