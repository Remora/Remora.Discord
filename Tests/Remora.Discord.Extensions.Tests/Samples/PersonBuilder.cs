//
//  PersonBuilder.cs
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
using System.ComponentModel.DataAnnotations;
using Remora.Discord.Extensions.Builder;
using Remora.Results;

namespace Remora.Discord.Extensions.Tests.Samples
{
    /// <summary>
    /// Builds a person.
    /// </summary>
    internal class PersonBuilder : BuilderBase<Person>
    {
        /// <summary>
        /// Gets the person's name.
        /// </summary>
        [MaxLength(5)]
        public string Name { get; }

        /// <summary>
        /// Gets the person's age.
        /// </summary>
        [Range(1, 100)]
        public int Age { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonBuilder"/> class.
        /// </summary>
        /// <param name="name">The person's name.</param>
        /// <param name="age">The person's age.</param>
        public PersonBuilder(string name, int age)
        {
            Name = name;
            Age = age;
        }

        /// <inheritdoc />
        public override Result<Person> Build()
        {
            var validationResult = this.Validate();

            return validationResult.IsSuccess
                ? new Person(Name, Age)
                : Result<Person>.FromError(validationResult);
        }
    }
}
