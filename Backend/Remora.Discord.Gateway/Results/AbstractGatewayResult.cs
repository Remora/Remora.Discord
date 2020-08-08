//
//  AbstractGatewayResult.cs
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
using System.Reflection;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Results;

namespace Remora.Discord.Gateway.Results
{
    /// <summary>
    /// Represents the result of a Discord gateway operation.
    /// </summary>
    /// <typeparam name="TActualResult">The actual result type.</typeparam>
    public abstract class AbstractGatewayResult<TActualResult> : ResultBase<TActualResult>
        where TActualResult : AbstractGatewayResult<TActualResult>
    {
        /// <summary>
        /// Gets the gateway close code, if any.
        /// </summary>
        public GatewayCloseStatus? GatewayCloseStatus { get; }

        /// <summary>
        /// Gets the websocket close status, if any.
        /// </summary>
        public WebSocketCloseStatus? WebSocketCloseStatus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractGatewayResult{TActualResult}"/> class.
        /// </summary>
        protected AbstractGatewayResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractGatewayResult{TActualResult}"/> class.
        /// </summary>
        /// <param name="errorReason">A human-readable error description.</param>
        /// <param name="exception">The exception that caused the error.</param>
        [UsedImplicitly]
        protected AbstractGatewayResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractGatewayResult{TActualResult}"/> class.
        /// </summary>
        /// <param name="errorReason">A human-readable error description. </param>
        /// <param name="closeStatus">The close code, if any.</param>
        protected AbstractGatewayResult
        (
            string? errorReason,
            GatewayCloseStatus closeStatus
        )
            : base(errorReason)
        {
            this.GatewayCloseStatus = closeStatus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractGatewayResult{TActualResult}"/> class.
        /// </summary>
        /// <param name="errorReason">A human-readable error description. </param>
        /// <param name="closeStatus">The close code, if any.</param>
        protected AbstractGatewayResult
        (
            string? errorReason,
            WebSocketCloseStatus closeStatus
        )
            : base(errorReason)
        {
            this.WebSocketCloseStatus = closeStatus;
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="errorReason">A more detailed error reason.</param>
        /// <param name="closeStatus">The Discord error that caused the failure, if any.</param>
        /// <returns>A failed result.</returns>
        [PublicAPI, Pure]
        public static TActualResult FromError
        (
            string errorReason,
            GatewayCloseStatus closeStatus
        )
        {
            var constructor = typeof(TActualResult).GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(string), typeof(GatewayCloseStatus) },
                null
            );

            if (constructor is null)
            {
                var typeName = typeof(TActualResult).Name;
                throw new MissingMethodException(typeName, $"{typeName}(string, GatewayCloseStatus)");
            }

            var resultInstance = constructor.Invoke(new object?[] { errorReason, closeStatus });
            return (TActualResult)resultInstance;
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="errorReason">A more detailed error reason.</param>
        /// <param name="closeStatus">The Discord error that caused the failure, if any.</param>
        /// <returns>A failed result.</returns>
        [PublicAPI, Pure]
        public static TActualResult FromError
        (
            string errorReason,
            WebSocketCloseStatus closeStatus
        )
        {
            var constructor = typeof(TActualResult).GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(string), typeof(WebSocketCloseStatus) },
                null
            );

            if (constructor is null)
            {
                var typeName = typeof(TActualResult).Name;
                throw new MissingMethodException(typeName, $"{typeName}(string, WebSocketCloseStatus)");
            }

            var resultInstance = constructor.Invoke(new object?[] { errorReason, closeStatus });
            return (TActualResult)resultInstance;
        }

        /// <summary>
        /// Creates a failed result from another failed result.
        /// </summary>
        /// <param name="otherResult">The other failed result.</param>
        /// <typeparam name="TOtherResult">The other result type.</typeparam>
        /// <returns>A failed result.</returns>
        public static TActualResult FromError<TOtherResult>(AbstractGatewayResult<TOtherResult> otherResult)
            where TOtherResult : AbstractGatewayResult<TOtherResult>
        {
            if (otherResult.IsSuccess)
            {
                throw new InvalidOperationException();
            }

            if (otherResult.GatewayCloseStatus.HasValue)
            {
                return FromError(otherResult.ErrorReason, otherResult.GatewayCloseStatus.Value);
            }

            if (otherResult.WebSocketCloseStatus.HasValue)
            {
                return FromError(otherResult.ErrorReason, otherResult.WebSocketCloseStatus.Value);
            }

            return FromError((IResult)otherResult);
        }
    }
}
