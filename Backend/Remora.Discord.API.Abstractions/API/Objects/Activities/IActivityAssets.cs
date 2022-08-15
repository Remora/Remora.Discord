//
//  IActivityAssets.cs
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
/// Represents a set of descriptive assets related to an activity.
/// </summary>
[PublicAPI]
public interface IActivityAssets
{
    /// <summary>
    /// Gets the ID for a large image related to the activity. Usually, this is a snowflake.
    /// </summary>
    Optional<string> LargeImage { get; }

    /// <summary>
    /// Gets the text displayed when hovering over the large image.
    /// </summary>
    Optional<string> LargeText { get; }

    /// <summary>
    /// Gets the ID for a small image related to the activity. Usually, this is a snowflake.
    /// </summary>
    Optional<string> SmallImage { get; }

    /// <summary>
    /// Gets the text displayed when hovering over the small image.
    /// </summary>
    Optional<string> SmallText { get; }
}
