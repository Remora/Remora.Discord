//
//  MultipartJsonPayloadRequestMatcher.cs
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

using System.Linq;
using System.Net.Http;
using System.Text.Json;
using RichardSzalay.MockHttp;

namespace Remora.Discord.Rest.Tests.Json
{
    /// <inheritdoc />
    public class MultipartJsonPayloadRequestMatcher : IMockedRequestMatcher
    {
        private readonly JsonElementMatcher _elementMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartJsonPayloadRequestMatcher"/> class.
        /// </summary>
        /// <param name="elementMatcher">The underlying element matcher.</param>
        public MultipartJsonPayloadRequestMatcher(JsonElementMatcher elementMatcher)
        {
            _elementMatcher = elementMatcher;
        }

        /// <inheritdoc />
        public bool Matches(HttpRequestMessage message)
        {
            if (message.Content is null)
            {
                return false;
            }

            if (message.Content is not MultipartFormDataContent multipart)
            {
                return false;
            }

            var payloadContent = multipart.FirstOrDefault
            (
                c => c is StringContent s && s.Headers.ContentDisposition?.Name == "payload_json"
            );

            if (payloadContent is null)
            {
                return false;
            }

            var content = payloadContent.ReadAsStreamAsync().GetAwaiter().GetResult();
            using var json = JsonDocument.Parse(content);

            return _elementMatcher.Matches(json.RootElement);
        }
    }
}
