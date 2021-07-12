//
//  DiscordDefaultPermissionAttribute.cs
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

namespace Remora.Discord.Commands.Attributes
{
    /// <summary>
    /// Marks a group with a default permission, that is, whether commands in the group will be visible to all users by
    /// default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DiscordDefaultPermissionAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether commands in the group will be visible to all users by default.
        /// </summary>
        public bool DefaultPermission { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordDefaultPermissionAttribute"/> class.
        /// </summary>
        /// <param name="defaultPermission">
        /// true if commands in the group should be visible to all users by default; otherwise, false.
        /// </param>
        public DiscordDefaultPermissionAttribute(bool defaultPermission)
        {
            this.DefaultPermission = defaultPermission;
        }
    }
}
