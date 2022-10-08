//
//  TypeExtensionsTests.cs
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

using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Tests.Responders;
using Xunit;

namespace Remora.Discord.Gateway.Tests.Tests;

/// <summary>
/// Tests for the <see cref="TypeExtensions"/> class.
/// </summary>
public class TypeExtensionsTests
{
    /// <summary>
    /// Tests for the IResponder method in <see cref="TypeExtensions"/> class.
    /// </summary>
    public class IsResponder
    {
        /// <summary>
        /// Tests whether a type implementing IResponder returns true.
        /// </summary>
        [Fact]
        public void ReturnsTrueForTypeImplementingIResponder()
        {
            var type = typeof(MockedResponder);
            var doesImplementResponder = type.IsResponder();

            Assert.True(doesImplementResponder);
        }

        /// <summary>
        /// Tests whether a type not implementing IResponder returns true.
        /// </summary>
        [Fact]
        public void ReturnsFalseForTypeNotImplementingIResponder()
        {
            var type = typeof(string);
            var doesImplementResponder = type.IsResponder();

            Assert.False(doesImplementResponder);
        }
    }
}
