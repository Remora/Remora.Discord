//
//  OneOfParserTests.cs
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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using Remora.Commands.Extensions;
using Remora.Commands.Parsers;
using Remora.Commands.Services;
using Remora.Discord.Commands.Parsers;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Commands.Tests.Parsers;

/// <summary>
/// Tests the <see cref="OneOfParser"/> class.
/// </summary>
public class OneOfParserTests
{
    private readonly OneOfParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOfParserTests"/> class.
    /// </summary>
    public OneOfParserTests()
    {
        var services = new ServiceCollection()
            .AddSingleton<TypeParserService>()
            .AddParser<Int32Parser>()
            .AddParser<DoubleParser>()
            .AddParser<StringParser>()
            .AddParser<OneOfParser>()
            .BuildServiceProvider(true);

        _parser = (OneOfParser)services.GetServices<ITypeParser>().First(p => p is OneOfParser);
    }

    /// <summary>
    /// Tests whether the parser can parse a single-argument OneOf.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanParseSingleArgumentOneOfAsync()
    {
        var type = typeof(OneOf<string>);
        var token = "wooga";

        var tryParse = await _parser.TryParseAsync(token, type);
        ResultAssert.Successful(tryParse);
        Assert.IsType<OneOf<string>>(tryParse.Entity);

        var result = (OneOf<string>)tryParse.Entity!;
        Assert.Equal(token, result);
    }

    /// <summary>
    /// Tests whether the parser can parse a multi-argument OneOf.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanParseMultiArgumentOneOfAsync()
    {
        var type = typeof(OneOf<int, double>);
        var integerToken = "0";

        var tryParseInteger = await _parser.TryParseAsync(integerToken, type);
        ResultAssert.Successful(tryParseInteger);
        Assert.IsType<OneOf<int, double>>(tryParseInteger.Entity);

        var integerResult = (OneOf<int, double>)tryParseInteger.Entity!;
        Assert.Equal(0, integerResult);

        var doubleToken = "1.0";

        var tryParseDouble = await _parser.TryParseAsync(doubleToken, type);
        ResultAssert.Successful(tryParseDouble);
        Assert.IsType<OneOf<int, double>>(tryParseDouble.Entity);

        var doubleResult = (OneOf<int, double>)tryParseDouble.Entity!;
        Assert.Equal(1.0, doubleResult);
    }
}
