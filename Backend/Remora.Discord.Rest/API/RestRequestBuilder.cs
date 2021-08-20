//
//  RestRequestBuilder.cs
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
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web;
using JetBrains.Annotations;
using Polly;

namespace Remora.Discord.Rest.API
{
    /// <summary>
    /// Represents a common way to define various parameters for a REST call.
    /// </summary>
    [PublicAPI]
    public class RestRequestBuilder
    {
        /// <summary>
        /// Holds the endpoint.
        /// </summary>
        private readonly string _endpoint;

        /// <summary>
        /// Holds the configured query parameters.
        /// </summary>
        private readonly Dictionary<string, string> _queryParameters;

        /// <summary>
        /// Holds the JSON body configurators.
        /// </summary>
        private readonly List<Action<Utf8JsonWriter>> _jsonObjectConfigurators;

        /// <summary>
        /// Holds the JSON array element configurators.
        /// </summary>
        private readonly List<Action<Utf8JsonWriter>> _jsonArrayConfigurators;

        /// <summary>
        /// Holds the configured additional headers.
        /// </summary>
        private readonly Dictionary<string, string> _additionalHeaders;

        /// <summary>
        /// Holds the configured additional content headers.
        /// </summary>
        private readonly Dictionary<string, string> _additionalContentHeaders;

        /// <summary>
        /// Holds the additional content.
        /// </summary>
        private readonly Dictionary<string, (HttpContent, string?)> _additionalContent;

        /// <summary>
        /// Holds the configured Http request method.
        /// </summary>
        private HttpMethod _method = HttpMethod.Get;

        /// <summary>
        /// Holds a value indicating whether JSON object start and end markers (curly braces) should be automatically
        /// added.
        /// </summary>
        private bool _addJsonObjectStartEndMarkers = true;

        /// <summary>
        /// Holds a value indicating whether JSON array start and end markers (angle brackets) should be automatically
        /// added.
        /// </summary>
        private bool _addJsonArrayStartEndMarkers = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestRequestBuilder"/> class.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        public RestRequestBuilder(string endpoint)
        {
            _endpoint = endpoint;

            _queryParameters = new Dictionary<string, string>();
            _jsonObjectConfigurators = new List<Action<Utf8JsonWriter>>();
            _jsonArrayConfigurators = new List<Action<Utf8JsonWriter>>();
            _additionalHeaders = new Dictionary<string, string>();
            _additionalContentHeaders = new Dictionary<string, string>();
            _additionalContent = new Dictionary<string, (HttpContent, string?)>();
        }

        /// <summary>
        /// Adds an additional content block to the request. This implicitly sets the content type to
        /// multipart/form-data.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <param name="name">The name of the content.</param>
        /// <param name="fileName">The file name of the content.</param>
        /// <returns>The request builder, with the content.</returns>
        public RestRequestBuilder AddContent(HttpContent content, string name, string? fileName = default)
        {
            _additionalContent.Add(name, (content, fileName));
            return this;
        }

        /// <summary>
        /// Configures the request method to use.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The request builder, with the configured method.</returns>
        public RestRequestBuilder WithMethod(HttpMethod method)
        {
            _method = method;
            return this;
        }

        /// <summary>
        /// Adds a query parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The parameter's value.</param>
        /// <returns>The builder, with the parameter added.</returns>
        public RestRequestBuilder AddQueryParameter(string name, string value)
        {
            _queryParameters.Add(name, value);
            return this;
        }

        /// <summary>
        /// Adds an arbitrary section inside the request body. This implicitly sets the request body to be a JSON
        /// object.
        ///
        /// This method is mutually exclusive with <see cref="WithJsonArray"/>.
        /// </summary>
        /// <param name="propertyWriter">The JSON configurator.</param>
        /// <param name="withStartEndMarkers">
        /// Whether JSON object start and end markers (curly braces) should be automatically added.
        /// </param>
        /// <returns>The builder, with the property added.</returns>
        public RestRequestBuilder WithJson(Action<Utf8JsonWriter> propertyWriter, bool withStartEndMarkers = true)
        {
            if (_jsonArrayConfigurators.Count > 0)
            {
                throw new InvalidOperationException();
            }

            _jsonObjectConfigurators.Add(propertyWriter);
            _addJsonObjectStartEndMarkers = withStartEndMarkers;
            return this;
        }

        /// <summary>
        /// Adds a set of arbitrary elements to the request body. This implicitly sets the request body to be a JSON
        /// array.
        ///
        /// This method is mutually exclusive with <see cref="WithJson"/>.
        /// </summary>
        /// <param name="arrayElementWriter">The JSON configurator.</param>
        /// <param name="withStartEndMarkers">
        /// Whether JSON array start and end markers (angle brackets) should be automatically added.
        /// </param>
        /// <returns>The builder, with the property added.</returns>
        public RestRequestBuilder WithJsonArray
        (
            Action<Utf8JsonWriter> arrayElementWriter,
            bool withStartEndMarkers = true
        )
        {
            if (_jsonObjectConfigurators.Count > 0)
            {
                throw new InvalidOperationException();
            }

            _jsonArrayConfigurators.Add(arrayElementWriter);
            _addJsonArrayStartEndMarkers = withStartEndMarkers;
            return this;
        }

        /// <summary>
        /// Adds a custom header to the request.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        /// <returns>The builder, with the header added.</returns>
        public RestRequestBuilder AddHeader(string name, string value)
        {
            _additionalHeaders.Add(name, value);
            return this;
        }

        /// <summary>
        /// Adds a custom content header to the request.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        /// <returns>The builder, with the header added.</returns>
        public RestRequestBuilder AddContentHeader(string name, string value)
        {
            _additionalContentHeaders.Add(name, value);
            return this;
        }

        /// <summary>
        /// Builds the request message.
        /// </summary>
        /// <returns>The request message.</returns>
        public HttpRequestMessage Build()
        {
            // Build the query parameters, if any
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            foreach (var (queryName, queryValue) in _queryParameters)
            {
                queryParameters.Add(queryName, queryValue);
            }

            var request = new HttpRequestMessage
            (
                _method,
                _endpoint + (queryParameters.Count > 0 ? "?" + queryParameters : string.Empty)
            );

            // Add headers
            foreach (var (headerName, headerValue) in _additionalHeaders)
            {
                request.Headers.Add(headerName, headerValue);
            }

            // Build the content of the request, if any
            request.Content = BuildRequestContent();

            // Set up the endpoint context for rate limiting purposes
            var context = new Context { { "endpoint", _endpoint } };
            request.SetPolicyExecutionContext(context);

            return request;
        }

        private HttpContent? BuildRequestContent()
        {
            HttpContent? requestContent;

            var jsonBody = BuildJsonPayload();
            if (_additionalContent.Count > 0)
            {
                var multipartContent = new MultipartFormDataContent();

                if (jsonBody is not null)
                {
                    multipartContent.Add(jsonBody, "payload_json");
                }

                foreach (var (name, (content, fileName)) in _additionalContent)
                {
                    if (fileName is null)
                    {
                        multipartContent.Add(content, name);
                    }
                    else
                    {
                        multipartContent.Add(content, name, fileName);
                    }
                }

                requestContent = multipartContent;
            }
            else
            {
                if (jsonBody is null)
                {
                    // No content
                    return null;
                }

                requestContent = jsonBody;
            }

            // Add content headers
            foreach (var (headerName, headerValue) in _additionalContentHeaders)
            {
                requestContent.Headers.Add(headerName, headerValue);
            }

            return requestContent;
        }

        private StringContent? BuildJsonPayload()
        {
            StringContent? jsonBody = null;
            if (_jsonObjectConfigurators.Count > 0)
            {
                using var jsonStream = new MemoryStream();
                var jsonWriter = new Utf8JsonWriter(jsonStream);

                if (_addJsonObjectStartEndMarkers)
                {
                    jsonWriter.WriteStartObject();
                }

                foreach (var jsonConfigurator in _jsonObjectConfigurators)
                {
                    jsonConfigurator(jsonWriter);
                }

                if (_addJsonObjectStartEndMarkers)
                {
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.Flush();

                jsonStream.Seek(0, SeekOrigin.Begin);
                jsonBody = new StringContent
                (
                    new StreamReader(jsonStream).ReadToEnd(),
                    Encoding.UTF8,
                    "application/json"
                );
            }
            else if (_jsonArrayConfigurators.Count > 0)
            {
                using var jsonStream = new MemoryStream();
                var jsonWriter = new Utf8JsonWriter(jsonStream);

                if (_addJsonArrayStartEndMarkers)
                {
                    jsonWriter.WriteStartArray();
                }

                foreach (var elementConfigurator in _jsonArrayConfigurators)
                {
                    elementConfigurator(jsonWriter);
                }

                if (_addJsonArrayStartEndMarkers)
                {
                    jsonWriter.WriteEndArray();
                }

                jsonWriter.Flush();

                jsonStream.Seek(0, SeekOrigin.Begin);
                jsonBody = new StringContent
                (
                    new StreamReader(jsonStream).ReadToEnd(),
                    Encoding.UTF8,
                    "application/json"
                );
            }

            return jsonBody;
        }
    }
}
