//
//  TypeExtensions.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.API.Extensions
{
    /// <summary>
    /// Defines extension methods to the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the given type is a closed <see cref="Optional{TValue}"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is a closed Optional; otherwise, false.</returns>
        public static bool IsOptional(this Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Optional<>);
        }

        /// <summary>
        /// Determines whether the given type is a closed <see cref="Nullable{TValue}"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is a closed Nullable; otherwise, false.</returns>
        public static bool IsNullable(this Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determines whether the given type allows null as a value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type allows null; otherwise, false.</returns>
        public static bool AllowsNull(this Type type)
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

        /// <summary>
        /// Gets all publicly visible properties of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The public properties.</returns>
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (!type.IsInterface)
            {
                foreach (var property in type.GetProperties())
                {
                    yield return property;
                }

                yield break;
            }

            var returnedNames = new HashSet<string>();
            foreach (var implementedInterface in type.GetInterfaces().Concat(new[] { type }))
            {
                foreach (var property in implementedInterface.GetProperties())
                {
                    if (returnedNames.Contains(property.Name))
                    {
                        continue;
                    }

                    returnedNames.Add(property.Name);
                    yield return property;
                }
            }
        }
    }
}
