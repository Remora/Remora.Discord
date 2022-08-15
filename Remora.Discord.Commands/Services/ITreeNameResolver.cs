//
//  ITreeNameResolver.cs
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
/// Resolves appropriate tree names for command execution based on a command context.
/// </summary>
[PublicAPI]
public interface ITreeNameResolver
{
    /// <summary>
    /// Gets the name of the tree to run commands from, given the provided context.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>
    /// The name of the tree to run commands from, and a boolean indicating whether the command executor may try the
    /// default tree if no command can be executed from the named tree. Whether this actually happens is up to the
    /// implementation.
    /// </returns>
    Task<Result<(string TreeName, bool AllowDefaultTree)>> GetTreeNameAsync
    (
        ICommandContext context,
        CancellationToken ct = default
    );
}
