//
//  Constants.cs
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

namespace Remora.Discord.API;

/// <summary>
/// Holds various constants.
/// </summary>
[PublicAPI]
public static class Constants
{
    /// <summary>
    /// Gets the base CDN URL.
    /// </summary>
    public static Uri CDNBaseURL { get; } = new("https://cdn.discordapp.com/");

    /// <summary>
    /// Gets the Discord epoch, used for timestamp offsetting.
    /// </summary>
    public static ulong DiscordEpoch => 1420070400000;
}
