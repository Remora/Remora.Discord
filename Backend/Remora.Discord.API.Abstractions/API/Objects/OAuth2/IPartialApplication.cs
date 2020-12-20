//
//  IPartialApplication.cs
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Remora.Discord.Core;

namespace Remora.Discord.API.Abstractions.Objects
{
    /// <summary>
    /// Represents information about an OAuth2 application.
    /// </summary>
    [PublicAPI]
    public interface IPartialApplication
    {
        /// <summary>
        /// Gets the application ID.
        /// </summary>
        Optional<Snowflake> ID { get; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        Optional<string> Name { get; }

        /// <summary>
        /// Gets the icon hash of the application.
        /// </summary>
        Optional<IImageHash?> Icon { get; }

        /// <summary>
        /// Gets the description of the application.
        /// </summary>
        Optional<string> Description { get; }

        /// <summary>
        /// Gets a list of RPC origin URLs.
        /// </summary>
        Optional<IReadOnlyList<string>> RPCOrigins { get; }

        /// <summary>
        /// Gets a value indicating whether the bot is a public bot.
        /// </summary>
        Optional<bool> IsBotPublic { get; }

        /// <summary>
        /// Gets a value indicating whether the bot will only join upon completion of a full OAuth2 flow.
        /// </summary>
        Optional<bool> DoesBotRequireCodeGrant { get; }

        /// <summary>
        /// Gets the user information of the application owner.
        /// </summary>
        Optional<IPartialUser> Owner { get; }

        /// <summary>
        /// Gets the summary of the game, if the application is a game sold on the Discord storefront.
        /// </summary>
        Optional<string> Summary { get; }

        /// <summary>
        /// Gets the base64-encoded key for GameSDK's GetTicket function.
        /// </summary>
        Optional<string> VerifyKey { get; }

        /// <summary>
        /// Gets the team the application belongs to, if any.
        /// </summary>
        Optional<ITeam?> Team { get; }

        /// <summary>
        /// Gets the guild the game is linked to, if the application is a game sold on the Discord storefront.
        /// </summary>
        Optional<Snowflake> GuildID { get; }

        /// <summary>
        /// Gets the primary SKU ID of the game, if the application is a game sold on the Discord storefront.
        /// </summary>
        Optional<Snowflake> PrimarySKUID { get; }

        /// <summary>
        /// Gets the URL slug that links to the store page, if the application is a game sold on the Discord storefront.
        /// </summary>
        Optional<string> Slug { get; }

        /// <summary>
        /// Gets the cover image, if the application is a game sold on the Discord storefront.
        /// </summary>
        Optional<IImageHash> CoverImage { get; }

        /// <summary>
        /// Gets the application's public flags.
        /// </summary>
        Optional<int> Flags { get; }
    }
}
