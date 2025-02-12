//
//  IPromptOption.cs
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
/// Represents an option for a prompt.
/// </summary>
[PublicAPI]
public interface IPromptOption
{
    /// <summary>
    /// Gets the ID of the prompt option.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets a list of IDs for channels a member is added to when the option is selected.
    /// </summary>
    IReadOnlyList<Snowflake> ChannelIDs { get; }

    /// <summary>
    /// Gets a list of IDs for roles assigned to a member when the option is selected.
    /// </summary>
    IReadOnlyList<Snowflake> RoleIDs { get; }

    /// <summary>
    /// Gets the emoji of the option.
    /// </summary>
    IEmoji Emoji { get; }

    /// <summary>
    /// Gets the title of the option.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the description of the option.
    /// </summary>
    string? Description { get; }
}
