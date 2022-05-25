//
//  IChannel.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a channel.
/// </summary>
[PublicAPI]
public interface IChannel : IPartialChannel
{
    /// <summary>
    /// Gets the ID of the channel.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the type of the channel.
    /// </summary>
    new ChannelType Type { get; }

    /// <summary>
    /// Gets the ID of the guild the channel is in.
    /// </summary>
    new Optional<Snowflake> GuildID { get; }

    /// <summary>
    /// Gets the sorting position of the channel.
    /// </summary>
    new Optional<int> Position { get; }

    /// <summary>
    /// Gets a list of explicit permission overwrites for members and roles.
    /// </summary>
    new Optional<IReadOnlyList<IPermissionOverwrite>> PermissionOverwrites { get; }

    /// <summary>
    /// Gets the name of the channel.
    /// </summary>
    new Optional<string?> Name { get; }

    /// <summary>
    /// Gets the topic of the channel.
    /// </summary>
    new Optional<string?> Topic { get; }

    /// <summary>
    /// Gets a value indicating whether the channel is NSFW.
    /// </summary>
    new Optional<bool> IsNsfw { get; }

    /// <summary>
    /// Gets the ID of the last message sent in the channel.
    /// </summary>
    new Optional<Snowflake?> LastMessageID { get; }

    /// <summary>
    /// Gets the bitrate (in bits) of the channel. Minimum 8000.
    /// </summary>
    new Optional<int> Bitrate { get; }

    /// <summary>
    /// Gets the user limit of the voice channel.
    /// </summary>
    new Optional<int> UserLimit { get; }

    /// <summary>
    /// Gets the number of seconds a user has to wait before sending another message (0-21600); bots, as well as
    /// users with the permission <see cref="DiscordPermission.ManageMessages"/> or
    /// <see cref="DiscordPermission.ManageChannels"/> are unaffected. This is colloquially known as "slow mode".
    /// </summary>
    new Optional<TimeSpan> RateLimitPerUser { get; }

    /// <summary>
    /// Gets the recipients of the DM.
    /// </summary>
    new Optional<IReadOnlyList<IUser>> Recipients { get; }

    /// <summary>
    /// Gets the icon of the group DM.
    /// </summary>
    new Optional<IImageHash?> Icon { get; }

    /// <summary>
    /// Gets the ID of the DM creator.
    /// </summary>
    new Optional<Snowflake> OwnerID { get; }

    /// <summary>
    /// Gets the application ID of the group DM creator, if it is bot-created.
    /// </summary>
    new Optional<Snowflake> ApplicationID { get; }

    /// <summary>
    /// Gets the ID of the parent category for a channel. Each category can contain up to 50 channels.
    /// </summary>
    new Optional<Snowflake?> ParentID { get; }

    /// <summary>
    /// Gets the time when the last pinned message was pinned.
    /// </summary>
    new Optional<DateTimeOffset?> LastPinTimestamp { get; }

    /// <summary>
    /// Gets the ID of the voice channel region.
    /// </summary>
    new Optional<string?> RTCRegion { get; }

    /// <summary>
    /// Gets the video quality mode of the channel.
    /// </summary>
    new Optional<VideoQualityMode> VideoQualityMode { get; }

    /// <summary>
    /// Gets an approximate count of the messages in the channel. Stops counting at 50.
    /// </summary>
    new Optional<int> MessageCount { get; }

    /// <summary>
    /// Gets an approximate count of the messages in the channel. Stops counting at 50.
    /// </summary>
    new Optional<int> MemberCount { get; }

    /// <summary>
    /// Gets a set of thread-specific fields.
    /// </summary>
    new Optional<IThreadMetadata> ThreadMetadata { get; }

    /// <summary>
    /// Gets the thread member object for the current user, if they have joined the thread.
    /// </summary>
    new Optional<IThreadMember> Member { get; }

    /// <summary>
    /// Gets the default duration for newly created threads in this channel.
    /// </summary>
    new Optional<AutoArchiveDuration> DefaultAutoArchiveDuration { get; }

    /// <summary>
    /// Gets the computed permission set for the invoking user in the channel. Typically present when the channel is
    /// resolved via a slash command interaction.
    /// </summary>
    new Optional<IDiscordPermissionSet> Permissions { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialChannel.ID => this.ID;

    /// <inheritdoc/>
    Optional<ChannelType> IPartialChannel.Type => this.Type;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialChannel.GuildID => this.GuildID;

    /// <inheritdoc/>
    Optional<int> IPartialChannel.Position => this.Position;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IPermissionOverwrite>> IPartialChannel.PermissionOverwrites => this.PermissionOverwrites;

    /// <inheritdoc/>
    Optional<string?> IPartialChannel.Name => this.Name;

    /// <inheritdoc/>
    Optional<string?> IPartialChannel.Topic => this.Topic;

    /// <inheritdoc/>
    Optional<bool> IPartialChannel.IsNsfw => this.IsNsfw;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialChannel.LastMessageID => this.LastMessageID;

    /// <inheritdoc/>
    Optional<int> IPartialChannel.Bitrate => this.Bitrate;

    /// <inheritdoc/>
    Optional<int> IPartialChannel.UserLimit => this.UserLimit;

    /// <inheritdoc/>
    Optional<TimeSpan> IPartialChannel.RateLimitPerUser => this.RateLimitPerUser;

    /// <inheritdoc/>
    Optional<IReadOnlyList<IUser>> IPartialChannel.Recipients => this.Recipients;

    /// <inheritdoc/>
    Optional<IImageHash?> IPartialChannel.Icon => this.Icon;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialChannel.OwnerID => this.OwnerID;

    /// <inheritdoc/>
    Optional<Snowflake> IPartialChannel.ApplicationID => this.ApplicationID;

    /// <inheritdoc/>
    Optional<Snowflake?> IPartialChannel.ParentID => this.ParentID;

    /// <inheritdoc/>
    Optional<DateTimeOffset?> IPartialChannel.LastPinTimestamp => this.LastPinTimestamp;

    /// <inheritdoc/>
    Optional<string?> IPartialChannel.RTCRegion => this.RTCRegion;

    /// <inheritdoc/>
    Optional<VideoQualityMode> IPartialChannel.VideoQualityMode => this.VideoQualityMode;

    /// <inheritdoc/>
    Optional<int> IPartialChannel.MessageCount => this.MessageCount;

    /// <inheritdoc/>
    Optional<int> IPartialChannel.MemberCount => this.MemberCount;

    /// <inheritdoc/>
    Optional<IThreadMetadata> IPartialChannel.ThreadMetadata => this.ThreadMetadata;

    /// <inheritdoc/>
    Optional<IThreadMember> IPartialChannel.Member => this.Member;

    /// <inheritdoc/>
    Optional<AutoArchiveDuration> IPartialChannel.DefaultAutoArchiveDuration => this.DefaultAutoArchiveDuration;

    /// <inheritdoc/>
    Optional<IDiscordPermissionSet> IPartialChannel.Permissions => this.Permissions;
}
