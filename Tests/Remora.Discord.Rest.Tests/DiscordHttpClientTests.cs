//
//  DiscordHttpClientTests.cs
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

using System.Net.Http;
using Remora.Discord.Rest.Tests.TestBases;

namespace Remora.Discord.Rest.Tests
{
    /// <summary>
    /// Tests the <see cref="DiscordHttpClient"/> class.
    /// </summary>
    public class DiscordHttpClientTests
    {
        /// <summary>
        /// Tests the <see cref="HttpMethod.Get"/> method.
        /// </summary>
        public class Get : DiscordHttpClientRequestTestBase
        {
            /// <inheritdoc />
            protected override HttpMethod RequestMethod => HttpMethod.Get;
        }

        /// <summary>
        /// Tests the <see cref="HttpMethod.Post"/> method.
        /// </summary>
        public class Post : DiscordHttpClientRequestTestBase
        {
            /// <inheritdoc />
            protected override HttpMethod RequestMethod => HttpMethod.Post;
        }

        /// <summary>
        /// Tests the <see cref="HttpMethod.Patch"/> method.
        /// </summary>
        public class Patch : DiscordHttpClientRequestTestBase
        {
            /// <inheritdoc />
            protected override HttpMethod RequestMethod => HttpMethod.Patch;
        }

        /// <summary>
        /// Tests the <see cref="HttpMethod.Delete"/> method.
        /// </summary>
        public class Delete : DiscordHttpClientRequestTestBase
        {
            /// <inheritdoc />
            protected override HttpMethod RequestMethod => HttpMethod.Delete;
        }

        /// <summary>
        /// Tests the <see cref="HttpMethod.Put"/> method.
        /// </summary>
        public class Put : DiscordHttpClientRequestTestBase
        {
            /// <inheritdoc />
            protected override HttpMethod RequestMethod => HttpMethod.Put;
        }
    }
}
