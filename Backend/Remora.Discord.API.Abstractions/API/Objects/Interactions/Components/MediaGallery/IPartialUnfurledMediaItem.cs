//
//  IPartialUnfurledMediaItem.cs
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
/// Represents a partial unfurled media item.
/// </summary>
[PublicAPI]
public interface IPartialUnfurledMediaItem
{
    /// <summary>
    /// Gets the url of this media item.
    /// </summary>
    Optional<string> Url { get; }

    /// <summary>
    /// Gets the proxy url of this media item.
    /// </summary>
    /// <remarks>This field is return-only.</remarks>
    Optional<string> ProxyUrl { get; }

    /// <summary>
    /// Gets the width of this media item.
    /// </summary>
    /// <remarks>This field is return-only.</remarks>
    Optional<int?> Width { get; }

    /// <summary>
    /// Gets the height of this media item.
    /// </summary>
    /// <remarks>This field is return-only.</remarks>
    Optional<int?> Height { get; }

    /// <summary>
    /// Gets the content type of this media item.
    /// </summary>
    /// <remarks>This field is return-only.</remarks>
    Optional<string> ContentType { get; }
}
