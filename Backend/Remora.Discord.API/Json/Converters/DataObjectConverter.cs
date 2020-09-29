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
        private readonly ConstructorInfo _dtoConstructor;
        private readonly IReadOnlyList<PropertyInfo> _dtoProperties;

        private readonly Dictionary<PropertyInfo, string[]> _readNameOverrides;
        private readonly Dictionary<PropertyInfo, string> _writeNameOverrides;

        private readonly Dictionary<PropertyInfo, JsonConverter> _converterOverrides;
        private readonly Dictionary<PropertyInfo, JsonConverterFactory> _converterFactoryOverrides;

        /// <summary>
        /// Holds a value indicating whether extra undefined properties should be allowed.
        /// </summary>
        private bool _allowExtraProperties = true;

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(TImplementation))
            {
                return true;
            }

            return base.CanConvert(typeToConvert);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObjectConverter{TInterface, TImplementation}"/> class.
        /// </summary>
        public DataObjectConverter()
        {
            _readNameOverrides = new Dictionary<PropertyInfo, string[]>();
            _writeNameOverrides = new Dictionary<PropertyInfo, string>();

            _converterOverrides = new Dictionary<PropertyInfo, JsonConverter>();
            _converterFactoryOverrides = new Dictionary<PropertyInfo, JsonConverterFactory>();

            var implementingType = typeof(TImplementation);
            var visibleProperties = implementingType.GetPublicProperties().ToArray();

            _dtoConstructor = FindBestMatchingConstructor(visibleProperties);
            _dtoProperties = ReorderProperties(visibleProperties, _dtoConstructor);
        }

        /// <summary>
        /// Reorders the input properties based on the order and names of the parameters in the given constructor.
        /// </summary>
        /// <param name="visibleProperties">The properties.</param>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The reordered properties.</returns>
        /// <exception cref="MissingMemberException">
        /// Thrown if no match between a property and a parameter can be established.
        /// </exception>
        private IReadOnlyList<PropertyInfo> ReorderProperties
        (
            PropertyInfo[] visibleProperties,
            ConstructorInfo constructor
        )
        {
            var reorderedProperties = new List<PropertyInfo>(visibleProperties.Length);

            var constructorParameters = constructor.GetParameters();
            foreach (var constructorParameter in constructorParameters)
            {
                var matchingProperty = visibleProperties.FirstOrDefault
                (
                    p =>
                        p.Name.Equals(constructorParameter.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        p.PropertyType == constructorParameter.ParameterType
                );

                if (matchingProperty is null)
                {
                    throw new MissingMemberException(typeof(TInterface).Name, constructorParameter.Name);
                }

                reorderedProperties.Add(matchingProperty);
            }

            return reorderedProperties;
        }

        /// <summary>
        /// Finds the best matching constructor on the implementation type. A valid constructor must have a matching
        /// set of types in its parameters as the visible properties that will be considered in serialization; the order
        /// need not match.
        /// </summary>
        /// <param name="visibleProperties">The visible set of properties.</param>
        /// <returns>The constructor.</returns>
        /// <exception cref="MissingMethodException">Thrown if no appropriate constructor can be found.</exception>
        private ConstructorInfo FindBestMatchingConstructor(PropertyInfo[] visibleProperties)
        {
            var visiblePropertyTypes = visibleProperties.Select(p => p.PropertyType).ToArray();

            var implementationType = typeof(TImplementation);

            var implementationConstructors = implementationType.GetConstructors();
            if (implementationConstructors.Length == 1)
            {
                var singleCandidate = implementationConstructors[0];
                return IsMatchingConstructor(singleCandidate, visiblePropertyTypes)
                    ? singleCandidate
                    : throw new MissingMethodException
                    (
                        implementationType.Name,
                        $"ctor({string.Join(", ", visiblePropertyTypes.Select(t => t.Name))})"
                    );
            }

            var matchingConstructors = implementationType.GetConstructors()
                .Where(c => IsMatchingConstructor(c, visiblePropertyTypes)).ToList();

            if (matchingConstructors.Count == 1)
            {
                return matchingConstructors[0];
            }

            throw new MissingMethodException
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
        /// Overrides the name of the given property when serializing and deserializing JSON.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="name">The new name.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyName<TProperty>
        (
            Expression<Func<TImplementation, TProperty>> propertyExpression,
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

            _writeNameOverrides.Add(property, name );
            _readNameOverrides.Add(property, new[] { name });
            return this;
        }

        /// <summary>
        /// Overrides the name of the given property when serializing JSON.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="name">The new name.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithWritePropertyName<TProperty>
        (
            Expression<Func<TImplementation, TProperty>> propertyExpression,
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

            _writeNameOverrides.Add(property, name );
            return this;
        }

        /// <summary>
        /// Overrides the name of the given property when deserializing JSON.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="name">The new name.</param>
        /// <param name="fallbacks">The fallback names to use if the primary name isn't present.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithReadPropertyName<TProperty>
        (
            Expression<Func<TImplementation, TProperty>> propertyExpression,
            string name,
            params string[] fallbacks
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

            if (fallbacks.Length == 0)
            {
                _readNameOverrides.Add(property, new[] { name });
            }
            else
            {
                _readNameOverrides.Add(property, new[] { name }.Concat(fallbacks).ToArray());
            }

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
            Expression<Func<TImplementation, TProperty>> propertyExpression,
            JsonConverter<TProperty> converter
        ) => AddPropertyConverter(propertyExpression, converter);

        /// <summary>
        /// Overrides the converter of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="converter">The JSON converter.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
        (
            Expression<Func<TImplementation, Optional<TProperty>>> propertyExpression,
            JsonConverter<TProperty> converter
        ) => AddPropertyConverter(propertyExpression, converter);

        /// <summary>
        /// Overrides the converter of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="converter">The JSON converter.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
        (
            Expression<Func<TImplementation, TProperty?>> propertyExpression,
            JsonConverter<TProperty> converter
        )
            where TProperty : struct
            => AddPropertyConverter(propertyExpression, converter);

        /// <summary>
        /// Overrides the converter of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="converter">The JSON converter.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
        (
            Expression<Func<TImplementation, Optional<TProperty?>>> propertyExpression,
            JsonConverter<TProperty> converter
        )
            where TProperty : struct
            => AddPropertyConverter(propertyExpression, converter);

        /// <summary>
        /// Overrides the converter of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="converter">The JSON converter.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TEnumerable">The enumerable type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty, TEnumerable>
        (
            Expression<Func<TImplementation, TEnumerable>> propertyExpression,
            JsonConverter<TProperty> converter
        )
            where TProperty : struct
            where TEnumerable : IEnumerable<TProperty>
            => AddPropertyConverter(propertyExpression, converter);

        /// <summary>
        /// Overrides the converter of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="converter">The JSON converter.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <typeparam name="TEnumerable">The enumerable type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty, TEnumerable>
        (
            Expression<Func<TImplementation, Optional<TEnumerable>>> propertyExpression,
            JsonConverter<TProperty> converter
        )
            where TProperty : struct
            where TEnumerable : IEnumerable<TProperty>
            => AddPropertyConverter(propertyExpression, converter);

        /// <summary>
        /// Overrides the converter of the given property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="converterFactory">The JSON converter factory.</param>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <returns>The converter, with the property name.</returns>
        public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
        (
            Expression<Func<TImplementation, TProperty>> propertyExpression,
            JsonConverterFactory converterFactory
        ) => AddPropertyConverter(propertyExpression, converterFactory);

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

                var isPrimaryChoice = true;

                // Search for a property that has this JSON property's name as its primary option
                var dtoProperty = _dtoProperties.FirstOrDefault
                (
                    p => GetReadJsonPropertyName(p, options)[0] == propertyName
                );

                if (dtoProperty is null)
                {
                    // Allow the fallbacks to be searched as well, but mark it as just an alternative
                    dtoProperty = _dtoProperties.FirstOrDefault
                    (
                        p => GetReadJsonPropertyName(p, options).Contains(propertyName)
                    );

                    if (!(dtoProperty is null))
                    {
                        isPrimaryChoice = false;
                    }
                }

                if (dtoProperty is null)
                {
                    if (!_allowExtraProperties)
                    {
                        throw new JsonException();
                    }

                    // No matching property - we'll skip it
                    reader.Skip();
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

                var converter = GetConverter(dtoProperty, options);

                object? propertyValue;
                if (converter is null)
                {
                    propertyValue = JsonSerializer.Deserialize(ref reader, propertyType, options);
                }
                else
                {
                    // This converter should only be in effect for the duration of this property; we'll need to clone
                    // the options.
                    var clonedOptions = options.Clone();
                    clonedOptions.Converters.Add(converter);

                    propertyValue = JsonSerializer.Deserialize(ref reader, propertyType, clonedOptions);
                }

                // Verify nullability
                if (!dtoProperty.AllowsNull() && propertyValue is null)
                {
                    throw new JsonException();
                }

                if (!readProperties.ContainsKey(dtoProperty))
                {
                    readProperties.Add(dtoProperty, propertyValue);
                }
                else if (isPrimaryChoice)
                {
                    readProperties[dtoProperty] = propertyValue;
                }

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

                var jsonName = GetWriteJsonPropertyName(dtoProperty, options);
                writer.WritePropertyName(jsonName);

                var propertyType = dtoProperty.PropertyType;
                var converter = GetConverter(dtoProperty, options);
                if (converter is null)
                {
                    JsonSerializer.Serialize(writer, propertyValue, propertyType, options);
                }
                else
                {
                    // This converter should only be in effect for the duration of this property; we'll need to clone
                    // the options.
                    var clonedOptions = options.Clone();
                    clonedOptions.Converters.Add(converter);

                    JsonSerializer.Serialize(writer, propertyValue, propertyType, clonedOptions);
                }
            }

            writer.WriteEndObject();
        }

        private string[] GetReadJsonPropertyName(PropertyInfo dtoProperty, JsonSerializerOptions options)
        {
            if (_readNameOverrides.TryGetValue(dtoProperty, out var overriddenName))
            {
                return overriddenName;
            }

            return new[] { options.PropertyNamingPolicy?.ConvertName(dtoProperty.Name) ?? dtoProperty.Name };
        }

        private string GetWriteJsonPropertyName(PropertyInfo dtoProperty, JsonSerializerOptions options)
        {
            if (_writeNameOverrides.TryGetValue(dtoProperty, out var overriddenName))
            {
                return overriddenName;
            }

            return options.PropertyNamingPolicy?.ConvertName(dtoProperty.Name) ?? dtoProperty.Name;
        }

        private JsonConverter? GetConverter(PropertyInfo dtoProperty, JsonSerializerOptions options)
        {
            if (_converterOverrides.TryGetValue(dtoProperty, out var converter))
            {
                return converter;
            }

            if (!_converterFactoryOverrides.TryGetValue(dtoProperty, out var converterFactory))
            {
                return null;
            }

            var innerType = dtoProperty.PropertyType.Unwrap();

            // Special case: enums
            if (converterFactory is JsonStringEnumConverter)
            {
                while (!innerType.IsEnum)
                {
                    if (!innerType.IsGenericType)
                    {
                        throw new InvalidOperationException("The innermost type of the property isn't an enum.");
                    }

                    innerType = innerType.GetGenericArguments()[0];
                }
            }

            var createdConverter = converterFactory.CreateConverter(innerType, options);
            return createdConverter;
        }

        private DataObjectConverter<TInterface, TImplementation> AddPropertyConverter<TExpression>
        (
            Expression<TExpression> expression,
            JsonConverter converter
        )
        {
            if (!(expression.Body is MemberExpression memberExpression))
            {
                throw new InvalidOperationException();
            }

            var member = memberExpression.Member;
            if (!(member is PropertyInfo property))
            {
                throw new InvalidOperationException();
            }

            _converterOverrides.Add(property, converter);
            return this;
        }

        private DataObjectConverter<TInterface, TImplementation> AddPropertyConverter<TExpression>
        (
            Expression<TExpression> expression,
            JsonConverterFactory converterFactory
        )
        {
            if (!(expression.Body is MemberExpression memberExpression))
            {
                throw new InvalidOperationException();
            }

            var member = memberExpression.Member;
            if (!(member is PropertyInfo property))
            {
                throw new InvalidOperationException();
            }

            _converterFactoryOverrides.Add(property, converterFactory);
            return this;
        }

        private bool IsMatchingConstructor(ConstructorInfo constructor, IReadOnlyCollection<Type> visiblePropertyTypes)
        {
            if (constructor.GetParameters().Length != visiblePropertyTypes.Count)
            {
                return false;
            }

            var parameterTypeCounts = new Dictionary<Type, int>();
            foreach (var parameterType in constructor.GetParameters().Select(p => p.ParameterType))
            {
                if (parameterTypeCounts.ContainsKey(parameterType))
                {
                    parameterTypeCounts[parameterType] += 1;
                }
                else
                {
                    parameterTypeCounts.Add(parameterType, 1);
                }
            }

            var propertyTypeCounts = new Dictionary<Type, int>();
            foreach (var propertyType in visiblePropertyTypes)
            {
                if (propertyTypeCounts.ContainsKey(propertyType))
                {
                    propertyTypeCounts[propertyType] += 1;
                }
                else
                {
                    propertyTypeCounts.Add(propertyType, 1);
                }
            }

            if (parameterTypeCounts.Count != propertyTypeCounts.Count)
            {
                return false;
            }

            foreach (var (propertyType, propertyTypeCount) in propertyTypeCounts)
            {
                if (!parameterTypeCounts.TryGetValue(propertyType, out var parameterTypeCount))
                {
                    return false;
                }

                if (propertyTypeCount != parameterTypeCount)
                {
                    return false;
                }
            }

            // This constructor matches
            return true;
        }
    }
}
