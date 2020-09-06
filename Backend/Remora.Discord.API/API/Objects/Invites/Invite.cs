//
//  Invite.cs
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

namespace Remora.Discord.API.Objects.Invites
{
    /// <inheritdoc />
    public class Invite : IInvite
    {
        /// <inheritdoc />
        public string Code { get; }

        /// <inheritdoc />
        public Optional<IGuild> Guild { get; }

        /// <inheritdoc />
        public IChannel Channel { get; }

        /// <inheritdoc />
        public Optional<IUser> Inviter { get; }

        /// <inheritdoc />
        public Optional<IUser> TargetUser { get; }

        /// <inheritdoc />
        public Optional<TargetUserType> TargetUserType { get; }

        /// <inheritdoc />
        public Optional<int> ApproximatePresenceCount { get; }

        /// <inheritdoc />
        public Optional<int> ApproximateMemberCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Invite"/> class.
        /// </summary>
        /// <param name="code">The invite code.</param>
        /// <param name="guild">The guild the invite is for.</param>
        /// <param name="channel">The channel the invite is for.</param>
        /// <param name="inviter">The user who created the invite.</param>
        /// <param name="targetUser">The target user for this invite.</param>
        /// <param name="targetUserType">The type of user target for this invite.</param>
        /// <param name="approximatePresenceCount">The approximate count of online members.</param>
        /// <param name="approximateMemberCount">The approximate total member count.</param>
        public Invite
        (
            string code,
            Optional<IGuild> guild,
            IChannel channel,
            Optional<IUser> inviter,
            Optional<IUser> targetUser,
            Optional<TargetUserType> targetUserType,
            Optional<int> approximatePresenceCount,
            Optional<int> approximateMemberCount
        )
        {
            this.Code = code;
            this.Guild = guild;
            this.Channel = channel;
            this.Inviter = inviter;
            this.TargetUser = targetUser;
            this.TargetUserType = targetUserType;
            this.ApproximatePresenceCount = approximatePresenceCount;
            this.ApproximateMemberCount = approximateMemberCount;
        }
    }
}
