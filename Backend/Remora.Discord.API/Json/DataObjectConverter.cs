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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Extensions;
using Remora.Discord.Core;

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
        private delegate object? ObjectJsonConverterRead
        (
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions option
        );

        private delegate void ObjectJsonConverterWrite
        (
            Utf8JsonWriter writer,
            object? value,
            JsonSerializerOptions options
        );

        [return: MaybeNull]
        private delegate TType JsonConverterRead<out TType>
        (
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions option
        );

        private delegate void JsonConverterWrite<in TType>
        (
            Utf8JsonWriter writer,
            [AllowNull] TType value,
            JsonSerializerOptions options
        );

        private readonly ConstructorInfo _dtoConstructor;
        private readonly IReadOnlyList<PropertyInfo> _dtoProperties;

        private readonly Dictionary<PropertyInfo, string> _nameOverrides;
        private readonly Dictionary
        <
            PropertyInfo,
            (JsonConverter Converter, ObjectJsonConverterRead Read, ObjectJsonConverterWrite Write)
        > _converterOverrides;

        /// <summary>
        /// Holds a value indicating whether extra undefined properties should be allowed.
        /// </summary>
        private bool _allowExtraProperties = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectConverter{TInterface, TImplementation}"/> class.
        /// </summary>
        public DataObjectConverter()
        {
            _nameOverrides = new Dictionary<PropertyInfo, string>();
            _converterOverrides = new Dictionary
            <
                PropertyInfo,
                (JsonConverter Converter, ObjectJsonConverterRead Read, ObjectJsonConverterWrite Write)
            >();

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

        /// <summary>
        /// Sets whether extra JSON properties without a matching DTO property are allowed. Such properties are, if
        /// allowed, ignored. Otherwise, they throw a <see cref="JsonException"/>.
        ///
        /// By default, this is true.
        /// </summary>
        /// <param name="allowExtraProperties">Whether to allow extra properties.</param>
        /// <returns>The converter, with the new setting.</returns>
        public DataObjectConverter<TInterface, TImplementation> AllowExtraProperties(bool allowExtraProperties = true)
        {
            _allowExtraProperties = allowExtraProperties;
            return this;
        }

        /// <summary>
        /// Overrides the name of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="name">The new name.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyName<TProperty>
        (
            Expression<Func<TInterface, TProperty>> propertyExpression,
            string name
        )
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new InvalidOperationException();
            }

            var member = memberExpression.Member;
            if (!(member is PropertyInfo property))
            {
                throw new InvalidOperationException();
            }

            _nameOverrides.Add(property, name);
            return this;
        }

        /// <summary>
        /// Overrides the converter of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="converter">The JSON converter.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
        (
            Expression<Func<TInterface, TProperty>> propertyExpression,
            JsonConverter<TProperty> converter
        )
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new InvalidOperationException();
            }

            var member = memberExpression.Member;
            if (!(member is PropertyInfo property))
            {
                throw new InvalidOperationException();
            }

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
                (writer, value, options) => writeDelegate(writer, (TProperty)value, options)
            );

            _converterOverrides.Add(property, (converter, readObjectDelegate, writeObjectDelegate));
            return this;
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

            var readProperties = new Dictionary<PropertyInfo, object?>();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                var dtoProperty = _dtoProperties.FirstOrDefault
                (
                    p => GetJsonPropertyName(p, options) == propertyName
                );

                if (dtoProperty is null)
                {
                    if (!_allowExtraProperties)
                    {
                        throw new JsonException();
                    }

                    // No matching property - we'll skip it
                    if (!reader.Read())
                    {
                        throw new JsonException
                        (
                            $"No matching DTO property for JSON property \"{propertyName}\" could be found."
                        );
                    }

                    continue;
                }

                var propertyType = dtoProperty.PropertyType;

                object? propertyValue;
                if
                (
                    _converterOverrides.TryGetValue(dtoProperty, out var tuple) &&
                    tuple.Converter.CanConvert(propertyType)
                )
                {
                    propertyValue = tuple.Read(ref reader, propertyType, options);
                }
                else
                {
                    propertyValue = JsonSerializer.Deserialize(ref reader, propertyType, options);
                }

                // Verify nullability
                if (!propertyType.AllowsNull() && propertyValue is null)
                {
                    throw new JsonException();
                }

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
            writer.WriteStartObject();

            foreach (var dtoProperty in _dtoProperties)
            {
                var propertyGetter = dtoProperty.GetGetMethod();
                if (propertyGetter is null)
                {
                    continue;
                }

                var propertyValue = propertyGetter.Invoke(value, new object?[] { });

                if (propertyValue is IOptional optional && !optional.HasValue)
                {
                    continue;
                }

                var jsonName = GetJsonPropertyName(dtoProperty, options);
                writer.WritePropertyName(jsonName);

                var propertyType = dtoProperty.PropertyType;
                if
                (
                    _converterOverrides.TryGetValue(dtoProperty, out var tuple) &&
                    tuple.Converter.CanConvert(propertyType)
                )
                {
                    tuple.Write(writer, propertyValue, options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, propertyValue, options);
                }
            }

            writer.WriteEndObject();
        }

        private string GetJsonPropertyName(PropertyInfo dtoProperty, JsonSerializerOptions options)
        {
            if (_nameOverrides.TryGetValue(dtoProperty, out var overriddenName))
            {
                return overriddenName;
            }

            return options.PropertyNamingPolicy?.ConvertName(dtoProperty.Name) ?? dtoProperty.Name;
        }
    }
}
