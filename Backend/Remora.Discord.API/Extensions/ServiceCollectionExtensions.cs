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
using Remora.Discord.API.Abstractions.Activities;
using Remora.Discord.API.Abstractions.Channels;
using Remora.Discord.API.Abstractions.Commands;
using Remora.Discord.API.Abstractions.Emojis;
using Remora.Discord.API.Abstractions.Events;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Guilds;
using Remora.Discord.API.Abstractions.Messages;
using Remora.Discord.API.Abstractions.Permissions;
using Remora.Discord.API.Abstractions.Presence;
using Remora.Discord.API.Abstractions.Reactions;
using Remora.Discord.API.Abstractions.Users;
using Remora.Discord.API.Abstractions.Voice;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Gateway.Events.ConnectingResuming;
using Remora.Discord.API.Gateway.Events.Guilds;
using Remora.Discord.API.Gateway.Events.Messages;
using Remora.Discord.API.Json;
using Remora.Discord.API.Objects.Activities;
using Remora.Discord.API.Objects.Channels;
using Remora.Discord.API.Objects.Emojis;
using Remora.Discord.API.Objects.Gateway;
using Remora.Discord.API.Objects.Guilds;
using Remora.Discord.API.Objects.Messages;
using Remora.Discord.API.Objects.Permissions;
using Remora.Discord.API.Objects.Reactions;
using Remora.Discord.API.Objects.Users;
using Remora.Discord.API.Objects.Voice;

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
        /// <returns>The service collection, with the services.</returns>
        public static IServiceCollection AddDiscordApi(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .Configure<JsonSerializerOptions>
                (
                    options =>
                    {
                        options
                            .AddConverter<OptionalConverterFactory>()
                            .AddConverter<NullableConverterFactory>()
                            .AddConverter<PartySizeConverter>()
                            .AddConverter<HeartbeatConverter>()
                            .AddConverter<ShardIdentificationConverter>()
                            .AddConverter<SnowflakeConverter>()
                            .AddConverter<PayloadConverter>()
                            .AddConverter<HeartbeatConverter>()
                            .AddConverter<ImageHashConverter>()
                            .AddConverter<ColorConverter>();

                        options.AddDataObjectConverter<IActivity, Activity>();
                        options.AddDataObjectConverter<IHello, Hello>();
                        options.AddDataObjectConverter<IGatewayEndpoint, GatewayEndpoint>();
                        options.AddDataObjectConverter<IRequestGuildMembers, RequestGuildMembers>();
                        options.AddDataObjectConverter<IEmoji, Emoji>();
                        options.AddDataObjectConverter<IChannelMention, ChannelMention>();
                        options.AddDataObjectConverter<IAttachment, Attachment>();

                        options.AddDataObjectConverter<IPermissionOverwrite, PermissionOverwrite>()
                            .WithPropertyConverter
                            (
                                p => p.Type,
                                new JsonStringEnumConverter(new SnakeCaseNamingPolicy())
                            );

                        options.AddDataObjectConverter<IActivityTimestamps, ActivityTimestamps>()
                            .WithPropertyConverter(t => t.Start, new UnixDateTimeConverter())
                            .WithPropertyConverter(t => t.End, new UnixDateTimeConverter());

                        options.AddDataObjectConverter<ISessionStartLimit, SessionStartLimit>()
                            .WithPropertyConverter(st => st.ResetAfter, new MillisecondTimeSpanConverter());

                        options.AddDataObjectConverter<IIdentify, Identify>()
                            .WithPropertyName(i => i.DispatchGuildSubscriptions, "guild_subscriptions");

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

                        options.AddDataObjectConverter<IConnectionProperties, ConnectionProperties>()
                            .WithPropertyName(p => p.OperatingSystem, "$os")
                            .WithPropertyName(p => p.Browser, "$browser")
                            .WithPropertyName(p => p.Device, "$device");

                        options.AddDataObjectConverter<IPresence, Presence>()
                            .WithPropertyConverter(p => p.Status, new JsonStringEnumConverter());

                        options.AddDataObjectConverter<IReady, Ready>()
                            .WithPropertyName(r => r.Version, "v");

                        options.AddDataObjectConverter<IUser, User>()
                            .WithPropertyName(u => u.IsBot, "bot")
                            .WithPropertyName(u => u.IsSystem, "system")
                            .WithPropertyName(u => u.IsVerified, "verified")
                            .WithPropertyName(u => u.IsMFAEnabled, "mfa_enabled");

                        options.AddDataObjectConverter<IUnavailableGuild, UnavailableGuild>()
                            .WithPropertyName(u => u.GuildID, "id")
                            .WithPropertyName(u => u.IsUnavailable, "unavailable");

                        options.AddDataObjectConverter<IGuild, Guild>()
                            .WithPropertyName(g => g.IsOwner, "owner")
                            .WithPropertyName(g => g.GuildFeatures, "features")
                            .WithPropertyName(g => g.IsLarge, "large")
                            .WithPropertyName(g => g.IsUnavailable, "unavailable")
                            .WithPropertyConverter(g => g.JoinedAt, new ISO8601DateTimeOffsetConverter());

                        options.AddDataObjectConverter<IGuildCreate, GuildCreate>()
                            .WithPropertyName(g => g.IsOwner, "owner")
                            .WithPropertyName(g => g.GuildFeatures, "features")
                            .WithPropertyName(g => g.IsLarge, "large")
                            .WithPropertyName(g => g.IsUnavailable, "unavailable")
                            .WithPropertyConverter(g => g.JoinedAt, new ISO8601DateTimeOffsetConverter());

                        options.AddDataObjectConverter<IRole, Role>()
                            .WithPropertyName(r => r.Colour, "color")
                            .WithPropertyName(r => r.IsHoisted, "hoist")
                            .WithPropertyName(r => r.IsManaged, "managed")
                            .WithPropertyName(r => r.IsMentionable, "mentionable");

                        options.AddDataObjectConverter<IVoiceState, VoiceState>()
                            .WithPropertyName(v => v.IsDeafened, "deaf")
                            .WithPropertyName(v => v.IsMuted, "mute")
                            .WithPropertyName(v => v.IsSelfDeafened, "self_deaf")
                            .WithPropertyName(v => v.IsSelfMuted, "self_mute")
                            .WithPropertyName(v => v.IsStreaming, "self_stream")
                            .WithPropertyName(v => v.IsVideoEnabled, "self_video")
                            .WithPropertyName(v => v.IsSuppressed, "suppress");

                        options.AddDataObjectConverter<IGuildMember, GuildMember>()
                            .WithPropertyName(m => m.Nickname, "nick")
                            .WithPropertyName(m => m.IsDeafened, "deaf")
                            .WithPropertyName(m => m.IsMuted, "mute");

                        options.AddDataObjectConverter<IChannel, Channel>()
                            .WithPropertyName(c => c.IsNsfw, "nsfw")
                            .WithPropertyConverter(c => c.LastPinTimestamp, new ISO8601DateTimeOffsetConverter());

                        options.AddDataObjectConverter<IPresence, Presence>()
                            .WithPropertyName(p => p.Nickname, "nick")
                            .WithPropertyConverter(p => p.PremiumSince, new ISO8601DateTimeOffsetConverter());

                        options.AddDataObjectConverter<IMessage, Message>()
                            .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
                            .WithPropertyName(m => m.MentionedRoles, "mention_roles")
                            .WithPropertyName(m => m.MentionedChannels, "mention_channels")
                            .WithPropertyName(m => m.IsTTS, "tts")
                            .WithPropertyName(m => m.IsPinned, "pinned");

                        options.AddDataObjectConverter<IEmbed, Embed>()
                            .WithPropertyConverter(e => e.Timestamp, new ISO8601DateTimeOffsetConverter())
                            .WithPropertyConverter(e => e.Colour, new ColorConverter())
                            .WithPropertyName(e => e.Colour, "color");

                        options.AddDataObjectConverter<IEmbedFooter, EmbedFooter>();
                        options.AddDataObjectConverter<IEmbedImage, EmbedImage>();
                        options.AddDataObjectConverter<IEmbedThumbnail, EmbedThumbnail>();
                        options.AddDataObjectConverter<IEmbedVideo, EmbedVideo>();
                        options.AddDataObjectConverter<IEmbedProvider, EmbedProvider>();
                        options.AddDataObjectConverter<IEmbedAuthor, EmbedAuthor>();
                        options.AddDataObjectConverter<IEmbedField, EmbedField>();
                        options.AddDataObjectConverter<IReaction, Reaction>();
                        options.AddDataObjectConverter<IMessageActivity, MessageActivity>();
                        options.AddDataObjectConverter<IMessageApplication, MessageApplication>();
                        options.AddDataObjectConverter<IMessageReference, MessageReference>();

                        options.AddDataObjectConverter<IMessageCreate, MessageCreate>()
                            .WithPropertyName(m => m.MentionsEveryone, "mention_everyone")
                            .WithPropertyName(m => m.MentionedRoles, "mention_roles")
                            .WithPropertyName(m => m.MentionedChannels, "mention_channels")
                            .WithPropertyName(m => m.IsTTS, "tts")
                            .WithPropertyName(m => m.IsPinned, "pinned");

                        var snakeCasePolicy = new SnakeCaseNamingPolicy();
                        options.PropertyNamingPolicy = snakeCasePolicy;
                        options.DictionaryKeyPolicy = snakeCasePolicy;
                    }
                );

            return serviceCollection;
        }
    }
}
