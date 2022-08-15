//
//  ButtonComponentStyle.cs
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
/// Enumerates various button styles.
/// </summary>
[PublicAPI]
public enum ButtonComponentStyle
{
    /// <summary>
    /// A standard-looking blurple button.
    /// </summary>
    /// <remarks>
    /// This button style requires a valid value in <see cref="IButtonComponent.CustomID"/>.
    /// </remarks>
    Primary = 1,

    /// <summary>
    /// A grey, incognito button.
    /// </summary>
    /// <remarks>
    /// This button style requires a valid value in <see cref="IButtonComponent.CustomID"/>.
    /// </remarks>
    Secondary = 2,

    /// <summary>
    /// A green button, indicating confirmation or success.
    /// </summary>
    /// <remarks>
    /// This button style requires a valid value in <see cref="IButtonComponent.CustomID"/>.
    /// </remarks>
    Success = 3,

    /// <summary>
    /// A red button, indicating rejection or danger.
    /// </summary>
    /// <remarks>
    /// This button style requires a valid value in <see cref="IButtonComponent.CustomID"/>.
    /// </remarks>
    Danger = 4,

    /// <summary>
    /// A grey button with a link.
    /// </summary>
    /// <remarks>
    /// This button style requires a valid value in <see cref="IButtonComponent.URL"/>.
    /// </remarks>
    Link = 5
}
