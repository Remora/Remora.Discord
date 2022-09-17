//
//  NumericalParameterWithValueConstraints.cs
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
using Remora.Discord.Commands.Attributes;
using Remora.Results;

namespace Remora.Discord.Commands.Tests.Data.Valid;

/// <summary>
/// Defines various permutations of groups with commands that have string parameters with constraints on their input
/// lengths.
/// </summary>
public class NumericalParameterWithValueConstraints
{
    /// <summary>
    /// Defines a command with a minimum length constraint set.
    /// </summary>
    public class MinValueConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MinValue(0)] ulong parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a maximum length constraint set.
    /// </summary>
    public class MaxValueConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MaxValue(1)] ulong parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a minimum and maximum length constraint set.
    /// </summary>
    public class MinAndMaxValueConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MinValue(0), MaxValue(1)] ulong parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a minimum length constraint set.
    /// </summary>
    public class MinFloatValueConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MinValue(0)] float parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a maximum length constraint set.
    /// </summary>
    public class MaxFloatValueConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MaxValue(1)] float parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a minimum and maximum length constraint set.
    /// </summary>
    public class MinAndMaxFloatValueConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("min")]
        public Task<IResult> Command([MinValue(0), MaxValue(1)] float parameter)
            => throw new NotImplementedException();
    }
}
