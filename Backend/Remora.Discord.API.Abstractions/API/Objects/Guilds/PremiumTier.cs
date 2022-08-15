//
//  PremiumTier.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various premium tiers of a guild.
/// </summary>
[PublicAPI]
public enum PremiumTier
{
    /// <summary>
    /// The guild hasn't been boosted.
    /// </summary>
    None = 0,

    /// <summary>
    /// The guild is boosted to tier 1.
    /// </summary>
    Tier1 = 1,

    /// <summary>
    /// The guild is boosted to tier 2.
    /// </summary>
    Tier2 = 2,

    /// <summary>
    /// The guild is boosted to tier 3.
    /// </summary>
    Tier3 = 3,

    /// <summary>
    /// The guild is boosted to tier 4.
    /// </summary>
    Tier4 = 4
}
