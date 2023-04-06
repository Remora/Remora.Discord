//
//  IOnboardingPrompt.cs
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
/// Represents a prompt shown during onboarding.
/// </summary>
[PublicAPI]
public interface IOnboardingPrompt
{
    /// <summary>
    /// Gets the ID of the prompt.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the type of the prompt.
    /// </summary>
    PromptType Type { get; }

    /// <summary>
    /// Gets a list of available options within the prompt.
    /// </summary>
    IReadOnlyList<IPromptOption> Options { get; }

    /// <summary>
    /// Gets the title of the prompt.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets a value indicating whether the users are limited to selecting one option for the prompt.
    /// </summary>
    bool IsSingleSelect { get; }

    /// <summary>
    /// Gets a value indicating whether the prompt is required before a user completes the onboarding flow.
    /// </summary>
    bool IsRequired { get; }

    /// <summary>
    /// Gets a value indicating whether the prompt is present in the onboarding flow.
    /// </summary>
    /// <remarks>If false, the prompt will only appear in the Channels &amp; Roles tab.</remarks>
    bool IsInOnboarding { get; }
}
