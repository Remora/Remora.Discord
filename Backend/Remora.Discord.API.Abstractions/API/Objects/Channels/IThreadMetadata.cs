//
//  IThreadMetadata.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents metadata information for a thread channel.
/// </summary>
[PublicAPI]
public interface IThreadMetadata
{
    /// <summary>
    /// Gets a value indicating whether the thread has been archived.
    /// </summary>
    bool IsArchived { get; }

    /// <summary>
    /// Gets a time of inactivity after which the thread is automatically archived.
    /// </summary>
    AutoArchiveDuration AutoArchiveDuration { get; }

    /// <summary>
    /// Gets a timestamp when the thread's archival status was last changed.
    /// </summary>
    DateTimeOffset ArchiveTimestamp { get; }

    /// <summary>
    /// Gets a value indicating whether the thread has been locked.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Gets a value indicating whether non-moderators can add other non-moderators.
    /// </summary>
    Optional<bool> IsInvitable { get; }

    /// <summary>
    /// Gets a value indicating when the thread was created. Only populated for threads created after 2022-01-09 (YYYY-MM-DD).
    /// </summary>
    Optional<DateTimeOffset?> CreateTimestamp { get; }
}
