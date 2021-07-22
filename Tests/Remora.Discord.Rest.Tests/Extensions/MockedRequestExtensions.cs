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

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Remora.Discord.Rest.Tests.Json;
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
        /// Adds a requirement that the request has an authorization header.
        /// </summary>
        /// <param name="request">The mocked request.</param>
        /// <returns>The request; with the new requirement.</returns>
        public static MockedRequest WithAuthentication(this MockedRequest request)
        {
            return request.With
            (
                m => m
                    .Headers.Authorization?.Scheme == "Bot" &&
                     !string.IsNullOrWhiteSpace(m.Headers.Authorization.Parameter)
            );
        }

        /// <summary>
        /// Adds a requirement that the request has a Json body.
        /// </summary>
        /// <param name="request">The mocked request.</param>
        /// <param name="elementMatcherBuilder">The additional requirements on the Json body.</param>
        /// <returns>The request; with the new requirements.</returns>
        public static MockedRequest WithJson
        (
            this MockedRequest request, Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
        )
        {
            var elementMatcher = new JsonElementMatcherBuilder();
            elementMatcherBuilder?.Invoke(elementMatcher);

            return request.With(new JsonRequestMatcher(elementMatcher.Build()));
        }

        /// <summary>
        /// Adds a requirement that the request has multipart form data with the given string field.
        /// </summary>
        /// <param name="request">The mocked request.</param>
        /// <param name="name">The name of the form field.</param>
        /// <param name="value">The value of the field.</param>
        /// <returns>The request, with the new requirements.</returns>
        public static MockedRequest WithMultipartFormData
        (
            this MockedRequest request,
            string name,
            string value
        )
        {
            return request.With
            (
                m =>
                {
                    if (m.Content is not MultipartFormDataContent formContent)
                    {
                        return false;
                    }

                    var contentWithName = formContent.FirstOrDefault
                    (
                        c => c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}")))
                    );

                    if (contentWithName is not StringContent stringContent)
                    {
                        return false;
                    }

                    using var reader = new StreamReader(stringContent.ReadAsStream());
                    var actualValue = reader.ReadToEnd();

                    return value == actualValue;
                }
            );
        }

        /// <summary>
        /// Adds a requirement that the request has multipart form data with the given file-type stream field.
        /// </summary>
        /// <param name="request">The mocked request.</param>
        /// <param name="name">The name of the form field.</param>
        /// <param name="fileName">The filename of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <returns>The request, with the new requirements.</returns>
        public static MockedRequest WithMultipartFormData
        (
            this MockedRequest request,
            string name,
            string fileName,
            Stream value
        )
        {
            return request.With
            (
                m =>
                {
                    if (m.Content is not MultipartFormDataContent formContent)
                    {
                        return false;
                    }

                    var contentWithName = formContent.FirstOrDefault
                    (
                        c =>
                            c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}"))) &&
                            c.Headers.Any(h => h.Value.Any(v => v.Contains($"filename={fileName}")))
                    );

                    if (contentWithName is not StreamContent streamContent)
                    {
                        return false;
                    }

                    // Reflection hackery
                    var innerStream = (Stream)typeof(StreamContent)
                        .GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic)!
                        .GetValue(streamContent)!;

                    return value == innerStream;
                }
            );
        }
    }
}
