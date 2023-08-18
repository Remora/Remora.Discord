//
//  StaticTokenStore.cs
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
using JetBrains.Annotations;

namespace Remora.Discord.Rest;

/// <summary>
/// Represents a storage class for a static token.
/// </summary>
[PublicAPI]
public class StaticTokenStore : IAsyncTokenStore
{
    private readonly ValueTask<string> _token;

    /// <inheritdoc />
    public ValueTask<string> GetTokenAsync() => _token;

    /// <inheritdoc />
    public DiscordTokenType TokenType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticTokenStore"/> class.
    /// </summary>
    /// <param name="token">The token to store.</param>
    /// <param name="tokenType">The type of token to store.</param>
    public StaticTokenStore(string token, DiscordTokenType tokenType)
    {
        _token = new(token);
        this.TokenType = tokenType;
    }
}
