//
//  IGuildOnboarding.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Represents the onboarding flow for a guild.
/// </summary>
[PublicAPI]
public interface IGuildOnboarding
{
    /// <summary>
    /// Gets the ID of the guild.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets a list of onboarding prompts.
    /// </summary>
    IReadOnlyList<IOnboardingPrompt> Prompts { get; }

    /// <summary>
    /// Gets a list of channel IDs that get opted into automatically.
    /// </summary>
    IReadOnlyList<Snowflake> DefaultChannelIDs { get; }

    /// <summary>
    /// Gets a value indicating whether onboarding is enabled in the guild.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the current onboarding mode.
    /// </summary>
    GuildOnboardingMode Mode { get; }
}
