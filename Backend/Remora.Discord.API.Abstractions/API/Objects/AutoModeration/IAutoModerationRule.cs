//
//  IAutoModerationRule.cs
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
/// Represents an auto-moderation rule.
/// </summary>
public interface IAutoModerationRule
{
    /// <summary>
    /// Gets the id of this rule.
    /// </summary>
    Snowflake ID { get; }

    /// <summary>
    /// Gets the guild which the rule belongs to.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the rule name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the user which first created this rule.
    /// </summary>
    Snowflake CreatorID { get; }

    /// <summary>
    /// Gets the rule <see cref="AutoModerationEventType"/>.
    /// </summary>
    AutoModerationEventType EventType { get; }

    /// <summary>
    /// Gets the rule <see cref="TriggerType"/>.
    /// </summary>
    AutoModerationTriggerType TriggerType { get; }

    /// <summary>
    /// Gets the rule <see cref="IAutoModerationTriggerMetadata"/>.
    /// </summary>
    IAutoModerationTriggerMetadata TriggerMetadata { get; }

    /// <summary>
    /// Gets the actions which will execute when the rule is triggered.
    /// </summary>
    IReadOnlyList<IAutoModerationAction> Actions { get; }

    /// <summary>
    /// Gets a value indicating whether the rule is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the role IDs that should not be affected by the rule (Maximum of 20).
    /// </summary>
    IReadOnlyList<Snowflake> ExemptRoles { get; }

    /// <summary>
    /// Gets the channel IDs that should not be affected by the rule (Maximum of 50).
    /// </summary>
    IReadOnlyList<Snowflake> ExemptChannels { get; }
}
