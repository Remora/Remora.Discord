//
//  VoiceState.cs
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

using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class VoiceState : IVoiceState
    {
        /// <inheritdoc />
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc />
        public Snowflake? ChannelID { get; }

        /// <inheritdoc />
        public Snowflake UserID { get; }

        /// <inheritdoc />
        public Optional<IGuildMember> Member { get; }

        /// <inheritdoc />
        public string SessionID { get; }

        /// <inheritdoc />
        public bool IsDeafened { get; }

        /// <inheritdoc />
        public bool IsMuted { get; }

        /// <inheritdoc />
        public bool IsSelfDeafened { get; }

        /// <inheritdoc />
        public bool IsSelfMuted { get; }

        /// <inheritdoc />
        public Optional<bool> IsStreaming { get; }

        /// <inheritdoc />
        public bool IsVideoEnabled { get; }

        /// <inheritdoc />
        public bool IsSuppressed { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceState"/> class.
        /// </summary>
        /// <param name="guildID">The ID of the guild that the voice state is in.</param>
        /// <param name="channelID">The ID of the voice channel.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="member">The guild member information about the user.</param>
        /// <param name="sessionID">The session ID.</param>
        /// <param name="isDeafened">Whether the user is deafened by the server.</param>
        /// <param name="isMuted">Whether the user is muted by the server.</param>
        /// <param name="isSelfDeafened">Whether the user has deafened themselves.</param>
        /// <param name="isSelfMuted">Whether the user has muted themselves.</param>
        /// <param name="isStreaming">Whether the user is streaming.</param>
        /// <param name="isVideoEnabled">Whether the user's camera is enabled.</param>
        /// <param name="isSuppressed">Whether the user is muted by the current user.</param>
        public VoiceState
        (
            Optional<Snowflake> guildID,
            Snowflake? channelID,
            Snowflake userID,
            Optional<IGuildMember> member,
            string sessionID,
            bool isDeafened,
            bool isMuted,
            bool isSelfDeafened,
            bool isSelfMuted,
            Optional<bool> isStreaming,
            bool isVideoEnabled,
            bool isSuppressed
        )
        {
            this.GuildID = guildID;
            this.ChannelID = channelID;
            this.UserID = userID;
            this.Member = member;
            this.SessionID = sessionID;
            this.IsDeafened = isDeafened;
            this.IsMuted = isMuted;
            this.IsSelfDeafened = isSelfDeafened;
            this.IsSelfMuted = isSelfMuted;
            this.IsStreaming = isStreaming;
            this.IsVideoEnabled = isVideoEnabled;
            this.IsSuppressed = isSuppressed;
        }
    }
}
