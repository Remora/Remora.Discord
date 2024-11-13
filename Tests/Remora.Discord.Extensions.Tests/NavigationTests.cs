//
//  NavigationTests.cs
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

using Remora.Discord.Extensions.Formatting;
using Xunit;

namespace Remora.Discord.Extensions.Tests;

/// <summary>
/// Tests the <see cref="Navigation"/> class.
/// </summary>
public class NavigationTests
{
    /// <summary>
    /// Tests whether the <see cref="Navigation.Guild"/> method produces correct results.
    /// </summary>
    /// <param name="target">The navigation target.</param>
    /// <param name="expected">The expected navigation string.</param>
    [Theory]
    [InlineData(GuildNavigationType.Customize, "<id:customize>")]
    [InlineData(GuildNavigationType.Browse, "<id:browse>")]
    [InlineData(GuildNavigationType.Guide, "<id:guide>")]
    public void FormatsGuildNavigationCorrectly(GuildNavigationType target, string expected)
    {
        var actual = Navigation.Guild(target);
        Assert.Equal(expected, actual);
    }
}
