//
//  HttpResultError.cs
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

using System.Net;
using Remora.Results;

namespace Remora.Discord.Rest.Results
{
    /// <summary>
    /// Represents a HTTP error returned by an endpoint.
    /// </summary>
    public record HttpResultError : ResultError
    {
        /// <summary>
        /// Gets the status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResultError"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">The human-readable error message.</param>
        public HttpResultError(HttpStatusCode statusCode, string? message = null)
            : base(message ?? "An HTTP error occurred.")
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Creates an error from a status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>The error.</returns>
        public static implicit operator HttpResultError(HttpStatusCode statusCode) => new(statusCode);
    }
}
