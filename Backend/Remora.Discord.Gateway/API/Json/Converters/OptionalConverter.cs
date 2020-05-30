//
//  OptionalConverter.cs
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Remora.Discord.Core;

namespace Remora.Discord.Gateway.API.Json.Converters
{
    /// <summary>
    /// Converts optional fields to their JSON representation.
    /// </summary>
    /// <typeparam name="TValue">The underlying type.</typeparam>
    public class OptionalConverter<TValue> : JsonConverter<Optional<TValue>>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Optional<TValue> value, JsonSerializer serializer)
        {
            if (value.Value is null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, value.Value);
        }

        /// <inheritdoc />
        public override Optional<TValue> ReadJson
        (
            JsonReader reader,
            Type objectType,
            Optional<TValue> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        ) => new Optional<TValue>(serializer.Deserialize<TValue>(reader));
    }
}
