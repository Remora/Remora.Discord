//
//  ChannelContextExtensions.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Conditions;
using static Remora.Discord.API.Abstractions.Objects.ChannelType;

namespace Remora.Discord.Commands.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="ChannelContext"/> class.
/// </summary>
[PublicAPI]
public static class ChannelContextExtensions
{
    /// <summary>
    /// Holds a mapping of channel contexts to their constituent channel types.
    /// </summary>
    private static readonly IReadOnlyDictionary<ChannelContext, IReadOnlyList<ChannelType>> _channelTypesMap =
        new Dictionary<ChannelContext, IReadOnlyList<ChannelType>>
        {
            {
                ChannelContext.Guild,
                new[]
                {
                    GuildText,
                    GuildVoice,
                    GuildCategory,
                    GuildAnnouncement,
                    PrivateThread,
                    PublicThread,
                    AnnouncementThread,
                    GuildStageVoice
                }
            },
            {
                ChannelContext.DM,
                new[]
                {
                    DM
                }
            },
            {
                ChannelContext.GroupDM,
                new[]
                {
                    GroupDM
                }
            }
        };

    /// <summary>
    /// Converts the general channel context into its constituent channel types.
    /// </summary>
    /// <param name="channelContext">The channel context to retrieve the channel types from.</param>
    /// <returns>The channel context's channels types.</returns>
    public static IReadOnlyList<ChannelType> ToChannelTypes(this ChannelContext channelContext)
    {
        return _channelTypesMap[channelContext];
    }
}
