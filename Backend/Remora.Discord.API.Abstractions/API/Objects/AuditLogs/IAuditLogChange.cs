//
//  IAuditLogChange.cs
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

using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions
{
    /// <summary>
    /// Represents a change to an audit log value.
    /// </summary>
    [PublicAPI]
    public interface IAuditLogChange
    {
        /// <summary>
        /// Gets the new value of the key.
        /// <remarks>The type of this value (if present) is only known at runtime by inspecting the <see cref="Key"/>
        /// property and matching it with the appropriate audit log change key.</remarks>
        /// </summary>
        Optional<object> NewValue { get; }

        /// <summary>
        /// Gets the old value of the key.
        /// </summary>
        Optional<object> OldValue { get; }

        /// <summary>
        /// Gets the name of the audit log change key.
        /// </summary>
        string Key { get; }
    }
}
