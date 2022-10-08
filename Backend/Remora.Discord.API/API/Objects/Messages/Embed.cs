//
//  Embed.cs
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
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

#pragma warning disable CS1591

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IEmbed" />
[PublicAPI]
public record Embed
(
    Optional<string> Title = default,
    Optional<EmbedType> Type = default,
    Optional<string> Description = default,
    Optional<string> Url = default,
    Optional<DateTimeOffset> Timestamp = default,
    Optional<Color> Colour = default,
    Optional<IEmbedFooter> Footer = default,
    Optional<IEmbedImage> Image = default,
    Optional<IEmbedThumbnail> Thumbnail = default,
    Optional<IEmbedVideo> Video = default,
    Optional<IEmbedProvider> Provider = default,
    Optional<IEmbedAuthor> Author = default,
    Optional<IReadOnlyList<IEmbedField>> Fields = default
) : IEmbed;
