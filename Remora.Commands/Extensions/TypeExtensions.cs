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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Remora.Commands.Attributes;

namespace Remora.Commands.Extensions
{
    /// <summary>
    /// Defines extensions to the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Attempts to get an annotated group name from the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="groupName">The group name.</param>
        /// <returns>true if a group name was retrieved; otherwise, false.</returns>
        public static bool TryGetGroupName(this Type type, [NotNullWhen(true)] out string? groupName)
        {
            groupName = null;

            var groupNameAttribute = type.GetCustomAttribute<GroupAttribute>();
            if (groupNameAttribute is null)
            {
                return false;
            }

            groupName = groupNameAttribute.Name;
            return true;
        }

        /// <summary>
        /// Determines whether the type is an enumerable type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is an enumerable type; otherwise, false.</returns>
        public static bool IsEnumerable(this Type type)
        {
            var typeInterfaces = type.GetInterfaces();
            return typeInterfaces.Contains(typeof(IEnumerable)) ||
                   typeInterfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        /// <summary>
        /// Gets the element type of the given type. The type is assumed to implement at least one
        /// <see cref="IEnumerable{T}"/> interface, and the element type of the first one is returned.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The element type.</returns>
        public static Type GetCollectionElementType(this Type type)
        {
            var typeInterfaces = type.GetInterfaces();
            var firstEnumerableInterface = typeInterfaces.First
            (
                t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            );

            return firstEnumerableInterface.GetGenericArguments()[0];
        }
    }
}
