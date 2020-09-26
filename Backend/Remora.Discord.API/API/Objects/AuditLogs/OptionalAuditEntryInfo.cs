//
//  OptionalAuditEntryInfo.cs
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

using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class OptionalAuditEntryInfo : IOptionalAuditEntryInfo
    {
        /// <inheritdoc/>
        public Optional<string> DeleteMemberDays { get; }

        /// <inheritdoc/>
        public Optional<string> MembersRemoved { get; }

        /// <inheritdoc/>
        public Optional<Snowflake> ChannelID { get; }

        /// <inheritdoc/>
        public Optional<Snowflake> MessageID { get; }

        /// <inheritdoc/>
        public Optional<string> Count { get; }

        /// <inheritdoc/>
        public Optional<Snowflake> ID { get; }

        /// <inheritdoc/>
        public Optional<string> Type { get; }

        /// <inheritdoc/>
        public Optional<string> RoleName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalAuditEntryInfo"/> class.
        /// </summary>
        /// <param name="deleteMemberDays">The number of days after which inactive members were kicked.</param>
        /// <param name="membersRemoved">The number of members removed by the prune.</param>
        /// <param name="channelID">The channel in which the entities were targeted.</param>
        /// <param name="messageID">The ID of the message that was targeted.</param>
        /// <param name="count">The number of entities that were targeted.</param>
        /// <param name="id">The ID of the overwritten entity.</param>
        /// <param name="type">The type of the overwritten entity.</param>
        /// <param name="roleName">The name of the role, if the <paramref name="type"/> is "role".</param>
        public OptionalAuditEntryInfo
        (
            Optional<string> deleteMemberDays,
            Optional<string> membersRemoved,
            Optional<Snowflake> channelID,
            Optional<Snowflake> messageID,
            Optional<string> count,
            Optional<Snowflake> id,
            Optional<string> type,
            Optional<string> roleName
        )
        {
            this.DeleteMemberDays = deleteMemberDays;
            this.MembersRemoved = membersRemoved;
            this.ChannelID = channelID;
            this.MessageID = messageID;
            this.Count = count;
            this.ID = id;
            this.Type = type;
            this.RoleName = roleName;
        }
    }
}
