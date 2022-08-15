//
//  SnowflakeParserTests.cs
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

using System.Threading.Tasks;
using Remora.Discord.Commands.Parsers;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Commands.Tests.Parsers;

/// <summary>
/// Tests the <see cref="SnowflakeParser"/> class.
/// </summary>
public class SnowflakeParserTests
{
    private readonly SnowflakeParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnowflakeParserTests"/> class.
    /// </summary>
    public SnowflakeParserTests()
    {
        _parser = new SnowflakeParser();
    }

    /// <summary>
    /// Tests whether the parser returns error on invalid value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CannotParseInvalidValue()
    {
        var tryParse = await _parser.TryParseAsync("invalid");
        ResultAssert.Unsuccessful(tryParse);
    }

    /// <summary>
    /// Tests whether the parser can parse snowflake value given by number.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanParseSnowflakeByNumber()
    {
        ulong snowflakeValue = 6823586735728;
        var tryParse = await _parser.TryParseAsync(snowflakeValue.ToString());
        ResultAssert.Successful(tryParse);
        Assert.Equal(snowflakeValue, tryParse.Entity.Value);
    }

    /// <summary>
    /// Tests whether the parser can parse snowflake value given by mentions.
    /// </summary>
    /// <param name="value">Mention that should be parsed correctly.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [InlineData("<@135347310845624320>")]
    [InlineData("<@!135347310845624320>")]
    [InlineData("<#135347310845624320>")]
    [InlineData("<@&135347310845624320>")]
    [Theory]
    public async Task CanParseSnowflakeByMention(string value)
    {
        ulong snowflakeValue = 135347310845624320;
        var tryParse = await _parser.TryParseAsync(value);
        ResultAssert.Successful(tryParse);
        Assert.Equal(snowflakeValue, tryParse.Entity.Value);
    }
}
