//
//  IMessageApplication.cs
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

namespace Remora.Discord.API.Abstractions
{
    /// <summary>
    /// Represents an application linked to a message.
    /// </summary>
    public interface IMessageApplication
    {
        /// <summary>
        /// Gets the ID of the application.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the cover image of the application.
        /// </summary>
        Optional<IImageHash> CoverImage { get; }

        /// <summary>
        /// Gets the description of the application.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the application's icon.
        /// </summary>
        IImageHash? Icon { get; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        string Name { get; }
    }
}
