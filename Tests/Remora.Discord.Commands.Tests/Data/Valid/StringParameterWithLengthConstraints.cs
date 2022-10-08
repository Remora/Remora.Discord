//
//  StringParameterWithLengthConstraints.cs
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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace Remora.Discord.Commands.Tests.Data.Valid;

/// <summary>
/// Defines various permutations of groups with commands that have string parameters with constraints on their input
/// lengths.
/// </summary>
public class StringParameterWithLengthConstraints
{
    /// <summary>
    /// Defines a command with a minimum length constraint set.
    /// </summary>
    public class MinLengthConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MinLength(0)] string parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a maximum length constraint set.
    /// </summary>
    public class MaxLengthConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MaxLength(1)] string parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a minimum and maximum length constraint set.
    /// </summary>
    public class MinAndMaxLengthConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MinLength(0), MaxLength(1)] string parameter)
            => throw new NotImplementedException();
    }
}
