//
//  DiscordRestGuildAPI.cs
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Rest.IDiscordRestGuildAPI" />
    [PublicAPI]
    public class DiscordRestGuildAPI : AbstractDiscordRestAPI, IDiscordRestGuildAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordRestGuildAPI"/> class.
        /// </summary>
        /// <param name="discordHttpClient">The Discord HTTP client.</param>
        /// <param name="jsonOptions">The json options.</param>
        public DiscordRestGuildAPI(DiscordHttpClient discordHttpClient, IOptions<JsonSerializerOptions> jsonOptions)
            : base(discordHttpClient, jsonOptions)
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

            var packIcon = await ImagePacker.PackImageAsync(new Optional<Stream?>(icon.Value), ct);
            if (!packIcon.IsSuccess)
            {
                return Result<IGuild>.FromError(packIcon);
            }

            var iconData = packIcon.Entity;

            return await this.DiscordHttpClient.PostAsync<IGuild>
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
                ),
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
            return this.DiscordHttpClient.GetAsync<IGuild>
            (
                $"guilds/{guildID}",
                b =>
                {
                    if (withCounts.HasValue)
                    {
                        b.AddQueryParameter("with_counts", withCounts.Value.ToString());
                    }
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
            return this.DiscordHttpClient.GetAsync<IGuildPreview>
            (
                $"guilds/{guildID}/preview",
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
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            await using var memoryStream = new MemoryStream();

            Optional<string?> iconData = default;
            if (icon.HasValue)
            {
                if (icon.Value is null)
                {
                    iconData = new Optional<string?>(null);
                }
                else
                {
                    var packImage = await ImagePacker.PackImageAsync(icon.Value, ct);
                    if (!packImage.IsSuccess)
                    {
                        return Result<IGuild>.FromError(packImage);
                    }

                    iconData = packImage.Entity;
                }
            }

            Optional<string?> splashData = default;
            if (splash.HasValue)
            {
                if (splash.Value is null)
                {
                    splashData = new Optional<string?>(null);
                }
                else
                {
                    var packImage = await ImagePacker.PackImageAsync(splash.Value, ct);
                    if (!packImage.IsSuccess)
                    {
                        return Result<IGuild>.FromError(packImage);
                    }

                    splashData = packImage.Entity;
                }
            }

            Optional<string?> discoverySplashData = default;
            if (discoverySplash.HasValue)
            {
                if (discoverySplash.Value is null)
                {
                    discoverySplashData = new Optional<string?>(null);
                }
                else
                {
                    var packImage = await ImagePacker.PackImageAsync(discoverySplash.Value, ct);
                    if (!packImage.IsSuccess)
                    {
                        return Result<IGuild>.FromError(packImage);
                    }

                    discoverySplashData = packImage.Entity;
                }
            }

            var packBanner = await ImagePacker.PackImageAsync(banner, ct);
            if (!packBanner.IsSuccess)
            {
                return Result<IGuild>.FromError(packBanner);
            }

            var bannerData = packBanner.Entity;

            return await this.DiscordHttpClient.PatchAsync<IGuild>
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
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteGuildAsync(Snowflake guildID, CancellationToken ct = default)
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"guilds/{guildID}",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<IChannel>>> GetGuildChannelsAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IChannel>>
            (
                $"guilds/{guildID}/channels",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IChannel>> CreateGuildChannelAsync
        (
            Snowflake guildID,
            string name,
            Optional<ChannelType> type = default,
            Optional<string> topic = default,
            Optional<int> bitrate = default,
            Optional<int> userLimit = default,
            Optional<int> rateLimitPerUser = default,
            Optional<int> position = default,
            Optional<IReadOnlyList<IPermissionOverwrite>> permissionOverwrites = default,
            Optional<Snowflake> parentID = default,
            Optional<bool> isNsfw = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PostAsync<IChannel>
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
                    }
                ),
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
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync
            (
                $"guilds/{guildID}/channels",
                b => b
                .AddAuditLogReason(reason)
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
                ),
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
            return this.DiscordHttpClient.GetAsync<IGuildMember>
            (
                $"guilds/{guildID}/members/{userID}",
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

            return await this.DiscordHttpClient.GetAsync<IReadOnlyList<IGuildMember>>
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

            return await this.DiscordHttpClient.GetAsync<IReadOnlyList<IGuildMember>>
            (
                $"guilds/{guildID}/members/search",
                b =>
                {
                    b.AddQueryParameter("query", query);

                    if (limit.HasValue)
                    {
                        b.AddQueryParameter("limit", limit.Value.ToString());
                    }
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
            return this.DiscordHttpClient.PutAsync<IGuildMember?>
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
                ),
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
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync
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
                    }
                ),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<string>> ModifyCurrentUserNickAsync
        (
            Snowflake guildID,
            Optional<string?> nickname = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync<string>
            (
                $"guilds/{guildID}/members/@me/nick",
                b => b
                    .AddAuditLogReason(reason)
                    .WithJson(json => json.Write("nick", nickname, this.JsonOptions)),
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
            return this.DiscordHttpClient.PutAsync
            (
                $"guilds/{guildID}/members/{userID}/roles/{roleID}",
                b => b.AddAuditLogReason(reason),
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
            return this.DiscordHttpClient.DeleteAsync
            (
                $"guilds/{guildID}/members/{userID}/roles/{roleID}",
                b => b.AddAuditLogReason(reason),
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
            return this.DiscordHttpClient.DeleteAsync
            (
                $"guilds/{guildID}/members/{userID}",
                b => b.AddAuditLogReason(reason),
                ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<IBan>>> GetGuildBansAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IBan>>
            (
                $"guilds/{guildID}/bans",
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
            return this.DiscordHttpClient.GetAsync<IBan>
            (
                $"guilds/{guildID}/bans/{userID}",
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
            return this.DiscordHttpClient.PutAsync
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
                ),
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
            return this.DiscordHttpClient.DeleteAsync
            (
                $"guilds/{guildID}/bans/{userID}",
                b => b.AddAuditLogReason(reason),
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
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IRole>>
            (
                $"guilds/{guildID}/roles",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IRole>> CreateGuildRoleAsync
        (
            Snowflake guildID,
            Optional<string> name = default,
            Optional<IDiscordPermissionSet> permissions = default,
            Optional<Color> colour = default,
            Optional<bool> isHoisted = default,
            Optional<bool> isMentionable = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PostAsync<IRole>
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
                        json.Write("mentionable", isMentionable, this.JsonOptions);
                    }
                ),
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
            return this.DiscordHttpClient.PatchAsync<IReadOnlyList<IRole>>
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
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IRole>> ModifyGuildRoleAsync
        (
            Snowflake guildID,
            Snowflake roleID,
            Optional<string?> name = default,
            Optional<IDiscordPermissionSet?> permissions = default,
            Optional<Color?> colour = default,
            Optional<bool?> isHoisted = default,
            Optional<bool?> isMentionable = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync<IRole>
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
                        json.Write("mentionable", isMentionable, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result> DeleteGuildRoleAsync
        (
            Snowflake guildId,
            Snowflake roleID,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.DeleteAsync
            (
                $"guilds/{guildId}/roles/{roleID}",
                b => b.AddAuditLogReason(reason),
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

            return await this.DiscordHttpClient.GetAsync<IPruneCount>
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

            return await this.DiscordHttpClient.PostAsync<IPruneCount>
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
                ),
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
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IVoiceRegion>>
            (
                $"guilds/{guildID}/regions",
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
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IInvite>>
            (
                $"guilds/{guildID}/invites",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IReadOnlyList<IIntegration>>> GetGuildIntegrationsAsync
        (
            Snowflake guildID,
            Optional<bool> includeApplications = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IReadOnlyList<IIntegration>>
            (
                $"guilds/{guildID}/integrations",
                b =>
                {
                    if (includeApplications.HasValue)
                    {
                        b.AddQueryParameter
                        (
                            "include_applications",
                            includeApplications.Value.ToString().ToLowerInvariant()
                        );
                    }
                },
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IGuildWidget>> GetGuildWidgetSettingsAsync
        (
            Snowflake guildID,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.GetAsync<IGuildWidget>
            (
                $"guilds/{guildID}/widget",
                ct: ct
            );
        }

        /// <inheritdoc />
        public virtual Task<Result<IGuildWidget>> ModifyGuildWidgetAsync
        (
            Snowflake guildID,
            Optional<bool> isEnabled = default,
            Optional<Snowflake?> channelID = default,
            Optional<string> reason = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync<IGuildWidget>
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
                ),
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
            return this.DiscordHttpClient.GetAsync<IPartialInvite>
            (
                $"guilds/{guildID}/vanity-url",
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
            return this.DiscordHttpClient.GetContentAsync
            (
                $"guilds/{guildID}/widget.png",
                b =>
                {
                    if (style.HasValue)
                    {
                        b.AddQueryParameter("style", style.Value.ToString().ToLowerInvariant());
                    }
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
            return this.DiscordHttpClient.GetAsync<IWelcomeScreen>
            (
                $"guilds/{guildID}/welcome-screen",
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
            return this.DiscordHttpClient.PatchAsync<IWelcomeScreen>
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
                ),
                ct: ct
            );
        }

        /// <inheritdoc/>
        public virtual Task<Result<IVoiceState>> ModifyCurrentUserVoiceStateAsync
        (
            Snowflake guildID,
            Snowflake channelID,
            Optional<bool> suppress = default,
            Optional<DateTimeOffset?> requestToSpeakTimestamp = default,
            CancellationToken ct = default
        )
        {
            return this.DiscordHttpClient.PatchAsync<IVoiceState>
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
                ),
                ct: ct
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
            return this.DiscordHttpClient.PatchAsync<IVoiceState>
            (
                $"guilds/{guildID}/voice-states/{userID}",
                b => b.WithJson
                (
                    json =>
                    {
                        json.WriteString("channel_id", channelID.ToString());
                        json.Write("suppress", suppress, this.JsonOptions);
                    }
                ),
                ct: ct
            );
        }
    }
}
