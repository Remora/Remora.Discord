//
//  ModuleBase.cs
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

namespace Remora.Commands.Modules
{
    /// <summary>
    /// Represents an abstract base for command modules.
    /// </summary>
    [PublicAPI]
    public abstract class ModuleBase
    {
        /// <summary>
        /// Gets the cancellation token for the command execution operation.
        /// </summary>
        protected CancellationToken CancellationToken { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for the command execution operation.</param>
        protected ModuleBase(CancellationToken cancellationToken)
        {
            this.CancellationToken = cancellationToken;
        }
    }
}
