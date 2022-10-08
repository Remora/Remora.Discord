//
//  IAuditLogChange.cs
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

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents a change to an audit log value.
/// </summary>
[PublicAPI]
public interface IAuditLogChange
{
    /// <summary>
    /// Gets the new value of the key.
    /// </summary>
    /// <remarks>
    /// This is the raw unparsed value from Discord's payload. Use the additional information available in the audit log
    /// entry to determine an appropriate type to parse it into.
    /// </remarks>
    Optional<string> NewValue { get; }

    /// <summary>
    /// Gets the old value of the key.
    /// </summary>
    /// <remarks>
    /// This is the raw unparsed value from Discord's payload. Use the additional information available in the audit log
    /// entry to determine an appropriate type to parse it into.
    /// </remarks>
    Optional<string> OldValue { get; }

    /// <summary>
    /// Gets the name of the audit log change key.
    /// </summary>
    string Key { get; }
}
