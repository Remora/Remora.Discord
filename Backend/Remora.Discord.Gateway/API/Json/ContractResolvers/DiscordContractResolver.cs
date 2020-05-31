//
//  DiscordContractResolver.cs
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
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Remora.Discord.Core;
using Remora.Discord.Gateway.API.Json.Converters;

namespace Remora.Discord.Gateway.API.Json.ContractResolvers
{
    /// <inheritdoc />
    public sealed class DiscordContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordContractResolver"/> class.
        /// </summary>
        public DiscordContractResolver()
        {
            this.NamingStrategy = new SnakeCaseNamingStrategy();
        }

        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var createdProperty = base.CreateProperty(member, memberSerialization);
            createdProperty = ConfigurePropertyRequirementContract(createdProperty, member);
            createdProperty = ConfigurePropertySerializationContract(createdProperty, member);

            return createdProperty;
        }

        /// <inheritdoc />
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            if (TryGetGenericOptionalConverter(objectType, out var optionalConverter))
            {
                return optionalConverter;
            }

            if (TryGetGenericConverter(objectType, out var genericConverter))
            {
                return genericConverter;
            }

            if (TryGetPayloadConverter(objectType, out var payloadConverter))
            {
                return payloadConverter;
            }

            return base.ResolveContractConverter(objectType);
        }

        private JsonProperty ConfigurePropertySerializationContract(JsonProperty createdProperty, MemberInfo member)
        {
            if (!(member is PropertyInfo propertyInfo))
            {
                return createdProperty;
            }

            var memberType = propertyInfo.PropertyType;
            if (!memberType.IsGenericType || memberType.GetGenericTypeDefinition() != typeof(Optional<>))
            {
                return createdProperty;
            }

            createdProperty.ShouldSerialize = o =>
            {
                var currentValue = propertyInfo.GetValue(o);
                if (currentValue is IOptional optional)
                {
                    return optional.HasValue;
                }

                // Serialize by default
                return true;
            };

            return createdProperty;
        }

        private JsonProperty ConfigurePropertyRequirementContract(JsonProperty createdProperty, MemberInfo member)
        {
            if (createdProperty.IsRequiredSpecified)
            {
                return createdProperty;
            }

            if (!(member is PropertyInfo propertyInfo))
            {
                return createdProperty;
            }

            var memberType = propertyInfo.PropertyType;
            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Optional<>))
            {
                var innerType = memberType.GenericTypeArguments[0];

                createdProperty.Required = IsTypeNullable(innerType) ? Required.Default : Required.DisallowNull;
                return createdProperty;
            }

            createdProperty.Required = IsTypeNullable(memberType) ? Required.AllowNull : Required.Always;
            return createdProperty;
        }

        private bool TryGetPayloadConverter(Type objectType, [NotNullWhen(true)] out JsonConverter? result)
        {
            result = null;

            if (objectType != typeof(Payload<>) && objectType != typeof(EventPayload<>))
            {
                return false;
            }

            result = new PayloadConverter();
            return true;
        }

        /// <summary>
        /// Attempts to get a <see cref="OptionalConverter{TValue}"/> for the given object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="result">The converter.</param>
        /// <returns>true if a converter was found; otherwise, false.</returns>
        private bool TryGetGenericOptionalConverter(Type objectType, [NotNullWhen(true)] out JsonConverter? result)
        {
            result = null;

            var typeInfo = objectType.GetTypeInfo();
            if (!typeInfo.IsGenericType || typeInfo.IsGenericTypeDefinition)
            {
                return false;
            }

            var genericType = typeInfo.GetGenericTypeDefinition();
            if (genericType != typeof(Optional<>))
            {
                return false;
            }

            try
            {
                var optionalType = typeof(OptionalConverter<>).MakeGenericType(typeInfo.GenericTypeArguments);
                var createdConverter = Activator.CreateInstance(optionalType) as JsonConverter;

                if (createdConverter is null)
                {
                    return false;
                }

                result = createdConverter;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to resolve a generic open type converter from an attribute on the type.
        /// </summary>
        /// <param name="objectType">The type.</param>
        /// <param name="result">The converter.</param>
        /// <returns>true if a converter was found; otherwise, false.</returns>
        private bool TryGetGenericConverter(Type objectType, [NotNullWhen(true)] out JsonConverter? result)
        {
            result = null;

            var typeInfo = objectType.GetTypeInfo();
            if (!typeInfo.IsGenericType || typeInfo.IsGenericTypeDefinition)
            {
                return false;
            }

            var jsonConverterAttribute = typeInfo.GetCustomAttribute<JsonConverterAttribute>();
            if (jsonConverterAttribute is null)
            {
                return false;
            }

            var converterType = jsonConverterAttribute.ConverterType;

            if (!converterType.GetTypeInfo().IsGenericTypeDefinition)
            {
                return false;
            }

            var createdInstance = Activator.CreateInstance
            (
                jsonConverterAttribute.ConverterType.MakeGenericType(typeInfo.GenericTypeArguments),
                jsonConverterAttribute.ConverterParameters
            ) as JsonConverter;

            if (createdInstance is null)
            {
                return false;
            }

            result = createdInstance;
            return true;
        }

        /// <summary>
        /// Determines whether the given type is logically nullable (that is, some variant of the T? pattern).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is nullable; otherwise, false.</returns>
        private bool IsTypeNullable(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }

            var nullableAttributeType = Type.GetType("System.Runtime.CompilerServices.NullableAttribute");
            if (nullableAttributeType is null)
            {
                // If we don't have access to nullability attributes, assume that we're not in a nullable context.
                return !type.IsValueType;
            }

            if (!(type.GetCustomAttribute(nullableAttributeType) is null))
            {
                // If there's a nullable attribute, this type is for sure nullable
                return true;
            }

            return !type.IsValueType;
        }
    }
}
