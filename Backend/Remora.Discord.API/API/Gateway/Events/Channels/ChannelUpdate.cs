//
//  ChannelUpdate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Gateway.Events.Channels;

/// <inheritdoc cref="Remora.Discord.API.Abstractions.Gateway.Events.IChannelUpdate" />
[PublicAPI]
public record ChannelUpdate
(
    Snowflake ID,
    ChannelType Type,
    Optional<Snowflake> GuildID = default,
    Optional<int> Position = default,
    Optional<IReadOnlyList<IPermissionOverwrite>> PermissionOverwrites = default,
    Optional<string?> Name = default,
    Optional<string?> Topic = default,
    Optional<bool> IsNsfw = default,
    Optional<Snowflake?> LastMessageID = default,
    Optional<int> Bitrate = default,
    Optional<int> UserLimit = default,
    Optional<TimeSpan> RateLimitPerUser = default,
    Optional<IReadOnlyList<IUser>> Recipients = default,
    Optional<IImageHash?> Icon = default,
    Optional<Snowflake> OwnerID = default,
    Optional<Snowflake> ApplicationID = default,
    Optional<Snowflake?> ParentID = default,
    Optional<DateTimeOffset?> LastPinTimestamp = default,
    Optional<string?> RTCRegion = default,
    Optional<VideoQualityMode> VideoQualityMode = default,
    Optional<int> MessageCount = default,
    Optional<int> MemberCount = default,
    Optional<IThreadMetadata> ThreadMetadata = default,
    Optional<IThreadMember> Member = default,
    Optional<AutoArchiveDuration> DefaultAutoArchiveDuration = default,
    Optional<IDiscordPermissionSet> Permissions = default
) : Channel
(
    ID,
    Type,
    GuildID,
    Position,
    PermissionOverwrites,
    Name,
    Topic,
    IsNsfw,
    LastMessageID,
    Bitrate,
    UserLimit,
    RateLimitPerUser,
    Recipients,
    Icon,
    OwnerID,
    ApplicationID,
    ParentID,
    LastPinTimestamp,
    RTCRegion,
    VideoQualityMode,
    MessageCount,
    MemberCount,
    ThreadMetadata,
    Member,
    DefaultAutoArchiveDuration,
    Permissions
), IChannelUpdate;
