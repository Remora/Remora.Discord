//
//  IVoiceSelectProtocol.cs
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
    /// Represents a protocol selection request.
    /// </summary>
    [PublicAPI]
    public interface IVoiceSelectProtocol : IVoiceGatewayCommand
    {
        /// <summary>
        /// Gets the name of the IP protocol the client wishes to communicate with. This should always be "udp".
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// Gets the connection data of the client.
        /// </summary>
        IVoiceProtocolData Data { get; }
    }
}
