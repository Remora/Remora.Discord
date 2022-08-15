//
//  MessageActivityType.cs
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
/// Enumerates various activity types.
/// </summary>
[PublicAPI]
public enum MessageActivityType
{
    /// <summary>
    /// An invitation to join the activity.
    /// </summary>
    Join = 1,

    /// <summary>
    /// An invitation to spectate the activity.
    /// </summary>
    Spectate = 2,

    /// <summary>
    /// An invitation to listen to the activity.
    /// </summary>
    Listen = 3,

    /// <summary>
    /// A request to join the activity.
    /// </summary>
    JoinRequest = 4
}
