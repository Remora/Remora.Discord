//
//  IOptionalAuditLogInfo.cs
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

using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.Rest.Abstractions
{
    /// <summary>
    /// Represents optional audit log information.
    /// </summary>
    [PublicAPI]
    public interface IOptionalAuditLogInfo
    {
        /// <summary>
        /// Gets the number of days after which inactive members were kicked.
        /// <remarks>Relevant for <see cref="AuditLogEvent.MemberPrune"/>.</remarks>
        /// </summary>
        string DeleteMemberDays { get; }

        /// <summary>
        /// Gets the number of members removed by the prune.
        /// <remarks>Relevant for <see cref="AuditLogEvent.MemberPrune"/>.</remarks>
        /// </summary>
        string MembersRemoved { get; }

        /// <summary>
        /// Gets the channel in which the entities were targeted.
        /// <remarks>
        /// Relevant for <see cref="AuditLogEvent.MemberMove"/>, <see cref="AuditLogEvent.MessagePin"/>,
        /// <see cref="AuditLogEvent.MessageUnpin"/>, and <see cref="AuditLogEvent.MessageDelete"/>.</remarks>
        /// </summary>
        Snowflake ChannelID { get; }

        /// <summary>
        /// Gets the ID of the message that was targeted.
        /// <remarks>
        /// Relevant for <see cref="AuditLogEvent.MessagePin"/> and <see cref="AuditLogEvent.MessageUnpin"/>.
        /// </remarks>
        /// </summary>
        Snowflake MessageID { get; }

        /// <summary>
        /// Gets the number of entities that were targeted.
        /// <remarks>
        /// Relevant for <see cref="AuditLogEvent.MessageDelete"/>, <see cref="AuditLogEvent.MessageBulkDelete"/>,
        /// <see cref="AuditLogEvent.MemberDisconnect"/>, and <see cref="AuditLogEvent.MemberMove"/>.
        /// </remarks>
        /// </summary>
        string Count { get; }

        /// <summary>
        /// Gets the ID of the overwritten entity.
        /// <remarks>
        /// Relevant for <see cref="AuditLogEvent.ChannelOverwriteCreate"/>,
        /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>, and <see cref="AuditLogEvent.ChannelOverwriteDelete"/>.
        /// </remarks>
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the type of the overwritten entity. This can be either "member" or "role".
        /// <remarks>
        /// Relevant for <see cref="AuditLogEvent.ChannelOverwriteCreate"/>,
        /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>, and <see cref="AuditLogEvent.ChannelOverwriteDelete"/>.
        /// </remarks>
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the name of the overwritten role, if <see cref="Type"/> is "role".
        /// <remarks>
        /// Relevant for <see cref="AuditLogEvent.ChannelOverwriteCreate"/>,
        /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>, and <see cref="AuditLogEvent.ChannelOverwriteDelete"/>.
        /// </remarks>
        /// </summary>
        string RoleName { get; }
    }
}
