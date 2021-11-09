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
using System.Numerics;
using System.Reflection;
using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.API.Extensions;

/// <summary>
/// Defines extension methods to the <see cref="Type"/> class.
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the given type is a collection.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is a collection; otherwise, false.</returns>
    public static bool IsCollection(this Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        switch (type.GetGenericTypeDefinition())
        {
            case var _ when type == typeof(IReadOnlyList<>):
            case var _ when type == typeof(IReadOnlyCollection<>):
            case var _ when type == typeof(IList<>):
            case var _ when type == typeof(ICollection<>):
            case var _ when type == typeof(List<>):
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Determines whether the given type is unsigned.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is unsigned; otherwise, false.</returns>
    public static bool IsUnsigned(this Type type)
    {
        switch (type)
        {
            case var _ when type == typeof(sbyte):
            case var _ when type == typeof(short):
            case var _ when type == typeof(int):
            case var _ when type == typeof(long):
            case var _ when type == typeof(float):
            case var _ when type == typeof(double):
            case var _ when type == typeof(decimal):
            case var _ when type == typeof(BigInteger):
            {
                return false;
            }
            case var _ when type == typeof(byte):
            case var _ when type == typeof(ushort):
            case var _ when type == typeof(uint):
            case var _ when type == typeof(ulong):
            {
                return true;
            }
        }

        throw new InvalidOperationException($"{nameof(type)} was not a numeric type.");
    }

    /// <summary>
    /// Checks whether the type is one of C#'s builtin types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is a builtin type; otherwise, false.</returns>
    public static bool IsBuiltin(this Type type)
    {
        switch (type)
        {
            case var _ when type == typeof(sbyte):
            case var _ when type == typeof(short):
            case var _ when type == typeof(int):
            case var _ when type == typeof(long):
            case var _ when type == typeof(float):
            case var _ when type == typeof(double):
            case var _ when type == typeof(decimal):
            case var _ when type == typeof(byte):
            case var _ when type == typeof(ushort):
            case var _ when type == typeof(uint):
            case var _ when type == typeof(ulong):
            case var _ when type == typeof(string):
            case var _ when type == typeof(bool):
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Checks whether the type is one of C#'s builtin numeric types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is a builtin numeric type; otherwise, false.</returns>
    public static bool IsNumeric(this Type type)
    {
        switch (type)
        {
            case var _ when type == typeof(sbyte):
            case var _ when type == typeof(short):
            case var _ when type == typeof(int):
            case var _ when type == typeof(long):
            case var _ when type == typeof(float):
            case var _ when type == typeof(double):
            case var _ when type == typeof(decimal):
            case var _ when type == typeof(byte):
            case var _ when type == typeof(ushort):
            case var _ when type == typeof(uint):
            case var _ when type == typeof(ulong):
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Checks whether the type is one of C#'s builtin integer types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is a builtin integer type; otherwise, false.</returns>
    public static bool IsInteger(this Type type)
    {
        switch (type)
        {
            case var _ when type == typeof(sbyte):
            case var _ when type == typeof(short):
            case var _ when type == typeof(int):
            case var _ when type == typeof(long):
            case var _ when type == typeof(byte):
            case var _ when type == typeof(ushort):
            case var _ when type == typeof(uint):
            case var _ when type == typeof(ulong):
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Checks whether the type is one of C#'s builtin floating-point types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>true if the type is a builtin floating-point type; otherwise, false.</returns>
    public static bool IsFloatingPoint(this Type type)
    {
        switch (type)
        {
            case var _ when type == typeof(float):
            case var _ when type == typeof(double):
            case var _ when type == typeof(decimal):
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

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
    /// Retrieves the innermost type from a type wrapped by
    /// <see cref="Nullable{T}"/> or <see cref="Optional{TValue}"/>.
    /// </summary>
    /// <param name="type">The type to unwrap.</param>
    /// <returns>The unwrapped type.</returns>
    public static Type Unwrap(this Type type)
    {
        var currentType = type;
        while (currentType.IsGenericType)
        {
            if (currentType.IsOptional() || currentType.IsNullable())
            {
                currentType = currentType.GetGenericArguments()[0];
                continue;
            }

            break;
        }

        return currentType;
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
                if (property.DeclaringType != type && property.DeclaringType is not null)
                {
                    // this is an inherited property, so we'll return the declaring class type's version of it
                    yield return property.DeclaringType.GetProperty(property.Name) ?? throw new MissingMemberException();
                    continue;
                }

                yield return property;
            }

            yield break;
        }

        foreach (var implementedInterface in type.GetInterfaces().Concat(new[] { type }))
        {
            foreach (var property in implementedInterface.GetProperties())
            {
                yield return property;
            }
        }
    }
}
