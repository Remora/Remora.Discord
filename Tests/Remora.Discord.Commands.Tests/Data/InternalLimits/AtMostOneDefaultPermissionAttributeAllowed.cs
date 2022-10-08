//
//  AtMostOneDefaultPermissionAttributeAllowed.cs
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
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Results;

namespace Remora.Discord.Commands.Tests.Data.InternalLimits;

/// <summary>
/// Wraps two test groups.
/// </summary>
public class AtMostOneDefaultPermissionAttributeAllowed
{
    /// <summary>
    /// Wraps named test groups.
    /// </summary>
    public class Named
    {
        /// <summary>
        /// The first group.
        /// </summary>
        [DiscordDefaultMemberPermissions(DiscordPermission.Administrator)]
        [Group("a")]
        public class GroupOne : CommandGroup
        {
            /// <summary>
            /// The first command.
            /// </summary>
            /// <returns>Nothing.</returns>
            [Command("b")]
            public Task<Result> B() => throw new NotImplementedException();
        }

        /// <summary>
        /// The second group.
        /// </summary>
        [DiscordDefaultMemberPermissions(DiscordPermission.BanMembers)]
        [Group("a")]
        public class GroupTwo : CommandGroup
        {
            /// <summary>
            /// The second command.
            /// </summary>
            /// <returns>Nothing.</returns>
            [Command("c")]
            public Task<Result> C() => throw new NotImplementedException();
        }
    }
}
