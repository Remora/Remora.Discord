//
//  ActivityTimestamps.cs
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
using Remora.Discord.API.Abstractions.Activities;
using Remora.Discord.Core;

namespace Remora.Discord.API.API.Objects.Activities
{
    /// <summary>
    /// Represents a set of activity timestamps - that is, when the activity started and/or stopped.
    /// </summary>
    public class ActivityTimestamps : IActivityTimestamps
    {
        /// <inheritdoc />
        public Optional<DateTimeOffset> Start { get; }

        /// <inheritdoc />
        public Optional<DateTimeOffset> End { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityTimestamps"/> class.
        /// </summary>
        /// <param name="start">The start timestamp.</param>
        /// <param name="end">The end timestamp.</param>
        public ActivityTimestamps(Optional<DateTimeOffset> start = default, Optional<DateTimeOffset> end = default)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
