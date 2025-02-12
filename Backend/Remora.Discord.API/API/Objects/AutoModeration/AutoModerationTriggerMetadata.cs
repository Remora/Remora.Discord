//
//  AutoModerationTriggerMetadata.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects;

/// <inheritdoc cref="IAutoModerationTriggerMetadata"/>
[PublicAPI]
public record AutoModerationTriggerMetadata
(
    Optional<IReadOnlyList<string>> KeywordFilter = default,
    Optional<IReadOnlyList<string>> RegexPatterns = default,
    Optional<IReadOnlyList<AutoModerationKeywordPresetType>> Presets = default,
    Optional<IReadOnlyList<string>> AllowList = default,
    Optional<byte> MentionTotalLimit = default,
    Optional<bool> MentionRaidProtectionEnabled = default
) : IAutoModerationTriggerMetadata;
