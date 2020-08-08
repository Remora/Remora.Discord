//
//  ClientStatuses.cs
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
    public class ClientStatuses : IClientStatuses
    {
        /// <inheritdoc />
        public Optional<ClientStatus> Desktop { get; }

        /// <inheritdoc />
        public Optional<ClientStatus> Mobile { get; }

        /// <inheritdoc />
        public Optional<ClientStatus> Browser { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStatuses"/> class.
        /// </summary>
        /// <param name="desktop">The desktop status.</param>
        /// <param name="mobile">The mobile status.</param>
        /// <param name="browser">The browser status.</param>
        public ClientStatuses
        (
            Optional<ClientStatus> desktop,
            Optional<ClientStatus> mobile,
            Optional<ClientStatus> browser
        )
        {
            this.Desktop = desktop;
            this.Mobile = mobile;
            this.Browser = browser;
        }
    }
}
