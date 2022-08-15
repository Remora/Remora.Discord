//
//  ICommandPrefixMatcher.cs
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
using Remora.Results;

namespace Remora.Discord.Commands.Services;

/// <summary>
/// Provides matching logic for text-based command prefixes.
/// </summary>
[PublicAPI]
public interface ICommandPrefixMatcher
{
    /// <summary>
    /// Determines whether the message contents begin or match some accepted command prefix.
    /// </summary>
    /// <param name="content">The message contents to check.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>
    /// A tuple which indicates whether the contents match an accepted prefix, and if so, the index at which the actual
    /// command contents start.
    /// </returns>
    ValueTask<Result<(bool Matches, int ContentStartIndex)>> MatchesPrefixAsync
    (
        string content,
        CancellationToken ct = default
    );
}
