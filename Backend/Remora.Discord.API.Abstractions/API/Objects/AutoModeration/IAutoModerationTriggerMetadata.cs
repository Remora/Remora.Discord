//
//  IAutoModerationTriggerMetadata.cs
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
/// Additional data used to determine whether a rule should be triggered. Different fields are relevant based
/// on the value of <see cref="AutoModerationTriggerType"/>.
/// </summary>
[PublicAPI]
public interface IAutoModerationTriggerMetadata
{
    /// <summary>
    /// Gets substrings which will be searched for in content.
    /// </summary>
    /// <remarks>
    /// This property's associated trigger type is <see cref="AutoModerationTriggerType.Keyword"/>.
    /// Max 1000 elements.
    /// </remarks>
    Optional<IReadOnlyList<string>> KeywordFilter { get; }

    /// <summary>
    /// Gets regular expression patterns which will be matched against the content.
    /// </summary>
    /// <remarks>
    /// This property's associated trigger type is <see cref="AutoModerationTriggerType.Keyword"/>.
    /// Only Rust-flavoured regex is supported.
    /// Max 10 elements.
    /// </remarks>
    Optional<IReadOnlyList<string>> RegexPatterns { get; }

    /// <summary>
    /// Gets the internally pre-defined word sets which will be searched for in content.
    /// </summary>
    /// <remarks>
    /// This property's associated trigger type is <see cref="AutoModerationTriggerType.KeywordPreset"/>.
    /// </remarks>
    Optional<IReadOnlyList<AutoModerationKeywordPresetType>> Presets { get; }

    /// <summary>
    /// Gets substrings which will be exempt from triggering the preset trigger type.
    /// </summary>
    /// <remarks>
    /// This property's associated trigger type is <see cref="AutoModerationTriggerType.KeywordPreset"/>.
    /// Max 1000 elements.
    /// </remarks>
    Optional<IReadOnlyList<string>> AllowList { get; }

    /// <summary>
    /// Gets the total number of unique allowed user and role mentions per message.
    /// </summary>
    /// <remarks>
    /// This property's associated trigger type is <see cref="AutoModerationTriggerType.MentionSpam"/>.
    /// Max 50.
    /// </remarks>
    Optional<byte> MentionTotalLimit { get; }
}
