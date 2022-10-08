//
//  BuilderValidationTests.cs
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

using Remora.Discord.Extensions.Tests.Samples;
using Xunit;

namespace Remora.Discord.Extensions.Tests;

/// <summary>
/// Ensures that the builder validation works correctly.
/// </summary>
public class BuilderValidationTests
{
    /// <summary>
    /// Tests a model where the input is within specifications.
    /// </summary>
    [Fact]
    public void ValidationSuccess()
    {
        var personBuilder = new PersonBuilder("Gary", 5);

        Assert.True(personBuilder.Validate().IsSuccess);
    }

    /// <summary>
    /// Tests a model where the input is not within specifications.
    /// </summary>
    [Fact]
    public void ValidationFailure()
    {
        var personBuilder = new PersonBuilder("Mary", 500);

        Assert.False(personBuilder.Validate().IsSuccess);
    }

    /// <summary>
    /// Ensures a builder built from a model is equivalent to the provided model.
    /// </summary>
    [Fact]
    public void PersonAndBuilderEqual()
    {
        var person = new Person("Gary", 5);
        var personBuilder = new PersonBuilder(person.Name, person.Age);

        Assert.Equal(person.Name, personBuilder.Name);
        Assert.Equal(person.Age, personBuilder.Age);
    }

    /// <summary>
    /// Ensures a person built from a builder is equivalent to a person built without a builder.
    /// </summary>
    [Fact]
    public void PersonAndBuiltBuilderEqual()
    {
        var person = new Person("Mary", 50);
        var personBuilder = new PersonBuilder(person.Name, person.Age);
        var person2 = personBuilder.Build().Entity;

        Assert.Equal(person.Name, person2.Name);
        Assert.Equal(person.Age, person2.Age);
    }
}
