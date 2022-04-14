//
//  IAttachment.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents an attachment in a message.
/// </summary>
[PublicAPI]
public interface IAttachment : IPartialAttachment
{
    /// <summary>
    /// Gets the ID of the attachment.
    /// </summary>
    new Snowflake ID { get; }

    /// <summary>
    /// Gets the filename of the attachment.
    /// </summary>
    new string Filename { get; }

    /// <summary>
    /// Gets the description of the attachment.
    /// </summary>
    new Optional<string> Description { get; }

    /// <summary>
    /// Gets the attachment's media type (e.g, "application/text" or similar).
    /// </summary>
    new Optional<string> ContentType { get; }

    /// <summary>
    /// Gets the size of the file in bytes.
    /// </summary>
    new int Size { get; }

    /// <summary>
    /// Gets the source URL of the file.
    /// </summary>
    new string Url { get; }

    /// <summary>
    /// Gets the proxied URL of the file.
    /// </summary>
    new string ProxyUrl { get; }

    /// <summary>
    /// Gets the height of the file (if image).
    /// </summary>
    new Optional<int?> Height { get; }

    /// <summary>
    /// Gets the width of the file (if image).
    /// </summary>
    new Optional<int?> Width { get; }

    /// <summary>
    /// Gets a value indicating whether the attachment is ephemeral.
    /// </summary>
    new Optional<bool> IsEphemeral { get; }

    /// <inheritdoc/>
    Optional<Snowflake> IPartialAttachment.ID => this.ID;

    /// <inheritdoc/>
    Optional<string> IPartialAttachment.Filename => this.Filename;

    /// <inheritdoc/>
    Optional<string> IPartialAttachment.Description => this.Description;

    /// <inheritdoc/>
    Optional<string> IPartialAttachment.ContentType => this.ContentType;

    /// <inheritdoc/>
    Optional<int> IPartialAttachment.Size => this.Size;

    /// <inheritdoc/>
    Optional<string> IPartialAttachment.Url => this.Url;

    /// <inheritdoc/>
    Optional<string> IPartialAttachment.ProxyUrl => this.ProxyUrl;

    /// <inheritdoc/>
    Optional<int?> IPartialAttachment.Height => this.Height;

    /// <inheritdoc/>
    Optional<int?> IPartialAttachment.Width => this.Width;

    /// <inheritdoc/>
    Optional<bool> IPartialAttachment.IsEphemeral => this.IsEphemeral;
}
