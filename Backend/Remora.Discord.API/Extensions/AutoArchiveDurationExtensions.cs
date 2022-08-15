//
//  AutoArchiveDurationExtensions.cs
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

using System;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.API.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="AutoArchiveDuration"/> enumeration.
/// </summary>
[PublicAPI]
public static class AutoArchiveDurationExtensions
{
    /// <summary>
    /// Converts the archival duration to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="duration">The archival duration.</param>
    /// <returns>The equivalent time span.</returns>
    public static TimeSpan ToTimeSpan(this AutoArchiveDuration duration)
    {
        return TimeSpan.FromMinutes((int)duration);
    }
}
