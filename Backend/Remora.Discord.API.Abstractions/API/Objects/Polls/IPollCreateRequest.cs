//
//  IPollCreateRequest.cs
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
/// Represents a poll creation request.
/// </summary>
[PublicAPI]
public interface IPollCreateRequest
{
    /// <summary>
    /// Gets the question of the poll.
    /// </summary>
    IPollMedia Question { get; }

    /// <summary>
    /// Gets a list of each available answer in the poll.
    /// </summary>
    IReadOnlyList<IPollAnswer> Answers { get; }

    /// <summary>
    /// Gets the number of hours the poll should be open for.
    /// </summary>
    /// <remarks>A poll can be open for up to 7 days.</remarks>
    int Duration { get; }

    /// <summary>
    /// Gets a value indicating whether a user can select multiple answers.
    /// </summary>
    bool IsMultiselectAllowed { get; }

    /// <summary>
    /// Gets the layout type of the poll.
    /// </summary>
    Optional<PollLayoutType> LayoutType { get; }
}
