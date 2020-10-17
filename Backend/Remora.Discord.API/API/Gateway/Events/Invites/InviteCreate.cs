//
//  InviteCreate.cs
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
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Gateway.Events
{
    /// <inheritdoc />
    public class InviteCreate : IInviteCreate
    {
        /// <inheritdoc />
        public Snowflake ChannelID { get; }

        /// <inheritdoc/>
        public string Code { get; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt { get; }

        /// <inheritdoc/>
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc/>
        public Optional<IUser> Inviter { get; }

        /// <inheritdoc/>
        public int MaxAge { get; }

        /// <inheritdoc/>
        public int MaxUses { get; }

        /// <inheritdoc/>
        public Optional<IPartialUser> TargetUser { get; }

        /// <inheritdoc/>
        public Optional<TargetUserType> TargetUserType { get; }

        /// <inheritdoc/>
        public bool IsTemporary { get; }

        /// <inheritdoc/>
        public int Uses { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InviteCreate"/> class.
        /// </summary>
        /// <param name="channelID">The ID of the channel the invite is for.</param>
        /// <param name="code">The unique invite code.</param>
        /// <param name="createdAt">When the invite was created.</param>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="inviter">The user that created the invite.</param>
        /// <param name="maxAge">The duration the invite is valid for (in seconds).</param>
        /// <param name="maxUses">The maximum number of uses.</param>
        /// <param name="targetUser">The target user for this invite.</param>
        /// <param name="targetUserType">The target user type.</param>
        /// <param name="isTemporary">Whether the invite is temporary.</param>
        /// <param name="uses">The number of times the invite has been used.</param>
        public InviteCreate
        (
            Snowflake channelID,
            string code,
            DateTimeOffset createdAt,
            Optional<Snowflake> guildID,
            Optional<IUser> inviter,
            int maxAge,
            int maxUses,
            Optional<IPartialUser> targetUser,
            Optional<TargetUserType> targetUserType,
            bool isTemporary,
            int uses
        )
        {
            this.ChannelID = channelID;
            this.Code = code;
            this.CreatedAt = createdAt;
            this.GuildID = guildID;
            this.Inviter = inviter;
            this.MaxAge = maxAge;
            this.MaxUses = maxUses;
            this.TargetUser = targetUser;
            this.TargetUserType = targetUserType;
            this.IsTemporary = isTemporary;
            this.Uses = uses;
        }
    }
}
