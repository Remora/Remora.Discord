//
//  ResultErrorExtensions.cs
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

using JetBrains.Annotations;
using Remora.Commands.Results;
using Remora.Results;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IResultError"/> interface.
/// </summary>
[PublicAPI]
public static class ResultErrorExtensions
{
    /// <summary>
    /// Determines whether the error has resulted from either invalid user input or the execution environment.
    /// </summary>
    /// <remarks>
    /// Currently, the following errors are considered user error.
    /// <list type="bullet">
    /// <item>CommandNotFoundError</item>
    /// <item>AmbiguousCommandInvocationError</item>
    /// <item>RequiredParameterValueMissingError</item>
    /// <item>ParameterParsingError</item>
    /// <item>ConditionNotSatisfiedError</item>
    /// </list>
    /// </remarks>
    /// <param name="error">The error.</param>
    /// <returns>true if the error is a user or environment error; otherwise, false.</returns>
    public static bool IsUserOrEnvironmentError(this IResultError error)
    {
        return error
            is CommandNotFoundError
            or AmbiguousCommandInvocationError
            or RequiredParameterValueMissingError
            or ParameterParsingError
            or ConditionNotSatisfiedError;
    }
}
