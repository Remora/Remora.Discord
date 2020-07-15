//
//  IPartialIntegration.cs
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

namespace Remora.Discord.Rest.Abstractions
{
    /// <summary>
    /// Represents a partial integration object.
    /// </summary>
    [PublicAPI]
    public interface IPartialIntegration
    {
        /// <summary>
        /// Gets the ID of the integration.
        /// </summary>
        Snowflake ID { get; }

        /// <summary>
        /// Gets the name of the integration.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of integration.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the integration's account information.
        /// </summary>
        IAccount Account { get; }
    }
}
