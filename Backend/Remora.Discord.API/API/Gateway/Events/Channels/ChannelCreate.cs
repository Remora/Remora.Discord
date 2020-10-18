//
//  ChannelCreate.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events.Channels
{
    /// <inheritdoc cref="Remora.Discord.API.Abstractions.Gateway.Events.IChannelCreate" />
    [PublicAPI]
    public class ChannelCreate : Channel, IChannelCreate
    {
        /// <inheritdoc cref="Channel"/>
        public ChannelCreate
        (
            Snowflake id,
            ChannelType type,
            Optional<Snowflake> guildID,
            Optional<int> position,
            Optional<IReadOnlyList<IPermissionOverwrite>> permissionOverwrites,
            Optional<string> name,
            Optional<string?> topic,
            Optional<bool> isNsfw,
            Optional<Snowflake?> lastMessageID,
            Optional<int> bitrate,
            Optional<int> userLimit,
            Optional<int> rateLimitPerUser,
            Optional<IReadOnlyList<IUser>> recipients,
            Optional<IImageHash?> icon,
            Optional<Snowflake> ownerID,
            Optional<Snowflake> applicationID,
            Optional<Snowflake?> parentID,
            Optional<DateTimeOffset?> lastPinTimestamp
        )
            : base
            (
                id,
                type,
                guildID,
                position,
                permissionOverwrites,
                name,
                topic,
                isNsfw,
                lastMessageID,
                bitrate,
                userLimit,
                rateLimitPerUser,
                recipients,
                icon,
                ownerID,
                applicationID,
                parentID,
                lastPinTimestamp
            )
        {
        }
    }
}
