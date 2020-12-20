//
//  DelimitedListConverter.cs
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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts lists delimited by strings to and from JSON.
    /// </summary>
    /// <typeparam name="TElement">The element type.</typeparam>
    public class DelimitedListConverter<TElement> : JsonConverter<IReadOnlyList<TElement>>
    {
        private readonly string _separator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedListConverter{TElement}"/> class.
        /// </summary>
        /// <param name="separator">The string separator.</param>
        public DelimitedListConverter(string separator)
        {
            _separator = separator;
        }

        /// <inheritdoc />
        public override IReadOnlyList<TElement> Read
        (
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var value = reader.GetString();
            if (value is null)
            {
                throw new JsonException();
            }

            var elements = value.Split(_separator);

            // special case: straight strings
            if (typeof(TElement) == typeof(string))
            {
                return elements.Select(e => e).Cast<TElement>().ToList();
            }

            return elements.Select
            (
                e =>
                {
                    var element = JsonSerializer.Deserialize(e, typeof(TElement), options);
                    if (element is null)
                    {
                        throw new JsonException();
                    }

                    return (TElement)element;
                }
            ).ToList();
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IReadOnlyList<TElement> value, JsonSerializerOptions options)
        {
            var completeValue = string.Join
            (
                _separator,
                value.Select
                (
                    v =>
                    {
                        if (v is null)
                        {
                            return "null";
                        }

                        return v.ToString();
                    }
                )
            );

            writer.WriteStringValue(completeValue);
        }
    }
}
