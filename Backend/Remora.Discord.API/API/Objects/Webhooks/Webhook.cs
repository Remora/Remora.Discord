//
//  Webhook.cs
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
    public class Webhook : IWebhook
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public WebhookType Type { get; }

        /// <inheritdoc />
        public Optional<Snowflake> GuildID { get; }

        /// <inheritdoc />
        public Snowflake ChannelID { get; }

        /// <inheritdoc />
        public Optional<IUser> User { get; }

        /// <inheritdoc />
        public string? Name { get; }

        /// <inheritdoc />
        public string? Avatar { get; }

        /// <inheritdoc />
        public Optional<string> Token { get; }

        /// <inheritdoc />
        public Optional<Snowflake> ApplicationID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Webhook"/> class.
        /// </summary>
        /// <param name="id">The ID of the webhook.</param>
        /// <param name="type">The type of the webhook.</param>
        /// <param name="guildID">The ID of the guild the webhook is for.</param>
        /// <param name="channelID">The ID of the channel the webhook is for.</param>
        /// <param name="user">The user that created the webhook.</param>
        /// <param name="name">The default name of the webhook.</param>
        /// <param name="avatar">The default avatar of the webhook.</param>
        /// <param name="token">The secure token of the webhook.</param>
        /// <param name="applicationID">The application ID, if any.</param>
        public Webhook
        (
            Snowflake id,
            WebhookType type,
            Optional<Snowflake> guildID,
            Snowflake channelID,
            Optional<IUser> user,
            string? name,
            string? avatar,
            Optional<string> token,
            Optional<Snowflake> applicationID
        )
        {
            this.ID = id;
            this.Type = type;
            this.GuildID = guildID;
            this.ChannelID = channelID;
            this.User = user;
            this.Name = name;
            this.Avatar = avatar;
            this.Token = token;
            this.ApplicationID = applicationID;
        }
    }
}
