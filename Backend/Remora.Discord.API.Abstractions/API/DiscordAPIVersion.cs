//
//  DiscordAPIVersion.cs
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

namespace Remora.Discord.API.Abstractions;

/// <summary>
/// Enumerates various released versions of the Discord API.
/// </summary>
[PublicAPI]
public enum DiscordAPIVersion
{
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
    /// The default version of the API.
    /// </summary>
    [Obsolete("The default API version is currently obsolete. Use a more recent version for new applications.")]
    Default = V6,

    /// <summary>
    /// The stable version of the API.
    /// </summary>
    Stable = V10
}
