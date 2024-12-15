//
//  IFileDisplayServerComponent.cs
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

using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a singular file as a component.
/// </summary>
public interface IFileDisplayServerComponent : IMessageComponent
{
    /// <summary>
    /// Gets the file associated with this component.
    /// </summary>
    /// <remarks>When creating a message, this file only supports attachment:// references, and not https:// urls.</remarks>
    IUnfurledMediaItem File { get; }

    /// <summary>
    /// Gets whether this file is spoilered.
    /// </summary>
    Optional<bool> IsSpoiler { get; }
}
