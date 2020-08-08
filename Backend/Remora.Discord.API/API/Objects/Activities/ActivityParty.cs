//
//  ActivityParty.cs
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
    /// <summary>
    /// Represents a party of people in an activity.
    /// </summary>
    public class ActivityParty : IActivityParty
    {
        /// <inheritdoc />
        public Optional<string> ID { get; }

        /// <inheritdoc />
        public Optional<IPartySize> Size { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityParty"/> class.
        /// </summary>
        /// <param name="id">The ID of the party.</param>
        /// <param name="size">The size of the party.</param>
        public ActivityParty(Optional<string> id = default, Optional<IPartySize> size = default)
        {
            this.ID = id;
            this.Size = size;
        }
    }
}
