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

using Newtonsoft.Json;
using Remora.Discord.API.Abstractions.Commands;

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents a resume command.
    /// </summary>
    public sealed class Resume : IResume
    {
        /// <inheritdoc />
        public string Token { get; }

        /// <inheritdoc />
        public string SessionID { get; }

        /// <inheritdoc />
        [JsonProperty("seq")]
        public int SequenceNumber { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resume"/> class.
        /// </summary>
        /// <param name="token">The session token.</param>
        /// <param name="sessionID">The ID of the session.</param>
        /// <param name="sequenceNumber">The last received sequence number.</param>
        public Resume(string token, string sessionID, [JsonProperty("seq")] int sequenceNumber)
        {
            this.Token = token;
            this.SessionID = sessionID;
            this.SequenceNumber = sequenceNumber;
        }
    }
}
