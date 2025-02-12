//
//  StagePrivacyLevel.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various privacy levels for stage instances.
/// </summary>
[PublicAPI]
public enum StagePrivacyLevel
{
    /// <summary>
    /// The stage instance is visible publicly, such as in stage discovery.
    /// </summary>
    [Obsolete("Marked obsolete by Discord")]
    Public = 1,

    /// <summary>
    /// The stage instance is only visible to guild members.
    /// </summary>
    GuildOnly = 2
}
