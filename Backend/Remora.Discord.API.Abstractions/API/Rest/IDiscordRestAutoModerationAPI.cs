//
//  IDiscordRestAutoModerationAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.API.Abstractions.Rest;

/// <summary>
/// Represents the Discord Auto Moderation API.
/// </summary>
[PublicAPI]
public interface IDiscordRestAutoModerationAPI
{
    /// <summary>
    /// Gets a list of all Auto Moderation rules.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IReadOnlyList<IAutoModerationRule>>> ListAutoModerationRulesAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a single Auto Moderation rule by ID.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ruleID">The ID of the rule.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<IAutoModerationRule>> GetAutoModerationRuleAsync
    (
        Snowflake guildID,
        Snowflake ruleID,
        CancellationToken ct = default
    );

    /// <summary>
    /// Creates a new Auto Moderation rule in a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="name">The name of the rule.</param>
    /// <param name="eventType">The type event on which the rule should be triggered.</param>
    /// <param name="triggerType">The type of content which can trigger the rule.</param>
    /// <param name="triggerMetadata">The associated trigger metadata.</param>
    /// <param name="actions">The actions which will execute when the rule is triggered.</param>
    /// <param name="enabled">Whether the rule is enabled.</param>
    /// <param name="exemptRoles">The role IDs that should not be affected by this rule.</param>
    /// <param name="exemptChannels">The channel IDs that should not be affected by this rule.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IAutoModerationRule>> CreateAutoModerationRuleAsync
    (
        Snowflake guildID,
        string name,
        AutoModerationEventType eventType,
        AutoModerationTriggerType triggerType,
        Optional<IAutoModerationTriggerMetadata> triggerMetadata,
        IReadOnlyList<IAutoModerationAction> actions,
        Optional<bool?> enabled = default,
        Optional<IReadOnlyList<Snowflake>?> exemptRoles = default,
        Optional<IReadOnlyList<Snowflake>?> exemptChannels = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Modifies an existing Auto Moderation rule in a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ruleID">The ID of the rule.</param>
    /// <param name="name">The name of the rule.</param>
    /// <param name="eventType">The type event on which the rule should be triggered.</param>
    /// <param name="triggerType">The type of content which can trigger the rule.</param>
    /// <param name="triggerMetadata">The associated trigger metadata.</param>
    /// <param name="actions">The actions which will execute when the rule is triggered.</param>
    /// <param name="enabled">Whether the rule is enabled.</param>
    /// <param name="exemptRoles">The role IDs that should not be affected by this rule.</param>
    /// <param name="exemptChannels">The channel IDs that should not be affected by this rule.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<IAutoModerationRule>> ModifyAutoModerationRuleAsync
    (
        Snowflake guildID,
        Snowflake ruleID,
        Optional<string> name = default,
        Optional<AutoModerationEventType> eventType = default,
        Optional<AutoModerationTriggerType> triggerType = default,
        Optional<IAutoModerationTriggerMetadata> triggerMetadata = default,
        Optional<IReadOnlyList<IAutoModerationAction>> actions = default,
        Optional<bool?> enabled = default,
        Optional<IReadOnlyList<Snowflake>?> exemptRoles = default,
        Optional<IReadOnlyList<Snowflake>?> exemptChannels = default,
        CancellationToken ct = default
    );

    /// <summary>
    /// Deletes an existing Auto Moderation rule in a guild.
    /// </summary>
    /// <param name="guildID">The ID of the guild.</param>
    /// <param name="ruleID">The ID of the rule.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteAutoModerationRuleAsync
    (
        Snowflake guildID,
        Snowflake ruleID,
        CancellationToken ct = default
    );
}
