//
//  MockedRequestExtensions.cs
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

using RichardSzalay.MockHttp;

namespace Remora.Discord.Rest.Tests.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="MockHttpMessageHandler"/> class.
    /// </summary>
    public static class MockedRequestExtensions
    {
        /// <summary>
        /// Adds a requirement that the request has no content.
        /// </summary>
        /// <param name="request">The mocked request.</param>
        /// <returns>The request; with the new requirement.</returns>
        public static MockedRequest WithNoContent(this MockedRequest request)
        {
            return request.With(m => m.Content is null);
        }

        /// <summary>
        /// Adds a requirement that the request has no authorization header.
        /// </summary>
        /// <param name="request">The mocked request.</param>
        /// <returns>The request; with the new requirement.</returns>
        public static MockedRequest WithAuthentication(this MockedRequest request)
        {
            return request.With
            (
                m => m
                    .Headers.Authorization.Scheme == "Bot" &&
                     !string.IsNullOrWhiteSpace(m.Headers.Authorization.Parameter)
            );
        }
    }
}
