//
//  User.cs
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
    /// <inheritdoc cref="IUser"/>
    public class User : IUser
    {
        /// <inheritdoc />
        public Snowflake ID { get; }

        /// <inheritdoc />
        public string Username { get; }

        /// <inheritdoc />
        public string Discriminator { get; }

        /// <inheritdoc />
        public IImageHash? Avatar { get; }

        /// <inheritdoc />
        public Optional<bool> IsBot { get; }

        /// <inheritdoc />
        public Optional<bool> IsSystem { get; }

        /// <inheritdoc />
        public Optional<bool> IsMFAEnabled { get; }

        /// <inheritdoc />
        public Optional<string> Locale { get; }

        /// <inheritdoc />
        public Optional<bool> IsVerified { get; }

        /// <inheritdoc />
        public Optional<string?> Email { get; }

        /// <inheritdoc />
        public Optional<UserFlags> Flags { get; }

        /// <inheritdoc />
        public Optional<PremiumType> PremiumType { get; }

        /// <inheritdoc />
        public Optional<UserFlags> PublicFlags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="username">The user's username.</param>
        /// <param name="discriminator">The user's discriminator.</param>
        /// <param name="avatar">The user's avatar.</param>
        /// <param name="isBot">Whether the user is a bot.</param>
        /// <param name="isSystem">Whether the user is a system user.</param>
        /// <param name="isMFAEnabled">Whether MFA is enabled.</param>
        /// <param name="locale">The user's locale.</param>
        /// <param name="isVerified">Whether the user is verified.</param>
        /// <param name="email">The user's email.</param>
        /// <param name="flags">The user's flags.</param>
        /// <param name="premiumType">The user's premium type.</param>
        /// <param name="publicFlags">The user's public flags.</param>
        public User
        (
            Snowflake id,
            string username,
            string discriminator,
            IImageHash? avatar,
            Optional<bool> isBot,
            Optional<bool> isSystem,
            Optional<bool> isMFAEnabled,
            Optional<string> locale,
            Optional<bool> isVerified,
            Optional<string?> email,
            Optional<UserFlags> flags,
            Optional<PremiumType> premiumType,
            Optional<UserFlags> publicFlags
        )
        {
            this.ID = id;
            this.Username = username;
            this.Discriminator = discriminator;
            this.Avatar = avatar;
            this.IsBot = isBot;
            this.IsSystem = isSystem;
            this.IsMFAEnabled = isMFAEnabled;
            this.Locale = locale;
            this.IsVerified = isVerified;
            this.Email = email;
            this.Flags = flags;
            this.PremiumType = premiumType;
            this.PublicFlags = publicFlags;
        }
    }
}
