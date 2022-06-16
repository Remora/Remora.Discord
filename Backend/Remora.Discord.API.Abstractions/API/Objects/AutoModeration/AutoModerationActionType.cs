//
//  AutoModerationActionType.cs
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

namespace Remora.Discord.API.Abstractions.Objects.AutoModeration;

/// <summary>
/// Enumerates the types of actions taken whenever a rule is triggered.
/// </summary>
public enum AutoModerationActionType
{
    /// <summary>
    /// Blocks the content of a message according to the rule.
    /// </summary>
    BlockMessage,

    /// <summary>
    /// Logs user content to a specified channel.
    /// </summary>
    SendAlertMessage,

    /// <summary>
    /// Timeout user for a specified duration.
    /// </summary>
    /// <remarks>
    /// A <see cref="Timeout"/> action can only be setup for <see cref="AutoModerationTriggerType.Keyword"/> rules and the
    /// <see cref="DiscordPermission.ModerateMembers"/> permission is required.
    /// </remarks>
    Timeout
}
