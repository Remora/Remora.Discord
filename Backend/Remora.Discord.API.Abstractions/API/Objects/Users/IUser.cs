//
//  IUser.cs
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
using Remora.Discord.API.Abstractions.Images;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Users
{
    /// <summary>
    /// Represents a Discord user.
    /// </summary>
    [PublicAPI]
    public interface IUser
    {
        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the username of the user. This is not a unique value.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the user's 4-digit discord tag.
        /// </summary>
        string Discriminator { get; }

        /// <summary>
        /// Gets the user's avatar hash.
        /// </summary>
        IImageHash? Avatar { get; }

        /// <summary>
        /// Gets a value indicating whether the user is a bot, belonging to an OAuth2 application.
        /// </summary>
        Optional<bool> IsBot { get; }

        /// <summary>
        /// Gets a value indicating whether the user is an official Discord system user (part of the urgent message
        /// system).
        /// </summary>
        Optional<bool> IsSystem { get; }

        /// <summary>
        /// Gets a value indicating whether the user has multi-factor authentication enabled on their account.
        /// </summary>
        Optional<bool> IsMFAEnabled { get; }

        /// <summary>
        /// Gets the user's chosen language option.
        /// </summary>
        Optional<string> Locale { get; }

        /// <summary>
        /// Gets a value indicating whether the email on the account has been verified.
        /// </summary>
        Optional<bool> IsVerified { get; }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        Optional<string?> Email { get; }

        /// <summary>
        /// Gets the flags on the user's account.
        /// </summary>
        Optional<UserFlags> Flags { get; }

        /// <summary>
        /// Gets the user's premium status.
        /// </summary>
        Optional<PremiumType> PremiumType { get; }

        /// <summary>
        /// Gets the flags on a user's account.
        /// </summary>
        Optional<UserFlags> PublicFlags { get; }
    }
}
