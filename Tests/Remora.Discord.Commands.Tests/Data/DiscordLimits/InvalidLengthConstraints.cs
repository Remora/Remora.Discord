//
//  InvalidLengthConstraints.cs
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

namespace Remora.Discord.Commands.Tests.Data.DiscordLimits;

/// <summary>
/// Defines various permutations of groups with commands that have parameters with invalid constraints on their input
/// lengths.
/// </summary>
public class InvalidLengthConstraints
{
    /// <summary>
    /// Defines a command with an invalid minimum length constraint set.
    /// </summary>
    public class InvalidMinLengthConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("command")]
        public Task<IResult> Command([MinLength(-1)] string parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with an invalid maximum length constraint set.
    /// </summary>
    public class InvalidMaxLengthConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("command")]
        public Task<IResult> Command([MaxLength(0)] string parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with an invalid minimum and valid maximum length constraint set.
    /// </summary>
    public class InvalidMinAndValidMaxLengthConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("command")]
        public Task<IResult> Command([MinLength(-1), MaxLength(1)] string parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a valid minimum and invalid maximum length constraint set.
    /// </summary>
    public class ValidMinAndInvalidMaxLengthConstraint : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("command")]
        public Task<IResult> Command([MinLength(0), MaxLength(0)] string parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a valid minimum length constraint set on an incompatible type.
    /// </summary>
    public class MinConstraintOnIncompatibleParameterType : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("command")]
        public Task<IResult> Command([MinLength(0)] int parameter)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// Defines a command with a valid minimum length constraint set on an incompatible type.
    /// </summary>
    public class MaxConstraintOnIncompatibleParameterType : CommandGroup
    {
        /// <summary>
        /// The command.
        /// </summary>
        /// <param name="parameter">The annotated parameter.</param>
        /// <returns>Nothing.</returns>
        [Command("command")]
        public Task<IResult> Command([MaxLength(1)] int parameter)
            => throw new NotImplementedException();
    }
}
