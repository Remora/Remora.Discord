//
//  ITeamMember.cs
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
/// Represents a Discord developer team member.
/// </summary>
[PublicAPI]
public interface ITeamMember
{
    /// <summary>
    /// Gets the user's membership state on the team.
    /// </summary>
    MembershipState MembershipState { get; }

    /// <summary>
    /// Gets the permissions of the member. Currently, always '[ "*" ]'.
    /// </summary>
    IReadOnlyList<string> Permissions { get; }

    /// <summary>
    /// Gets the ID of the parent team of which the user is a member.
    /// </summary>
    Snowflake TeamID { get; }

    /// <summary>
    /// Gets the user that's part of the team.
    /// </summary>
    IPartialUser User { get; }
}
