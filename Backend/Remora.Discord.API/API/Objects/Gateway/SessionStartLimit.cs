//
//  SessionStartLimit.cs
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
using Remora.Discord.API.Abstractions.Gateway;

namespace Remora.Discord.API.Objects.Gateway
{
    /// <summary>
    /// Represents a session start limit.
    /// </summary>
    public class SessionStartLimit : ISessionStartLimit
    {
        /// <inheritdoc/>
        public int Total { get; }

        /// <inheritdoc/>
        public int Remaining { get; }

        /// <inheritdoc/>
        public TimeSpan ResetAfter { get; }

        /// <inheritdoc />
        public int MaxConcurrency { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionStartLimit"/> class.
        /// </summary>
        /// <param name="total">The total allowed session starts.</param>
        /// <param name="remaining">The remaining allowed session starts.</param>
        /// <param name="resetAfter">The time after which the limit resets.</param>
        /// <param name="maxConcurrency">The maximum degree of concurrency.</param>
        public SessionStartLimit(int total, int remaining, TimeSpan resetAfter, int maxConcurrency)
        {
            this.Total = total;
            this.Remaining = remaining;
            this.ResetAfter = resetAfter;
            this.MaxConcurrency = maxConcurrency;
        }
    }
}
