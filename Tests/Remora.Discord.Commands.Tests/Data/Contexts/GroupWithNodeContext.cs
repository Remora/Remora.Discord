//
//  GroupWithNodeContext.cs
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
using Remora.Discord.Commands.Services;
using Remora.Results;

#pragma warning disable CS1591, SA1600

namespace Remora.Discord.Commands.Tests.Data.Contexts;

[Group("node")]
public class GroupWithNodeContext : CommandGroup
{
    private readonly CommandNodeInjectionService _nodeContext;

    public GroupWithNodeContext(CommandNodeInjectionService nodeContext)
    {
        _nodeContext = nodeContext;
    }

    [Command("success")]
    public Task<Result> Success()
    {
        return Task.FromResult(Result.FromSuccess());
    }

    [Command("failure")]
    public Task<Result> FailAsync()
    {
        return Task.FromResult(Result.FromError(new InvalidOperationError("Failed")));
    }

    [Command("throw")]
    public Task<Result> ThrowAsync()
    {
        throw new InvalidOperationException("Thrown");
    }
}
