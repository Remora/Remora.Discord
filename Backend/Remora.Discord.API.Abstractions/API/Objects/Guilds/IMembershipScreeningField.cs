//
//  IMembershipScreeningField.cs
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

using System.Collections.Generic;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents the public API of a field in a membership screening form.
    /// </summary>
    public interface IMembershipScreeningField
    {
        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        MembershipScreeningFieldType FieldType { get; }

        /// <summary>
        /// Gets the title of the field.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the values associated with the field.
        /// </summary>
        Optional<IReadOnlyList<string>> Values { get; }

        /// <summary>
        /// Gets a value indicating whether the field is required.
        /// </summary>
        bool IsRequired { get; }
    }
}
