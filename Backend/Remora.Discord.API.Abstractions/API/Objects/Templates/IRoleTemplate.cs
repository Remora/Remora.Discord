//
//  IRoleTemplate.cs
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

using System.Drawing;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a Discord role.
    /// </summary>
    public interface IRoleTemplate
    {
        /// <summary>
        /// Gets the relative ID of the role.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the colour of the role.
        /// </summary>
        Color Colour { get; }

        /// <summary>
        /// Gets a value indicating whether the role is displayed separately in the sidebar.
        /// </summary>
        bool IsHoisted { get; }

        /// <summary>
        /// Gets a value indicating whether this role is mentionable.
        /// </summary>
        bool IsMentionable { get; }

        /// <summary>
        /// Gets the permission set for this role.
        /// </summary>
        IDiscordPermissionSet Permissions { get; }
    }
}
