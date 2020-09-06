//
//  Ban.cs
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

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class Ban : IBan
    {
        /// <inheritdoc />
        public string? Reason { get; }

        /// <inheritdoc/>
        public IUser User { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ban"/> class.
        /// </summary>
        /// <param name="reason">The reason for the ban.</param>
        /// <param name="user">The banned user.</param>
        public Ban(string? reason, IUser user)
        {
            this.Reason = reason;
            this.User = user;
        }
    }
}
