//
//  Navigation.cs
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

namespace Remora.Discord.Extensions.Formatting;

/// <summary>
/// Provides quick access to various navigation links.
/// </summary>
[PublicAPI]
public static class Navigation
{
    /// <summary>
    /// Creates a guild navigation link.
    /// </summary>
    /// <param name="target">The navigation target.</param>
    /// <returns>The formatted link.</returns>
    public static string Guild(GuildNavigationType target) => $"<id:{target.ToString().ToLowerInvariant()}>";
}
