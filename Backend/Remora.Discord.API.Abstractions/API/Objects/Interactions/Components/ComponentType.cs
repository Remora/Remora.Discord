//
//  ComponentType.cs
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
/// Enumerates the various message component types.
/// </summary>
[PublicAPI]
public enum ComponentType
{
    /// <summary>
    /// A row of actions.
    /// </summary>
    ActionRow = 1,

    /// <summary>
    /// A clickable button.
    /// </summary>
    Button = 2,

    /// <summary>
    /// A menu of selectable strings.
    /// </summary>
    StringSelect = 3,

    /// <summary>
    /// A text field input.
    /// </summary>
    TextInput = 4,

    /// <summary>
    /// A menu of selectable users.
    /// </summary>
    UserSelect = 5,

    /// <summary>
    /// A menu of selectable roles.
    /// </summary>
    RoleSelect = 6,

    /// <summary>
    /// A menu of selectable mentionables (roles, users).
    /// </summary>
    MentionableSelect = 7,

    /// <summary>
    /// A menu of selectable channels.
    /// </summary>
    ChannelSelect = 8,

    /// <summary>
    /// A section.
    /// </summary>
    Section = 9,

    /// <summary>
    /// A block of text.
    /// </summary>
    TextDisplay = 10,

    /// <summary>
    /// A thumbnail.
    /// </summary>
    Thumbnail = 11,

    /// <summary>
    /// An array of media.
    /// </summary>
    MediaGallery = 12,

    /// <summary>
    /// A file.
    /// </summary>
    File = 13,

    /// <summary>
    /// A separator to vertically distance components.
    /// </summary>
    Separator = 14,

    /// <summary>
    /// A container for other components.
    /// </summary>
    Container = 17,
}
