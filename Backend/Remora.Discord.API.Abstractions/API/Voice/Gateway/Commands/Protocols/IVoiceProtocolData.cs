//
//  IVoiceProtocolData.cs
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

namespace Remora.Discord.API.Abstractions.Voice.Gateway.Commands
{
    /// <summary>
    /// Represents clientside connection information provided to the voice server.
    /// </summary>
    [PublicAPI]
    public interface IVoiceProtocolData
    {
        /// <summary>
        /// Gets the public address on which we will be receiving voice data.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Gets the port on which we will be receiving voice data.
        /// </summary>
        ushort Port { get; }

        /// <summary>
        /// Gets the requested encryption mode for our connection.
        /// </summary>
        string Mode { get; }
    }
}
