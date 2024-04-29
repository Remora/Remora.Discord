//
//  DiscordRestApplicationAPI.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public DiscordRestApplicationAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IApplicationCommand>>> GetGlobalApplicationCommandsAsync
    (
        Snowflake applicationID,
        Optional<bool> withLocalizations = default,
        Optional<string> locale = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IApplicationCommand>>
        (
            $"applications/{applicationID}/commands",
            b =>
            {
                if (withLocalizations.TryGet(out var realWithLocalizations))
                {
                    b.AddQueryParameter("with_localizations", realWithLocalizations.ToString());
                }

                if (locale.TryGet(out var realLocale))
                {
                    b.AddHeader(Constants.LocaleHeaderName, realLocale);
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IApplicationCommand>> CreateGlobalApplicationCommandAsync
    (
        Snowflake applicationID,
        string name,
        Optional<string> description = default,
        Optional<IReadOnlyList<IApplicationCommandOption>> options = default,
        Optional<ApplicationCommandType> type = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        Optional<bool?> dmPermission = default,
        Optional<bool> isNsfw = default,
        Optional<IReadOnlyList<ApplicationIntegrationType>> allowedIntegrationTypes = default,
        Optional<IReadOnlyList<InteractionContextType>> allowedContextTypes = default,
        CancellationToken ct = default
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
            if (!description.TryGet(out var value) || value.Length is < 1 or > 100)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(description),
                    "The description must be between 1 and 100 characters."
                );
            }
        }
        else if (description.HasValue)
        {
            return new ArgumentInvalidError
            (
                nameof(description),
                "Descriptions are not supported for this command type."
            );
        }

        return await this.RestHttpClient.PostAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/commands",
            b => b.WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("type", type, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write("options", options, this.JsonOptions);
                        json.Write("name_localizations", nameLocalizations, this.JsonOptions);
                        json.Write("description_localizations", descriptionLocalizations, this.JsonOptions);
                        json.Write("default_member_permissions", defaultMemberPermissions, this.JsonOptions);
                        json.Write("dm_permission", dmPermission, this.JsonOptions);
                        json.Write("nsfw", isNsfw, this.JsonOptions);
                        json.Write("contexts", allowedContextTypes, this.JsonOptions);
                        json.Write("integration_types", allowedIntegrationTypes, this.JsonOptions);
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
                    (!c.Type.TryGet(out var type) || type is ApplicationCommandType.ChatInput) &&
                    c.Description.Length is < 1 or > 100
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
            b => b
                .WithJsonArray(json => JsonSerializer.Serialize(json, commands, this.JsonOptions), false)
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
        Optional<string> name = default,
        Optional<string> description = default,
        Optional<IReadOnlyList<IApplicationCommandOption>?> options = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        Optional<bool?> dmPermission = default,
        Optional<bool> isNsfw = default,
        Optional<IReadOnlyList<ApplicationIntegrationType>> allowedIntegrationTypes = default,
        Optional<IReadOnlyList<InteractionContextType>> allowedContextTypes = default,
        CancellationToken ct = default
    )
    {
        if (name is { HasValue: true, Value.Length: < 1 or > 32 })
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 32 characters."
            );
        }

        if (description is { HasValue: true, Value.Length: < 1 or > 100 })
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
                        json.Write("name_localizations", nameLocalizations, this.JsonOptions);
                        json.Write("description_localizations", descriptionLocalizations, this.JsonOptions);
                        json.Write("default_member_permissions", defaultMemberPermissions, this.JsonOptions);
                        json.Write("dm_permission", dmPermission, this.JsonOptions);
                        json.Write("nsfw", isNsfw, this.JsonOptions);
                        json.Write("contexts", allowedContextTypes, this.JsonOptions);
                        json.Write("integration_types", allowedIntegrationTypes, this.JsonOptions);
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
        CancellationToken ct = default
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
        Optional<bool> withLocalizations = default,
        Optional<string> locale = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IApplicationCommand>>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands",
            b =>
            {
                if (withLocalizations.TryGet(out var realWithLocalizations))
                {
                    b.AddQueryParameter("with_localizations", realWithLocalizations.ToString());
                }

                if (locale.TryGet(out var realLocale))
                {
                    b.AddHeader(Constants.LocaleHeaderName, realLocale);
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<IApplicationCommand>>> BulkOverwriteGuildApplicationCommandsAsync
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
                    (!c.Type.TryGet(out var type) || type is ApplicationCommandType.ChatInput) &&
                    c.Description.Length is < 1 or > 100
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
            b => b
                .WithJsonArray(json => JsonSerializer.Serialize(json, commands, this.JsonOptions), false)
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
        Optional<string> description = default,
        Optional<IReadOnlyList<IApplicationCommandOption>> options = default,
        Optional<ApplicationCommandType> type = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        Optional<bool> isNsfw = default,
        Optional<IReadOnlyList<ApplicationIntegrationType>> allowedIntegrationTypes = default,
        Optional<IReadOnlyList<InteractionContextType>> allowedContextTypes = default,
        CancellationToken ct = default
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
            if (!description.TryGet(out var value) || value.Length is < 1 or > 100)
            {
                return new ArgumentOutOfRangeError
                (
                    nameof(description),
                    "The description must be between 1 and 100 characters."
                );
            }
        }
        else if (description.HasValue)
        {
            return new ArgumentInvalidError
            (
                nameof(description),
                "Descriptions are not supported for this command type."
            );
        }

        return await this.RestHttpClient.PostAsync<IApplicationCommand>
        (
            $"applications/{applicationID}/guilds/{guildID}/commands",
            b => b.WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("type", type, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write("options", options, this.JsonOptions);
                        json.Write("name_localizations", nameLocalizations, this.JsonOptions);
                        json.Write("description_localizations", descriptionLocalizations, this.JsonOptions);
                        json.Write("default_member_permissions", defaultMemberPermissions, this.JsonOptions);
                        json.Write("nsfw", isNsfw, this.JsonOptions);
                        json.Write("contexts", allowedContextTypes, this.JsonOptions);
                        json.Write("integration_types", allowedIntegrationTypes, this.JsonOptions);
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
        Optional<string> name = default,
        Optional<string> description = default,
        Optional<IReadOnlyList<IApplicationCommandOption>?> options = default,
        Optional<IReadOnlyDictionary<string, string>?> nameLocalizations = default,
        Optional<IReadOnlyDictionary<string, string>?> descriptionLocalizations = default,
        Optional<IDiscordPermissionSet?> defaultMemberPermissions = default,
        Optional<bool> isNsfw = default,
        Optional<IReadOnlyList<ApplicationIntegrationType>> allowedIntegrationTypes = default,
        Optional<IReadOnlyList<InteractionContextType>> allowedContextTypes = default,
        CancellationToken ct = default
    )
    {
        if (name is { HasValue: true, Value.Length: < 1 or > 32 })
        {
            return new ArgumentOutOfRangeError
            (
                nameof(name),
                "The name must be between 1 and 32 characters."
            );
        }

        if (description is { HasValue: true, Value.Length: < 1 or > 100 })
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
                        json.Write("name_localizations", nameLocalizations, this.JsonOptions);
                        json.Write("description_localizations", descriptionLocalizations, this.JsonOptions);
                        json.Write("default_member_permissions", defaultMemberPermissions, this.JsonOptions);
                        json.Write("nsfw", isNsfw, this.JsonOptions);
                        json.Write("contexts", allowedContextTypes, this.JsonOptions);
                        json.Write("integration_types", allowedIntegrationTypes, this.JsonOptions);
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
        CancellationToken ct = default
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
    public virtual Task<Result<IReadOnlyList<IApplicationRoleConnectionMetadata>>>
    GetApplicationRoleConnectionMetadataRecordsAsync
    (
        Snowflake applicationID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IApplicationRoleConnectionMetadata>>
        (
            $"applications/{applicationID}/role-connections/metadata",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<IApplicationRoleConnectionMetadata>>>
    UpdateApplicationRoleConnectionMetadataRecordsAsync
    (
        Snowflake applicationID,
        IReadOnlyList<IApplicationRoleConnectionMetadata> records,
        CancellationToken ct = default
    )
    {
        if (records.Any(r => r.Key.Length > 50))
        {
            return new ArgumentOutOfRangeError
            (
                nameof(records),
                "Role connection metadata keys must be max. 50 characters."
            );
        }

        if (records.Any(r => r.Name.Length > 100))
        {
            return new ArgumentOutOfRangeError
            (
                nameof(records),
                "Role connection metadata names must be max. 100 characters."
            );
        }

        if (records.Any(r => r.Description.Length > 200))
        {
            return new ArgumentOutOfRangeError
            (
                nameof(records),
                "Role connection metadata descriptions must be max. 200 characters."
            );
        }

        return await this.RestHttpClient.PutAsync<IReadOnlyList<IApplicationRoleConnectionMetadata>>
        (
            $"applications/{applicationID}/role-connections/metadata",
            b => b
                .WithJsonArray(json => JsonSerializer.Serialize(json, records, this.JsonOptions), false)
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IApplication>> GetCurrentApplicationAsync(CancellationToken ct = default)
    {
        return this.RestHttpClient.GetAsync<IApplication>
        (
            "applications/@me",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public async Task<Result<IApplication>> EditCurrentApplicationAsync
    (
        Optional<Uri> customInstallUrl = default,
        Optional<string> description = default,
        Optional<Uri> roleConnectionsVerificationUrl = default,
        Optional<IApplicationInstallParameters> installParams = default,
        Optional<ApplicationFlags> flags = default,
        Optional<Stream> icon = default,
        Optional<Stream> coverImage = default,
        Optional<Uri> interactionsEndpointUrl = default,
        Optional<IReadOnlyList<string>> tags = default,
        Optional<IReadOnlyList<ApplicationIntegrationType>> integrationTypes = default,
        CancellationToken ct = default
    )
    {
        var packIcon = await ImagePacker.PackImageAsync(icon!, ct);
        if (!packIcon.IsSuccess)
        {
            return Result<IApplication>.FromError(packIcon);
        }

        Optional<string> base64EncodedIcon = packIcon.Entity!;

        var packCover = await ImagePacker.PackImageAsync(coverImage!, ct);
        if (!packCover.IsSuccess)
        {
            return Result<IApplication>.FromError(packCover);
        }

        Optional<string> base64EncodedCover = packCover.Entity!;

        return await this.RestHttpClient.PatchAsync<IApplication>
        (
            "applications/@me",
            b =>
            {
                b.WithJson
                (
                    json =>
                    {
                        json.Write("custom_install_url", customInstallUrl, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write
                        (
                            "role_connections_verification_url",
                            roleConnectionsVerificationUrl,
                            this.JsonOptions
                        );
                        json.Write("install_params", installParams, this.JsonOptions);
                        json.Write("flags", flags, this.JsonOptions);
                        json.Write("icon", base64EncodedIcon, this.JsonOptions);
                        json.Write("cover_image", base64EncodedCover, this.JsonOptions);
                        json.Write("interactions_endpoint_url", interactionsEndpointUrl, this.JsonOptions);
                        json.Write("tags", tags, this.JsonOptions);
                        json.Write("integration_types", integrationTypes, this.JsonOptions);
                    }
                );

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }
}
