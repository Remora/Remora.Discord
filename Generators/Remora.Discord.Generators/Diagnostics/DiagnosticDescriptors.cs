//
//  DiagnosticDescriptors.cs
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

using Microsoft.CodeAnalysis;

namespace Remora.Discord.Generators
{
    /// <summary>
    /// Holds diagnostic information instances for various generators.
    /// </summary>
    public static class DiagnosticDescriptors
    {
        /// <summary>
        /// Gets an error-level diagnostic enforcing that updateable records are partial.
        /// </summary>
        public static DiagnosticDescriptor UpdateableRecordsMustBePartial { get; } = new
        (
            "RE0001",
            "Updateable records must be partial",
            "Updateable records must be partial",
            "Syntax",
            DiagnosticSeverity.Error,
            true
        );

        /// <summary>
        /// Gets an error-level diagnostic enforcing that updateable records are positional.
        /// </summary>
        public static DiagnosticDescriptor UpdateableRecordsMustBePositional { get; } = new
        (
            "SC0002",
            "Updateable records must be positional",
            "Updateable records must be positional",
            "Syntax",
            DiagnosticSeverity.Error,
            true
        );
    }
}
