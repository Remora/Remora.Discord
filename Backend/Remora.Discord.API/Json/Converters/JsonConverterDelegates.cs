//
//  JsonConverterDelegates.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Represents a set of bound converter delegates.
    /// </summary>
    public class JsonConverterDelegates
    {
        /// <inheritdoc cref="JsonConverter{T}.Read"/>
        public delegate object? ObjectJsonConverterRead
        (
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions option
        );

        /// <inheritdoc cref="JsonConverter{T}.Write"/>
        public delegate void ObjectJsonConverterWrite
        (
            Utf8JsonWriter writer,
            object? value,
            JsonSerializerOptions options
        );

        /// <inheritdoc cref="JsonConverter{T}.Read"/>
        [return: MaybeNull]
        public delegate TType JsonConverterRead<out TType>
        (
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions option
        );

        /// <inheritdoc cref="JsonConverter{T}.Write"/>
        public delegate void JsonConverterWrite<in TType>
        (
            Utf8JsonWriter writer,
            [AllowNull] TType value,
            JsonSerializerOptions options
        );

        /// <summary>
        /// Gets the wrapped converter.
        /// </summary>
        public JsonConverter Converter { get; }

        /// <inheritdoc cref="JsonConverter{T}.Read"/>
        public ObjectJsonConverterRead Read { get; }

        /// <inheritdoc cref="JsonConverter{T}.Write"/>
        public ObjectJsonConverterWrite Write { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConverterDelegates"/> class.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="read">The read delegate.</param>
        /// <param name="write">The write delegate.</param>
        private JsonConverterDelegates
        (
            JsonConverter converter,
            ObjectJsonConverterRead read,
            ObjectJsonConverterWrite write
        )
        {
            this.Converter = converter;
            this.Read = read;
            this.Write = write;
        }

        /// <summary>
        /// Creates a new delegate set.
        /// </summary>
        /// <param name="converter">The converter to create delegates for.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The delegate set.</returns>
        public static JsonConverterDelegates Create<TProperty>(JsonConverter converter)
        {
            var readDelegate = (JsonConverterRead<TProperty>)Delegate.CreateDelegate
            (
                typeof(JsonConverterRead<TProperty>),
                converter,
                nameof(JsonConverter<int>.Read)
            );

            var writeDelegate = (JsonConverterWrite<TProperty>)Delegate.CreateDelegate
            (
                typeof(JsonConverterWrite<TProperty>),
                converter,
                nameof(JsonConverter<int>.Write)
            );

            var readObjectDelegate = new ObjectJsonConverterRead
            (
                (ref Utf8JsonReader reader, Type convert, JsonSerializerOptions option) => readDelegate
                (
                    ref reader,
                    convert,
                    option
                )
            );

            var writeObjectDelegate = new ObjectJsonConverterWrite
            (
                (writer, value, options) => writeDelegate(writer, value is null ? default : (TProperty)value, options)
            );

            return new JsonConverterDelegates(converter, readObjectDelegate, writeObjectDelegate);
        }
    }
}
