//
//  IEmbed.cs
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
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an embed.
/// </summary>
[PublicAPI]
public interface IEmbed
{
    /// <summary>
    /// Gets the title of the embed.
    /// </summary>
    Optional<string> Title { get; }

    /// <summary>
    /// Gets the type of the embed.
    /// </summary>
    Optional<EmbedType> Type { get; }

    /// <summary>
    /// Gets the description of the embed.
    /// </summary>
    Optional<string> Description { get; }

    /// <summary>
    /// Gets the URL of the embed.
    /// </summary>
    Optional<string> Url { get; }

    /// <summary>
    /// Gets the timestamp of the embed content.
    /// </summary>
    Optional<DateTimeOffset> Timestamp { get; }

    /// <summary>
    /// Gets the colour code of the embed.
    /// </summary>
    Optional<Color> Colour { get; }

    /// <summary>
    /// Gets the footer information.
    /// </summary>
    Optional<IEmbedFooter> Footer { get; }

    /// <summary>
    /// Gets the image information.
    /// </summary>
    Optional<IEmbedImage> Image { get; }

    /// <summary>
    /// Gets the thumbnail information.
    /// </summary>
    Optional<IEmbedThumbnail> Thumbnail { get; }

    /// <summary>
    /// Gets the video information.
    /// </summary>
    Optional<IEmbedVideo> Video { get; }

    /// <summary>
    /// Gets the provider information.
    /// </summary>
    Optional<IEmbedProvider> Provider { get; }

    /// <summary>
    /// Gets the author information.
    /// </summary>
    Optional<IEmbedAuthor> Author { get; }

    /// <summary>
    /// Gets the embed fields.
    /// </summary>
    Optional<IReadOnlyList<IEmbedField>> Fields { get; }
}
