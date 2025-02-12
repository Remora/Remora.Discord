//
//  IEmbedImage.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents information about an image in an embed.
/// </summary>
[PublicAPI]
public interface IEmbedImage
{
    /// <summary>
    /// Gets the source URL of the image. Only supports http(s) and attachments.
    /// </summary>
    [UriString("GET")]
    string Url { get; }

    /// <summary>
    /// Gets the proxied URL of the image.
    /// </summary>
    Optional<string> ProxyUrl { get; }

    /// <summary>
    /// Gets the height of the image.
    /// </summary>
    Optional<int> Height { get; }

    /// <summary>
    /// Gets the width of the image.
    /// </summary>
    Optional<int> Width { get; }
}
