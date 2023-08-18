//
//  LazyTokenStore.cs
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
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Remora.Discord.Rest;

/// <summary>
/// Represents a storage class for a token fetched asynchronously.
/// </summary>
[PublicAPI]
public class LazyTokenStore : IAsyncTokenStore
{
    private readonly Lazy<ValueTask<string>> _token;

    /// <inheritdoc />
    public async ValueTask<string> GetTokenAsync() => await _token.Value;

    /// <inheritdoc />
    public DiscordTokenType TokenType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyTokenStore"/> class.
    /// </summary>
    /// <param name="func">A func that returns a task resolving to the token.</param>
    /// <param name="tokenType">The type of token to store.</param>
    public LazyTokenStore(Func<ValueTask<string>> func, DiscordTokenType tokenType)
    {
        _token = new(func);
        this.TokenType = tokenType;
    }
}
