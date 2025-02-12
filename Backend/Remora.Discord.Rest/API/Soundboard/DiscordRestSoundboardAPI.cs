//
//  DiscordRestSoundboardAPI.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects.Soundboard;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestSoundboardAPI" />
[PublicAPI]
public class DiscordRestSoundboardAPI : AbstractDiscordRestAPI, IDiscordRestSoundboardAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestSoundboardAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestSoundboardAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public Task<Result> SendSoundboardSoundAsync
    (
        Snowflake channelID,
        Snowflake soundID,
        Optional<Snowflake> sourceGuildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync
        (
            $"channels/{channelID}/send-soundboard-sound",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("sound_id", soundID, this.JsonOptions);
                        json.Write("source_guild_id", sourceGuildID, this.JsonOptions);
                    }
                );

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<ISoundboardSound>>> ListDefaultSoundboardSoundsAsync
    (
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<ISoundboardSound>>
        (
            "soundboard-default-sounds",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IListGuildSoundboardSoundsResponse>> ListGuildSoundboardSoundsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IListGuildSoundboardSoundsResponse>
        (
            $"guilds/{guildID}/soundboard-sounds",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<ISoundboardSound>> GetGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        Snowflake soundID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<ISoundboardSound>
        (
            $"guilds/{guildID}/soundboard-sounds/{soundID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<ISoundboardSound>> CreateGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        string name,
        byte[] sound,
        Optional<double?> volume = default,
        Optional<Snowflake?> emojiID = default,
        Optional<string?> emojiName = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<ISoundboardSound>
        (
            $"guilds/{guildID}/soundboard-sounds",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("sound", sound, this.JsonOptions);
                        json.Write("volume", volume, this.JsonOptions);
                        json.Write("emoji_id", emojiID, this.JsonOptions);
                        json.Write("emoji_name", emojiName, this.JsonOptions);
                    }
                );

                b.AddAuditLogReason(reason);
                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<ISoundboardSound>> ModifyGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        Snowflake soundID,
        string name,
        Optional<double?> volume = default,
        Optional<Snowflake?> emojiID = default,
        Optional<string?> emojiName = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync<ISoundboardSound>
        (
            $"guilds/{guildID}/soundboard-sounds/{soundID}",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("volume", volume, this.JsonOptions);
                        json.Write("emoji_id", emojiID, this.JsonOptions);
                        json.Write("emoji_name", emojiName, this.JsonOptions);
                    }
                );

                b.AddAuditLogReason(reason);
                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result> DeleteGuildSoundboardSoundAsync
    (
        Snowflake guildID,
        Snowflake soundID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/soundboard-sounds/{soundID}",
            b =>
            {
                b.AddAuditLogReason(reason);
                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }
}
