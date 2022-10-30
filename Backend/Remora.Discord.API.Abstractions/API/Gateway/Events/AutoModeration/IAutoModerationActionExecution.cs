//
//  IAutoModerationActionExecution.cs
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

using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.API.Abstractions.Gateway.Events;

/// <summary>
/// Represents the execution of an auto moderation action.
/// </summary>
public interface IAutoModerationActionExecution : IGatewayEvent
{
    /// <summary>
    /// Gets the ID of the guild in which action was executed.
    /// </summary>
    Snowflake GuildID { get; }

    /// <summary>
    /// Gets the action which was executed.
    /// </summary>
    IAutoModerationAction Action { get; }

    /// <summary>
    /// Gets the ID of the rule which action belongs to.
    /// </summary>
    Snowflake RuleID { get; }

    /// <summary>
    /// Gets the trigger type of rule which was triggered.
    /// </summary>
    AutoModerationTriggerType RuleTriggerType { get; }

    /// <summary>
    /// Gets the ID of the user which generated the content which triggered the rule.
    /// </summary>
    Snowflake UserID { get; }

    /// <summary>
    /// Gets the ID of the channel in which user content was posted.
    /// </summary>
    Optional<Snowflake> ChannelID { get; }

    /// <summary>
    /// Gets the ID of any user message which content belongs to.
    /// </summary>
    /// <remarks>
    /// This property will not exist if message was blocked by Automod or if the content was not part of any message.
    /// </remarks>
    Optional<Snowflake> MessageID { get; }

    /// <summary>
    /// Gets the ID of any system auto moderation messages posted as a result of this action.
    /// </summary>
    /// <remarks>
    /// This will not exist if this event does not correspond to an action with type <see cref="AutoModerationActionType.SendAlertMessage"/>.
    /// </remarks>
    Optional<Snowflake> AlertSystemMessageID { get; }

    /// <summary>
    /// Gets the user generated text content.
    /// </summary>
    string Content { get; }

    /// <summary>
    /// Gets the word or phrase configured in the rule that triggered the rule.
    /// </summary>
    string? MatchedKeyword { get; }

    /// <summary>
    /// Gets the substring in content that triggered the rule.
    /// </summary>
    string? MatchedContent { get; }
}
