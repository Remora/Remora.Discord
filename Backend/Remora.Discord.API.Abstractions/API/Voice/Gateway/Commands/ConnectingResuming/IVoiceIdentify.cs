//
//  IVoiceIdentify.cs
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

namespace Remora.Discord.API.Abstractions.Voice.Gateway.Commands
{
    /// <summary>
    /// Represents a request to identify with the voice gateway.
    /// </summary>
    [PublicAPI]
    public interface IVoiceIdentify
    {
        /// <summary>
        /// Gets the ID of the guild to identify with.
        /// </summary>
        Snowflake ServerID { get; }

        /// <summary>
        /// Gets the ID of the user that's trying to identify themselves.
        /// </summary>
        Snowflake UserID { get; }

        /// <summary>
        /// Gets the voice session ID to initialize.
        /// </summary>
        string SessionID { get; }

        /// <summary>
        /// Gets the authentication token of the identifying user.
        /// </summary>
        string Token { get; }
    }
}
