//
//  ParameterShapeExtensions.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using JetBrains.Annotations;
using Remora.Commands.Extensions;
using Remora.Commands.Signatures;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Rest.Extensions;
using static Remora.Discord.API.Abstractions.Objects.ApplicationCommandOptionType;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IParameterShape"/> interface.
/// </summary>
[PublicAPI]
public static class ParameterShapeExtensions
{
    /// <summary>
    /// Gets the actual underlying type of the parameter, unwrapping things like nullables and optionals.
    /// </summary>
    /// <param name="shape">The parameter shape.</param>
    /// <param name="unwrapCollections">Whether to unwrap collections as well.</param>
    /// <returns>The actual type.</returns>
    public static Type GetActualParameterType(this IParameterShape shape, bool unwrapCollections = false)
    {
        var parameterType = shape.Parameter.ParameterType;

        // Unwrap the parameter type if it's a Nullable<T> or Optional<T>
        // TODO: Maybe more cases?
        parameterType = parameterType.IsNullable() || parameterType.IsOptional()
            ? parameterType.GetGenericArguments().Single()
            : parameterType;

        // IsCollection loves to inexplicably return false for IReadOnlyList<T> and friends, so we'll just do it manually
        if (!unwrapCollections || parameterType == typeof(string))
        {
            return parameterType;
        }

        var interfaces = parameterType.GetInterfaces();
        var collectionTypes = interfaces
                              .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                              .ToList();

        return collectionTypes.Count switch
        {
            0 => parameterType,
            1 => collectionTypes[0].GetGenericArguments()[0],
            _ => throw new InvalidOperationException($"{parameterType.Name} has multiple implementations for IEnumerable<>, which is ambiguous.")
        };
    }

    /// <summary>
    /// Gets the application option type the parameter's type maps to.
    /// </summary>
    /// <param name="shape">The parameter shape.</param>
    /// <returns>The option type.</returns>
    public static ApplicationCommandOptionType GetDiscordType(this IParameterShape shape)
    {
        var typeHint = shape.Parameter.GetCustomAttribute<DiscordTypeHintAttribute>();
        if (typeHint is not null)
        {
            return (ApplicationCommandOptionType)typeHint.TypeHint;
        }

        return shape.GetActualParameterType(true) switch
        {
            var t when t == typeof(bool) => ApplicationCommandOptionType.Boolean,
            var t when typeof(IPartialRole).IsAssignableFrom(t) => Role,
            var t when typeof(IPartialUser).IsAssignableFrom(t) => User,
            var t when typeof(IPartialGuildMember).IsAssignableFrom(t) => User,
            var t when typeof(IPartialChannel).IsAssignableFrom(t) => Channel,
            var t when t.IsInteger() => Integer,
            var t when t.IsFloatingPoint() => Number,
            var t when typeof(IPartialAttachment).IsAssignableFrom(t) => Attachment,
            _ => ApplicationCommandOptionType.String
        };
    }
}
