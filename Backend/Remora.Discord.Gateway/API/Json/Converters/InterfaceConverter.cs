//
//  InterfaceConverter.cs
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

namespace Remora.Discord.Gateway.API.Json.Converters
{
    /// <summary>
    /// Defines the concrete implementation type for an interface conversion.
    /// </summary>
    /// <typeparam name="TInterface">The interface.</typeparam>
    /// <typeparam name="TConcrete">The concrete type.</typeparam>
    public class InterfaceConverter<TInterface, TConcrete> : JsonConverter
        where TConcrete : TInterface
    {
        /// <inheritdoc />
        public override void WriteJson
        (
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
        )
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, (TConcrete)value);
        }

        /// <inheritdoc />
        public override object? ReadJson
        (
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            return serializer.Deserialize<TConcrete>(reader);
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TInterface);
        }
    }
}
