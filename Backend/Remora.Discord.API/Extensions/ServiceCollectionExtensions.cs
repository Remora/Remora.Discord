//
//  ServiceCollectionExtensions.cs
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

using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Gateway.Events.Channels;
using Remora.Discord.API.Json;
using Remora.Discord.API.Objects;

namespace Remora.Discord.API.Extensions
{
    /// <summary>
    /// Defines various extension methods to the <see cref="IServiceCollection"/> class.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds supporting services for the Discord API.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="allowUnknownEvents">Whether the API will deserialize unknown events.</param>
        /// <returns>The service collection, with the services.</returns>
        public static IServiceCollection AddDiscordApi
        (
            this IServiceCollection serviceCollection,
            bool allowUnknownEvents = true
        )
        {
            serviceCollection
                .Configure<JsonSerializerOptions>
                (
                    options =>
                    {
                        var snakeCasePolicy = new SnakeCaseNamingPolicy();

                        options.Converters.Add(new PayloadConverter(allowUnknownEvents));

                        options
                            .AddGatewayBidirectionalConverters()
                            .AddGatewayCommandConverters()
                            .AddGatewayEventConverters()
                            .AddActivityObjectConverters()
                            .AddAuditLogObjectConverters()
                            .AddChannelObjectConverters()
                            .AddEmojiObjectConverters()
                            .AddGatewayObjectConverters()
                            .AddGuildObjectConverters()
                            .AddImageObjectConverters()
                            .AddIntegrationObjectConverters()
                            .AddInviteObjectConverters()
                            .AddMessageObjectConverters()
                            .AddPermissionObjectConverters()
                            .AddPresenceObjectConverters()
                            .AddReactionObjectConverters()
                            .AddUserObjectConverters()
                            .AddVoiceObjectConverters()
                            .AddWebhookObjectConverters()
                            .AddErrorObjectConverters()
                            .AddTemplateObjectConverters()
                            .AddInteractionObjectConverters()
                            .AddOAuth2ObjectConverters()
                            .AddTeamObjectConverters()
                            .AddStageInstanceObjectConverters();

                        options.AddDataObjectConverter<IUnknownEvent, UnknownEvent>();

                        options
                            .AddConverter<OptionalConverterFactory>()
                            .AddConverter<NullableConverterFactory>()
                            .AddConverter<SnowflakeConverter>()
                            .AddConverter<ColorConverter>()
                            .AddConverter<PropertyErrorDetailsConverter>()
                            .AddConverter<OneOfConverterFactory>();

                        options.PropertyNamingPolicy = snakeCasePolicy;
                        options.DictionaryKeyPolicy = snakeCasePolicy;
                    }
                );

            return serviceCollection;
        }

        /// <summary>
        /// Adds the JSON converters that handle bidirectional gateway payloads.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddGatewayBidirectionalConverters(this JsonSerializerOptions options)
        {
            options
                .AddConverter<HeartbeatConverter>()
                .AddDataObjectConverter<IHeartbeatAcknowledge, HeartbeatAcknowledge>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle gateway command payloads.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddGatewayCommandConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IIdentify, Identify>();

            options.AddDataObjectConverter<IConnectionProperties, ConnectionProperties>()
                .WithPropertyName(p => p.OperatingSystem, "$os")
                .WithPropertyName(p => p.Browser, "$browser")
                .WithPropertyName(p => p.Device, "$device");

            options.AddConverter<ShardIdentificationConverter>();

            options.AddDataObjectConverter<IRequestGuildMembers, RequestGuildMembers>()
                .WithPropertyName(r => r.UserIDs, "user_ids");

            options.AddDataObjectConverter<IResume, Resume>()
                .WithPropertyName(r => r.SequenceNumber, "seq");

            options.AddDataObjectConverter<IUpdatePresence, UpdatePresence>()
                .WithPropertyName(u => u.IsAFK, "afk")
                .WithPropertyConverter
                (
                    u => u.Status,
                    new StringEnumConverter<ClientStatus>(new SnakeCaseNamingPolicy())
                )
                .WithPropertyConverter(u => u.Since, new UnixMillisecondsDateTimeOffsetConverter());

            options.AddDataObjectConverter<IUpdateVoiceState, UpdateVoiceState>()
                .WithPropertyName(u => u.IsSelfMuted, "self_mute")
                .WithPropertyName(u => u.IsSelfDeafened, "self_deaf");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle gateway event payloads.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddGatewayEventConverters(this JsonSerializerOptions options)
        {
            // Connecting and resuming
            options.AddConverter<InvalidSessionConverter>();

            options.AddDataObjectConverter<IHello, Hello>()
                .WithPropertyConverter(h => h.HeartbeatInterval, new UnitTimeSpanConverter(TimeUnit.Milliseconds));

            options.AddDataObjectConverter<IReady, Ready>()
                .WithPropertyName(r => r.Version, "v");

            options.AddDataObjectConverter<IReconnect, Reconnect>();
            options.AddDataObjectConverter<IResumed, Resumed>();

            // Channels
            options.AddDataObjectConverter<IChannelCreate, ChannelCreate>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.DefaultAutoArchiveDuration, new UnitTimeSpanConverter(TimeUnit.Minutes))
                .WithPropertyConverter(c => c.LastPinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IChannelUpdate, ChannelUpdate>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.DefaultAutoArchiveDuration, new UnitTimeSpanConverter(TimeUnit.Minutes))
                .WithPropertyConverter(c => c.LastPinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IChannelDelete, ChannelDelete>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.DefaultAutoArchiveDuration, new UnitTimeSpanConverter(TimeUnit.Minutes))
                .WithPropertyConverter(c => c.LastPinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IChannelPinsUpdate, ChannelPinsUpdate>()
                .WithPropertyConverter(c => c.LastPinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IThreadCreate, ThreadCreate>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.DefaultAutoArchiveDuration, new UnitTimeSpanConverter(TimeUnit.Minutes));

            options.AddDataObjectConverter<IThreadUpdate, ThreadUpdate>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.DefaultAutoArchiveDuration, new UnitTimeSpanConverter(TimeUnit.Minutes));

            options.AddDataObjectConverter<IThreadDelete, ThreadDelete>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.DefaultAutoArchiveDuration, new UnitTimeSpanConverter(TimeUnit.Minutes));

            options.AddDataObjectConverter<IThreadListSync, ThreadListSync>()
                .WithPropertyName(t => t.ChannelIDs, "channel_ids");

            options.AddDataObjectConverter<IThreadMemberUpdate, ThreadMemberUpdate>()
                .WithPropertyConverter(m => m.JoinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IThreadMembersUpdate, ThreadMembersUpdate>()
                .WithPropertyName(m => m.RemovedMemberIDs, "removed_member_ids");

            options.AddDataObjectConverter<IStageInstanceCreate, StageInstanceCreate>()
                .WithPropertyName(i => i.IsDiscoveryDisabled, "discoverable_disabled");

            options.AddDataObjectConverter<IStageInstanceUpdate, StageInstanceUpdate>()
                .WithPropertyName(i => i.IsDiscoveryDisabled, "discoverable_disabled");

            options.AddDataObjectConverter<IStageInstanceDelete, StageInstanceDelete>()
                .WithPropertyName(i => i.IsDiscoveryDisabled, "discoverable_disabled");

            // Guilds
            options.AddDataObjectConverter<IGuildCreate, GuildCreate>()
                .WithPropertyName(g => g.IsOwner, "owner")
                .WithPropertyName(g => g.GuildFeatures, "features")
                .WithPropertyConverter(g => g.GuildFeatures, new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true)))
                .WithPropertyName(g => g.IsLarge, "large")
                .WithPropertyName(g => g.IsUnavailable, "unavailable")
                .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
                .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(g => g.JoinedAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IGuildUpdate, GuildUpdate>()
                .WithPropertyName(g => g.IsOwner, "owner")
                .WithPropertyName(g => g.GuildFeatures, "features")
                .WithPropertyConverter(g => g.GuildFeatures, new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true)))
                .WithPropertyName(g => g.IsLarge, "large")
                .WithPropertyName(g => g.IsUnavailable, "unavailable")
                .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
                .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(g => g.JoinedAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IGuildDelete, GuildDelete>()
                .WithPropertyName(d => d.IsUnavailable, "unavailable");

            options.AddDataObjectConverter<IGuildBanAdd, GuildBanAdd>();
            options.AddDataObjectConverter<IGuildBanRemove, GuildBanRemove>();

            options.AddDataObjectConverter<IGuildEmojisUpdate, GuildEmojisUpdate>();
            options.AddDataObjectConverter<IGuildIntegrationsUpdate, GuildIntegrationsUpdate>();

            options.AddDataObjectConverter<IGuildMemberAdd, GuildMemberAdd>()
                .WithPropertyName(m => m.Nickname, "nick")
                .WithPropertyName(m => m.IsDeafened, "deaf")
                .WithPropertyName(m => m.IsMuted, "mute")
                .WithPropertyName(m => m.IsPending, "pending")
                .WithPropertyConverter(m => m.JoinedAt, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(m => m.PremiumSince, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IGuildMemberRemove, GuildMemberRemove>();
            options.AddDataObjectConverter<IGuildMemberUpdate, GuildMemberUpdate>()
                .WithPropertyName(u => u.Nickname, "nick")
                .WithPropertyName(u => u.IsPending, "pending")
                .WithPropertyName(m => m.IsDeafened, "deaf")
                .WithPropertyName(m => m.IsMuted, "mute")
                .WithPropertyConverter(m => m.JoinedAt, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(m => m.PremiumSince, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IGuildMembersChunk, GuildMembersChunk>();

            options.AddDataObjectConverter<IGuildRoleCreate, GuildRoleCreate>();
            options.AddDataObjectConverter<IGuildRoleUpdate, GuildRoleUpdate>();
            options.AddDataObjectConverter<IGuildRoleDelete, GuildRoleDelete>();

            // Invites
            options.AddDataObjectConverter<IInviteCreate, InviteCreate>()
                .WithPropertyName(c => c.IsTemporary, "temporary")
                .WithPropertyConverter(c => c.CreatedAt, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(c => c.MaxAge, new UnitTimeSpanConverter(TimeUnit.Seconds));

            options.AddDataObjectConverter<IInviteDelete, InviteDelete>();

            // Messages
            options.AddDataObjectConverter<IMessageCreate, MessageCreate>()
                .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
                .WithPropertyName(m => m.MentionedRoles, "mention_roles")
                .WithPropertyName(m => m.MentionedChannels, "mention_channels")
                .WithPropertyName(m => m.IsTTS, "tts")
                .WithPropertyName(m => m.IsPinned, "pinned")
                .WithPropertyConverter(e => e.Timestamp, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(e => e.EditedTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IMessageUpdate, MessageUpdate>()
                .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
                .WithPropertyName(m => m.MentionedRoles, "mention_roles")
                .WithPropertyName(m => m.MentionedChannels, "mention_channels")
                .WithPropertyName(m => m.IsTTS, "tts")
                .WithPropertyName(m => m.IsPinned, "pinned")
                .WithPropertyConverter(e => e.Timestamp, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(e => e.EditedTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IMessageDelete, MessageDelete>();
            options.AddDataObjectConverter<IMessageDeleteBulk, MessageDeleteBulk>()
                .WithPropertyName(d => d.IDs, "ids");

            options.AddDataObjectConverter<IMessageReactionAdd, MessageReactionAdd>();
            options.AddDataObjectConverter<IMessageReactionRemove, MessageReactionRemove>();
            options.AddDataObjectConverter<IMessageReactionRemoveAll, MessageReactionRemoveAll>();
            options.AddDataObjectConverter<IMessageReactionRemoveEmoji, MessageReactionRemoveEmoji>();

            // Presences
            options.AddDataObjectConverter<IPresenceUpdate, PresenceUpdate>()
                .WithPropertyConverter(p => p.Status, new StringEnumConverter<ClientStatus>(new SnakeCaseNamingPolicy()));

            // Users
            options.AddDataObjectConverter<ITypingStart, TypingStart>()
                .WithPropertyConverter(t => t.Timestamp, new UnixSecondsDateTimeOffsetConverter());

            options.AddDataObjectConverter<IUserUpdate, UserUpdate>()
                .WithPropertyConverter(u => u.Discriminator, new DiscriminatorConverter())
                .WithPropertyName(u => u.IsBot, "bot")
                .WithPropertyName(u => u.IsSystem, "system")
                .WithPropertyName(u => u.IsVerified, "verified")
                .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled");

            // Voice
            options.AddDataObjectConverter<IVoiceStateUpdate, VoiceStateUpdate>()
                .WithPropertyName(v => v.IsDeafened, "deaf")
                .WithPropertyName(v => v.IsMuted, "mute")
                .WithPropertyName(v => v.IsSelfDeafened, "self_deaf")
                .WithPropertyName(v => v.IsSelfMuted, "self_mute")
                .WithPropertyName(v => v.IsStreaming, "self_stream")
                .WithPropertyName(v => v.IsVideoEnabled, "self_video")
                .WithPropertyName(v => v.IsSuppressed, "suppress");

            options.AddDataObjectConverter<IVoiceServerUpdate, VoiceServerUpdate>();

            // Webhooks
            options.AddDataObjectConverter<IWebhooksUpdate, WebhooksUpdate>();

            // Interactions
            options.AddDataObjectConverter<IInteractionCreate, InteractionCreate>();
            options.AddDataObjectConverter<IApplicationCommandCreate, ApplicationCommandCreate>();
            options.AddDataObjectConverter<IApplicationCommandUpdate, ApplicationCommandUpdate>();
            options.AddDataObjectConverter<IApplicationCommandDelete, ApplicationCommandDelete>();

            // Integrations
            options.AddDataObjectConverter<IIntegrationCreate, IntegrationCreate>()
                .WithPropertyName(i => i.IsEnabled, "enabled")
                .WithPropertyName(i => i.IsSyncing, "syncing")
                .WithPropertyName(i => i.IsRevoked, "revoked")
                .WithPropertyConverter(g => g.ExpireGracePeriod, new UnitTimeSpanConverter(TimeUnit.Days));

            options.AddDataObjectConverter<IIntegrationUpdate, IntegrationUpdate>()
                .WithPropertyName(i => i.IsEnabled, "enabled")
                .WithPropertyName(i => i.IsSyncing, "syncing")
                .WithPropertyName(i => i.IsRevoked, "revoked")
                .WithPropertyConverter(g => g.ExpireGracePeriod, new UnitTimeSpanConverter(TimeUnit.Days));

            options.AddDataObjectConverter<IIntegrationDelete, IntegrationDelete>();

            // Other
            options.AddDataObjectConverter<IUnknownEvent, UnknownEvent>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle activity objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddActivityObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IActivity, Activity>()
                .WithPropertyConverter(a => a.CreatedAt, new UnixMillisecondsDateTimeOffsetConverter());
            options.AddDataObjectConverter<IActivityAssets, ActivityAssets>();
            options.AddDataObjectConverter<IActivityButton, ActivityButton>();
            options.AddDataObjectConverter<IActivityEmoji, ActivityEmoji>();
            options.AddDataObjectConverter<IActivityParty, ActivityParty>();
            options.AddConverter<PartySizeConverter>();
            options.AddDataObjectConverter<IActivitySecrets, ActivitySecrets>();
            options.AddDataObjectConverter<IActivityTimestamps, ActivityTimestamps>()
                .WithPropertyConverter(t => t.Start, new UnixMillisecondsDateTimeOffsetConverter())
                .WithPropertyConverter(t => t.End, new UnixMillisecondsDateTimeOffsetConverter());

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle audit log objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddAuditLogObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IAuditLog, AuditLog>();
            options.AddDataObjectConverter<IAuditLogEntry, AuditLogEntry>();
            options.AddDataObjectConverter<IOptionalAuditEntryInfo, OptionalAuditEntryInfo>()
                .WithPropertyConverter
                (
                    ae => ae.Type,
                    new StringEnumConverter<PermissionOverwriteType>(asInteger: true)
                );

            options.AddConverter<AuditLogChangeConverter>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle channel objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddChannelObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IChannel, Channel>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.LastPinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IPartialChannel, PartialChannel>()
                .WithPropertyName(c => c.IsNsfw, "nsfw")
                .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(c => c.LastPinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IChannelMention, ChannelMention>();
            options.AddDataObjectConverter<IAllowedMentions, AllowedMentions>()
                .WithPropertyName(a => a.MentionRepliedUser, "replied_user")
                .WithPropertyConverter(m => m.Parse, new StringEnumListConverter<MentionType>(new SnakeCaseNamingPolicy()));

            options.AddDataObjectConverter<IFollowedChannel, FollowedChannel>();

            options.AddDataObjectConverter<IThreadMetadata, ThreadMetadata>()
                .WithPropertyName(m => m.IsArchived, "archived")
                .WithPropertyName(m => m.IsLocked, "locked")
                .WithPropertyConverter(m => m.AutoArchiveDuration, new UnitTimeSpanConverter(TimeUnit.Minutes))
                .WithPropertyConverter(m => m.ArchiveTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IThreadMember, ThreadMember>()
                .WithPropertyConverter(m => m.JoinTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IThreadQueryResponse, ThreadQueryResponse>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle emoji objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddEmojiObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IEmoji, Emoji>()
                .WithPropertyName(e => e.IsManaged, "managed")
                .WithPropertyName(e => e.IsAnimated, "animated")
                .WithPropertyName(e => e.IsAvailable, "available");

            options.AddDataObjectConverter<IPartialEmoji, PartialEmoji>()
                .WithPropertyName(e => e.IsManaged, "managed")
                .WithPropertyName(e => e.IsAnimated, "animated")
                .WithPropertyName(e => e.IsAvailable, "available");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle gateway objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddGatewayObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IGatewayEndpoint, GatewayEndpoint>();
            options.AddDataObjectConverter<ISessionStartLimit, SessionStartLimit>()
                .WithPropertyConverter(st => st.ResetAfter, new UnitTimeSpanConverter(TimeUnit.Milliseconds));

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle guild objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddGuildObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IGuild, Guild>()
                .WithPropertyName(g => g.IsOwner, "owner")
                .WithPropertyName(g => g.GuildFeatures, "features")
                .WithPropertyConverter(g => g.GuildFeatures, new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true)))
                .WithPropertyName(g => g.IsLarge, "large")
                .WithPropertyName(g => g.IsUnavailable, "unavailable")
                .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
                .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(g => g.JoinedAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IPartialGuild, PartialGuild>()
                .WithPropertyName(g => g.IsOwner, "owner")
                .WithPropertyName(g => g.GuildFeatures, "features")
                .WithPropertyConverter(g => g.GuildFeatures, new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true)))
                .WithPropertyName(g => g.IsLarge, "large")
                .WithPropertyName(g => g.IsUnavailable, "unavailable")
                .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
                .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds))
                .WithPropertyConverter(g => g.JoinedAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IGuildMember, GuildMember>()
                .WithPropertyName(m => m.Nickname, "nick")
                .WithPropertyName(m => m.IsDeafened, "deaf")
                .WithPropertyName(m => m.IsMuted, "mute")
                .WithPropertyName(m => m.IsPending, "pending")
                .WithPropertyConverter(m => m.JoinedAt, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(m => m.PremiumSince, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IPartialGuildMember, PartialGuildMember>()
                .WithPropertyName(m => m.Nickname, "nick")
                .WithPropertyName(m => m.IsDeafened, "deaf")
                .WithPropertyName(m => m.IsMuted, "mute")
                .WithPropertyName(m => m.IsPending, "pending")
                .WithPropertyConverter(m => m.JoinedAt, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(m => m.PremiumSince, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IUnavailableGuild, UnavailableGuild>()
                .WithPropertyName(u => u.GuildID, "id")
                .WithPropertyName(u => u.IsUnavailable, "unavailable");

            options.AddDataObjectConverter<IPruneCount, PruneCount>();
            options.AddDataObjectConverter<IBan, Ban>();
            options.AddDataObjectConverter<IGuildPreview, GuildPreview>()
                .WithPropertyConverter(p => p.Features, new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true)));

            options.AddDataObjectConverter<IGuildWidget, GuildWidget>()
                .WithPropertyName(w => w.IsEnabled, "enabled");

            options.AddDataObjectConverter<IWelcomeScreen, WelcomeScreen>();
            options.AddDataObjectConverter<IWelcomeScreenChannel, WelcomeScreenChannel>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle image objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddImageObjectConverters(this JsonSerializerOptions options)
        {
            options.AddConverter<ImageHashConverter>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle integration objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddIntegrationObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IAccount, Account>();

            options.AddDataObjectConverter<IIntegration, Integration>()
                .WithPropertyName(i => i.IsEnabled, "enabled")
                .WithPropertyName(i => i.IsSyncing, "syncing")
                .WithPropertyName(i => i.IsRevoked, "revoked")
                .WithPropertyConverter(g => g.ExpireGracePeriod, new UnitTimeSpanConverter(TimeUnit.Days))
                .WithPropertyConverter(i => i.SyncedAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IPartialIntegration, PartialIntegration>()
                .WithPropertyName(i => i.IsEnabled, "enabled")
                .WithPropertyName(i => i.IsSyncing, "syncing")
                .WithPropertyName(i => i.IsRevoked, "revoked")
                .WithPropertyConverter(g => g.ExpireGracePeriod, new UnitTimeSpanConverter(TimeUnit.Days))
                .WithPropertyConverter(i => i.SyncedAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IIntegrationApplication, IntegrationApplication>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle invite objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddInviteObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IInvite, Invite>()
                .WithPropertyConverter(i => i.ExpiresAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IPartialInvite, PartialInvite>()
                .WithPropertyConverter(i => i.ExpiresAt, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IInviteStageInstance, InviteStageInstance>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle message objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddMessageObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IAttachment, Attachment>();

            options.AddDataObjectConverter<IEmbed, Embed>()
                .WithPropertyConverter(e => e.Type, new StringEnumConverter<EmbedType>(new SnakeCaseNamingPolicy()))
                .WithPropertyConverter(e => e.Colour, new ColorConverter())
                .WithPropertyName(e => e.Colour, "color")
                .WithPropertyConverter(e => e.Timestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IEmbedAuthor, EmbedAuthor>();
            options.AddDataObjectConverter<IEmbedField, EmbedField>()
                .WithPropertyName(f => f.IsInline, "inline");

            options.AddDataObjectConverter<IEmbedFooter, EmbedFooter>();
            options.AddDataObjectConverter<IEmbedImage, EmbedImage>();
            options.AddDataObjectConverter<IEmbedProvider, EmbedProvider>();
            options.AddDataObjectConverter<IEmbedThumbnail, EmbedThumbnail>();
            options.AddDataObjectConverter<IEmbedVideo, EmbedVideo>();

            options.AddDataObjectConverter<IMessage, Message>()
                .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
                .WithPropertyName(m => m.MentionedRoles, "mention_roles")
                .WithPropertyName(m => m.MentionedChannels, "mention_channels")
                .WithPropertyName(m => m.IsTTS, "tts")
                .WithPropertyName(m => m.IsPinned, "pinned")
                .WithPropertyConverter(e => e.Timestamp, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(e => e.EditedTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IPartialMessage, PartialMessage>()
                .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
                .WithPropertyName(m => m.MentionedRoles, "mention_roles")
                .WithPropertyName(m => m.MentionedChannels, "mention_channels")
                .WithPropertyName(m => m.IsTTS, "tts")
                .WithPropertyName(m => m.IsPinned, "pinned")
                .WithPropertyConverter(e => e.Timestamp, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(e => e.EditedTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IMessageActivity, MessageActivity>();
            options.AddDataObjectConverter<IMessageReference, MessageReference>();

            options.AddDataObjectConverter<IMessageSticker, MessageSticker>()
                .WithPropertyConverter(s => s.Tags, new DelimitedListConverter<string>(";"));

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle permission objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddPermissionObjectConverters(this JsonSerializerOptions options)
        {
            options.AddConverter<DiscordPermissionSetConverter>();

            options.AddDataObjectConverter<IPermissionOverwrite, PermissionOverwrite>();

            options.AddDataObjectConverter<IRole, Role>()
                .WithPropertyName(r => r.Colour, "color")
                .WithPropertyName(r => r.IsHoisted, "hoist")
                .WithPropertyName(r => r.IsManaged, "managed")
                .WithPropertyName(r => r.IsMentionable, "mentionable");

            options.AddDataObjectConverter<IPartialRole, PartialRole>()
                .WithPropertyName(r => r.Colour, "color")
                .WithPropertyName(r => r.IsHoisted, "hoist")
                .WithPropertyName(r => r.IsManaged, "managed")
                .WithPropertyName(r => r.IsMentionable, "mentionable");

            options.AddDataObjectConverter<IRoleTags, RoleTags>()
                .WithPropertyName(t => t.IsPremiumSubscriberRole, "premium_subscriber");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle presence objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddPresenceObjectConverters(this JsonSerializerOptions options)
        {
            var snakeCase = new SnakeCaseNamingPolicy();

            options.AddDataObjectConverter<IClientStatuses, ClientStatuses>()
                .WithPropertyConverter(p => p.Desktop, new StringEnumConverter<ClientStatus>(snakeCase))
                .WithPropertyConverter(p => p.Mobile, new StringEnumConverter<ClientStatus>(snakeCase))
                .WithPropertyConverter(p => p.Web, new StringEnumConverter<ClientStatus>(snakeCase));

            options.AddDataObjectConverter<IPresence, Presence>()
                .WithPropertyConverter(p => p.Status, new StringEnumConverter<ClientStatus>(snakeCase));

            options.AddDataObjectConverter<IPartialPresence, PartialPresence>()
                .WithPropertyConverter(p => p.Status, new StringEnumConverter<ClientStatus>(snakeCase));

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle reaction objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddReactionObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IReaction, Reaction>()
                .WithPropertyName(r => r.HasCurrentUserReacted, "me");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle user objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddUserObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IUser, User>()
                .WithPropertyConverter(u => u.Discriminator, new DiscriminatorConverter())
                .WithPropertyName(u => u.IsBot, "bot")
                .WithPropertyName(u => u.IsSystem, "system")
                .WithPropertyName(u => u.IsVerified, "verified")
                .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled");

            options.AddDataObjectConverter<IPartialUser, PartialUser>()
                .WithPropertyConverter(u => u.Discriminator, new DiscriminatorConverter())
                .WithPropertyName(u => u.IsBot, "bot")
                .WithPropertyName(u => u.IsSystem, "system")
                .WithPropertyName(u => u.IsVerified, "verified")
                .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled");

            options.AddDataObjectConverter<IUserMention, UserMention>()
                .WithPropertyConverter(u => u.Discriminator, new DiscriminatorConverter())
                .WithPropertyName(m => m.IsBot, "bot")
                .WithPropertyName(m => m.IsSystem, "system")
                .WithPropertyName(m => m.IsVerified, "verified")
                .WithPropertyName(m => m.IsMFAEnabled, "mfa_enabled");

            options.AddDataObjectConverter<IConnection, Connection>()
                .WithPropertyName(c => c.IsRevoked, "revoked")
                .WithPropertyName(c => c.IsVerified, "verified")
                .WithPropertyName(c => c.IsFriendSyncEnabled, "friend_sync")
                .WithPropertyName(c => c.ShouldShowActivity, "show_activity");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle voice objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddVoiceObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IVoiceState, VoiceState>()
                .WithPropertyName(v => v.IsDeafened, "deaf")
                .WithPropertyName(v => v.IsMuted, "mute")
                .WithPropertyName(v => v.IsSelfDeafened, "self_deaf")
                .WithPropertyName(v => v.IsSelfMuted, "self_mute")
                .WithPropertyName(v => v.IsStreaming, "self_stream")
                .WithPropertyName(v => v.IsVideoEnabled, "self_video")
                .WithPropertyName(v => v.IsSuppressed, "suppress")
                .WithPropertyConverter(v => v.RequestToSpeakTimestamp, new ISO8601DateTimeOffsetConverter());

            options.AddDataObjectConverter<IPartialVoiceState, PartialVoiceState>()
                .WithPropertyName(v => v.IsDeafened, "deaf")
                .WithPropertyName(v => v.IsMuted, "mute")
                .WithPropertyName(v => v.IsSelfDeafened, "self_deaf")
                .WithPropertyName(v => v.IsSelfMuted, "self_mute")
                .WithPropertyName(v => v.IsStreaming, "self_stream")
                .WithPropertyName(v => v.IsVideoEnabled, "self_video")
                .WithPropertyName(v => v.IsSuppressed, "suppress");

            options.AddDataObjectConverter<IVoiceRegion, VoiceRegion>()
                .WithPropertyName(r => r.IsVIP, "vip")
                .WithPropertyName(r => r.IsDeprecated, "deprecated")
                .WithPropertyName(r => r.IsOptimal, "optimal")
                .WithPropertyName(r => r.IsCustom, "custom");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle webhook objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddWebhookObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IWebhook, Webhook>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle error objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddErrorObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IRestError, RestError>();
            options.AddDataObjectConverter<IErrorDetails, ErrorDetails>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle template objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddTemplateObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<ITemplate, Template>()
                .WithPropertyConverter(t => t.CreatedAt, new ISO8601DateTimeOffsetConverter())
                .WithPropertyConverter(t => t.UpdatedAt, new ISO8601DateTimeOffsetConverter());
            options.AddDataObjectConverter<IGuildTemplate, GuildTemplate>();
            options.AddDataObjectConverter<IRoleTemplate, RoleTemplate>()
                .WithPropertyName(r => r.Colour, "color")
                .WithPropertyName(r => r.IsHoisted, "hoist")
                .WithPropertyName(r => r.IsMentionable, "mentionable");

            options.AddDataObjectConverter<IChannelTemplate, ChannelTemplate>()
                .WithPropertyName(c => c.IsNsfw, "nsfw");

            options.AddDataObjectConverter<IPermissionOverwriteTemplate, PermissionOverwriteTemplate>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle interaction objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddInteractionObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IApplicationCommandInteractionData, ApplicationCommandInteractionData>();
            options.AddDataObjectConverter
            <
                IApplicationCommandInteractionDataOption, ApplicationCommandInteractionDataOption
            >();

            options.AddDataObjectConverter<IInteraction, Interaction>();
            options.AddDataObjectConverter
            <
                IInteractionApplicationCommandCallbackData, InteractionApplicationCommandCallbackData
            >()
            .WithPropertyName(d => d.IsTTS, "tts");

            options.AddDataObjectConverter<IInteractionResponse, InteractionResponse>();

            options.AddDataObjectConverter<IApplicationCommand, ApplicationCommand>();
            options.AddDataObjectConverter<IApplicationCommandOption, ApplicationCommandOption>()
                .WithPropertyName(o => o.IsDefault, "default")
                .WithPropertyName(o => o.IsRequired, "required");
            options.AddDataObjectConverter<IApplicationCommandOptionChoice, ApplicationCommandOptionChoice>();
            options.AddDataObjectConverter<IMessageInteraction, MessageInteraction>();

            options.AddDataObjectConverter
            <
                IApplicationCommandInteractionDataResolved,
                ApplicationCommandInteractionDataResolved
            >()
                .WithPropertyConverter(r => r.Users, new SnowflakeDictionaryConverter<IUser>())
                .WithPropertyConverter(r => r.Members, new SnowflakeDictionaryConverter<IPartialGuildMember>())
                .WithPropertyConverter(r => r.Roles, new SnowflakeDictionaryConverter<IRole>())
                .WithPropertyConverter(r => r.Channels, new SnowflakeDictionaryConverter<IPartialChannel>());

            options.AddDataObjectConverter<IGuildApplicationCommandPermissions, GuildApplicationCommandPermissions>();
            options.AddDataObjectConverter
            <
                IPartialGuildApplicationCommandPermissions,
                PartialGuildApplicationCommandPermissions
            >();
            options.AddDataObjectConverter<IApplicationCommandPermissions, ApplicationCommandPermissions>()
                .WithPropertyName(p => p.HasPermission, "permission");

            options.AddConverter<MessageComponentConverter>();

            options.AddDataObjectConverter<IComponent, Component>()
                .WithPropertyName(c => c.IsDisabled, "disabled");

            options.AddDataObjectConverter<IActionRowComponent, ActionRowComponent>();
            options.AddDataObjectConverter<IButtonComponent, ButtonComponent>()
                .WithPropertyName(c => c.IsDisabled, "disabled");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle OAuth2 objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddOAuth2ObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IApplication, Application>()
                .WithPropertyName(a => a.IsBotPublic, "bot_public")
                .WithPropertyName(a => a.DoesBotRequireCodeGrant, "bot_require_code_grant")
                .WithPropertyName(a => a.PrimarySKUID, "primary_sku_id");

            options.AddDataObjectConverter<IPartialApplication, PartialApplication>()
                .WithPropertyName(a => a.IsBotPublic, "bot_public")
                .WithPropertyName(a => a.DoesBotRequireCodeGrant, "bot_require_code_grant")
                .WithPropertyName(a => a.PrimarySKUID, "primary_sku_id");

            options.AddDataObjectConverter<IAuthorizationInformation, AuthorizationInformation>()
                .WithPropertyConverter(a => a.Expires, new ISO8601DateTimeOffsetConverter());

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle team objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddTeamObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<ITeam, Team>();
            options.AddDataObjectConverter<ITeamMember, TeamMember>();

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle stage instance objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddStageInstanceObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IStageInstance, StageInstance>()
                .WithPropertyName(i => i.IsDiscoveryDisabled, "discoverable_disabled");

            return options;
        }
    }
}
