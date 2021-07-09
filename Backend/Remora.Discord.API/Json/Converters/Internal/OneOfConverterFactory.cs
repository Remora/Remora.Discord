//
//  OneOfConverterFactory.cs
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
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Creates OneOf converters.
    /// </summary>
    internal class OneOfConverterFactory : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            var genericType = typeToConvert.GetGenericTypeDefinition();
            return genericType switch
            {
                var t when t == typeof(OneOf<>) => true,
                var t when t == typeof(OneOf<,>) => true,
                var t when t == typeof(OneOf<,,>) => true,
                var t when t == typeof(OneOf<,,,>) => true,
                var t when t == typeof(OneOf<,,,,>) => true,
                var t when t == typeof(OneOf<,,,,,>) => true,
                var t when t == typeof(OneOf<,,,,,,>) => true,
                var t when t == typeof(OneOf<,,,,,,,>) => true,
                var t when t == typeof(OneOf<,,,,,,,,>) => true,
                _ => false
            };
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var typeInfo = typeToConvert.GetTypeInfo();
            var optionalType = typeof(OneOfConverter<>).MakeGenericType(typeInfo);

            if (!(Activator.CreateInstance(optionalType) is JsonConverter createdConverter))
            {
                throw new JsonException();
            }

            return createdConverter;
        }
    }
}
