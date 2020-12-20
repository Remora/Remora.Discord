//
//  IAttachment.cs
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

using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents an attachment in a message.
    /// </summary>
    [PublicAPI]
    public interface IAttachment
    {
        /// <summary>
        /// Gets the ID of the attachment.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the filename of the attachment.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the source URL of the file.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Gets the proxied URL of the file.
        /// </summary>
        string ProxyUrl { get; }

        /// <summary>
        /// Gets the height of the file (if image).
        /// </summary>
        int? Height { get; }

        /// <summary>
        /// Gets the width of the file (if image).
        /// </summary>
        int? Width { get; }
    }
}
