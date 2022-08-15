//
//  CustomIDTestBase.cs
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
using Xunit;

namespace Remora.Discord.Interactivity.Tests.Bases;

/// <summary>
/// Acts as a base class for custom ID tests.
/// </summary>
public abstract class CustomIDTestBase
{
    /// <summary>
    /// Creates a simple, ungrouped custom ID.
    /// </summary>
    /// <param name="name">The name used to identify the component.</param>
    /// <returns>The custom ID.</returns>
    protected delegate string SimpleDelegate(string name);

    /// <summary>
    /// Creates a grouped custom ID.
    /// </summary>
    /// <param name="name">The name used to identify the component.</param>
    /// <param name="path">The group path to the component.</param>
    /// <returns>The custom ID.</returns>
    protected delegate string GroupedDelegate(string name, params string[] path);

    /// <summary>
    /// Gets the name of the component type under test.
    /// </summary>
    protected abstract string TypeName { get; }

    /// <summary>
    /// Gets the simple method under test.
    /// </summary>
    protected abstract SimpleDelegate Simple { get; }

    /// <summary>
    /// Gets the group-aware method under test.
    /// </summary>
    protected abstract GroupedDelegate Grouped { get; }

    /// <summary>
    /// Tests whether simple method returns a correctly formatted value.
    /// </summary>
    [Fact]
    public void ReturnsCorrectValueForSimpleCase()
    {
        const string name = "ooga";

        var expected = $"{Constants.InteractionTree}::{this.TypeName}::{name}";
        var actual = this.Simple(name);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests whether the group-aware method returns a correctly formatted
    /// value.
    /// </summary>
    [Fact]
    public void ReturnsCorrectValueForGroupedCase()
    {
        const string name = "ooga";
        const string group1 = "wooga";
        const string group2 = "booga";

        var expected = $"{Constants.InteractionTree}::{group1} {group2} {this.TypeName}::{name}";
        var actual = this.Grouped(name, group1, group2);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests whether the simple method throws if the provided name is composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void SimpleCaseThrowsIfNameIsWhitespace()
    {
        const string name = " ";
        Assert.Throws<ArgumentException>(() => this.Simple(name));
    }

    /// <summary>
    /// Tests whether the grouped method throws if the provided name is composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void GroupedCaseThrowsIfNameIsWhitespace()
    {
        const string name = " ";
        Assert.Throws<ArgumentException>(() => this.Grouped(name));
    }

    /// <summary>
    /// Tests whether the grouped method throws if any provided path component is composed entirely of whitespace.
    /// </summary>
    [Fact]
    public void GroupedCaseThrowsIfAnyPathComponentIsWhitespace()
    {
        const string name = "ooga";
        const string group1 = "wooga";
        const string group2 = " ";

        Assert.Throws<ArgumentException>(() => this.Grouped(name, group1, group2));
    }

    /// <summary>
    /// Tests whether the simple method throws if the provided name contains whitespace.
    /// </summary>
    [Fact]
    public void SimpleThrowsIfNameContainsWhitespace()
    {
        const string name = "oo ga";
        Assert.Throws<ArgumentException>(() => this.Simple(name));
    }

    /// <summary>
    /// Tests whether the grouped method throws if the provided name contains whitespace.
    /// </summary>
    [Fact]
    public void GroupedThrowsIfNameContainsWhitespace()
    {
        const string name = "oo ga";
        Assert.Throws<ArgumentException>(() => this.Grouped(name));
    }

    /// <summary>
    /// Tests whether the simple method throws if any provided path component contains whitespace.
    /// </summary>
    [Fact]
    public void GroupedThrowsIfAnyPathComponentContainsWhitespace()
    {
        const string name = "ooga";
        const string group1 = "wooga";
        const string group2 = "boo ga";

        Assert.Throws<ArgumentException>(() => this.Grouped(name, group1, group2));
    }

    /// <summary>
    /// Tests whether the simple method throws if the final ID is longer than 100 characters.
    /// </summary>
    [Fact]
    public void SimpleThrowsIfNameIsTooLong()
    {
        var name = new string('a', 80);

        Assert.Throws<ArgumentException>(() => this.Simple(name));
    }

    /// <summary>
    /// Tests whether the group-aware method throws if the final ID is longer than 100 characters.
    /// </summary>
    [Fact]
    public void GroupedThrowsIfNameIsTooLong()
    {
        var name = new string('a', 80);

        Assert.Throws<ArgumentException>(() => this.Grouped(name));
    }

    /// <summary>
    /// Tests whether the group-aware method throws if the final ID is longer than 100 characters.
    /// </summary>
    [Fact]
    public void GroupedThrowsIfArgumentCombinationIsTooLong()
    {
        var name = new string('a', 40);
        var group1 = new string('b', 40);

        Assert.Throws<ArgumentException>(() => this.Grouped(name, group1));
    }
}
