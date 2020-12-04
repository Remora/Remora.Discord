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

namespace Remora.Discord.Generators.Support
{
    /// <summary>
    /// Defines extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the identifier of the type. This strips away special decorations from generic types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The identifier.</returns>
        public static string GetIdentifierName(this Type type)
        {
            var name = type.Name;
            if (!type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return name;
            }

            var backtickIndex = name.IndexOf('`');
            name = name.Substring(0, backtickIndex);

            return name;
        }
    }
}
