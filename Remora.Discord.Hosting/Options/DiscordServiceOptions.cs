//
//  DiscordServiceOptions.cs
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

namespace Remora.Discord.Hosting.Options;

/// <summary>
/// Defines a set of options used by the background gateway service.
/// </summary>
/// <param name="TerminateApplicationOnCriticalGatewayErrors">
/// Whether the service should stop the application if a critical gateway error is encountered.
/// </param>
[PublicAPI]
public record DiscordServiceOptions(bool TerminateApplicationOnCriticalGatewayErrors = true);
