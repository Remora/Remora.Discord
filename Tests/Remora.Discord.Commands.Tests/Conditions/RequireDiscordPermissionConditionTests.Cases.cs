//
//  RequireDiscordPermissionConditionTests.Cases.cs
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

namespace Remora.Discord.Commands.Tests.Conditions;

public partial class RequireDiscordPermissionConditionTests
{
    /// <summary>
    /// Gets the test cases.
    /// </summary>
    public static IEnumerable<object[]> Cases
    {
        get
        {
            foreach (var andCase in AndCases())
            {
                yield return andCase;
            }

            foreach (var orCases in OrCases())
            {
                yield return orCases;
            }

            foreach (var xorCase in XorCases())
            {
                yield return xorCase;
            }

            foreach (var notCase in NotCases())
            {
                yield return notCase;
            }
        }
    }
}
