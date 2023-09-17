//
//  TeamMemberRole.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various roles a team member can have.
/// </summary>
public enum TeamMemberRole
{
    /// <summary>
    ///  Owners are the most permissible role, and can take destructive, irreversible actions like deleting team-owned
    /// apps or the team itself. Teams are limited to 1 owner.
    /// </summary>
    Owner,

    /// <summary>
    /// Admins have similar access as owners, except they cannot take destructive actions on the team or team-owned
    /// apps.
    /// </summary>
    Admin,

    /// <summary>
    ///  Developers can access information about team-owned apps, like the client secret or public key. They can also
    /// take limited actions on team-owned apps, like configuring interaction endpoints or resetting the bot token.
    /// Members with the Developer role *cannot* manage the team or its members, or take destructive actions on
    /// team-owned apps.
    /// </summary>
    Developer,

    /// <summary>
    /// Read-only members can access information about a team and any team-owned apps. Some examples include getting the
    /// IDs of applications and exporting payout records.
    /// </summary>
    ReadOnly
}
