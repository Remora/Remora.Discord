//
//  UnavailableGuild.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class UnavailableGuild : IUnavailableGuild
    {
        /// <inheritdoc />
        public Snowflake GuildID { get; }

        /// <inheritdoc />
        public Optional<bool> IsUnavailable { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnavailableGuild"/> class.
        /// </summary>
        /// <param name="guildID">The ID of the guild.</param>
        /// <param name="isUnavailable">Whether the guild is unavailable.</param>
        public UnavailableGuild(Snowflake guildID, Optional<bool> isUnavailable)
        {
            this.GuildID = guildID;
            this.IsUnavailable = isUnavailable;
        }
    }
}
