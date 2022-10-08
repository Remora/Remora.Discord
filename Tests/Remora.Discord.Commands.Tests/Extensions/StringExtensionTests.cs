//
//  StringExtensionTests.cs
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

using Remora.Discord.Commands.Extensions;
using Xunit;

namespace Remora.Discord.Commands.Tests.Extensions;

/// <summary>
/// Tests the <see cref="StringExtensions"/> class.
/// </summary>
public static class StringExtensionTests
{
    /// <summary>
    /// Tests the <see cref="StringExtensions.Unmention"/> method.
    /// </summary>
    public class Unmention
    {
        /// <summary>
        /// Tests whether the method correctly unwraps various mention variants.
        /// </summary>
        /// <param name="value">The variant under test.</param>
        [InlineData("<@135347310845624320>")]
        [InlineData("<@!135347310845624320>")]
        [InlineData("<#135347310845624320>")]
        [InlineData("<@&135347310845624320>")]
        [Theory]
        public void UnmentionsValueCorrectly(string value)
        {
            Assert.Equal("135347310845624320", value.Unmention());
        }
    }
}
