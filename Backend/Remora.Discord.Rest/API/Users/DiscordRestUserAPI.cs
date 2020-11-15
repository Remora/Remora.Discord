//
//  DiscordRestUserAPI.cs
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
    public class DiscordRestUserAPI : IDiscordRestUserAPI
    {
        private readonly DiscordHttpClient _discordHttpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestUserAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The json options.</param>
        public DiscordRestUserAPI(DiscordHttpClient discordHttpClient, IOptions<JsonSerializerOptions> jsonOptions)
        {
            _discordHttpClient = discordHttpClient;
            _jsonOptions = jsonOptions.Value;
        }

        /// <inheritdoc />
        public virtual Task<IRetrieveRestEntityResult<IUser>> GetCurrentUserAsync(CancellationToken ct = default)
        {
            return _discordHttpClient.GetAsync<IUser>
            (
                "users/@me",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<IRetrieveRestEntityResult<IUser>> GetUserAsync
        (
            Snowflake userID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IUser>
            (
                $"users/{userID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<IModifyRestEntityResult<IUser>> ModifyCurrentUserAsync
        (
            Optional<string> username,
            Optional<Stream?> avatar = default,
            CancellationToken ct = default
        )
        {
            Optional<string?> avatarData = default;
            if (avatar.HasValue)
            {
                if (avatar.Value is null)
                {
                    avatarData = new Optional<string?>(null);
                }
                else
                {
                    var packImage = await ImagePacker.PackImageAsync(avatar.Value, ct);
                    if (!packImage.IsSuccess)
                    {
                        return ModifyRestEntityResult<IUser>.FromError(packImage);
                    }

                    avatarData = packImage.Entity;
                }
            }

            return await _discordHttpClient.PatchAsync<IUser>
            (
                "users/@me",
                b => b.WithJson
                (
                    json =>
                    {
                        json.Write("username", username, _jsonOptions);
                        json.Write("avatar", avatarData, _jsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual async Task<IRetrieveRestEntityResult<IReadOnlyList<IPartialGuild>>> GetCurrentUserGuildsAsync
        (
            Optional<Snowflake> before = default,
            Optional<Snowflake> after = default,
            Optional<int> limit = default,
            CancellationToken ct = default
        )
        {
            if (limit.HasValue && (limit.Value < 1 || limit.Value > 100))
            {
                return RetrieveRestEntityResult<IReadOnlyList<IPartialGuild>>.FromError
                (
                    "The limit must be between 1 and 100."
                );
            }

            return await _discordHttpClient.GetAsync<IReadOnlyList<IPartialGuild>>
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
                },
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<IDeleteRestEntityResult> LeaveGuildAsync(Snowflake guildID, CancellationToken ct = default)
        {
            return _discordHttpClient.DeleteAsync
            (
                $"users/@me/guilds/{guildID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<IRetrieveRestEntityResult<IReadOnlyList<IChannel>>> GetUserDMsAsync
        (
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IReadOnlyList<IChannel>>
            (
                "users/@me/channels",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<ICreateRestEntityResult<IChannel>> CreateDMAsync
        (
            Snowflake recipientID,
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.PostAsync<IChannel>
            (
                "users/@me/channels",
                b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("recipient_id", recipientID.ToString());
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<IRetrieveRestEntityResult<IReadOnlyList<IConnection>>> GetUserConnectionsAsync
        (
            CancellationToken ct = default
        )
        {
            return _discordHttpClient.GetAsync<IReadOnlyList<IConnection>>
            (
                "users/@me/connections",
                ct: ct
            );
        }
    }
}
