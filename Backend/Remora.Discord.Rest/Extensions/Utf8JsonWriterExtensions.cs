//
//  Utf8JsonWriterExtensions.cs
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
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Extensions;
using Remora.Discord.Core;

namespace Remora.Discord.Rest.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="Utf8JsonWriter"/> class.
    /// </summary>
    public static class Utf8JsonWriterExtensions
    {
        /// <summary>
        /// Writes the given optional to the json writer as its serialized representation. If the value is null, a null
        /// is written.
        /// </summary>
        /// <param name="json">The JSON writer.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="jsonOptions">The json options, if any.</param>
        /// <typeparam name="T">The underlying type.</typeparam>
        public static void Write<T>
        (
            this Utf8JsonWriter json,
            string name,
            in Optional<T> value,
            JsonSerializerOptions? jsonOptions = default
        )
        {
            if (!value.HasValue)
            {
                return;
            }

            if (value.Value is null)
            {
                json.WriteNull(name);
                return;
            }

            json.WritePropertyName(name);
            JsonSerializer.Serialize(json, value.Value, jsonOptions);
        }

        /// <summary>
        /// Writes the given optional to the json writer as its serialized representation. If the value is null, a null
        /// is written.
        /// </summary>
        /// <param name="json">The JSON writer.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="asInteger">Whether to serialize the value as an integer.</param>
        /// <param name="jsonOptions">The json options, if any.</param>
        /// <typeparam name="T">The underlying type.</typeparam>
        public static void WriteEnum<T>
        (
            this Utf8JsonWriter json,
            string name,
            in Optional<T?> value,
            bool asInteger = true,
            JsonSerializerOptions? jsonOptions = default
        )
            where T : struct, Enum
        {
            if (!value.HasValue)
            {
                return;
            }

            if (value.Value is null)
            {
                json.WriteNull(name);
                return;
            }

            json.WriteEnum(name, new Optional<T>(value.Value!.Value), asInteger, jsonOptions);
        }

        /// <summary>
        /// Writes the given optional to the json writer as its serialized representation. If the value is null, a null
        /// is written.
        /// </summary>
        /// <param name="json">The JSON writer.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="asInteger">Whether to serialize the value as an integer.</param>
        /// <param name="jsonOptions">The json options, if any.</param>
        /// <typeparam name="T">The underlying type.</typeparam>
        public static void WriteEnum<T>
        (
            this Utf8JsonWriter json,
            string name,
            in Optional<T> value,
            bool asInteger = true,
            JsonSerializerOptions? jsonOptions = default
        )
            where T : struct, Enum
        {
            if (!value.HasValue)
            {
                return;
            }

            if (!asInteger)
            {
                if (!(jsonOptions is null))
                {
                    var clonedOptions = jsonOptions.Clone();

                    var converter = new JsonStringEnumConverter().CreateConverter(typeof(T), clonedOptions);
                    clonedOptions.Converters.Add(converter);

                    json.WritePropertyName(name);
                    JsonSerializer.Serialize(json, value.Value, clonedOptions);
                }
                else
                {
                    json.WriteString(name, value.Value.ToString());
                }

                return;
            }

            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            if (underlyingType.IsUnsigned())
            {
                json.WriteNumber(name, Convert.ToUInt64(value.Value));
            }
            else
            {
                json.WriteNumber(name, Convert.ToInt64(value.Value));
            }
        }
    }
}
