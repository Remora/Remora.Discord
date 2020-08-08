//
//  IWebhook.cs
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

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents a webhook object.
    /// </summary>
    [PublicAPI]
    public interface IWebhook
    {
        /// <summary>
        /// Gets the ID of the webhook.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the type of the webhook.
        /// </summary>
        WebhookType Type { get; }

        /// <summary>
        /// Gets the ID of the guild that the webhook belongs to.
        /// </summary>
        Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Gets the ID of the channel that the webhook belongs to.
        /// </summary>
        Snowflake ChannelID { get; }

        /// <summary>
        /// Gets the user this webhook was created by. This is not returned when getting a webhook by its token.
        /// </summary>
        Optional<IUser> User { get; }

        /// <summary>
        /// Gets the default name of the webhook.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets the default avatar of the webhook.
        /// </summary>
        string? Avatar { get; }

        /// <summary>
        /// Gets the secure token of the webhook. Returned for webhooks with type <see cref="WebhookType.Incoming"/>.
        /// </summary>
        Optional<string> Token { get; }
    }
}
