//
//  FollowedChannel.cs
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

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class FollowedChannel : IFollowedChannel
    {
        /// <inheritdoc />
        public Snowflake ChannelID { get; }

        /// <inheritdoc />
        public Snowflake WebhookID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowedChannel"/> class.
        /// </summary>
        /// <param name="channelID">The ID of the news channel.</param>
        /// <param name="webhookID">The ID of the created webhook.</param>
        public FollowedChannel(Snowflake channelID, Snowflake webhookID)
        {
            this.ChannelID = channelID;
            this.WebhookID = webhookID;
        }
    }
}
