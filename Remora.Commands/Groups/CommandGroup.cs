//
//  CommandGroup.cs
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

using System.Threading;
using JetBrains.Annotations;

namespace Remora.Commands.Groups
{
    /// <summary>
    /// Represents an abstract base for command groups.
    /// </summary>
    [PublicAPI]
    public abstract class CommandGroup
    {
        /// <summary>
        /// Gets the cancellation token for the command execution operation.
        /// </summary>
        protected CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// Sets the cancellation token of the module.
        /// </summary>
        /// <param name="ct">The token.</param>
        internal void SetCancellationToken(CancellationToken ct)
        {
            this.CancellationToken = ct;
        }
    }
}
