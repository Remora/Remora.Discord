//
//  GuildScheduledEventStatus.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various states a scheduled event can be in.
/// </summary>
[PublicAPI]
public enum GuildScheduledEventStatus
{
    /// <summary>
    /// The event is scheduled, but hasn't begun yet.
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// The event is currently ongoing.
    /// </summary>
    Active = 2,

    /// <summary>
    /// The event has completed.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// The event has been canceled.
    /// </summary>
    Canceled = 4
}
