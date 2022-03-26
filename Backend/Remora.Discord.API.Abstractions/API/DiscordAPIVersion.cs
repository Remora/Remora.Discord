//
//  DiscordAPIVersion.cs
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

namespace Remora.Discord.API.Abstractions;

/// <summary>
/// Enumerates various released versions of the Discord API.
/// </summary>
public enum DiscordAPIVersion
{
    /// <summary>
    /// Version 3 of the API.
    /// </summary>
    [Obsolete("Discontinued", true)]
    V3 = 3,

    /// <summary>
    /// Version 4 of the API.
    /// </summary>
    [Obsolete("Discontinued", true)]
    V4 = 4,

    /// <summary>
    /// Version 5 of the API.
    /// </summary>
    [Obsolete("Discontinued", true)]
    V5 = 5,

    /// <summary>
    /// Version 6 of the API.
    /// </summary>
    [Obsolete]
    V6 = 6,

    /// <summary>
    /// Version 7 of the API.
    /// </summary>
    [Obsolete("FUBAR", true)]
    V7 = 7,

    /// <summary>
    /// Version 8 of the API.
    /// </summary>
    [Obsolete]
    V8 = 8,

    /// <summary>
    /// Version 9 of the API.
    /// </summary>
    V9 = 9,

    /// <summary>
    /// Version 10 of the API.
    /// </summary>
    V10 = 10,

    /// <summary>
    /// The stable version of the API.
    /// </summary>
    Stable = V9
}
