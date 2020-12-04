//
//  ParameterSyntaxExtensions.cs
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

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remora.Discord.Generators.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="ParameterSyntax"/> class.
    /// </summary>
    public static class ParameterSyntaxExtensions
    {
        /// <summary>
        /// Determines whether the parameter's type is an Optional.
        /// </summary>
        /// <param name="parameterSyntax">The parameter syntax node.</param>
        /// <returns>true if the parameter is an Optional; otherwise, false.</returns>
        public static bool IsOptional(this ParameterSyntax parameterSyntax)
        {
            return parameterSyntax.Type is not null &&
                   parameterSyntax.Type is GenericNameSyntax genericName &&
                   genericName.Identifier.ToString() == "Optional";
        }

        /// <summary>
        /// Determines whether the parameter's type is a nullable Optional.
        /// </summary>
        /// <param name="parameterSyntax">The parameter syntax node.</param>
        /// <returns>true if the parameter is a nullable Optional; otherwise, false.</returns>
        public static bool IsNullableOptional(this ParameterSyntax parameterSyntax)
        {
            return parameterSyntax.Type is not null &&
                   parameterSyntax.Type is NullableTypeSyntax nullableType &&
                   nullableType.ElementType is GenericNameSyntax nullableGenericName &&
                   nullableGenericName.Identifier.ToString() == "Optional";
        }
    }
}
