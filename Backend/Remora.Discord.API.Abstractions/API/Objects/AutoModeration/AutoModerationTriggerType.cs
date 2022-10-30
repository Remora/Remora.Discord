//
//  AutoModerationTriggerType.cs
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

using JetBrains.Annotations;

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Characterizes the type of content which can trigger the rule.
/// </summary>
[PublicAPI]
public enum AutoModerationTriggerType
{
    /// <summary>
    /// Check if content contains words from a user defined list of keywords.
    /// </summary>
    Keyword = 1,

    /// <summary>
    /// Check if content contains any harmful links.
    /// </summary>
    HarmfulLink = 2,

    /// <summary>
    /// Check if content represents generic spam.
    /// </summary>
    Spam = 3,

    /// <summary>
    /// Check if content contains words from internal pre-defined wordsets.
    /// </summary>
    KeywordPreset = 4
}
