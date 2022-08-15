//
//  Constants.cs
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

using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

namespace Remora.Discord.Gateway.Tests;

/// <summary>
/// Holds various constants for use in tests.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Gets the default bot user.
    /// </summary>
    public static IUser BotUser { get; } = new User
    (
        DiscordSnowflake.New(0),
        "mock-bot",
        0,
        null
    );

    /// <summary>
    /// Gets the default mocked token.
    /// </summary>
    public static string MockToken => "mock-token";

    /// <summary>
    /// Gets the default mocked session ID.
    /// </summary>
    public static string MockSessionID => "mock-session";

    /// <summary>
    /// Gets the default mocked resume gateway URL.
    /// </summary>
    public static string MockResumeGatewayUrl => "wss://us-east1-b.gateway.discord.gg";
}
