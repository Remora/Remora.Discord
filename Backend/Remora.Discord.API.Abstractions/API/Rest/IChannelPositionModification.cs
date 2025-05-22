//
//  IChannelPositionModification.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents a channel position modification parameter payload.
/// </summary>
[PublicAPI]
public interface IChannelPositionModification
{
    /// <summary>
    /// Gets the ID of the channel to modify.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the new position of the channel, or null if it is to be reset to its default value.
    /// </summary>
    Optional<int?> Position { get; }

    /// <summary>
    /// Gets a value indicating whether the channel's permissions should be synced with its parent, or null if defaults
    /// (to this parameter, not the permissions) should apply.
    /// </summary>
    Optional<bool?> LockPermissions { get; }

    /// <summary>
    /// Gets the new parent ID of the channel, or null if the channel should have no parent.
    /// </summary>
    Optional<Snowflake?> ParentID { get; }
}
