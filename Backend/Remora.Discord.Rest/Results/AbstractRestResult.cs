//
//  AbstractRestResult.cs
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
using System.Net;
using System.Reflection;
using JetBrains.Annotations;
using Remora.Discord.Rest.Abstractions.Results;
using Remora.Results;

namespace Remora.Discord.Rest.Results
{
    /// <summary>
    /// Represents an abstract REST API result.
    /// </summary>
    /// <typeparam name="TActualResult">The actual result type.</typeparam>
    public abstract class AbstractRestResult<TActualResult> : ResultBase<TActualResult>, IRestResult
        where TActualResult : ResultBase<TActualResult>
    {
        /// <inheritdoc />
        public HttpStatusCode? HttpError { get; }

        /// <inheritdoc />
        public DiscordError? DiscordError { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractRestResult{TResultType}"/> class.
        /// </summary>
        [PublicAPI]
        protected AbstractRestResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractRestResult{TResultType}"/> class.
        /// </summary>
        /// <param name="errorReason">A more detailed error description.</param>
        /// <param name="exception">The exception that caused the error (if any).</param>
        [PublicAPI]
        protected AbstractRestResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractRestResult{TResultType}"/> class.
        /// </summary>
        /// <param name="errorReason">A more detailed error description.</param>
        /// <param name="discordError">The Discord status code of the error (if any).</param>
        [PublicAPI]
        protected AbstractRestResult
        (
            string? errorReason,
            DiscordError? discordError = null
        )
            : base(errorReason)
        {
            this.DiscordError = discordError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractRestResult{TResultType}"/> class.
        /// </summary>
        /// <param name="errorReason">A more detailed error description.</param>
        /// <param name="httpError">The HTTP status code of the error (if any).</param>
        [PublicAPI]
        protected AbstractRestResult
        (
            string? errorReason,
            HttpStatusCode? httpError = null
        )
            : base(errorReason)
        {
            this.HttpError = httpError;
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="errorReason">A more detailed error reason.</param>
        /// <param name="discordError">The Discord error that caused the failure, if any.</param>
        /// <returns>A failed result.</returns>
        [PublicAPI, Pure]
        public static TActualResult FromError
        (
            string errorReason,
            DiscordError? discordError = null
        )
        {
            var constructor = typeof(TActualResult).GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(string), typeof(DiscordError) },
                null
            );

            if (constructor is null)
            {
                var typeName = typeof(TActualResult).Name;
                throw new MissingMethodException(typeName, $"{typeName}(string, Exception)");
            }

            var resultInstance = constructor.Invoke(new object?[] { errorReason, discordError });
            return (TActualResult)resultInstance;
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="errorReason">A more detailed error reason.</param>
        /// <param name="httpStatusCode">The HTTP error that caused the failure, if any.</param>
        /// <returns>A failed result.</returns>
        [PublicAPI, Pure]
        public static TActualResult FromError
        (
            string errorReason,
            HttpStatusCode? httpStatusCode = null
        )
        {
            var constructor = typeof(TActualResult).GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(string), typeof(HttpStatusCode) },
                null
            );

            if (constructor is null)
            {
                var typeName = typeof(TActualResult).Name;
                throw new MissingMethodException(typeName, $"{typeName}(string, Exception)");
            }

            var resultInstance = constructor.Invoke(new object?[] { errorReason, httpStatusCode });
            return (TActualResult)resultInstance;
        }
    }
}
