//
//  InteractionContextType.cs
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

namespace Remora.Discord.API.Abstractions.Objects;

/// <summary>
/// Enumerates various interaction context types.
/// </summary>
public enum InteractionContextType
{
    /// <summary>
    /// The interaction was executed in the context of a guild.
    /// </summary>
    Guild = 1,

    /// <summary>
    /// The interaction was executed in the context of a direct message to the bot associated with the application.
    /// </summary>
    BotDM = 2,

    /// <summary>
    /// The interaction was executed in the context of a direct message or group direct message.
    /// </summary>
    PrivateChannel = 3,
}
