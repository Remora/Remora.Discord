//
//  DiscordRestUserAPI.cs
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

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestUserAPI" />
[PublicAPI]
public class DiscordRestUserAPI : AbstractDiscordRestAPI, IDiscordRestUserAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestUserAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestUserAPI(IRestHttpClient restHttpClient, JsonSerializerOptions jsonOptions, ICacheProvider rateLimitCache)
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual Task<Result<IUser>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        return this.RestHttpClient.GetAsync<IUser>
        (
            "users/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IUser>> GetUserAsync
    (
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IUser>
        (
            $"users/{userID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IUser>> ModifyCurrentUserAsync
    (
        Optional<string> username,
        Optional<Stream?> avatar = default,
        CancellationToken ct = default
    )
    {
        var packAvatar = await ImagePacker.PackImageAsync(avatar, ct);
        if (!packAvatar.IsSuccess)
        {
            return Result<IUser>.FromError(packAvatar);
        }

        var avatarData = packAvatar.Entity;

        return await this.RestHttpClient.PatchAsync<IUser>
        (
            "users/@me",
            b => b.WithJson
                (
                    json =>
                    {
                        json.Write("username", username, this.JsonOptions);
                        json.Write("avatar", avatarData, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<IPartialGuild>>> GetCurrentUserGuildsAsync
    (
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        if (limit.HasValue && limit.Value is < 1 or > 200)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(limit),
                "The limit must be between 1 and 200."
            );
        }

        return await this.RestHttpClient.GetAsync<IReadOnlyList<IPartialGuild>>
        (
            "users/@me/guilds",
            b =>
            {
                if (before.HasValue)
                {
                    b.AddQueryParameter("before", before.Value.ToString());
                }

                if (after.HasValue)
                {
                    b.AddQueryParameter("after", after.Value.ToString());
                }

                if (limit.HasValue)
                {
                    b.AddQueryParameter("limit", limit.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> LeaveGuildAsync(Snowflake guildID, CancellationToken ct = default)
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"users/@me/guilds/{guildID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc/>
    public virtual Task<Result<IGuildMember>> GetCurrentUserGuildMemberAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuildMember>
        (
            $"users/@me/guilds/{guildID}/member",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IChannel>>> GetUserDMsAsync
    (
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IChannel>>
        (
            "users/@me/channels",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> CreateDMAsync
    (
        Snowflake recipientID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<IChannel>
        (
            "users/@me/channels",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("recipient_id", recipientID.ToString());
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IConnection>>> GetUserConnectionsAsync
    (
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IConnection>>
        (
            "users/@me/connections",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }
}
