//
//  IThreadMetadata.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents metadata information for a thread channel.
    /// </summary>
    public interface IThreadMetadata
    {
        /// <summary>
        /// Gets a value indicating whether the thread has been archived.
        /// </summary>
        bool IsArchived { get; }

        /// <summary>
        /// Gets a duration in minutes of inactivity after which the thread is automatically archived. Accepted values
        /// are, at present, 60, 1440, 4320, and 10080.
        /// </summary>
        TimeSpan AutoArchiveDuration { get; }

        /// <summary>
        /// Gets a timestamp when the thread's archival status was last changed.
        /// </summary>
        DateTimeOffset ArchiveTimestamp { get; }

        /// <summary>
        /// Gets a value indicating whether the thread has been locked.
        /// </summary>
        Optional<bool> IsLocked { get; }
    }
}
