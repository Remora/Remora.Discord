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

            return base.ResolveContractConverter(objectType);
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
                var optionalType = typeof(OptionalConverter<>).MakeGenericType(typeInfo.GenericTypeParameters);
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
    }
}
