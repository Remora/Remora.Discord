//
//  KeyHelpers.cs
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
using Remora.Discord.Core;

namespace Remora.Discord.Caching
{
    /// <summary>
    /// Contains methods that create cache keys for various identified entities.
    /// </summary>
    public static class KeyHelpers
    {
        /// <summary>
        /// Creates a cache key for an <see cref="IPermissionOverwrite"/> instance.
        /// </summary>
        /// <param name="channelID">The ID of the channel the overwrite is for.</param>
        /// <param name="overwriteID">The ID of the overwrite.</param>
        /// <returns>The cache key.</returns>
        public static object CreateChannelPermissionCacheKey(Snowflake channelID, Snowflake overwriteID)
        {
            return (nameof(IPermissionOverwrite), overwriteID, CreateChannelCacheKey(channelID));
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IInvite"/> instance.
        /// </summary>
        /// <param name="channelID">The ID of the channel the invite is for.</param>
        /// <param name="code">The invite code.</param>
        /// <returns>The cache key.</returns>
        public static object CreateInviteCacheKey(Snowflake channelID, string code)
        {
            return (nameof(IInvite), code, CreateChannelCacheKey(channelID));
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IChannel"/> instance.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <returns>The cache key.</returns>
        public static object CreateChannelCacheKey(Snowflake channelID)
        {
            return (nameof(IChannel), channelID);
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IMessage"/> instance.
        /// </summary>
        /// <param name="channelID">The ID of the channel the message is in.</param>
        /// <param name="messageID">The ID of the message.</param>
        /// <returns>The cache key.</returns>
        public static object CreateMessageCacheKey(Snowflake channelID, Snowflake messageID)
        {
            var key = (nameof(IMessage), messageID, CreateChannelCacheKey(channelID));
            return key;
        }

        /// <summary>
        /// Creates a cache key for an <see cref="IEmoji"/> instance.
        /// </summary>
        /// <param name="guildID">The ID of the guild the emoji is in.</param>
        /// <param name="emojiID">The ID of the emoji.</param>
        /// <returns>The cache key.</returns>
        public static object CreateEmojiCacheKey(Snowflake guildID, Snowflake emojiID)
        {
            return (nameof(IEmoji), emojiID, (nameof(IGuild), guildID));
        }
    }
}
