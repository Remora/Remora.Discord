//
//  ActivitySecrets.cs
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

using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Objects
{
    /// <summary>
    /// Represents a set of secrets used for interacting with the activity.
    /// </summary>
    public sealed class ActivitySecrets
    {
        /// <summary>
        /// Gets the secret used for joining the party.
        /// </summary>
        public Optional<string> Join { get; }

        /// <summary>
        /// Gets the secret used for spectating the party.
        /// </summary>
        public Optional<string> Spectate { get; }

        /// <summary>
        /// Gets the secret used for joining a specific instanced match.
        /// </summary>
        public Optional<string> Match { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitySecrets"/> class.
        /// </summary>
        /// <param name="join">The joining secret.</param>
        /// <param name="spectate">The spectator secret.</param>
        /// <param name="match">The match secret.</param>
        public ActivitySecrets
        (
            Optional<string> join = default,
            Optional<string> spectate = default,
            Optional<string> match = default
        )
        {
            this.Join = @join;
            this.Spectate = spectate;
            this.Match = match;
        }
    }
}
