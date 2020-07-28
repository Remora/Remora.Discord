//
//  GatewayReceiverResult.cs
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

using System;
using System.Net.WebSockets;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions;

namespace Remora.Discord.Gateway.Results
{
    /// <summary>
    /// Represents the result of the gateway receiver task, which handles continuous accepting of payloads from the
    /// gateway.
    /// </summary>
    public class GatewayReceiverResult : AbstractGatewayResult<GatewayReceiverResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayReceiverResult"/> class.
        /// </summary>
        private GatewayReceiverResult()
        {
        }

        /// <inheritdoc cref="GatewayReceiverResult"/>
        [UsedImplicitly]
        private GatewayReceiverResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <inheritdoc cref="GatewayConnectionResult"/>
        [UsedImplicitly]
        private GatewayReceiverResult
        (
            string? errorReason,
            GatewayCloseStatus closeStatus
        )
            : base(errorReason, closeStatus)
        {
        }

        /// <inheritdoc cref="GatewayConnectionResult"/>
        [UsedImplicitly]
        private GatewayReceiverResult
        (
            string? errorReason,
            WebSocketCloseStatus closeStatus
        )
            : base(errorReason, closeStatus)
        {
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <returns>A successful result.</returns>
        [PublicAPI, Pure]
        public static GatewayReceiverResult FromSuccess()
        {
            return new GatewayReceiverResult();
        }
    }
}
