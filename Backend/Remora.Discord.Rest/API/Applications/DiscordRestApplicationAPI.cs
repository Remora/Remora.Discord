//
//  DiscordRestApplicationAPI.cs
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
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.API;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestApplicationAPI" />
[PublicAPI]
public class DiscordRestApplicationAPI : AbstractDiscordRestAPI, IDiscordRestApplicationAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestApplicationAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestApplicationAPI(IRestHttpClient restHttpClient, JsonSerializerOptions jsonOptions, ICacheProvider rateLimitCache)
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IApplicationCommand>>> GetGlobalApplicationCommandsAsync
    (
        Snowflake applicationID,
        CancellationToken ct
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IApplicationCommand>>
        (
            $"applications/{applicationID}/commands",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IApplicationCommand>> CreateGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        string name,
        string description,
        Optional<IReadOnlyList<IApplicationCommandOption>> options,
        Optional<bool> defaultPermission,
        Optional<ApplicationCommandType> type,
        CancellationToken ct
    )
    {
        if (name.Length is < 1 or > 32)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 32 characters."
            );
        }

        if (!type.IsDefined() || type.Value is ApplicationCommandType.ChatInput)
        {
            if (description.Length is < 1 or > 100)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(description),
                    "The description must be between 1 and 100 characters."
                );
            }
        }
        else
        {
            description = string.Empty;
        }

        return await this.RestHttpClient.PostAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/commands",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.Write("type", type, this.JsonOptions);
                        json.WriteString("description", description);
                        json.Write("options", options, this.JsonOptions);
                        json.Write("default_permission", defaultPermission, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<IApplicationCommand>>> BulkOverwriteGlobalApplicationCommandsAsync
    (
        Snowflake applicationID,
        IReadOnlyList<IBulkApplicationCommandData> commands,
        CancellationToken ct = default)
    {
        if (commands.Any(c => c.Name.Length is < 1 or > 32))
        {
            return new ArgumentOutOfRangeError
            (
                nameof(commands),
                "Command names must be between 1 and 32 characters."
            );
        }

        if
        (
            commands.Any
            (
                c =>
                    (!c.Type.IsDefined(out var type) || type is ApplicationCommandType.ChatInput) &&
                    c.Description.IsDefined(out var description) && description.Length is < 1 or > 100
            )
        )
        {
            return new ArgumentOutOfRangeError
            (
                nameof(commands),
                "Command descriptions must be between 1 and 100 characters."
            );
        }

        return await this.RestHttpClient.PutAsync<IReadOnlyList<IApplicationCommand>>
        (
            $"applications/{applicationID}/commands",
            b => b.WithJsonArray
                (
                    json =>
                    {
                        foreach (var command in commands)
                        {
                            json.WriteStartObject();
                            json.WriteString("name", command.Name);
                            json.Write("type", command.Type, this.JsonOptions);

                            if (!command.Type.IsDefined(out var type) || type is ApplicationCommandType.ChatInput)
                            {
                                json.Write("description", command.Description, this.JsonOptions);
                            }

                            json.Write("options", command.Options, this.JsonOptions);
                            json.Write("default_permission", command.DefaultPermission);
                            json.WriteEndObject();
                        }
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IApplicationCommand>> GetGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake commandID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/commands/{commandID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IApplicationCommand>> EditGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake commandID,
        Optional<string> name,
        Optional<string> description,
        Optional<IReadOnlyList<IApplicationCommandOption>?> options,
        Optional<bool> defaultPermission,
        CancellationToken ct
    )
    {
        if (name.HasValue && name.Value.Length is < 1 or > 32)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 32 characters."
            );
        }

        if (description.HasValue && description.Value.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(description),
                "The description must be between 1 and 100 characters."
            );
        }

        return await this.RestHttpClient.PatchAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/commands/{commandID}",
            b => b
                .WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write("options", options, this.JsonOptions);
                        json.Write("default_permission", defaultPermission, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> DeleteGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake commandID,
        CancellationToken ct
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"applications/{applicationID}/commands/{commandID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IApplicationCommand>>> GetGuildApplicationCommandsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        CancellationToken ct
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IApplicationCommand>>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<IApplicationCommand>>>
        BulkOverwriteGuildApplicationCommandsAsync
        (
            Snowflake applicationID,
            Snowflake guildID,
            IReadOnlyList<IBulkApplicationCommandData> commands,
            CancellationToken ct = default
        )
    {
        if (commands.Any(c => c.Name.Length is < 1 or > 32))
        {
            return new ArgumentOutOfRangeError
            (
                nameof(commands),
                "Command names must be between 1 and 32 characters."
            );
        }

        if
        (
            commands.Any
            (
                c =>
                    (!c.Type.IsDefined(out var type) || type is ApplicationCommandType.ChatInput) &&
                    c.Description.IsDefined(out var description) && description.Length is < 1 or > 100
            )
        )
        {
            return new ArgumentOutOfRangeError
            (
                nameof(commands),
                "Command descriptions must be between 1 and 100 characters."
            );
        }

        return await this.RestHttpClient.PutAsync<IReadOnlyList<IApplicationCommand>>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands",
            b => b.WithJsonArray
                (
                    json =>
                    {
                        foreach (var command in commands)
                        {
                            json.WriteStartObject();
                            json.WriteString("name", command.Name);
                            json.Write("type", command.Type, this.JsonOptions);

                            if (!command.Type.IsDefined(out var type) || type is ApplicationCommandType.ChatInput)
                            {
                                json.Write("description", command.Description, this.JsonOptions);
                            }

                            json.Write("options", command.Options, this.JsonOptions);
                            json.Write("default_permission", command.DefaultPermission);
                            json.WriteEndObject();
                        }
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IApplicationCommand>> CreateGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        string name,
        string description,
        Optional<IReadOnlyList<IApplicationCommandOption>> options,
        Optional<bool> defaultPermission,
        Optional<ApplicationCommandType> type,
        CancellationToken ct
    )
    {
        if (name.Length is < 1 or > 32)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 32 characters."
            );
        }

        if (!type.IsDefined() || type.Value is ApplicationCommandType.ChatInput)
        {
            if (description.Length is < 1 or > 100)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(description),
                    "The description must be between 1 and 100 characters."
                );
            }
        }
        else
        {
            description = string.Empty;
        }

        return await this.RestHttpClient.PostAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.Write("type", type, this.JsonOptions);
                        json.WriteString("description", description);
                        json.Write("options", options, this.JsonOptions);
                        json.Write("default_permission", defaultPermission, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IApplicationCommand>> GetGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands/{commandID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IApplicationCommand>> EditGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        Optional<string> name,
        Optional<string> description,
        Optional<IReadOnlyList<IApplicationCommandOption>?> options,
        Optional<bool> defaultPermission,
        CancellationToken ct
    )
    {
        if (name.HasValue && name.Value.Length is < 1 or > 32)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 32 characters."
            );
        }

        if (description.HasValue && description.Value.Length is < 1 or > 100)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(description),
                "The description must be between 1 and 100 characters."
            );
        }

        return await this.RestHttpClient.PatchAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands/{commandID}",
            b => b.WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write("options", options, this.JsonOptions);
                        json.Write("default_permission", defaultPermission, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> DeleteGuildApplicationCommandAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        CancellationToken ct
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"applications/{applicationID}/guilds/{guildID}/commands/{commandID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IGuildApplicationCommandPermissions>>>
        GetGuildApplicationCommandPermissionsAsync
        (
            Snowflake applicationID,
            Snowflake guildID,
            CancellationToken ct = default
        )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IGuildApplicationCommandPermissions>>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands/permissions",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuildApplicationCommandPermissions>> GetApplicationCommandPermissionsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuildApplicationCommandPermissions>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands/{commandID}/permissions",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuildApplicationCommandPermissions>> EditApplicationCommandPermissionsAsync
    (
        Snowflake applicationID,
        Snowflake guildID,
        Snowflake commandID,
        IReadOnlyList<IApplicationCommandPermissions> permissions,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PutAsync<IGuildApplicationCommandPermissions>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands/{commandID}/permissions",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WritePropertyName("permissions");
                        JsonSerializer.Serialize(json, permissions, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IGuildApplicationCommandPermissions>>>
        BatchEditApplicationCommandPermissionsAsync
        (
            Snowflake applicationID,
            Snowflake guildID,
            IReadOnlyList<IPartialGuildApplicationCommandPermissions> permissions,
            CancellationToken ct = default
        )
    {
        return this.RestHttpClient.PutAsync<IReadOnlyList<IGuildApplicationCommandPermissions>>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands/permissions",
            b => b.WithJsonArray
                (
                    json =>
                    {
                        foreach (var permission in permissions)
                        {
                            JsonSerializer.Serialize(json, permission, this.JsonOptions);
                        }
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }
}
