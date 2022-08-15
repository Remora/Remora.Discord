//
//  SimpleGroup.cs
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

using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace Remora.Discord.Commands.Tests.Data.Events;

/// <summary>
/// A simple command group for testing execution events.
/// </summary>
public class SimpleGroup : CommandGroup
{
    /// <summary>
    /// A command that will always execute successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("successful")]
    public Task<Result> Successful() => Task.FromResult(Result.FromSuccess());

    /// <summary>
    /// A command that will always execute unsuccessfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("unsuccessful")]
    public Task<Result> Unsuccessful() => Task.FromResult(Result.FromError(new InvalidOperationError("Oh no!")));
}
