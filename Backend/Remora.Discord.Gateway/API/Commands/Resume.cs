//
//  Resume.cs
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

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents a resume command.
    /// </summary>
    internal sealed class Resume
    {
        /// <summary>
        /// Gets the session token.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the last session ID.
        /// </summary>
        public string SessionID { get; }

        /// <summary>
        /// Gets the last received sequence number.
        /// </summary>
        public int Seq { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resume"/> class.
        /// </summary>
        /// <param name="token">The session token.</param>
        /// <param name="sessionID">The ID of the session.</param>
        /// <param name="seq">The last received sequence number.</param>
        public Resume(string token, string sessionID, int seq)
        {
            this.Token = token;
            this.SessionID = sessionID;
            this.Seq = seq;
        }
    }
}
