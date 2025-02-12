//
//  IPreparationErrorEvent.cs
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
using JetBrains.Annotations;
using Remora.Discord.Commands.Contexts;
using Remora.Results;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Represents the public interface of a service that can perform a command preparation error event.
/// </summary>
[PublicAPI]
public interface IPreparationErrorEvent
{
    /// <summary>
    /// Runs when attempted preparation of a command fails.
    /// </summary>
    /// <param name="context">The operation context.</param>
    /// <param name="preparationResult">The result of the command preparation.</param>
    /// <param name="ct">The cancellation token of the current operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result> PreparationFailed
    (
        IOperationContext context,
        IResult preparationResult,
        CancellationToken ct = default
    );
}
