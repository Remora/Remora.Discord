//
//  Mention.cs
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
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;

namespace Remora.Discord.Extensions.Formatting;

/// <summary>
/// Provides helper methods to mention various Discord objects.
/// </summary>
[PublicAPI]
public static class Mention
{
    /// <summary>
    /// Creates a mention string for a user, displaying their username.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>
    /// A user mention string.
    /// </returns>
    public static string User(IUser user) => $"<@{user.ID.Value}>";

    /// <summary>
    /// Creates a mention string for a user from their ID, displaying their username.
    /// </summary>
    /// <param name="snowflake">The user Snowflake ID.</param>
    /// <returns>
    /// A user mention string.
    /// </returns>
    public static string User(Snowflake snowflake) => $"<@{snowflake.Value}>";

    /// <summary>
    /// Creates a mention string for a channel.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <returns>
    /// A channel mention string.
    /// </returns>
    public static string Channel(IChannel channel) => $"<#{channel.ID.Value}>";

    /// <summary>
    /// Creates a mention string for a channel from its ID.
    /// </summary>
    /// <param name="snowflake">The channel Snowflake ID.</param>
    /// <returns>
    /// A channel mention string.
    /// </returns>
    public static string Channel(Snowflake snowflake) => $"<#{snowflake.Value}>";

    /// <summary>
    /// Creates a mention string for a role from its ID.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>
    /// A role mention string.
    /// </returns>
    public static string Role(IRole role) => $"<@&{role.ID.Value}>";

    /// <summary>
    /// Creates a mention string for a role.
    /// </summary>
    /// <param name="snowflake">The role Snowflake ID.</param>
    /// <returns>
    /// A role mention string.
    /// </returns>
    public static string Role(Snowflake snowflake) => $"<@&{snowflake.Value}>";

    /// <summary>
    /// Creates a mention string for a slash command.
    /// </summary>
    /// <param name="command">The slash command.</param>
    /// <returns></returns>
    public static string SlashCommand(IApplicationCommand command) => $"</{command.Name}:{command.ID}>";

    /// <summary>
    /// Creates a mention string for a slash command.
    /// </summary>
    /// <param name="name">The name of the slash command.</param>
    /// <param name="id">The ID of the slash command.</param>
    /// <returns></returns>
    public static string SlashCommand(string name, Snowflake id) => $"</{name}:{id}>";
}
