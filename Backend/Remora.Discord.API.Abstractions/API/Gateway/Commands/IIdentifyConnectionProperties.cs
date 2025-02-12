//
//  IIdentifyConnectionProperties.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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

namespace Remora.Discord.API.Abstractions.Gateway.Commands;

/// <summary>
/// Represents a set of connection properties sent to the Discord gateway.
/// </summary>
[PublicAPI]
public interface IIdentifyConnectionProperties
{
    /// <summary>
    /// Gets the operating system in use by the connection.
    /// </summary>
    string OperatingSystem { get; }

    /// <summary>
    /// Gets the browser in use by the connection. Typically, this is the name of the library that initialized the
    /// connection.
    /// </summary>
    string Browser { get; }

    /// <summary>
    /// Gets the device in use by the connection. Typically, this is the name of the library that initializes the
    /// connection.
    /// </summary>
    string Device { get; }
}
