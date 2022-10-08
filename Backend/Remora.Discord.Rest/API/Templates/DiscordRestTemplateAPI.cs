//
//  DiscordRestTemplateAPI.cs
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
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Rest.Utility;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestTemplateAPI" />
[PublicAPI]
public class DiscordRestTemplateAPI : AbstractDiscordRestAPI, IDiscordRestTemplateAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestTemplateAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestTemplateAPI(IRestHttpClient restHttpClient, JsonSerializerOptions jsonOptions, ICacheProvider rateLimitCache)
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual Task<Result<ITemplate>> GetTemplateAsync
    (
        string templateCode,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<ITemplate>
        (
            $"guilds/templates/{templateCode}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IGuild>> CreateGuildFromTemplateAsync
    (
        string templateCode,
        string name,
        Optional<Stream> icon = default,
        CancellationToken ct = default
    )
    {
        if (name.Length is < 2 or > 100)
        {
            return new ArgumentOutOfRangeError(nameof(name), "The name must be between 2 and 100 characters.");
        }

        var packIcon = await ImagePacker.PackImageAsync(icon!, ct);
        if (!packIcon.IsDefined(out var iconData))
        {
            return Result<IGuild>.FromError(packIcon);
        }

        return await this.RestHttpClient.PostAsync<IGuild>
        (
            $"guilds/templates/{templateCode}",
            b => b.WithJson
                (
                    j =>
                    {
                        j.WriteString("name", name);
                        j.Write("icon", iconData);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<ITemplate>>> GetGuildTemplatesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<ITemplate>>
        (
            $"guilds/{guildID}/templates",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<ITemplate>> CreateGuildTemplateAsync
    (
        Snowflake guildID,
        string name,
        Optional<string?> description = default,
        CancellationToken ct = default
    )
    {
        if (name.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters.");
        }

        if (description.IsDefined(out var descriptionValue) && descriptionValue.Length > 120)
        {
            return new ArgumentOutOfRangeError(nameof(description), "The description must be between 0 and 120 characters.");
        }

        return await this.RestHttpClient.PostAsync<ITemplate>
        (
            $"guilds/{guildID}/templates",
            b => b.WithJson
                (
                    j =>
                    {
                        j.WriteString("name", name);
                        j.Write("description", description, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<ITemplate>> SyncGuildTemplateAsync
    (
        Snowflake guildID,
        string templateCode,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PutAsync<ITemplate>
        (
            $"guilds/{guildID}/templates/{templateCode}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<ITemplate>> ModifyGuildTemplateAsync
    (
        Snowflake guildID,
        string templateCode,
        Optional<string> name,
        Optional<string?> description,
        CancellationToken ct = default
    )
    {
        if (name.IsDefined(out var nameValue) && nameValue.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError(nameof(name), "The name must be between 1 and 100 characters.");
        }

        if (description.IsDefined(out var descriptionValue) && descriptionValue.Length > 120)
        {
            return new ArgumentOutOfRangeError(nameof(description), "The description must be between 0 and 120 characters.");
        }

        return await this.RestHttpClient.PatchAsync<ITemplate>
        (
            $"guilds/{guildID}/templates/{templateCode}",
            b => b.WithJson
                (
                    j =>
                    {
                        j.Write("name", name, this.JsonOptions);
                        j.Write("description", description, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<ITemplate>> DeleteGuildTemplateAsync
    (
        Snowflake guildID,
        string templateCode,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync<ITemplate>
        (
            $"guilds/{guildID}/templates/{templateCode}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }
}
