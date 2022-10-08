//
//  IPartialChannel.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a partial channel.
/// </summary>
[PublicAPI]
public interface IPartialChannel
{
    /// <inheritdoc cref="IChannel.ID" />
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IChannel.Type" />
    Optional<ChannelType> Type { get; }

    /// <inheritdoc cref="IChannel.GuildID" />
    Optional<Snowflake> GuildID { get; }

    /// <inheritdoc cref="IChannel.Position" />
    Optional<int> Position { get; }

    /// <inheritdoc cref="IChannel.PermissionOverwrites" />
    Optional<IReadOnlyList<IPermissionOverwrite>> PermissionOverwrites { get; }

    /// <inheritdoc cref="IChannel.Name" />
    Optional<string?> Name { get; }

    /// <inheritdoc cref="IChannel.Topic" />
    Optional<string?> Topic { get; }

    /// <inheritdoc cref="IChannel.IsNsfw" />
    Optional<bool> IsNsfw { get; }

    /// <inheritdoc cref="IChannel.LastMessageID" />
    Optional<Snowflake?> LastMessageID { get; }

    /// <inheritdoc cref="IChannel.Bitrate" />
    Optional<int> Bitrate { get; }

    /// <inheritdoc cref="IChannel.UserLimit" />
    Optional<int> UserLimit { get; }

    /// <inheritdoc cref="IChannel.RateLimitPerUser" />
    Optional<TimeSpan> RateLimitPerUser { get; }

    /// <inheritdoc cref="IChannel.Recipients" />
    Optional<IReadOnlyList<IUser>> Recipients { get; }

    /// <inheritdoc cref="IChannel.Icon" />
    Optional<IImageHash?> Icon { get; }

    /// <inheritdoc cref="IChannel.OwnerID" />
    Optional<Snowflake> OwnerID { get; }

    /// <inheritdoc cref="IChannel.ApplicationID" />
    Optional<Snowflake> ApplicationID { get; }

    /// <inheritdoc cref="IChannel.ParentID" />
    Optional<Snowflake?> ParentID { get; }

    /// <inheritdoc cref="IChannel.LastPinTimestamp" />
    Optional<DateTimeOffset?> LastPinTimestamp { get; }

    /// <inheritdoc cref="IChannel.RTCRegion" />
    Optional<string?> RTCRegion { get; }

    /// <inheritdoc cref="IChannel.VideoQualityMode" />
    Optional<VideoQualityMode> VideoQualityMode { get; }

    /// <inheritdoc cref="IChannel.MessageCount" />
    Optional<int> MessageCount { get; }

    /// <inheritdoc cref="IChannel.MemberCount" />
    Optional<int> MemberCount { get; }

    /// <inheritdoc cref="IChannel.ThreadMetadata" />
    Optional<IThreadMetadata> ThreadMetadata { get; }

    /// <inheritdoc cref="IChannel.Member" />
    Optional<IThreadMember> Member { get; }

    /// <inheritdoc cref="IChannel.DefaultAutoArchiveDuration" />
    Optional<AutoArchiveDuration> DefaultAutoArchiveDuration { get; }

    /// <inheritdoc cref="IChannel.Permissions" />
    Optional<IDiscordPermissionSet> Permissions { get; }

    /// <inheritdoc cref="IChannel.Flags" />
    Optional<ChannelFlags> Flags { get; }

    /// <inheritdoc cref="IChannel.TotalMessageSent" />
    Optional<int> TotalMessageSent { get; }
}
