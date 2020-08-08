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
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Json;
using Remora.Discord.API.Objects;

namespace Remora.Discord.API.Extensions
{
    /// <summary>
    /// Defines various extension methods to the <see cref="IServiceCollection"/> class.
    /// </summary>
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
                            .AddChannelObjectConverters()
                            .AddEmojiObjectConverters()
                            .AddGatewayObjectConverters()
                            .AddGuildObjectConverters()
                            .AddImageObjectConverters()
                            .AddMessageObjectConverters()
                            .AddPermissionObjectConverters()
                            .AddPresenceObjectConverters()
                            .AddReactionObjectConverters()
                            .AddUserObjectConverters()
                            .AddVoiceObjectConverters();

                        options
                            .AddConverter<ISO8601DateTimeOffsetConverter>()
                            .AddConverter<OptionalConverterFactory>()
                            .AddConverter<NullableConverterFactory>()
                            .AddConverter<SnowflakeConverter>()
                            .AddConverter<ColorConverter>();

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
            options.AddDataObjectConverter<IIdentify, Identify>()
                .WithPropertyName(i => i.DispatchGuildSubscriptions, "guild_subscriptions");

            options.AddDataObjectConverter<IConnectionProperties, ConnectionProperties>()
                .WithPropertyName(p => p.OperatingSystem, "$os")
                .WithPropertyName(p => p.Browser, "$browser")
                .WithPropertyName(p => p.Device, "$device");

            options.AddConverter<ShardIdentificationConverter>();

            options.AddDataObjectConverter<IRequestGuildMembers, RequestGuildMembers>();
            options.AddDataObjectConverter<IResume, Resume>()
                .WithPropertyName(r => r.SequenceNumber, "seq");

            options.AddDataObjectConverter<IUpdateStatus, UpdateStatus>()
                .WithPropertyName(u => u.IsAFK, "afk")
                .WithPropertyConverter
                (
                    u => u.Status,
                    new JsonStringEnumConverter(new SnakeCaseNamingPolicy())
                )
                .WithPropertyConverter(u => u.Since, new UnixDateTimeConverter());

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
            options.AddDataObjectConverter<IHello, Hello>();
            options.AddDataObjectConverter<IInvalidSession, InvalidSession>();
            options.AddDataObjectConverter<IReady, Ready>()
                .WithPropertyName(r => r.Version, "v");
            options.AddDataObjectConverter<IReconnect, Reconnect>();

            // Guilds
            options.AddDataObjectConverter<IGuildCreate, GuildCreate>()
                .WithPropertyName(g => g.IsOwner, "owner")
                .WithPropertyName(g => g.GuildFeatures, "features")
                .WithPropertyName(g => g.IsLarge, "large")
                .WithPropertyName(g => g.IsUnavailable, "unavailable")
                .WithReadPropertyName(g => g.Permissions, "permissions_new", "permissions");

            options.AddDataObjectConverter<IGuildRoleCreate, GuildRoleCreate>();
            options.AddDataObjectConverter<IGuildRoleUpdate, GuildRoleUpdate>();
            options.AddDataObjectConverter<IGuildRoleDelete, GuildRoleDelete>();

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

            // Presences
            options.AddDataObjectConverter<IPresenceUpdate, PresenceUpdate>()
                .WithPropertyName(p => p.Nickname, "nick")
                .WithPropertyConverter(p => p.Status, new JsonStringEnumConverter());

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
            options.AddDataObjectConverter<IActivity, Activity>();
            options.AddDataObjectConverter<IActivityAssets, ActivityAssets>();
            options.AddDataObjectConverter<IActivityEmoji, ActivityEmoji>();
            options.AddDataObjectConverter<IActivityParty, ActivityParty>();
            options.AddConverter<PartySizeConverter>();
            options.AddDataObjectConverter<IActivitySecrets, ActivitySecrets>();
            options.AddDataObjectConverter<IActivityTimestamps, ActivityTimestamps>()
                .WithPropertyConverter(t => t.Start, new UnixDateTimeConverter())
                .WithPropertyConverter(t => t.End, new UnixDateTimeConverter());

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
                .WithPropertyName(c => c.IsNsfw, "nsfw");

            options.AddDataObjectConverter<IChannelMention, ChannelMention>();

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
                .WithPropertyConverter(st => st.ResetAfter, new MillisecondTimeSpanConverter());

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
                .WithPropertyName(g => g.IsLarge, "large")
                .WithPropertyName(g => g.IsUnavailable, "unavailable")
                .WithReadPropertyName(g => g.Permissions, "permissions_new", "permissions");

            options.AddDataObjectConverter<IGuildMember, GuildMember>()
                .WithPropertyName(m => m.Nickname, "nick")
                .WithPropertyName(m => m.IsDeafened, "deaf")
                .WithPropertyName(m => m.IsMuted, "mute");

            options.AddDataObjectConverter<IUnavailableGuild, UnavailableGuild>()
                .WithPropertyName(u => u.GuildID, "id")
                .WithPropertyName(u => u.IsUnavailable, "unavailable");

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
        /// Adds the JSON converters that handle message objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddMessageObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IAttachment, Attachment>();

            options.AddDataObjectConverter<IEmbed, Embed>()
                .WithPropertyConverter(e => e.Type, new JsonStringEnumConverter(new SnakeCaseNamingPolicy()))
                .WithPropertyConverter(e => e.Colour, new ColorConverter())
                .WithPropertyName(e => e.Colour, "color");

            options.AddDataObjectConverter<IEmbedAuthor, EmbedAuthor>();
            options.AddDataObjectConverter<IEmbedField, EmbedField>();
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

            options.AddDataObjectConverter<IMessageActivity, MessageActivity>();
            options.AddDataObjectConverter<IMessageApplication, MessageApplication>();
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

            options.AddDataObjectConverter<IPermissionOverwrite, PermissionOverwrite>()
                .WithPropertyConverter
                (
                    p => p.Type,
                    new JsonStringEnumConverter(new SnakeCaseNamingPolicy())
                )
                .WithReadPropertyName(g => g.Allow, "allow_new", "allow")
                .WithReadPropertyName(g => g.Deny, "deny_new", "deny");

            options.AddDataObjectConverter<IRole, Role>()
                .WithPropertyName(r => r.Colour, "color")
                .WithPropertyName(r => r.IsHoisted, "hoist")
                .WithPropertyName(r => r.IsManaged, "managed")
                .WithPropertyName(r => r.IsMentionable, "mentionable")
                .WithReadPropertyName(g => g.Permissions, "permissions_new", "permissions");

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle presence objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddPresenceObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IClientStatuses, ClientStatuses>();

            options.AddDataObjectConverter<IPresence, Presence>()
                .WithPropertyName(p => p.Nickname, "nick")
                .WithPropertyConverter(p => p.Status, new JsonStringEnumConverter());

            return options;
        }

        /// <summary>
        /// Adds the JSON converters that handle reaction objects.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        /// <returns>The options, with the converters added.</returns>
        private static JsonSerializerOptions AddReactionObjectConverters(this JsonSerializerOptions options)
        {
            options.AddDataObjectConverter<IReaction, Reaction>();

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
                .WithPropertyName(u => u.IsBot, "bot")
                .WithPropertyName(u => u.IsSystem, "system")
                .WithPropertyName(u => u.IsVerified, "verified")
                .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled");

            options.AddDataObjectConverter<IUserMention, UserMention>();

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

            return options;
        }
    }
}
