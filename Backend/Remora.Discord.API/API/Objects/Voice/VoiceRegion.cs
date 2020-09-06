//
//  VoiceRegion.cs
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

namespace Remora.Discord.API.Objects
{
    /// <inheritdoc />
    public class VoiceRegion : IVoiceRegion
    {
        /// <inheritdoc/>
        public string ID { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public bool IsVIP { get; }

        /// <inheritdoc/>
        public bool IsOptimal { get; }

        /// <inheritdoc/>
        public bool IsDeprecated { get; }

        /// <inheritdoc/>
        public bool IsCustom { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceRegion"/> class.
        /// </summary>
        /// <param name="id">The ID of the region.</param>
        /// <param name="name">The name of the region.</param>
        /// <param name="isVIP">Whether this is VIP-only server.</param>
        /// <param name="isOptimal">Whether this is the closest server to the current user.</param>
        /// <param name="isDeprecated">Whether this is a deprecated region (avoid it).</param>
        /// <param name="isCustom">Whether this is a custom region.</param>
        public VoiceRegion(string id, string name, bool isVIP, bool isOptimal, bool isDeprecated, bool isCustom)
        {
            this.ID = id;
            this.Name = name;
            this.IsVIP = isVIP;
            this.IsOptimal = isOptimal;
            this.IsDeprecated = isDeprecated;
            this.IsCustom = isCustom;
        }
    }
}
