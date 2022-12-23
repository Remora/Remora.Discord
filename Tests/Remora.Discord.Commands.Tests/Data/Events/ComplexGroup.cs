//
//  ComplexGroup.cs
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

using System.Threading;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Conditions;
using Remora.Commands.Groups;
using Remora.Results;

#pragma warning disable SA1402

namespace Remora.Discord.Commands.Tests.Data.Events;

/// <summary>
/// A slightly more complex group for testing failure cases.
/// </summary>
public class ComplexGroup : CommandGroup
{
    /// <summary>
    /// A command that will never execute due to a condition.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("failing-condition")]
    [AlwaysFailCondition]
    public Task<Result> FailingCondition() => Task.FromResult(Result.FromSuccess());

    /// <summary>
    /// A command that takes a parameter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("with-parameter")]
    public Task<Result> WithParameter(int value) => Task.FromResult(Result.FromSuccess());
}

/// <summary>
/// Marks a command as always failing no matter what.
/// </summary>
public class AlwaysFailConditionAttribute : ConditionAttribute
{
}

/// <summary>
/// Always fails no matter what.
/// </summary>
public class AlwaysFailCondition : ICondition<AlwaysFailConditionAttribute>
{
    /// <inheritdoc />
    public ValueTask<Result> CheckAsync(AlwaysFailConditionAttribute attribute, CancellationToken ct = default)
    {
        return new(new InvalidOperationError("Always failing!"));
    }
}
