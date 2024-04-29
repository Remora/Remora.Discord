//
//  JsonElementMatcherBuilderExtensions.cs
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

using Remora.Rest.Core;
using Remora.Rest.Xunit.Json;

namespace Remora.Discord.Rest.Tests.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="JsonElementMatcherBuilder"/> class.
/// </summary>
public static class JsonElementMatcherBuilderExtensions
{
    /// <summary>
    /// Adds a requirement that the element should be a snowflake with the given value.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the added requirement.</returns>
    public static JsonElementMatcherBuilder Is(this JsonElementMatcherBuilder builder, Snowflake value)
        => builder.Is(value.ToString());
}
