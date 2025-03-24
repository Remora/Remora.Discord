//
//  ISectionComponent.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using OneOf;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a section.
/// </summary>
[PublicAPI]
public interface ISectionComponent : IMessageComponent, IPartialSectionComponent
{
    /// <summary>
    /// Gets the components of the section.
    /// </summary>
    new IReadOnlyList<ITextDisplayComponent> Components { get; }

    /// <summary>
    /// Gets the accessory associated with this section.
    /// </summary>
    /// <remarks>Currently, only a <see cref="IThumbnailComponent"/> or <see cref="IButtonComponent"/> may be used.</remarks>
    new IMessageComponent Accessory { get; }

    /// <inheritdoc/>
    // N.B: This should technically be OneOf<IPartialThumbnailComponent, IPartialButtonComponent>, but due to the nature
    // of OneOf and how Roslyn handles interface casting, this requires using hacky code; instead we just deal with
    // the generic interface and hope that users use the right type. This is unprecedented, but it'll have to do.
    Optional<IPartialMessageComponent> IPartialSectionComponent.Accessory => new(this.Accessory);

    /// <inheritdoc/>
    Optional<IReadOnlyList<IPartialTextDisplayComponent>> IPartialSectionComponent.Components => new(this.Components);
}
