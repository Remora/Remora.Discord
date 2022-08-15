//
//  WidgetImageStyle.cs
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
/// Enumerates various widget image styles.
/// </summary>
[PublicAPI]
public enum WidgetImageStyle
{
    /// <summary>
    /// A small shield-style (GitHub shields) with the Discord icon and a member count.
    /// </summary>
    Shield,

    /// <summary>
    /// A large image with the guild icon, name, and online member count. "POWERED BY DISCORD" as the footer of the
    /// widget.
    /// </summary>
    Banner1,

    /// <summary>
    /// A smaller widget style with the guild icon, name, and online member count. Split on the right with the
    /// Discord logo.
    /// </summary>
    Banner2,

    /// <summary>
    /// A large image with the guild icon, name, and online member count. In the footer, the Discord logo s on the
    /// left, and "Chat Now" on the right.
    /// </summary>
    Banner3,

    /// <summary>
    /// A large Discord logo at the top of the widget with the guild icon, name, and online member count in the
    /// middle portion of the widget, and a "JOIN MY SERVER" button at the bottom.
    /// </summary>
    Banner4
}
