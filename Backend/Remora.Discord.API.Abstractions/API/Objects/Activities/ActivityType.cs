//
//  ActivityType.cs
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
/// Enumerates the activity types supported by Discord.
/// </summary>
[PublicAPI]
public enum ActivityType
{
    /// <summary>
    /// The user is playing a game.
    /// </summary>
    Game = 0,

    /// <summary>
    /// The user is streaming video.
    /// </summary>
    Streaming = 1,

    /// <summary>
    /// The user is listening to music.
    /// </summary>
    Listening = 2,

    /// <summary>
    /// The user is watching a video.
    /// </summary>
    Watching = 3,

    /// <summary>
    /// The user has a custom status.
    /// </summary>
    Custom = 4,

    /// <summary>
    /// The user is competing in something.
    /// </summary>
    Competing = 5
}
