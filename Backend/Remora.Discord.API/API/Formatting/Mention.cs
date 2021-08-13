//
//  Mention.cs
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

using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.API.Formatting
{
    /// <summary>
    /// Provides helper methods to mention various Discord objects.
    /// </summary>
    [PublicAPI]
    public static class Mention
    {
        /// <summary>
        /// Creates a mention string for a user.
        /// </summary>
        /// <param name="snowflake">The user Snowflake ID.</param>
        /// <returns>
        /// A user mention string.
        /// </returns>
        public static string User(Snowflake snowflake) => $"<@{snowflake.Value}>";

        /// <summary>
        /// Creates a mention string for a channel.
        /// </summary>
        /// <param name="snowflake">The channel Snowflake ID.</param>
        /// <returns>
        /// A channel mention string.
        /// </returns>
        public static string Channel(Snowflake snowflake) => $"<#{snowflake.Value}>";

        /// <summary>
        /// Creates a mention string for a role.
        /// </summary>
        /// <param name="snowflake">The role Snowflake ID.</param>
        /// <returns>
        /// A role mention string.
        /// </returns>
        public static string Role(Snowflake snowflake) => $"<@&{snowflake.Value}>";
    }
}
