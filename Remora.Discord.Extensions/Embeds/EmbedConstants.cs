//
//  EmbedConstants.cs
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

using System.Drawing;
using JetBrains.Annotations;

namespace Remora.Discord.Extensions.Embeds;

/// <summary>
/// Provides a set of constant values for embed validation.
/// </summary>
[PublicAPI]
public static class EmbedConstants
{
    /// <summary>
    /// Gets the maximum length of a title.
    /// </summary>
    public const int MaxTitleLength = 256;

    /// <summary>
    /// Gets the maximum length of a description.
    /// </summary>
    public const int MaxDescriptionLength = 4096;

    /// <summary>
    /// Gets the maximum number of fields.
    /// </summary>
    public const int MaxFieldCount = 25;

    /// <summary>
    /// Gets the maximum length of a field name.
    /// </summary>
    public const int MaxFieldNameLength = 256;

    /// <summary>
    /// Gets the maximum length of a field value.
    /// </summary>
    public const int MaxFieldValueLength = 1024;

    /// <summary>
    /// Gets the maximum length of an author's name.
    /// </summary>
    public const int MaxAuthorNameLength = 256;

    /// <summary>
    /// Gets the maximum length of a footer.
    /// </summary>
    public const int MaxFooterTextLength = 2048;

    /// <summary>
    /// Gets the maximum overall size of an embed.
    /// </summary>
    public const int MaxEmbedLength = 6000;

    /// <summary>
    /// Gets the default embed color.
    /// </summary>
    public static Color DefaultColour { get; } = Color.FromArgb(95, 186, 125);
}
