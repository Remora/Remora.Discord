//
//  ResultAssert.cs
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

using Remora.Results;
using Xunit;

namespace Remora.Discord.Tests;

/// <summary>
/// Contains helper assertions for results.
/// </summary>
public static class ResultAssert
{
    /// <summary>
    /// Asserts that the given result is successful.
    /// </summary>
    /// <typeparam name="TResult">The result type to inspect.</typeparam>
    /// <param name="result">The result.</param>
    public static void Successful<TResult>(TResult result) where TResult : struct, IResult
    {
        Assert.True
        (
            result.IsSuccess,
            result.IsSuccess ? string.Empty : result.Error?.Message ?? "Unknown error."
        );
    }

    /// <summary>
    /// Asserts that a given result is unsuccessful.
    /// </summary>
    /// <typeparam name="TResult">The result type to inspect.</typeparam>
    /// <param name="result">The result.</param>
    public static void Unsuccessful<TResult>(TResult result) where TResult : struct, IResult
    {
        Assert.False(result.IsSuccess, "The result was successful.");
    }
}
