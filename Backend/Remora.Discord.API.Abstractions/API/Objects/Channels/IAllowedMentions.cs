//
//  IAllowedMentions.cs
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
/// Represents a set of allowed mentions.
/// </summary>
[PublicAPI]
public interface IAllowedMentions
{
    /// <summary>
    /// Gets a list of mention types to parse. The contents of this field has a mutually exclusive relationship with
    /// the <see cref="Roles"/> and <see cref="Users"/> fields - that is, if that type is contained in this field,
    /// the corresponding field in the type may not appear.
    /// </summary>
    Optional<IReadOnlyList<MentionType>> Parse { get; }

    /// <summary>
    /// Gets a list of allowed roles to mention.
    /// </summary>
    Optional<IReadOnlyList<Snowflake>> Roles { get; }

    /// <summary>
    /// Gets a list of allowed users to mention.
    /// </summary>
    Optional<IReadOnlyList<Snowflake>> Users { get; }

    /// <summary>
    /// Gets a value indicating whether the replied-to user should be mentioned.
    /// </summary>
    Optional<bool> MentionRepliedUser { get; }
}
