//
//  TypeExtensions.cs
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
using System.Linq;
using JetBrains.Annotations;
using Remora.Discord.Gateway.Responders;

namespace Remora.Discord.Gateway.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="Type"/> class.
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    /// <summary>
    /// Checks if the <see cref="Type"/> implements <see cref="IResponder{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check against.</param>
    /// <returns>True if the type implements <see cref="IResponder{T}"/>.</returns>
    public static bool IsResponder(this Type type)
    {
        var interfaces = type.GetInterfaces();
        return interfaces.Any(
            i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResponder<>)
        );
    }
}
