//
//  AutoModerationRule.cs
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
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects.AutoModeration;
using Remora.Rest.Core;

namespace Remora.Discord.API.Objects.AutoModeration;

/// <inheritdoc cref="IAutoModerationRule"/>
[PublicAPI]
public record AutoModerationRule
(
    Snowflake ID,
    Snowflake GuildID,
    string Name,
    Snowflake CreatorID,
    AutoModerationEventType EventType,
    AutoModerationTriggerType TriggerType,
    IAutoModerationTriggerMetadata TriggerMetadata,
    IReadOnlyList<IAutoModerationAction> Actions,
    bool IsEnabled,
    IReadOnlyList<Snowflake> ExemptRoles,
    IReadOnlyList<Snowflake> ExemptChannels
);