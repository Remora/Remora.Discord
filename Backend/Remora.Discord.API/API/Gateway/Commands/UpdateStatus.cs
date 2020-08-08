//
//  UpdateStatus.cs
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

using System;
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Commands;

namespace Remora.Discord.API.Gateway.Commands
{
    /// <summary>
    /// Represents a command to update the status of a user.
    /// </summary>
    public class UpdateStatus : IUpdateStatus
    {
        /// <inheritdoc />
        public DateTime? Since { get; }

        /// <inheritdoc />
        public IActivity? Game { get; }

        /// <inheritdoc />
        public ClientStatus Status { get; }

        /// <inheritdoc />
        public bool IsAFK { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStatus"/> class.
        /// </summary>
        /// <param name="status">The user's status.</param>
        /// <param name="isAFK">Whether the user is AFK.</param>
        /// <param name="since">The time that the client went idle.</param>
        /// <param name="game">The activity that the user is performing.</param>
        public UpdateStatus
        (
            ClientStatus status,
            bool isAFK,
            DateTime? since = null,
            IActivity? game = null
        )
        {
            this.Since = since;
            this.Game = game;
            this.Status = status;
            this.IsAFK = isAFK;
        }
    }
}
