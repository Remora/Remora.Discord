//
//  IAutoModerationTriggerMetadata.cs
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

using System.Collections.Generic;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Objects.AutoModeration;

/// <summary>
/// Additional data used to determine whether a rule should be triggered. Different fields are relevant based
/// on the value of <see cref="AutoModerationTriggerType"/>.
/// </summary>
public interface IAutoModerationTriggerMetadata
{
    /// <summary>
    /// Gets substrings which will be searched for in content.
    /// </summary>
    /// <remarks>
    /// This property's associated trigger type is <see cref="AutoModerationTriggerType.Keyword"/>.
    /// </remarks>
    Optional<IReadOnlyList<string>> KeywordFilter { get; }

    /// <summary>
    /// Gets the internally pre-defined wordsets which will be searched for in content.
    /// </summary>
    /// /// <remarks>
    /// This property's associated trigger type is <see cref="AutoModerationTriggerType.KeywordPreset"/>.
    /// </remarks>
    Optional<IReadOnlyList<AutoModerationKeywordPresetType>> Presets { get; }
}
