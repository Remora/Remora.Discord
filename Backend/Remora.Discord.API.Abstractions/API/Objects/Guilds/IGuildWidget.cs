//
//  IGuildWidget.cs
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
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents the status and invite widget for a guild.
/// </summary>
[PublicAPI]
public interface IGuildWidget
{
    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the name of the guild (2-100 characters).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the instant invite link or code for the guild.
    /// </summary>
    string? InstantInvite { get; }

    /// <summary>
    /// Gets the voice and stage channels accessible by @everyone.
    /// </summary>
    IReadOnlyList<IPartialChannel> Channels { get; }

    /// <summary>
    /// Gets "special" widget user objects with presence information (max 100).
    /// </summary>
    /// <remarks>
    /// The Discord docs are extremely vague about what "special" means here. Your mileage may vary.
    /// </remarks>
    IReadOnlyList<IPartialUser> Members { get; }

    /// <summary>
    /// Gets the number of online members in this guild.
    /// </summary>
    int PresenceCount { get; }
}
