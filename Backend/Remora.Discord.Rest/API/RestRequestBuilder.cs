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
using System.Text.Json;
using System.Web;
using Polly;

namespace Remora.Discord.Rest.API
{
    /// <summary>
    /// Represents a common way to define various parameters for a REST call.
    /// </summary>
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
        private readonly List<Action<Utf8JsonWriter>> _jsonConfigurators;

        /// <summary>
        /// Holds the configured additional headers.
        /// </summary>
        private readonly Dictionary<string, string> _additionalHeaders;

        /// <summary>
        /// Holds the additional content.
        /// </summary>
        private readonly List<HttpContent> _additionalContent;

        /// <summary>
        /// Holds the configured Http request method.
        /// </summary>
        private HttpMethod _method = HttpMethod.Get;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestRequestBuilder"/> class.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        public RestRequestBuilder(string endpoint)
        {
            _endpoint = endpoint;

            _queryParameters = new Dictionary<string, string>();
            _jsonConfigurators = new List<Action<Utf8JsonWriter>>();
            _additionalHeaders = new Dictionary<string, string>();
            _additionalContent = new List<HttpContent>();
        }

        /// <summary>
        /// Adds an additional content block to the request. This implicitly sets the content type to
        /// multipart/form-data.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <returns>The request builder, with the content.</returns>
        public RestRequestBuilder AddContent(HttpContent content)
        {
            _additionalContent.Add(content);
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
        /// Adds a JSON property to the request body.
        /// </summary>
        /// <param name="jsonConfigurator">The JSON configurator.</param>
        /// <returns>The builder, with the property added.</returns>
        public RestRequestBuilder AddJsonConfigurator(Action<Utf8JsonWriter> jsonConfigurator)
        {
            _jsonConfigurators.Add(jsonConfigurator);
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
        /// Builds the request message.
        /// </summary>
        /// <returns>The request message.</returns>
        public HttpRequestMessage Build()
        {
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            foreach (var (queryName, queryValue) in _queryParameters)
            {
                queryParameters.Add(queryName, queryValue);
            }

            var request = new HttpRequestMessage(_method, _endpoint + queryParameters);

            foreach (var (headerName, headerValue) in _additionalHeaders)
            {
                request.Headers.Add(headerName, headerValue);
            }

            StreamContent? jsonBody = null;
            if (_jsonConfigurators.Count > 0)
            {
                var jsonStream = new MemoryStream();
                var jsonWriter = new Utf8JsonWriter(jsonStream);
                foreach (var jsonConfigurator in _jsonConfigurators)
                {
                    jsonConfigurator(jsonWriter);
                }

                jsonBody = new StreamContent(jsonStream);
            }

            if (_additionalContent.Count > 0)
            {
                var multipartContent = new MultipartContent("form-data")
                {
                    jsonBody
                };

                foreach (var content in _additionalContent)
                {
                    multipartContent.Add(content);
                }

                request.Content = multipartContent;
            }
            else
            {
                request.Content = jsonBody;
            }

            var context = new Context { { "endpoint", _endpoint } };
            request.SetPolicyExecutionContext(context);

            return request;
        }
    }
}
