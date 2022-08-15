//
//  ITemplate.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a guild template.
/// </summary>
[PublicAPI]
public interface ITemplate
{
    /// <summary>
    /// Gets the template code (a unique ID).
    /// </summary>
    string Code { get; }

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description for the template.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Gets the number of times this template has been used.
    /// </summary>
    int UsageCount { get; }

    /// <summary>
    /// Gets the ID of the user who created the template.
    /// </summary>
    Snowflake CreatorID { get; }

    /// <summary>
    /// Gets the user who created the template.
    /// </summary>
    IUser Creator { get; }

    /// <summary>
    /// Gets the time when the template was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the last time the template was updated.
    /// </summary>
    DateTimeOffset UpdatedAt { get; }

    /// <summary>
    /// Gets the ID of the guild the template is based on.
    /// </summary>
    Snowflake SourceGuildID { get; }

    /// <summary>
    /// Gets the guild snapshot this template contains.
    /// </summary>
    IGuildTemplate SerializedSourceGuild { get; }

    /// <summary>
    /// Gets a value indicating whether the template has unsynchronized changes.
    /// </summary>
    bool? IsDirty { get; }
}
