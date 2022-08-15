//
//  IOptionalAuditEntryInfo.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents optional audit log information.
/// </summary>
[PublicAPI]
public interface IOptionalAuditEntryInfo
{
    /// <summary>
    /// Gets the application for which entities were targeted.
    /// </summary>
    /// <remarks>
    /// Relevant for <see cref="AuditLogEvent.ApplicationCommandPermissionUpdate"/>.
    /// </remarks>
    Optional<Snowflake> ApplicationID { get; }

    /// <summary>
    /// Gets the channel in which the entities were targeted.
    /// <remarks>
    /// Relevant for <see cref="AuditLogEvent.MemberMove"/>, <see cref="AuditLogEvent.MessagePin"/>,
    /// <see cref="AuditLogEvent.MessageUnpin"/>, <see cref="AuditLogEvent.MessageDelete"/>,
    /// <see cref="AuditLogEvent.StageInstanceCreate"/>, <see cref="AuditLogEvent.StageInstanceUpdate"/>, and
    /// <see cref="AuditLogEvent.StageInstanceDelete"/>.</remarks>
    /// </summary>
    Optional<Snowflake> ChannelID { get; }

    /// <summary>
    /// Gets the number of entities that were targeted.
    /// <remarks>
    /// Relevant for <see cref="AuditLogEvent.MessageDelete"/>, <see cref="AuditLogEvent.MessageBulkDelete"/>,
    /// <see cref="AuditLogEvent.MemberDisconnect"/>, and <see cref="AuditLogEvent.MemberMove"/>.
    /// </remarks>
    /// </summary>
    Optional<string> Count { get; }

    /// <summary>
    /// Gets the number of days after which inactive members were kicked.
    /// <remarks>Relevant for <see cref="AuditLogEvent.MemberPrune"/>.</remarks>
    /// </summary>
    Optional<string> DeleteMemberDays { get; }

    /// <summary>
    /// Gets the ID of the overwritten entity.
    /// <remarks>
    /// Relevant for <see cref="AuditLogEvent.ChannelOverwriteCreate"/>,
    /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>, and <see cref="AuditLogEvent.ChannelOverwriteDelete"/>.
    /// </remarks>
    /// </summary>
    Optional<Snowflake> ID { get; }

    /// <summary>
    /// Gets the number of members removed by the prune.
    /// <remarks>Relevant for <see cref="AuditLogEvent.MemberPrune"/>.</remarks>
    /// </summary>
    Optional<string> MembersRemoved { get; }

    /// <summary>
    /// Gets the ID of the message that was targeted.
    /// <remarks>
    /// Relevant for <see cref="AuditLogEvent.MessagePin"/> and <see cref="AuditLogEvent.MessageUnpin"/>.
    /// </remarks>
    /// </summary>
    Optional<Snowflake> MessageID { get; }

    /// <summary>
    /// Gets the name of the overwritten role, if <see cref="Type"/> is "role".
    /// <remarks>
    /// Relevant for <see cref="AuditLogEvent.ChannelOverwriteCreate"/>,
    /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>, and <see cref="AuditLogEvent.ChannelOverwriteDelete"/>.
    /// </remarks>
    /// </summary>
    Optional<string> RoleName { get; }

    /// <summary>
    /// Gets the type of the overwritten entity. This can be either "0" for roles, or "1" for members.
    /// <remarks>
    /// Relevant for <see cref="AuditLogEvent.ChannelOverwriteCreate"/>,
    /// <see cref="AuditLogEvent.ChannelOverwriteUpdate"/>, and <see cref="AuditLogEvent.ChannelOverwriteDelete"/>.
    /// </remarks>
    /// </summary>
    Optional<PermissionOverwriteType> Type { get; }
}
