//
//  SnakeCaseNamingPolicyTests.cs
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

using Remora.Discord.API.Json;
using Xunit;

namespace Remora.Discord.Tests.Tests
{
    /// <summary>
    /// Tests the <see cref="SnakeCaseNamingPolicy"/> class.
    /// </summary>
    public class SnakeCaseNamingPolicyTests
    {
        /// <summary>
        /// Tests whether the naming policy converts names correctly for a variety of cases.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData("IOStream", "io_stream")]
        [InlineData("IOStreamAPI", "io_stream_api")]
        [InlineData("already_snake", "already_snake")]
        [InlineData("SCREAMING_CASE", "screaming_case")]
        [InlineData("NormalPascalCase", "normal_pascal_case")]
        [InlineData("camelCase", "camel_case")]
        [InlineData("camelCaseAPI", "camel_case_api")]
        [InlineData("IOStreamAPIForReal", "io_stream_api_for_real")]
        [InlineData("OnceUponATime", "once_upon_a_time")]
        public void ConvertsCorrectly(string input, string expected)
        {
            var snakeCase = new SnakeCaseNamingPolicy();

            Assert.Equal(expected, snakeCase.ConvertName(input));
        }
    }
}
