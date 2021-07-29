//
//  ChannelContextExtensions.cs
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

using System;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Conditions;
using static Remora.Discord.API.Abstractions.Objects.ChannelType;

namespace Remora.Discord.Commands.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="ChannelContext"/> class.
    /// </summary>
    public static class ChannelContextExtensions
    {
        /// <summary>
        /// Determines whether a channel type belongs to a ChannelContext.
        /// </summary>
        /// <param name="channelContext">The channel context to check if a channel type belongs to.</param>
        /// <param name="type">The channel type.</param>
        /// <returns>Whether channel type belongs to the channel context.</returns>
        public static bool IsChannelTypeInContext(this ChannelContext channelContext, ChannelType type)
        {
            return channelContext switch
            {
                ChannelContext.DM => type is DM,
                ChannelContext.GroupDM => type is GroupDM,
                ChannelContext.Guild => type is GuildText or GuildVoice or GuildCategory or GuildNews
                    or GuildStore or GuildPrivateThread or GuildPublicThread or GuildNewsThread,
                _ => throw new ArgumentOutOfRangeException(nameof(channelContext))
            };
        }

        /// <summary>
        /// Gets the channel types a channel context describes.
        /// </summary>
        /// <param name="channelContext">The channel context to retrieve the channel types from.</param>
        /// <returns>The channel context's channels types.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if there are no channel types defined for a specific channel context.</exception>
        public static ChannelType[] GetChannelTypes(this ChannelContext channelContext)
        {
            return channelContext switch
            {
                ChannelContext.DM => new[] { DM },
                ChannelContext.GroupDM => new[] { GroupDM },
                ChannelContext.Guild => new[]
                {
                    GuildText, GuildVoice, GuildCategory, GuildNews, GuildStore,
                    GuildPrivateThread, GuildPublicThread, GuildNewsThread
                },
                _ => throw new ArgumentOutOfRangeException(nameof(channelContext))
            };
        }
    }
}
