//
//  DiscordServiceOptions.cs
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

using Remora.Rest.Core;

namespace Remora.Discord.Hosting.Options;

/// <summary>
/// Defines a set of options used by the background gateway service.
/// </summary>
/// <param name="TerminateApplicationOnCriticalGatewayErrors">
/// Whether the service should stop the application if a critical gateway error is encountered.
/// </param>
/// <param name="CheckSlashCommandsSupport">
/// Whether the service should check if slash commands are supported.
/// </param>
/// <param name="UpdateSlashCommands">
/// Whether the service should update slash commands automatically.
/// </param>
/// <param name="UpdateSlashGuild">
/// The guild that the service should update slash commands for.
/// When null, it updates global commands.
/// </param>
public record DiscordServiceOptions
(
    bool TerminateApplicationOnCriticalGatewayErrors = true,
    bool CheckSlashCommandsSupport = false,
    bool UpdateSlashCommands = false,
    Snowflake? UpdateSlashGuild = null
);
