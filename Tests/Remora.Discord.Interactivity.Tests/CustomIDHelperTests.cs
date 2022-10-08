//
//  CustomIDHelperTests.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Interactivity.Tests.Bases;
using Xunit;

namespace Remora.Discord.Interactivity.Tests;

/// <summary>
/// Tests the <see cref="CustomIDHelpers"/> class.
/// </summary>
public class CustomIDHelperTests
{
    /// <summary>
    /// Tests the <see cref="CustomIDHelpers.CreateButtonID(string)"/> method and its overloads.
    /// </summary>
    public class CreateButtonID : CustomIDTestBase
    {
        /// <inheritdoc />
        protected override string TypeName => "button";

        /// <inheritdoc />
        protected override SimpleDelegate Simple => CustomIDHelpers.CreateButtonID;

        /// <inheritdoc />
        protected override GroupedDelegate Grouped => CustomIDHelpers.CreateButtonID;
    }

    /// <summary>
    /// Tests the <see cref="CustomIDHelpers.CreateSelectMenuID(string)"/> method and its overloads.
    /// </summary>
    public class CreateSelectMenuID : CustomIDTestBase
    {
        /// <inheritdoc />
        protected override string TypeName => "select-menu";

        /// <inheritdoc />
        protected override SimpleDelegate Simple => CustomIDHelpers.CreateSelectMenuID;

        /// <inheritdoc />
        protected override GroupedDelegate Grouped => CustomIDHelpers.CreateSelectMenuID;
    }

    /// <summary>
    /// Tests the <see cref="CustomIDHelpers.CreateModalID(string)"/> method and its overloads.
    /// </summary>
    public class CreateModalID : CustomIDTestBase
    {
        /// <inheritdoc />
        protected override string TypeName => "modal";

        /// <inheritdoc />
        protected override SimpleDelegate Simple => CustomIDHelpers.CreateModalID;

        /// <inheritdoc />
        protected override GroupedDelegate Grouped => CustomIDHelpers.CreateModalID;
    }

    /// <summary>
    /// Tests the <see cref="CustomIDHelpers.CreateID(string, ComponentType)"/> method and its overloads.
    /// </summary>
    public class CreateID
    {
        /// <summary>
        /// Tests whether simple method returns a correctly formatted value.
        /// </summary>
        [Fact]
        public void ReturnsCorrectValueForSimpleCase()
        {
            const string name = "ooga";

            const string expected = $"{Constants.InteractionTree}::button::{name}";
            var actual = CustomIDHelpers.CreateID(name, ComponentType.Button);

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

            const string expected = $"{Constants.InteractionTree}::{group1} {group2} button::{name}";
            var actual = CustomIDHelpers.CreateID(ComponentType.Button, name, group1, group2);

            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Tests whether the simple method throws if the provided name is composed entirely of whitespace.
        /// </summary>
        [Fact]
        public void SimpleCaseThrowsIfNameIsWhitespace()
        {
            const string name = " ";
            Assert.Throws<ArgumentException>(() => CustomIDHelpers.CreateID(name, ComponentType.Button));
        }

        /// <summary>
        /// Tests whether the grouped method throws if the provided name is composed entirely of whitespace.
        /// </summary>
        [Fact]
        public void GroupedCaseThrowsIfNameIsWhitespace()
        {
            const string name = " ";
            Assert.Throws<ArgumentException>(() => CustomIDHelpers.CreateID(ComponentType.Button, name));
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

            Assert.Throws<ArgumentException>
            (
                () => CustomIDHelpers.CreateID
                (
                    ComponentType.Button,
                    name,
                    group1,
                    group2
                )
            );
        }

        /// <summary>
        /// Tests whether the simple method throws if the provided name contains whitespace.
        /// </summary>
        [Fact]
        public void SimpleThrowsIfNameContainsWhitespace()
        {
            const string name = "oo ga";
            Assert.Throws<ArgumentException>(() => CustomIDHelpers.CreateID(name, ComponentType.Button));
        }

        /// <summary>
        /// Tests whether the grouped method throws if the provided name contains whitespace.
        /// </summary>
        [Fact]
        public void GroupedThrowsIfNameContainsWhitespace()
        {
            const string name = "oo ga";
            Assert.Throws<ArgumentException>(() => CustomIDHelpers.CreateID(ComponentType.Button, name));
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

            Assert.Throws<ArgumentException>
            (
                () => CustomIDHelpers.CreateID
                (
                    ComponentType.Button,
                    name,
                    group1,
                    group2
                )
            );
        }

        /// <summary>
        /// Tests whether the simple method throws if the final ID is longer than 100 characters.
        /// </summary>
        [Fact]
        public void SimpleThrowsIfNameIsTooLong()
        {
            var name = new string('a', 80);

            Assert.Throws<ArgumentException>(() => CustomIDHelpers.CreateID(name, ComponentType.Button));
        }

        /// <summary>
        /// Tests whether the group-aware method throws if the final ID is longer than 100 characters.
        /// </summary>
        [Fact]
        public void GroupedThrowsIfNameIsTooLong()
        {
            var name = new string('a', 80);

            Assert.Throws<ArgumentException>(() => CustomIDHelpers.CreateID(ComponentType.Button, name));
        }

        /// <summary>
        /// Tests whether the group-aware method throws if the final ID is longer than 100 characters.
        /// </summary>
        [Fact]
        public void GroupedThrowsIfArgumentCombinationIsTooLong()
        {
            var name = new string('a', 40);
            var group1 = new string('b', 40);

            Assert.Throws<ArgumentException>(() => CustomIDHelpers.CreateID(ComponentType.Button, name, group1));
        }
    }
}
