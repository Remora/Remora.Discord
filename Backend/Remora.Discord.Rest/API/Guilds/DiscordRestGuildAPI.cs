//
//  DiscordRestGuildAPI.cs
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
using System.Drawing;
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

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestGuildAPI" />
[PublicAPI]
public class DiscordRestGuildAPI : AbstractDiscordRestAPI, IDiscordRestGuildAPI
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestGuildAPI"/> class.
    /// </summary>
    /// <param name="restHttpClient">The Discord HTTP client.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    /// <param name="rateLimitCache">The memory cache used for rate limits.</param>
    public DiscordRestGuildAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
    }

    /// <inheritdoc />
    public virtual async Task<Result<IGuild>> CreateGuildAsync
    (
        string name,
        Optional<Stream> icon = default,
        Optional<VerificationLevel> verificationLevel = default,
        Optional<MessageNotificationLevel> defaultMessageNotifications = default,
        Optional<ExplicitContentFilterLevel> explicitContentFilter = default,
        Optional<IReadOnlyList<IRole>> roles = default,
        Optional<IReadOnlyList<IPartialChannel>> channels = default,
        Optional<Snowflake> afkChannelID = default,
        Optional<TimeSpan> afkTimeout = default,
        Optional<Snowflake> systemChannelID = default,
        Optional<SystemChannelFlags> systemChannelFlags = default,
        CancellationToken ct = default
    )
    {
        if (name.Length is < 2 or > 100)
        {
            return new ArgumentOutOfRangeError(nameof(name), "The name must be between 2 and 100 characters.");
        }

        await using var memoryStream = new MemoryStream();

        var iconData = default(Optional<string?>);

        // ReSharper disable once InvertIf
        if (icon.IsDefined(out var iconStream))
        {
            var packIcon = await ImagePacker.PackImageAsync(iconStream, ct);
            if (!packIcon.IsSuccess)
            {
                return Result<IGuild>.FromError(packIcon);
            }

            iconData = packIcon.Entity;
        }

        return await this.RestHttpClient.PostAsync<IGuild>
        (
            "guilds",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.Write("icon", iconData, this.JsonOptions);
                        json.Write("verification_level", verificationLevel, this.JsonOptions);
                        json.Write("default_message_notifications", defaultMessageNotifications, this.JsonOptions);
                        json.Write("explicit_content_filter", explicitContentFilter, this.JsonOptions);
                        json.Write("roles", roles, this.JsonOptions);
                        json.Write("channels", channels, this.JsonOptions);
                        json.Write("afk_channel_id", afkChannelID, this.JsonOptions);

                        if (afkTimeout.HasValue)
                        {
                            json.WriteNumber("afk_timeout", (ulong)afkTimeout.Value.TotalSeconds);
                        }

                        json.Write("system_channel_id", systemChannelID, this.JsonOptions);
                        json.Write("system_channel_flags", systemChannelFlags, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuild>> GetGuildAsync
    (
        Snowflake guildID,
        Optional<bool> withCounts = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuild>
        (
            $"guilds/{guildID}",
            b =>
            {
                if (withCounts.HasValue)
                {
                    b.AddQueryParameter("with_counts", withCounts.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuildPreview>> GetGuildPreviewAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuildPreview>
        (
            $"guilds/{guildID}/preview",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IGuild>> ModifyGuildAsync
    (
        Snowflake guildID,
        Optional<string> name = default,
        Optional<VerificationLevel?> verificationLevel = default,
        Optional<MessageNotificationLevel?> defaultMessageNotifications = default,
        Optional<ExplicitContentFilterLevel?> explicitContentFilter = default,
        Optional<Snowflake?> afkChannelID = default,
        Optional<TimeSpan> afkTimeout = default,
        Optional<Stream?> icon = default,
        Optional<Snowflake> ownerID = default,
        Optional<Stream?> splash = default,
        Optional<Stream?> discoverySplash = default,
        Optional<Stream?> banner = default,
        Optional<Snowflake?> systemChannelID = default,
        Optional<SystemChannelFlags> systemChannelFlags = default,
        Optional<Snowflake?> rulesChannelID = default,
        Optional<Snowflake?> publicUpdatesChannelID = default,
        Optional<string?> preferredLocale = default,
        Optional<IReadOnlyList<GuildFeature>> features = default,
        Optional<string?> description = default,
        Optional<bool> isPremiumProgressBarEnabled = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        await using var memoryStream = new MemoryStream();

        var packIcon = await ImagePacker.PackImageAsync(icon, ct);
        if (!packIcon.IsSuccess)
        {
            return Result<IGuild>.FromError(packIcon);
        }

        var iconData = packIcon.Entity;

        var packSplash = await ImagePacker.PackImageAsync(splash, ct);
        if (!packSplash.IsSuccess)
        {
            return Result<IGuild>.FromError(packSplash);
        }

        var splashData = packSplash.Entity;

        var packDiscoverySplash = await ImagePacker.PackImageAsync(discoverySplash, ct);
        if (!packDiscoverySplash.IsSuccess)
        {
            return Result<IGuild>.FromError(packDiscoverySplash);
        }

        var discoverySplashData = packDiscoverySplash.Entity;

        var packBanner = await ImagePacker.PackImageAsync(banner, ct);
        if (!packBanner.IsSuccess)
        {
            return Result<IGuild>.FromError(packBanner);
        }

        var bannerData = packBanner.Entity;

        return await this.RestHttpClient.PatchAsync<IGuild>
        (
            $"guilds/{guildID}",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("verification_level", verificationLevel, this.JsonOptions);
                        json.Write("default_message_notifications", defaultMessageNotifications, this.JsonOptions);

                        json.Write("explicit_content_filter", explicitContentFilter, this.JsonOptions);
                        json.Write("afk_channel_id", afkChannelID, this.JsonOptions);

                        if (afkTimeout.HasValue)
                        {
                            json.WriteNumber("afk_timeout", (ulong)afkTimeout.Value.TotalSeconds);
                        }

                        json.Write("icon", iconData, this.JsonOptions);
                        json.Write("owner_id", ownerID, this.JsonOptions);
                        json.Write("splash", splashData, this.JsonOptions);
                        json.Write("discovery_splash", discoverySplashData, this.JsonOptions);
                        json.Write("banner", bannerData, this.JsonOptions);
                        json.Write("system_channel_id", systemChannelID, this.JsonOptions);
                        json.Write("system_channel_flags", systemChannelFlags, this.JsonOptions);
                        json.Write("rules_channel_id", rulesChannelID, this.JsonOptions);
                        json.Write("public_updates_channel_id", publicUpdatesChannelID, this.JsonOptions);
                        json.Write("preferred_locale", preferredLocale, this.JsonOptions);
                        json.Write("features", features, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                        json.Write("premium_progress_bar_enabled", isPremiumProgressBarEnabled, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> DeleteGuildAsync(Snowflake guildID, CancellationToken ct = default)
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IChannel>>> GetGuildChannelsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IChannel>>
        (
            $"guilds/{guildID}/channels",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IChannel>> CreateGuildChannelAsync
    (
        Snowflake guildID,
        string name,
        Optional<ChannelType?> type = default,
        Optional<string?> topic = default,
        Optional<int?> bitrate = default,
        Optional<int?> userLimit = default,
        Optional<int?> rateLimitPerUser = default,
        Optional<int?> position = default,
        Optional<IReadOnlyList<IPartialPermissionOverwrite>?> permissionOverwrites = default,
        Optional<Snowflake?> parentID = default,
        Optional<bool?> isNsfw = default,
        Optional<string?> rtcRegion = default,
        Optional<VideoQualityMode?> videoQualityMode = default,
        Optional<AutoArchiveDuration?> defaultAutoArchiveDuration = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<IChannel>
        (
            $"guilds/{guildID}/channels",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.WriteString("name", name);
                        json.Write("type", type, this.JsonOptions);
                        json.Write("topic", topic, this.JsonOptions);
                        json.Write("bitrate", bitrate, this.JsonOptions);
                        json.Write("user_limit", userLimit, this.JsonOptions);
                        json.Write("rate_limit_per_user", rateLimitPerUser, this.JsonOptions);
                        json.Write("position", position, this.JsonOptions);
                        json.Write("permission_overwrites", permissionOverwrites, this.JsonOptions);
                        json.Write("parent_id", parentID, this.JsonOptions);
                        json.Write("nsfw", isNsfw, this.JsonOptions);
                        json.Write("rtc_region", rtcRegion, this.JsonOptions);
                        json.Write("video_quality_mode", videoQualityMode, this.JsonOptions);
                        json.Write("default_auto_archive_duration", defaultAutoArchiveDuration, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> ModifyGuildChannelPositionsAsync
    (
        Snowflake guildID,
        IReadOnlyList
        <
            (
            Snowflake ChannelID,
            int? Position,
            bool? LockPermissions,
            Snowflake? ParentID
            )
        > positionModifications,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync
        (
            $"guilds/{guildID}/channels",
            b => b
                .WithJsonArray
                (
                    json =>
                    {
                        foreach (var (channelID, position, lockPermissions, parentID) in positionModifications)
                        {
                            json.WriteStartObject();
                            {
                                json.WriteString("id", channelID.ToString());
                                if (position is null)
                                {
                                    json.WriteNull("position");
                                }
                                else
                                {
                                    json.WriteNumber("position", position.Value);
                                }

                                if (lockPermissions is null)
                                {
                                    json.WriteNull("lock_permissions");
                                }
                                else
                                {
                                    json.WriteBoolean("lock_permissions", lockPermissions.Value);
                                }

                                if (parentID is null)
                                {
                                    json.WriteNull("parent_id");
                                }
                                else
                                {
                                    json.WriteString("parent_id", parentID.Value.ToString());
                                }
                            }
                            json.WriteEndObject();
                        }
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuildMember>> GetGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuildMember>
        (
            $"guilds/{guildID}/members/{userID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<IGuildMember>>> ListGuildMembersAsync
    (
        Snowflake guildID,
        Optional<int> limit = default,
        Optional<Snowflake> after = default,
        CancellationToken ct = default
    )
    {
        if (limit.HasValue && limit.Value is < 1 or > 1000)
        {
            return new ArgumentOutOfRangeError(nameof(limit), "The limit must be between 1 and 1000.");
        }

        return await this.RestHttpClient.GetAsync<IReadOnlyList<IGuildMember>>
        (
            $"guilds/{guildID}/members",
            b =>
            {
                if (limit.HasValue)
                {
                    b.AddQueryParameter("limit", limit.Value.ToString());
                }

                if (after.HasValue)
                {
                    b.AddQueryParameter("after", after.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IReadOnlyList<IGuildMember>>> SearchGuildMembersAsync
    (
        Snowflake guildID,
        string query,
        Optional<int> limit = default,
        CancellationToken ct = default
    )
    {
        if (limit.HasValue && limit.Value is < 1 or > 1000)
        {
            return new ArgumentOutOfRangeError(nameof(limit), "The limit must be between 1 and 1000.");
        }

        return await this.RestHttpClient.GetAsync<IReadOnlyList<IGuildMember>>
        (
            $"guilds/{guildID}/members/search",
            b =>
            {
                b.AddQueryParameter("query", query);

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
    public virtual Task<Result<IGuildMember?>> AddGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        string accessToken,
        Optional<string> nickname = default,
        Optional<IReadOnlyList<Snowflake>> roles = default,
        Optional<bool> isMuted = default,
        Optional<bool> isDeafened = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PutAsync<IGuildMember?>
        (
            $"guilds/{guildID}/members/{userID}",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("access_token", accessToken);
                        json.Write("nick", nickname, this.JsonOptions);
                        json.Write("roles", roles, this.JsonOptions);
                        json.Write("mute", isMuted, this.JsonOptions);
                        json.Write("deaf", isDeafened, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            true,
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> ModifyGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string?> nickname = default,
        Optional<IReadOnlyList<Snowflake>?> roles = default,
        Optional<bool?> isMuted = default,
        Optional<bool?> isDeafened = default,
        Optional<Snowflake?> channelID = default,
        Optional<DateTimeOffset?> communicationDisabledUntil = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync
        (
            $"guilds/{guildID}/members/{userID}",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("nick", nickname, this.JsonOptions);
                        json.Write("roles", roles, this.JsonOptions);
                        json.Write("mute", isMuted, this.JsonOptions);
                        json.Write("deaf", isDeafened, this.JsonOptions);
                        json.Write("channel_id", channelID, this.JsonOptions);
                        json.Write("communication_disabled_until", communicationDisabledUntil, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuildMember>> ModifyCurrentMemberAsync
    (
        Snowflake guildID,
        Optional<string?> nickname = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync<IGuildMember>
        (
            $"guilds/{guildID}/members/@me",
            b => b
                .AddAuditLogReason(reason)
                .WithJson(json => json.Write("nick", nickname, this.JsonOptions))
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> AddGuildMemberRoleAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PutAsync
        (
            $"guilds/{guildID}/members/{userID}/roles/{roleID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> RemoveGuildMemberRoleAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/members/{userID}/roles/{roleID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> RemoveGuildMemberAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/members/{userID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IBan>>> GetGuildBansAsync
    (
        Snowflake guildID,
        Optional<int> limit = default,
        Optional<Snowflake> before = default,
        Optional<Snowflake> after = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IBan>>
        (
            $"guilds/{guildID}/bans",
            b =>
            {
                if (limit.HasValue)
                {
                    b.AddQueryParameter("limit", limit.Value.ToString());
                }

                if (before.HasValue)
                {
                    b.AddQueryParameter("before", before.Value.ToString());
                }

                if (after.HasValue)
                {
                    b.AddQueryParameter("after", after.Value.ToString());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IBan>> GetGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IBan>
        (
            $"guilds/{guildID}/bans/{userID}",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> CreateGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<int> deleteMessageDays = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PutAsync
        (
            $"guilds/{guildID}/bans/{userID}",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("delete_message_days", deleteMessageDays, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> RemoveGuildBanAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/bans/{userID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IRole>>> GetGuildRolesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IRole>>
        (
            $"guilds/{guildID}/roles",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IRole>> CreateGuildRoleAsync
    (
        Snowflake guildID,
        Optional<string> name = default,
        Optional<IDiscordPermissionSet> permissions = default,
        Optional<Color> colour = default,
        Optional<bool> isHoisted = default,
        Optional<Stream?> icon = default,
        Optional<string?> unicodeEmoji = default,
        Optional<bool> isMentionable = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var iconData = default(Optional<string>);

        // ReSharper disable once InvertIf
        if (icon.IsDefined(out var iconStream))
        {
            var packIcon = await ImagePacker.PackImageAsync(iconStream, ct);
            if (!packIcon.IsSuccess)
            {
                return Result<IRole>.FromError(packIcon);
            }

            iconData = packIcon.Entity;
        }

        return await this.RestHttpClient.PostAsync<IRole>
        (
            $"guilds/{guildID}/roles",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("permissions", permissions, this.JsonOptions);
                        json.Write("color", colour, this.JsonOptions);
                        json.Write("hoist", isHoisted, this.JsonOptions);
                        json.Write("icon", iconData, this.JsonOptions);
                        json.Write("unicode_emoji", unicodeEmoji, this.JsonOptions);
                        json.Write("mentionable", isMentionable, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IRole>>> ModifyGuildRolePositionsAsync
    (
        Snowflake guildID,
        IReadOnlyList<(Snowflake RoleID, Optional<int?> Position)> modifiedPositions,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync<IReadOnlyList<IRole>>
        (
            $"guilds/{guildID}/roles",
            b => b
                .AddAuditLogReason(reason)
                .WithJsonArray
                (
                    json =>
                    {
                        foreach (var (roleID, position) in modifiedPositions)
                        {
                            json.WriteStartObject();
                            {
                                json.WriteString("id", roleID.ToString());
                                json.Write("position", position, this.JsonOptions);
                            }
                            json.WriteEndObject();
                        }
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IRole>> ModifyGuildRoleAsync
    (
        Snowflake guildID,
        Snowflake roleID,
        Optional<string?> name = default,
        Optional<IDiscordPermissionSet?> permissions = default,
        Optional<Color?> colour = default,
        Optional<bool?> isHoisted = default,
        Optional<Stream?> icon = default,
        Optional<string?> unicodeEmoji = default,
        Optional<bool?> isMentionable = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var packIcon = await ImagePacker.PackImageAsync(icon, ct);
        if (!packIcon.IsSuccess)
        {
            return Result<IRole>.FromError(packIcon);
        }

        var iconData = packIcon.Entity;

        return await this.RestHttpClient.PatchAsync<IRole>
        (
            $"guilds/{guildID}/roles/{roleID}",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("name", name, this.JsonOptions);
                        json.Write("permissions", permissions, this.JsonOptions);
                        json.Write("color", colour, this.JsonOptions);
                        json.Write("hoist", isHoisted, this.JsonOptions);
                        json.Write("icon", iconData, this.JsonOptions);
                        json.Write("unicode_emoji", unicodeEmoji, this.JsonOptions);
                        json.Write("mentionable", isMentionable, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<MultiFactorAuthenticationLevel>> ModifyGuildMFALevelAsync
    (
        Snowflake guildID,
        MultiFactorAuthenticationLevel level,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PostAsync<MultiFactorAuthenticationLevel>
        (
            $"guilds/{guildID}/mfa",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("level", level, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> DeleteGuildRoleAsync
    (
        Snowflake guildID,
        Snowflake roleID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/roles/{roleID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IPruneCount>> GetGuildPruneCountAsync
    (
        Snowflake guildID,
        Optional<int> days = default,
        Optional<IReadOnlyList<Snowflake>> includeRoles = default,
        CancellationToken ct = default
    )
    {
        if (days.HasValue && days.Value is < 1 or > 30)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(days),
                "The requested number of days must be between 1 and 30."
            );
        }

        return await this.RestHttpClient.GetAsync<IPruneCount>
        (
            $"guilds/{guildID}/prune",
            b =>
            {
                if (days.HasValue)
                {
                    b.AddQueryParameter("days", days.Value.ToString());
                }

                if (includeRoles.HasValue)
                {
                    b.AddQueryParameter
                    (
                        "include_roles",
                        string.Join(',', includeRoles.Value.Select(s => s.ToString()))
                    );
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual async Task<Result<IPruneCount>> BeginGuildPruneAsync
    (
        Snowflake guildID,
        Optional<int> days = default,
        Optional<bool> computePruneCount = default,
        Optional<IReadOnlyList<Snowflake>> includeRoles = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        if (days.HasValue && days.Value is < 1 or > 30)
        {
            return new ArgumentOutOfRangeError
            (
                nameof(days),
                "The requested number of days must be between 1 and 30."
            );
        }

        return await this.RestHttpClient.PostAsync<IPruneCount>
        (
            $"guilds/{guildID}/prune",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("days", days, this.JsonOptions);
                        json.Write("compute_prune_count", computePruneCount, this.JsonOptions);
                        json.Write("include_roles", includeRoles, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IVoiceRegion>>> GetGuildVoiceRegionsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IVoiceRegion>>
        (
            $"guilds/{guildID}/regions",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IInvite>>> GetGuildInvitesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IInvite>>
        (
            $"guilds/{guildID}/invites",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IReadOnlyList<IIntegration>>> GetGuildIntegrationsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IReadOnlyList<IIntegration>>
        (
            $"guilds/{guildID}/integrations",
            b =>
            {
                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result> DeleteGuildIntegrationAsync
    (
        Snowflake guildID,
        Snowflake integrationID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.DeleteAsync
        (
            $"guilds/{guildID}/integrations/{integrationID}",
            b => b.AddAuditLogReason(reason).WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuildWidgetSettings>> GetGuildWidgetSettingsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuildWidgetSettings>
        (
            $"guilds/{guildID}/widget",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IGuildWidgetSettings>> ModifyGuildWidgetAsync
    (
        Snowflake guildID,
        Optional<bool> isEnabled = default,
        Optional<Snowflake?> channelID = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync<IGuildWidgetSettings>
        (
            $"guilds/{guildID}/widget",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("enabled", isEnabled, this.JsonOptions);
                        json.Write("channel_id", channelID, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IGuildWidget>> GetGuildWidgetAsync(Snowflake guildID, CancellationToken ct = default)
    {
        return this.RestHttpClient.GetAsync<IGuildWidget>
        (
            $"guilds/{guildID}/widget.json",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IPartialInvite>> GetGuildVanityUrlAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IPartialInvite>
        (
            $"guilds/{guildID}/vanity-url",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<Stream>> GetGuildWidgetImageAsync
    (
        Snowflake guildID,
        Optional<WidgetImageStyle> style = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetContentAsync
        (
            $"guilds/{guildID}/widget.png",
            b =>
            {
                if (style.HasValue)
                {
                    b.AddQueryParameter("style", style.Value.ToString().ToLowerInvariant());
                }

                b.WithRateLimitContext(this.RateLimitCache);
            },
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IWelcomeScreen>> GetGuildWelcomeScreenAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IWelcomeScreen>
        (
            $"guilds/{guildID}/welcome-screen",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IWelcomeScreen>> ModifyGuildWelcomeScreenAsync
    (
        Snowflake guildID,
        Optional<bool?> isEnabled = default,
        Optional<IReadOnlyList<IWelcomeScreenChannel>?> welcomeChannels = default,
        Optional<string?> description = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync<IWelcomeScreen>
        (
            $"guilds/{guildID}/welcome-screen",
            b => b
                .AddAuditLogReason(reason)
                .WithJson
                (
                    json =>
                    {
                        json.Write("enabled", isEnabled, this.JsonOptions);
                        json.Write("welcome_channels", welcomeChannels, this.JsonOptions);
                        json.Write("description", description, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc/>
    public virtual Task<Result> ModifyCurrentUserVoiceStateAsync
    (
        Snowflake guildID,
        Snowflake channelID,
        Optional<bool> suppress = default,
        Optional<DateTimeOffset?> requestToSpeakTimestamp = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync
        (
            $"guilds/{guildID}/voice-states/@me",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("channel_id", channelID.ToString());
                        json.Write("suppress", suppress, this.JsonOptions);
                        json.Write("request_to_speak_timestamp", requestToSpeakTimestamp, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct
        );
    }

    /// <inheritdoc />
    public virtual Task<Result<IVoiceState>> ModifyUserVoiceStateAsync
    (
        Snowflake guildID,
        Snowflake userID,
        Snowflake channelID,
        Optional<bool> suppress = default,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.PatchAsync<IVoiceState>
        (
            $"guilds/{guildID}/voice-states/{userID}",
            b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("channel_id", channelID.ToString());
                        json.Write("suppress", suppress, this.JsonOptions);
                    }
                )
                .WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }

    /// <inheritdoc />
    public Task<Result<IGuildThreadQueryResponse>> ListActiveGuildThreadsAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        return this.RestHttpClient.GetAsync<IGuildThreadQueryResponse>
        (
            $"guilds/{guildID}/threads/active",
            b => b.WithRateLimitContext(this.RateLimitCache),
            ct: ct
        );
    }
}
