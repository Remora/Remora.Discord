//
//  ResultExtensions.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Discord.Gateway.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="Result"/> class.
/// </summary>
internal static class ResultExtensions
{
    /// <summary>
    /// Gets a value indicating whether this result was unsuccessful,
    /// and contains an <see cref="ExceptionError"/> wrapping either a
    /// <see cref="OperationCanceledException"/> or <see cref="TaskCanceledException"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <returns>A value indicating whether this result contains a cancellation error.</returns>
    [Pure]
    internal static bool HasCancellationError(this Result result)
    {
        return result.Error is ExceptionError { Exception: OperationCanceledException or TaskCanceledException };
    }
}
