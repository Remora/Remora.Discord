//
//  UnnamedGroupWithDefaultPermission.cs
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

namespace Remora.Discord.Commands.Tests.Data.Valid;

/// <summary>
/// A group with a default permission set.
/// </summary>
[DiscordDefaultMemberPermissions(DiscordPermission.Administrator)]
public class UnnamedGroupWithDefaultPermission : CommandGroup
{
    /// <summary>
    /// The first command.
    /// </summary>
    /// <returns>Nothing.</returns>
    [Command("a")]
    public Task<Result> A() => throw new NotImplementedException();
}
