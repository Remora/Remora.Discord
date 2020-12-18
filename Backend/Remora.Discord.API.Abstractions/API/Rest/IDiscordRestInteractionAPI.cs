//
//  IDiscordRestInteractionAPI.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Results;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Rest
{
    /// <summary>
    /// Represents the Discord interaction API.
    /// </summary>
    [PublicAPI]
    public interface IDiscordRestInteractionAPI
    {
        /// <summary>
        /// TODO: Check if we actually get an IInteractionResponse back from this.
        /// Creates a response to an interaction from the gateway.
        /// </summary>
        /// <param name="interactionID">The ID of the interaction.</param>
        /// <param name="interactionToken">The interaction token.</param>
        /// <param name="response">The response.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        Task<ICreateRestEntityResult<IInteractionResponse>> CreateInteractionResponseAsync
        (
            Snowflake interactionID,
            string interactionToken,
            IInteractionResponse response,
            CancellationToken ct = default
        );
    }
}
