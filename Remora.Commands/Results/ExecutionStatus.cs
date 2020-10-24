//
//  ExecutionStatus.cs
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

namespace Remora.Commands.Results
{
    /// <summary>
    /// Enumerates the resulting statuses an execution might have.
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// A matching command was found, and the execution was successful.
        /// </summary>
        Successful,

        /// <summary>
        /// A matching command was found, but the execution failed in user code.
        /// </summary>
        Failed,

        /// <summary>
        /// A matching command was found, but user code failed in an uncontrolled manner.
        /// </summary>
        Faulted,

        /// <summary>
        /// The matching commands were ambiguous, and no single command could be matched against.
        /// </summary>
        Ambiguous,

        /// <summary>
        /// No matching command was found.
        /// </summary>
        NotFound
    }
}
