//
//  AuditLogChange.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    [PublicAPI]
    public class AuditLogChange : IAuditLogChange
    {
        /// <inheritdoc />
        public Optional<object> NewValue { get; }

        /// <inheritdoc />
        public Optional<object> OldValue { get; }

        /// <inheritdoc/>
        public string Key { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogChange"/> class.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="key">The name of the audit log change key.</param>
        public AuditLogChange(Optional<object> newValue, Optional<object> oldValue, string key)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
            this.Key = key;
        }
    }
}
