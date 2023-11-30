//
//  ServiceCollectionExtensions.cs
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

using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Abstractions.VoiceGateway.Commands;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Gateway.Events.Channels;
using Remora.Discord.API.Json;
using Remora.Discord.API.Objects;
using Remora.Discord.API.Rest;
using Remora.Discord.API.VoiceGateway.Commands;
using Remora.Discord.API.VoiceGateway.Events;
using Remora.Rest.Extensions;
using Remora.Rest.Json;
using Remora.Rest.Json.Policies;
using ClientStatus = Remora.Discord.API.Objects.ClientStatus;

namespace Remora.Discord.API.Extensions;

/// <summary>
/// Defines various extension methods to the <see cref="IServiceCollection"/> class.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures Discord-specific JSON converters.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="optionsName">The name of the serializer options, if any.</param>
    /// <param name="allowUnknownEvents">Whether the API will deserialize unknown events.</param>
    /// <returns>The service collection, with the services.</returns>
    public static IServiceCollection ConfigureDiscordJsonConverters
    (
        this IServiceCollection serviceCollection,
        string? optionsName = "Discord",
        bool allowUnknownEvents = true
    )
    {
        var snakeCase = new SnakeCaseNamingPolicy();

        serviceCollection.TryAddSingleton(snakeCase);
        serviceCollection.ConfigureRestJsonConverters(optionsName);

        serviceCollection
            .Configure<JsonSerializerOptions>
            (
                optionsName,
                options =>
                {
                    options.PropertyNamingPolicy = snakeCase;

                    options.Converters.Add(new PayloadConverter(allowUnknownEvents));
                    options.Converters.Add(new VoicePayloadConverter());

                    options
                        .AddGatewayBidirectionalConverters()
                        .AddGatewayCommandConverters()
                        .AddGatewayEventConverters()
                        .AddVoiceGatewayCommandConverters()
                        .AddVoiceGatewayEventConverters()
                        .AddActivityObjectConverters()
                        .AddAuditLogObjectConverters()
                        .AddAutoModerationObjectConverters()
                        .AddChannelObjectConverters()
                        .AddEmojiObjectConverters()
                        .AddGatewayObjectConverters()
                        .AddGuildObjectConverters()
                        .AddGuildScheduledEventObjectConverters()
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
                        .AddStageInstanceObjectConverters()
                        .AddStickerObjectConverters()
                        .AddApplicationRoleConnectionObjectConverters()
                        .AddMonetizationConverters();

                    options.AddDataObjectConverter<IUnknownEvent, UnknownEvent>();
                    options.AddConverter<PropertyErrorDetailsConverter>();

                    options.Converters.Insert(0, new SnowflakeConverter(Constants.DiscordEpoch));
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

        options.AddDataObjectConverter<IIdentifyConnectionProperties, IdentifyConnectionProperties>()
            .WithPropertyName(p => p.OperatingSystem, "os")
            .WithPropertyName(p => p.Browser, "browser")
            .WithPropertyName(p => p.Device, "device");

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
                new StringEnumConverter<UserStatus>(new SnakeCaseNamingPolicy())
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

        // Auto Moderation
        options.AddDataObjectConverter<IAutoModerationActionExecution, AutoModerationActionExecution>();

        options.AddDataObjectConverter<IAutoModerationRuleCreate, AutoModerationRuleCreate>()
            .WithPropertyName(r => r.IsEnabled, "enabled");

        options.AddDataObjectConverter<IAutoModerationRuleUpdate, AutoModerationRuleUpdate>()
            .WithPropertyName(r => r.IsEnabled, "enabled");

        options.AddDataObjectConverter<IAutoModerationRuleDelete, AutoModerationRuleDelete>()
            .WithPropertyName(r => r.IsEnabled, "enabled");

        // Audit logs
        options.AddDataObjectConverter<IGuildAuditLogEntryCreate, GuildAuditLogEntryCreate>();

        // Channels
        options.AddDataObjectConverter<IChannelCreate, ChannelCreate>()
            .WithPropertyName(c => c.IsNsfw, "nsfw")
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IChannelUpdate, ChannelUpdate>()
            .WithPropertyName(c => c.IsNsfw, "nsfw")
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IChannelDelete, ChannelDelete>()
            .WithPropertyName(c => c.IsNsfw, "nsfw")
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IChannelPinsUpdate, ChannelPinsUpdate>();

        options.AddDataObjectConverter<IThreadCreate, ThreadCreate>()
            .WithPropertyName(c => c.IsNewlyCreated, "newly_created")
            .WithPropertyName(c => c.IsNsfw, "nsfw")
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IThreadUpdate, ThreadUpdate>()
            .WithPropertyName(c => c.IsNsfw, "nsfw")
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IThreadDelete, ThreadDelete>()
            .WithPropertyName(c => c.IsNsfw, "nsfw")
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IThreadListSync, ThreadListSync>()
            .WithPropertyName(t => t.ChannelIDs, "channel_ids");

        options.AddDataObjectConverter<IThreadMemberUpdate, ThreadMemberUpdate>();

        options.AddDataObjectConverter<IThreadMembersUpdate, ThreadMembersUpdate>()
            .WithPropertyName(m => m.RemovedMemberIDs, "removed_member_ids");

        options.AddDataObjectConverter<IStageInstanceCreate, StageInstanceCreate>()
            .WithPropertyName(i => i.IsDiscoveryDisabled, "discoverable_disabled");

        options.AddDataObjectConverter<IStageInstanceUpdate, StageInstanceUpdate>()
            .WithPropertyName(i => i.IsDiscoveryDisabled, "discoverable_disabled");

        options.AddDataObjectConverter<IStageInstanceDelete, StageInstanceDelete>()
            .WithPropertyName(i => i.IsDiscoveryDisabled, "discoverable_disabled");

        // Guilds
        options.AddConverter<GuildCreateConverter>();
        options.AddDataObjectConverter<IGuildCreate.IAvailableGuild, GuildCreate.AvailableGuild>()
            .WithPropertyName(g => g.IsOwner, "owner")
            .WithPropertyName(g => g.GuildFeatures, "features")
            .WithPropertyConverter
            (
                g => g.GuildFeatures,
                new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true))
            )
            .WithPropertyName(g => g.IsLarge, "large")
            .WithPropertyName(g => g.IsUnavailable, "unavailable")
            .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
            .WithPropertyName(g => g.IsPremiumProgressBarEnabled, "premium_progress_bar_enabled")
            .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IGuildUpdate, GuildUpdate>()
            .WithPropertyName(g => g.IsOwner, "owner")
            .WithPropertyName(g => g.GuildFeatures, "features")
            .WithPropertyConverter
            (
                g => g.GuildFeatures,
                new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true))
            )
            .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
            .WithPropertyName(g => g.IsPremiumProgressBarEnabled, "premium_progress_bar_enabled")
            .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds));

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
            .WithPropertyName(m => m.IsPending, "pending");

        options.AddDataObjectConverter<IGuildMemberRemove, GuildMemberRemove>();
        options.AddDataObjectConverter<IGuildMemberUpdate, GuildMemberUpdate>()
            .WithPropertyName(u => u.Nickname, "nick")
            .WithPropertyName(u => u.IsPending, "pending")
            .WithPropertyName(m => m.IsDeafened, "deaf")
            .WithPropertyName(m => m.IsMuted, "mute");

        options.AddDataObjectConverter<IGuildMembersChunk, GuildMembersChunk>();

        options.AddDataObjectConverter<IGuildRoleCreate, GuildRoleCreate>();
        options.AddDataObjectConverter<IGuildRoleUpdate, GuildRoleUpdate>();
        options.AddDataObjectConverter<IGuildRoleDelete, GuildRoleDelete>();

        // Guild scheduled events
        options.AddDataObjectConverter<IGuildScheduledEventCreate, GuildScheduledEventCreate>();
        options.AddDataObjectConverter<IGuildScheduledEventDelete, GuildScheduledEventDelete>();
        options.AddDataObjectConverter<IGuildScheduledEventUpdate, GuildScheduledEventUpdate>();

        // Invites
        options.AddDataObjectConverter<IInviteCreate, InviteCreate>()
            .WithPropertyName(c => c.IsTemporary, "temporary")
            .WithPropertyConverter(c => c.MaxAge, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IInviteDelete, InviteDelete>();

        // Messages
        options.AddDataObjectConverter<IMessageCreate, MessageCreate>()
            .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
            .WithPropertyName(m => m.MentionedRoles, "mention_roles")
            .WithPropertyName(m => m.MentionedChannels, "mention_channels")
            .WithPropertyName(m => m.IsTTS, "tts")
            .WithPropertyName(m => m.IsPinned, "pinned");

        options.AddDataObjectConverter<IMessageUpdate, MessageUpdate>()
            .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
            .WithPropertyName(m => m.MentionedRoles, "mention_roles")
            .WithPropertyName(m => m.MentionedChannels, "mention_channels")
            .WithPropertyName(m => m.IsTTS, "tts")
            .WithPropertyName(m => m.IsPinned, "pinned");

        options.AddDataObjectConverter<IMessageDelete, MessageDelete>();
        options.AddDataObjectConverter<IMessageDeleteBulk, MessageDeleteBulk>()
            .WithPropertyName(d => d.IDs, "ids");

        options.AddDataObjectConverter<IMessageReactionAdd, MessageReactionAdd>();
        options.AddDataObjectConverter<IMessageReactionRemove, MessageReactionRemove>();
        options.AddDataObjectConverter<IMessageReactionRemoveAll, MessageReactionRemoveAll>();
        options.AddDataObjectConverter<IMessageReactionRemoveEmoji, MessageReactionRemoveEmoji>();

        // Presences
        options.AddDataObjectConverter<IPresenceUpdate, PresenceUpdate>()
            .WithPropertyConverter(p => p.Status, new StringEnumConverter<UserStatus>(new SnakeCaseNamingPolicy()));

        // Users
        options.AddDataObjectConverter<ITypingStart, TypingStart>()
            .WithPropertyConverter(t => t.Timestamp, new UnixSecondsDateTimeOffsetConverter());

        options.AddDataObjectConverter<IUserUpdate, UserUpdate>()
            .WithPropertyConverter(u => u.Discriminator, new DiscriminatorConverter())
            .WithPropertyName(u => u.IsBot, "bot")
            .WithPropertyName(u => u.IsSystem, "system")
            .WithPropertyName(u => u.IsVerified, "verified")
            .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled")
            .WithPropertyName(u => u.AccentColour, "accent_color");

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

        // Stickers
        options.AddDataObjectConverter<IGuildStickersUpdate, GuildStickersUpdate>();

        // Application commands
        options.AddDataObjectConverter<IApplicationCommandPermissionsUpdate, ApplicationCommandPermissionsUpdate>();

        // Monetization
        options.AddDataObjectConverter<IEntitlementCreate, EntitlementCreate>()
            .WithPropertyName(e => e.SKUID, "sku_id")
            .WithPropertyName(e => e.IsDeleted, "deleted");

        options.AddDataObjectConverter<IEntitlementUpdate, EntitlementUpdate>()
            .WithPropertyName(e => e.SKUID, "sku_id")
            .WithPropertyName(e => e.IsDeleted, "deleted");

        options.AddDataObjectConverter<IEntitlementDelete, EntitlementDelete>()
            .WithPropertyName(e => e.SKUID, "sku_id")
            .WithPropertyName(e => e.IsDeleted, "deleted");

        // Other
        options.AddDataObjectConverter<IUnknownEvent, UnknownEvent>();

        return options;
    }

    /// <summary>
    /// Adds the JSON converters that handle gateway command payloads.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddVoiceGatewayCommandConverters(this JsonSerializerOptions options)
    {
        // ConnectingResuming
        options.AddDataObjectConverter<IVoiceIdentify, VoiceIdentify>();
        options.AddDataObjectConverter<IVoiceResume, VoiceResume>();

        // Heartbeats
        options.AddConverter<VoiceHeartbeatConverter>();

        // Protocols
        options.AddDataObjectConverter<IVoiceProtocolData, VoiceProtocolData>();
        options.AddDataObjectConverter<IVoiceSelectProtocol, VoiceSelectProtocol>();
        options.AddDataObjectConverter<IVoiceSpeakingCommand, VoiceSpeakingCommand>();

        return options;
    }

    /// <summary>
    /// Adds the JSON converters that handle gateway event payloads.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddVoiceGatewayEventConverters(this JsonSerializerOptions options)
    {
        // Connecting/Resuming
        options.AddDataObjectConverter<IVoiceHello, VoiceHello>()
            .WithPropertyConverter(v => v.HeartbeatInterval, new UnitTimeSpanConverter(TimeUnit.Milliseconds));

        options.AddConverter<IPAddressConverter>();
        options.AddDataObjectConverter<IVoiceReady, VoiceReady>();

        // Heartbeats
        options.AddConverter<VoiceHeartbeatAcknowledgeConverter>();

        // Sessions
        options.AddDataObjectConverter<IVoiceSessionDescription, VoiceSessionDescription>();

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
    /// Adds the JSON converters that handle auto moderation objects.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddAutoModerationObjectConverters(this JsonSerializerOptions options)
    {
        options.AddDataObjectConverter<IAutoModerationAction, AutoModerationAction>();

        options.AddDataObjectConverter<IAutoModerationActionMetadata, AutoModerationActionMetadata>()
            .WithPropertyName(m => m.Duration, "duration_seconds")
            .WithPropertyConverter(am => am.Duration, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IAutoModerationTriggerMetadata, AutoModerationTriggerMetadata>();

        options.AddDataObjectConverter<IAutoModerationRule, AutoModerationRule>()
            .WithPropertyName(r => r.IsEnabled, "enabled");

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
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IPartialChannel, PartialChannel>()
            .WithPropertyName(c => c.IsNsfw, "nsfw")
            .WithPropertyName(c => c.IsManaged, "managed")
            .WithPropertyConverter(c => c.RateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds))
            .WithPropertyConverter(c => c.DefaultThreadRateLimitPerUser, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IChannelMention, ChannelMention>();
        options.AddDataObjectConverter<IAllowedMentions, AllowedMentions>()
            .WithPropertyName(a => a.MentionRepliedUser, "replied_user")
            .WithPropertyConverter(m => m.Parse, new StringEnumListConverter<MentionType>(new SnakeCaseNamingPolicy()));

        options.AddDataObjectConverter<IFollowedChannel, FollowedChannel>();

        options.AddDataObjectConverter<IThreadMetadata, ThreadMetadata>()
            .WithPropertyName(m => m.IsArchived, "archived")
            .WithPropertyName(m => m.IsInvitable, "invitable")
            .WithPropertyName(m => m.IsLocked, "locked");

        options.AddDataObjectConverter<IThreadMember, ThreadMember>();

        options.AddDataObjectConverter<IChannelThreadQueryResponse, ChannelThreadQueryResponse>();
        options.AddDataObjectConverter<IDefaultReaction, DefaultReaction>();
        options.AddDataObjectConverter<IForumTag, ForumTag>()
            .WithPropertyName(t => t.IsModerated, "moderated");

        options.AddDataObjectConverter<IPartialForumTag, PartialForumTag>()
            .WithPropertyName(t => t.IsModerated, "moderated");

        // REST-related types not strictly defined by Discord
        options.AddDataObjectConverter<IChannelPositionModification, ChannelPositionModification>();

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
            .WithPropertyConverter
            (
                g => g.GuildFeatures,
                new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true))
            )
            .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
            .WithPropertyName(g => g.IsPremiumProgressBarEnabled, "premium_progress_bar_enabled")
            .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IPartialGuild, PartialGuild>()
            .WithPropertyName(g => g.IsOwner, "owner")
            .WithPropertyName(g => g.GuildFeatures, "features")
            .WithPropertyConverter
            (
                g => g.GuildFeatures,
                new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true))
            )
            .WithPropertyName(g => g.IsWidgetEnabled, "widget_enabled")
            .WithPropertyName(g => g.IsPremiumProgressBarEnabled, "premium_progress_bar_enabled")
            .WithPropertyConverter(g => g.AFKTimeout, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IGuildMember, GuildMember>()
            .WithPropertyName(m => m.Nickname, "nick")
            .WithPropertyName(m => m.IsDeafened, "deaf")
            .WithPropertyName(m => m.IsMuted, "mute")
            .WithPropertyName(m => m.IsPending, "pending");

        options.AddDataObjectConverter<IPartialGuildMember, PartialGuildMember>()
            .WithPropertyName(m => m.Nickname, "nick")
            .WithPropertyName(m => m.IsDeafened, "deaf")
            .WithPropertyName(m => m.IsMuted, "mute")
            .WithPropertyName(m => m.IsPending, "pending");

        options.AddDataObjectConverter<IUnavailableGuild, UnavailableGuild>()
            .WithPropertyName(u => u.IsUnavailable, "unavailable");

        options.AddDataObjectConverter<IPruneCount, PruneCount>();
        options.AddDataObjectConverter<IBan, Ban>();
        options.AddDataObjectConverter<IGuildPreview, GuildPreview>()
            .WithPropertyConverter
            (
                p => p.Features,
                new StringEnumListConverter<GuildFeature>(new SnakeCaseNamingPolicy(true))
            );

        options.AddDataObjectConverter<IGuildWidgetSettings, GuildWidgetSettings>()
            .WithPropertyName(w => w.IsEnabled, "enabled");

        options.AddDataObjectConverter<IGuildWidget, GuildWidget>();
        options.AddDataObjectConverter<IWelcomeScreen, WelcomeScreen>();
        options.AddDataObjectConverter<IWelcomeScreenChannel, WelcomeScreenChannel>();
        options.AddDataObjectConverter<IGuildThreadQueryResponse, GuildThreadQueryResponse>();
        options.AddDataObjectConverter<IGuildOnboarding, GuildOnboarding>()
            .WithPropertyName(o => o.DefaultChannelIDs, "default_channel_ids")
            .WithPropertyName(o => o.IsEnabled, "enabled");

        options.AddDataObjectConverter<IOnboardingPrompt, OnboardingPrompt>()
            .WithPropertyName(p => p.IsSingleSelect, "single_select")
            .WithPropertyName(p => p.IsRequired, "required")
            .WithPropertyName(p => p.IsInOnboarding, "in_onboarding");

        options.AddDataObjectConverter<IPromptOption, PromptOption>()
            .WithPropertyName(o => o.ChannelIDs, "channel_ids")
            .WithPropertyName(o => o.RoleIDs, "role_ids");

        return options;
    }

    /// <summary>
    /// Adds the JSON converters that handle guild objects.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddGuildScheduledEventObjectConverters(this JsonSerializerOptions options)
    {
        options.AddDataObjectConverter<IGuildScheduledEvent, GuildScheduledEvent>();
        options.AddDataObjectConverter<IGuildScheduledEventEntityMetadata, GuildScheduledEventEntityMetadata>();
        options.AddDataObjectConverter<IGuildScheduledEventUser, GuildScheduledEventUser>();
        options.AddDataObjectConverter<IGuildScheduledEvent, GuildScheduledEvent>();

        options.AddDataObjectConverter<IGuildScheduledEventUserAdd, GuildScheduledEventUserAdd>();
        options.AddDataObjectConverter<IGuildScheduledEventUserRemove, GuildScheduledEventUserRemove>();

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
        options.AddDataObjectConverter<IIntegrationAccount, IntegrationAccount>();

        options.AddDataObjectConverter<IIntegration, Integration>()
            .WithPropertyName(i => i.IsEnabled, "enabled")
            .WithPropertyName(i => i.IsSyncing, "syncing")
            .WithPropertyName(i => i.IsRevoked, "revoked")
            .WithPropertyConverter(g => g.ExpireGracePeriod, new UnitTimeSpanConverter(TimeUnit.Days));

        options.AddDataObjectConverter<IPartialIntegration, PartialIntegration>()
            .WithPropertyName(i => i.IsEnabled, "enabled")
            .WithPropertyName(i => i.IsSyncing, "syncing")
            .WithPropertyName(i => i.IsRevoked, "revoked")
            .WithPropertyConverter(g => g.ExpireGracePeriod, new UnitTimeSpanConverter(TimeUnit.Days));

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
        options.AddDataObjectConverter<IInvite, Invite>();
        options.AddDataObjectConverter<IPartialInvite, PartialInvite>();

        return options;
    }

    /// <summary>
    /// Adds the JSON converters that handle message objects.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddMessageObjectConverters(this JsonSerializerOptions options)
    {
        options.AddDataObjectConverter<IAttachment, Attachment>()
            .WithPropertyName(a => a.IsEphemeral, "ephemeral")
            .WithPropertyName(a => a.Duration, "duration_secs")
            .WithPropertyConverter(a => a.Duration, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IPartialAttachment, PartialAttachment>()
            .WithPropertyName(a => a.IsEphemeral, "ephemeral")
            .WithPropertyName(a => a.Duration, "duration_secs")
            .WithPropertyConverter(a => a.Duration, new UnitTimeSpanConverter(TimeUnit.Seconds));

        options.AddDataObjectConverter<IEmbed, Embed>()
            .WithPropertyConverter(e => e.Type, new StringEnumConverter<EmbedType>(new SnakeCaseNamingPolicy()))
            .WithPropertyConverter(e => e.Colour, new ColorConverter())
            .WithPropertyName(e => e.Colour, "color");

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
            .WithPropertyName(m => m.IsPinned, "pinned");

        options.AddDataObjectConverter<IPartialMessage, PartialMessage>()
            .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
            .WithPropertyName(m => m.MentionedRoles, "mention_roles")
            .WithPropertyName(m => m.MentionedChannels, "mention_channels")
            .WithPropertyName(m => m.IsTTS, "tts")
            .WithPropertyName(m => m.IsPinned, "pinned");

        options.AddDataObjectConverter<IMessageActivity, MessageActivity>();
        options.AddDataObjectConverter<IMessageReference, MessageReference>();

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
        options.AddDataObjectConverter<IPartialPermissionOverwrite, PartialPermissionOverwrite>();

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
            .WithPropertyName(t => t.IsPremiumSubscriberRole, "premium_subscriber")
            .WithPropertyName(t => t.IsAvailableForPurchase, "available_for_purchase")
            .WithPropertyName(t => t.HasGuildConnections, "guild_connections");

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

        options.AddDataObjectConverter<IClientStatus, ClientStatus>()
            .WithPropertyConverter(p => p.Desktop, new StringEnumConverter<UserStatus>(snakeCase))
            .WithPropertyConverter(p => p.Mobile, new StringEnumConverter<UserStatus>(snakeCase))
            .WithPropertyConverter(p => p.Web, new StringEnumConverter<UserStatus>(snakeCase));

        options.AddDataObjectConverter<IPresence, Presence>()
            .WithPropertyConverter(p => p.Status, new StringEnumConverter<UserStatus>(snakeCase));

        options.AddDataObjectConverter<IPartialPresence, PartialPresence>()
            .WithPropertyConverter(p => p.Status, new StringEnumConverter<UserStatus>(snakeCase));

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
            .WithPropertyName(r => r.HasCurrentUserReacted, "me")
            .WithPropertyName(r => r.HasCurrentUserBurstReacted, "me_burst")
            .WithPropertyName(r => r.BurstColours, "burst_colors")
            .WithPropertyConverter(r => r.BurstColours, new HexCodeColourConverter());

        options.AddDataObjectConverter<IReactionCountDetails, ReactionCountDetails>();

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
            .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled")
            .WithPropertyName(u => u.AccentColour, "accent_color");

        options.AddDataObjectConverter<IPartialUser, PartialUser>()
            .WithPropertyConverter(u => u.Discriminator, new DiscriminatorConverter())
            .WithPropertyName(u => u.IsBot, "bot")
            .WithPropertyName(u => u.IsSystem, "system")
            .WithPropertyName(u => u.IsVerified, "verified")
            .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled")
            .WithPropertyName(u => u.AccentColour, "accent_color");

        options.AddDataObjectConverter<IUserMention, UserMention>()
            .WithPropertyConverter(u => u.Discriminator, new DiscriminatorConverter())
            .WithPropertyName(m => m.IsBot, "bot")
            .WithPropertyName(m => m.IsSystem, "system")
            .WithPropertyName(m => m.IsVerified, "verified")
            .WithPropertyName(m => m.IsMFAEnabled, "mfa_enabled")
            .WithPropertyName(u => u.AccentColour, "accent_color");

        options.AddDataObjectConverter<IConnection, Connection>()
            .WithPropertyName(c => c.IsRevoked, "revoked")
            .WithPropertyName(c => c.IsVerified, "verified")
            .WithPropertyName(c => c.IsFriendSyncEnabled, "friend_sync")
            .WithPropertyName(c => c.IsTwoWayLink, "two_way_link")
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
            .WithPropertyName(v => v.IsSuppressed, "suppress");

        options.AddDataObjectConverter<IPartialVoiceState, PartialVoiceState>()
            .WithPropertyName(v => v.IsDeafened, "deaf")
            .WithPropertyName(v => v.IsMuted, "mute")
            .WithPropertyName(v => v.IsSelfDeafened, "self_deaf")
            .WithPropertyName(v => v.IsSelfMuted, "self_mute")
            .WithPropertyName(v => v.IsStreaming, "self_stream")
            .WithPropertyName(v => v.IsVideoEnabled, "self_video")
            .WithPropertyName(v => v.IsSuppressed, "suppress");

        options.AddDataObjectConverter<IVoiceRegion, VoiceRegion>()
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
        options.AddDataObjectConverter<IRestError, RestError>()
            .WithPropertyName(e => e.IsGlobal, "global");

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
        options.AddDataObjectConverter<ITemplate, Template>();
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
        options.AddDataObjectConverter<IApplicationCommandData, ApplicationCommandData>();
        options.AddDataObjectConverter<IMessageComponentData, MessageComponentData>();
        options.AddDataObjectConverter<IModalSubmitData, ModalSubmitData>();
        options.AddDataObjectConverter
            <
                IApplicationCommandInteractionDataOption,
                ApplicationCommandInteractionDataOption
            >()
            .WithPropertyName(o => o.IsFocused, "focused");

        options.AddDataObjectConverter<IInteraction, Interaction>();
        options.AddDataObjectConverter
            <
                IInteractionMessageCallbackData,
                InteractionMessageCallbackData
            >()
            .WithPropertyName(d => d.IsTTS, "tts");

        options.AddDataObjectConverter<IInteractionAutocompleteCallbackData, InteractionAutocompleteCallbackData>();
        options.AddDataObjectConverter<IInteractionModalCallbackData, InteractionModalCallbackData>();
        options.AddDataObjectConverter<IInteractionResponse, InteractionResponse>();

        options.AddDataObjectConverter<IApplicationCommand, ApplicationCommand>()
            .WithPropertyName(d => d.IsNsfw, "nsfw")
            .WithPropertyName(d => d.AllowedContextTypes, "contexts")
            .WithPropertyName(d => d.AllowedIntegrationTypes, "integration_types");

        options.AddDataObjectConverter<IApplicationCommandOption, ApplicationCommandOption>()
            .WithPropertyName(o => o.IsDefault, "default")
            .WithPropertyName(o => o.IsRequired, "required")
            .WithPropertyName(o => o.EnableAutocomplete, "autocomplete");

        options.AddDataObjectConverter<IApplicationCommandOptionChoice, ApplicationCommandOptionChoice>();
        options.AddDataObjectConverter<IMessageInteraction, MessageInteraction>();
        options.AddDataObjectConverter<IBulkApplicationCommandData, BulkApplicationCommandData>()
            .WithPropertyName(d => d.IsNsfw, "nsfw")
            .WithPropertyName(d => d.AllowedContextTypes, "contexts")
            .WithPropertyName(d => d.AllowedIntegrationTypes, "integration_types");

        options.AddDataObjectConverter
            <
                IApplicationCommandInteractionDataResolved,
                ApplicationCommandInteractionDataResolved
            >()
            .WithPropertyConverter
            (
                r => r.Users,
                new SnowflakeDictionaryConverter<IUser>(Constants.DiscordEpoch)
            )
            .WithPropertyConverter
            (
                r => r.Members,
                new SnowflakeDictionaryConverter<IPartialGuildMember>(Constants.DiscordEpoch)
            )
            .WithPropertyConverter
            (
                r => r.Roles,
                new SnowflakeDictionaryConverter<IRole>(Constants.DiscordEpoch)
            )
            .WithPropertyConverter
            (
                r => r.Channels,
                new SnowflakeDictionaryConverter<IPartialChannel>(Constants.DiscordEpoch)
            )
            .WithPropertyConverter
            (
                r => r.Messages,
                new SnowflakeDictionaryConverter<IPartialMessage>(Constants.DiscordEpoch)
            )
            .WithPropertyConverter
            (
                r => r.Attachments,
                new SnowflakeDictionaryConverter<IAttachment>(Constants.DiscordEpoch)
            );

        options.AddDataObjectConverter<IGuildApplicationCommandPermissions, GuildApplicationCommandPermissions>();
        options.AddDataObjectConverter
        <
            IPartialGuildApplicationCommandPermissions,
            PartialGuildApplicationCommandPermissions
        >();

        options.AddDataObjectConverter<IApplicationCommandPermissions, ApplicationCommandPermissions>()
            .WithPropertyName(p => p.HasPermission, "permission");

        options.AddConverter<MessageComponentConverter>();
        options.AddConverter<PartialMessageComponentConverter>();

        options.AddDataObjectConverter<IActionRowComponent, ActionRowComponent>()
            .IncludeWhenSerializing(c => c.Type);

        options.AddDataObjectConverter<IPartialActionRowComponent, PartialActionRowComponent>()
            .IncludeWhenSerializing(c => c.Type);

        options.AddDataObjectConverter<IButtonComponent, ButtonComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IPartialButtonComponent, PartialButtonComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IStringSelectComponent, StringSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IUserSelectComponent, UserSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IRoleSelectComponent, RoleSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IMentionableSelectComponent, MentionableSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IChannelSelectComponent, ChannelSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IPartialStringSelectComponent, PartialStringSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IPartialUserSelectComponent, PartialUserSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IPartialRoleSelectComponent, PartialRoleSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IPartialMentionableSelectComponent, PartialMentionableSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<IPartialChannelSelectComponent, PartialChannelSelectComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(c => c.IsDisabled, "disabled");

        options.AddDataObjectConverter<ITextInputComponent, TextInputComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(i => i.IsRequired, "required");

        options.AddDataObjectConverter<IPartialTextInputComponent, PartialTextInputComponent>()
            .IncludeWhenSerializing(c => c.Type)
            .WithPropertyName(i => i.IsRequired, "required");

        options.AddDataObjectConverter<ISelectOption, SelectOption>()
            .WithPropertyName(o => o.IsDefault, "default");

        options.AddDataObjectConverter<IPartialSelectOption, PartialSelectOption>()
            .WithPropertyName(o => o.IsDefault, "default");

        options.AddDataObjectConverter<ISelectDefaultValue, SelectDefaultValue>();

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

        options.AddDataObjectConverter<IApplicationInstallParameters, ApplicationInstallParameters>();

        options.AddDataObjectConverter<IAuthorizationInformation, AuthorizationInformation>();

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
        options.AddDataObjectConverter<ITeamMember, TeamMember>()
            .WithPropertyConverter(m => m.Role, new StringEnumConverter<TeamMemberRole>(new SnakeCaseNamingPolicy()));

        return options;
    }

    /// <summary>
    /// Adds the JSON converters that handle monetization objects.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddMonetizationConverters(this JsonSerializerOptions options)
    {
        options.AddDataObjectConverter<IEntitlement, Entitlement>()
            .WithPropertyName(e => e.SKUID, "sku_id")
            .WithPropertyName(e => e.IsDeleted, "deleted");
        options.AddDataObjectConverter<IPartialEntitlement, PartialEntitlement>()
            .WithPropertyName(e => e.SKUID, "sku_id")
            .WithPropertyName(e => e.IsDeleted, "deleted");

        options.AddDataObjectConverter<ISKU, SKU>();

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

    /// <summary>
    /// Adds the JSON converters that handle sticker objects.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddStickerObjectConverters(this JsonSerializerOptions options)
    {
        options.AddDataObjectConverter<ISticker, Sticker>()
            .WithPropertyName(s => s.IsAvailable, "available");

        options.AddDataObjectConverter<IStickerItem, StickerItem>();

        options.AddDataObjectConverter<IStickerPack, StickerPack>()
            .WithPropertyName(s => s.SKUID, "sku_id");

        options.AddDataObjectConverter<INitroStickerPacks, NitroStickerPacks>();

        return options;
    }

    /// <summary>
    /// Adds the JSON converters that handle application role connection objects.
    /// </summary>
    /// <param name="options">The serializer options.</param>
    /// <returns>The options, with the converters added.</returns>
    private static JsonSerializerOptions AddApplicationRoleConnectionObjectConverters
    (
        this JsonSerializerOptions options
    )
    {
        options.AddDataObjectConverter<IApplicationRoleConnectionMetadata, ApplicationRoleConnectionMetadata>();
        options.AddDataObjectConverter<IApplicationRoleConnection, ApplicationRoleConnection>();

        return options;
    }
}
