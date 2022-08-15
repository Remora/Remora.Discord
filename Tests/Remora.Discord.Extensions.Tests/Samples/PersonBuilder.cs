//
//  PersonBuilder.cs
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

using Remora.Discord.Extensions.Builder;
using Remora.Discord.Extensions.Errors;
using Remora.Results;

namespace Remora.Discord.Extensions.Tests.Samples;

/// <summary>
/// Builds a person.
/// </summary>
internal class PersonBuilder : BuilderBase<Person>
{
    /// <summary>
    /// Gets the person's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the person's age.
    /// </summary>
    public int Age { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonBuilder"/> class.
    /// </summary>
    /// <param name="name">The person's name.</param>
    /// <param name="age">The person's age.</param>
    public PersonBuilder(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }

    /// <inheritdoc />
    public override Result<Person> Build()
    {
        var validationResult = Validate();

        return validationResult.IsSuccess
            ? new Person(this.Name, this.Age)
            : Result<Person>.FromError(validationResult);
    }

    /// <inheritdoc />
    public override Result Validate()
    {
        var nameValidate = ValidateLength(nameof(this.Name), this.Name, 5, false);
        if (!nameValidate.IsSuccess)
        {
            return nameValidate;
        }

        return this.Age is < 1 or > 100
            ? new ValidationError(nameof(this.Age), "Age must be between 1 and 100.")
            : Result.FromSuccess();
    }
}
