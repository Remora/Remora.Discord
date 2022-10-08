//
//  TokenStoreTests.cs
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

using Remora.Discord.Rest;
using Xunit;

// ReSharper disable SA1600
#pragma warning disable 1591, SA1600

namespace Remora.Discord.Tests.Tests.Core;

/// <summary>
/// Tests the <see cref="TokenStore"/> class.
/// </summary>
public class TokenStoreTests
{
    /// <summary>
    /// Tests the <see cref="TokenStore.Token"/> property.
    /// </summary>
    public class Token
    {
        [Fact]
        public void ReturnsCorrectValue()
        {
            var tokenStore = new TokenStore("Hello world!", DiscordTokenType.Bearer);

            Assert.Equal("Hello world!", tokenStore.Token);
        }
    }

    /// <summary>
    /// Tests the <see cref="TokenStore.TokenType"/> property.
    /// </summary>
    public class TokenType
    {
        [Fact]
        public void ReturnsCorrectValueForBotTokenType()
        {
            var tokenStore = new TokenStore("Hello world!", DiscordTokenType.Bot);

            Assert.Equal(DiscordTokenType.Bot, tokenStore.TokenType);
        }

        [Fact]
        public void ReturnsCorrectValueForBearerTokenType()
        {
            var tokenStore = new TokenStore("Hello world!", DiscordTokenType.Bearer);

            Assert.Equal(DiscordTokenType.Bearer, tokenStore.TokenType);
        }
    }
}
