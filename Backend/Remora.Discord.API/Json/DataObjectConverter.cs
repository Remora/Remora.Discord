//
//  DataObjectConverter.cs
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
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.Rest.Extensions;

namespace Remora.Discord.API.Json
{
    /// <summary>
    /// Converts to and from a gateway endpoint instance.
    /// </summary>
    /// <typeparam name="TInterface">The interface that is seen in the objects.</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation.</typeparam>
    public class DataObjectConverter<TInterface, TImplementation> : JsonConverter<TInterface>
        where TImplementation : TInterface
    {
        private readonly ConstructorInfo _dtoConstructor;
        private readonly IReadOnlyList<PropertyInfo> _dtoProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectConverter{TInterface, TImplementation}"/> class.
        /// </summary>
        public DataObjectConverter()
        {
            var visibleType = typeof(TInterface);
            var visibleProperties = visibleType.GetProperties();
            var visiblePropertyTypes = visibleProperties.Select(p => p.PropertyType).ToArray();

            var implementationType = typeof(TImplementation);
            var implementationConstructor = implementationType.GetConstructor
            (
                visiblePropertyTypes
            );

            _dtoProperties = visibleProperties;
            _dtoConstructor = implementationConstructor ?? throw new MissingMethodException
            (
                implementationType.Name,
                $"ctor({string.Join(", ", visiblePropertyTypes.Select(t => t.Name))})"
            );
        }

        /// <inheritdoc />
        public override TInterface Read
        (
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }

            var readProperties = new Dictionary<PropertyInfo, object>();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                var dtoProperty = _dtoProperties.FirstOrDefault
                (
                    p =>
                    {
                        var convertedName = options.PropertyNamingPolicy?.ConvertName(p.Name) ?? p.Name;
                        return convertedName == propertyName;
                    });

                if (dtoProperty is null)
                {
                    throw new JsonException($"No matching DTO property was found for JSON property \"{propertyName}\"");
                }

                var propertyValue = JsonSerializer.Deserialize(ref reader, dtoProperty.PropertyType, options);
                readProperties.Add(dtoProperty, propertyValue);

                if (!reader.Read())
                {
                    throw new JsonException();
                }
            }

            // Eat the end object token.
            if (!reader.IsFinalBlock && !reader.Read())
            {
                throw new JsonException();
            }

            // Reorder and polyfill the read properties
            var constructorArguments = new object?[_dtoProperties.Count];
            for (var i = 0; i < _dtoProperties.Count; i++)
            {
                var dtoProperty = _dtoProperties[i];
                if (!readProperties.TryGetValue(dtoProperty, out var propertyValue))
                {
                    if (dtoProperty.PropertyType.IsOptional())
                    {
                        propertyValue = Activator.CreateInstance(dtoProperty.PropertyType);
                    }
                    else
                    {
                        throw new InvalidOperationException
                        (
                            $"The data property \"{dtoProperty.Name}\" did not have a corresponding value in the JSON."
                        );
                    }
                }

                constructorArguments[i] = propertyValue;
            }

            return (TInterface)_dtoConstructor.Invoke(constructorArguments);
        }

        /// <inheritdoc />
        public override void Write
        (
            Utf8JsonWriter writer,
            TInterface value,
            JsonSerializerOptions options
        )
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            JsonSerializer.Serialize(writer, (TImplementation)value, options);
        }
    }
}
