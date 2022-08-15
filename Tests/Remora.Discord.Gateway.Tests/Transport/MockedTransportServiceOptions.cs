//
//  MockedTransportServiceOptions.cs
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

using System;

namespace Remora.Discord.Gateway.Tests.Transport;

/// <summary>
/// Defines various options for a mocked transport service.
/// </summary>
public class MockedTransportServiceOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to ignore unexpected received payloads.
    /// </summary>
    public bool IgnoreUnexpected { get; set; }

    /// <summary>
    /// Gets or sets the global advancement timeout.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
}
