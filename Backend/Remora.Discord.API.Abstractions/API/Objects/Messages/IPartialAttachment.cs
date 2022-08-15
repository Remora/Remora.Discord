//
//  IPartialAttachment.cs
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

/// <inheritdoc cref="IAttachment"/>
[PublicAPI]
public interface IPartialAttachment
{
    /// <inheritdoc cref="IAttachment.ID"/>
    Optional<Snowflake> ID { get; }

    /// <inheritdoc cref="IAttachment.Filename"/>
    Optional<string> Filename { get; }

    /// <inheritdoc cref="IAttachment.Description"/>
    Optional<string> Description { get; }

    /// <inheritdoc cref="IAttachment.ContentType"/>
    Optional<string> ContentType { get; }

    /// <inheritdoc cref="IAttachment.Size"/>
    Optional<int> Size { get; }

    /// <inheritdoc cref="IAttachment.Url"/>
    Optional<string> Url { get; }

    /// <inheritdoc cref="IAttachment.ProxyUrl"/>
    Optional<string> ProxyUrl { get; }

    /// <inheritdoc cref="IAttachment.Height"/>
    Optional<int?> Height { get; }

    /// <inheritdoc cref="IAttachment.Width"/>
    Optional<int?> Width { get; }

    /// <inheritdoc cref="IAttachment.IsEphemeral"/>
    Optional<bool> IsEphemeral { get; }
}
