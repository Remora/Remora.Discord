//
//  ConnectionProperties.cs
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

using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Remora.Discord.Gateway.API.Commands
{
    /// <summary>
    /// Represents a set of connection properties sent to the Discord gateway.
    /// </summary>
    internal sealed class ConnectionProperties
    {
        /// <summary>
        /// Gets the operating system in use by the connection.
        /// </summary>
        [JsonProperty("$os")]
        public string OperatingSystem { get; }

        /// <summary>
        /// Gets the browser in use by the connection. Typically, this is the name of the library that initialized the
        /// connection.
        /// </summary>
        [JsonProperty("$browser")]
        public string Browser { get; }

        /// <summary>
        /// Gets the device in use by the connection. Typically, this is the name of the library that initializes the
        /// connection.
        /// </summary>
        [JsonProperty("$device")]
        public string Device { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionProperties"/> class.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="device">The device.</param>
        public ConnectionProperties(string operatingSystem, string browser, string device)
        {
            this.OperatingSystem = operatingSystem;
            this.Browser = browser;
            this.Device = device;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionProperties"/> class.
        /// </summary>
        /// <param name="libraryName">The name of the library.</param>
        public ConnectionProperties(string libraryName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.OperatingSystem = "linux";
            }
        #if NETCOREAPP3_1
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                this.OperatingSystem = "freebsd";
            }
        #endif
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                this.OperatingSystem = "osx";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.OperatingSystem = "windows";
            }
            else
            {
                this.OperatingSystem = "unknown";
            }

            this.Browser = libraryName;
            this.Device = libraryName;
        }
    }
}
