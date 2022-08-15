//
//  RequireBotPermissionsConditionTests.Cases.And.cs
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

using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Conditions;

namespace Remora.Discord.Commands.Tests.Conditions;

public partial class RequireBotPermissionsConditionTests
{
    /// <summary>
    /// Produces a set of test cases for the <see cref="LogicalOperator.And"/> operator.
    /// </summary>
    /// <returns>The test cases.</returns>
    private static IEnumerable<object[]> AndCases()
    {
        var a = DiscordPermission.SendMessages;
        var b = DiscordPermission.ReadMessageHistory;
        var c = DiscordPermission.EmbedLinks;

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a },
            new[] { a },
            true
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a },
            new[] { b },
            false
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a },
            new[] { a, b },
            true
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a },
            new[] { b, c },
            false
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a, b },
            new[] { a, b },
            true
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a, b },
            new[] { a, c },
            false
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a, b },
            new[] { a },
            false
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a, b },
            new[] { b },
            false
        };

        yield return new object[]
        {
            LogicalOperator.And,
            new[] { a, b },
            new[] { c },
            false
        };
    }
}
