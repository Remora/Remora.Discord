//
//  UpdatePresenceTests.cs
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

using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Tests.TestBases;
using Remora.Rest.Xunit;

namespace Remora.Discord.API.Tests.Gateway.Commands;

/// <summary>
/// Tests the <see cref="UpdatePresence"/> command.
/// </summary>
public class UpdatePresenceTests : GatewayCommandTestBase<IUpdatePresence>
{
    /// <inheritdoc />
    protected override JsonAssertOptions AssertOptions { get; } = JsonAssertOptions.Default with
    {
        AllowMissing = new[]
        {
            "id" // undocumented value upon "activities[]" objects
        }
    };
}
