//
//  IAsyncTokenStore.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Remora.Discord.Rest;

/// <summary>
/// Represents a storage class for a single token.
/// </summary>
[PublicAPI]
public interface IAsyncTokenStore
{
    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The token's value.</returns>
    ValueTask<string> GetTokenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the type of the token.
    /// </summary>
    DiscordTokenType TokenType { get; }
}
