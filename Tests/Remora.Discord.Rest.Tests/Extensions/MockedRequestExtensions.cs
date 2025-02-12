//
//  MockedRequestExtensions.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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

using System.Collections.Generic;
using RichardSzalay.MockHttp;

namespace Remora.Discord.Rest.Tests.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="MockedRequest"/> class.
/// </summary>
public static class MockedRequestExtensions
{
    /// <summary>
    /// Includes requests that contain all of the specified query string values and no others.
    /// </summary>
    /// <param name="source">The source mocked request.</param>
    /// <param name="name">The query string key to match.</param>
    /// <param name="value">The query string value to match.</param>
    /// <returns>The <see cref="T:MockedRequest" /> instance.</returns>
    public static MockedRequest WithExactQueryString(this MockedRequest source, string name, string value) =>
        source.WithExactQueryString(new Dictionary<string, string> { { name, value } });
}
