//
//  ConnectionProperties.cs
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

using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Gateway.Commands;

#pragma warning disable CS1591

namespace Remora.Discord.API.Gateway.Commands;

/// <summary>
/// Represents a set of connection properties sent to the Discord gateway.
/// </summary>
[PublicAPI]
public record ConnectionProperties(string OperatingSystem, string Browser, string Device) : IConnectionProperties
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionProperties"/> class.
    /// </summary>
    /// <param name="libraryName">The name of the library.</param>
    public ConnectionProperties(string libraryName)
        : this(GetCurrentOSPlatformName(), libraryName, libraryName)
    {
    }

    /// <summary>
    /// Gets the name of the current operating system.
    /// </summary>
    /// <returns>The name of the current operating system.</returns>
    private static string GetCurrentOSPlatformName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "linux";
        }

        #if NET5_0_OR_GREATER
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return "freebsd";
        }
        #endif

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "osx";
        }

        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "windows"
            : "unknown";
    }
}
