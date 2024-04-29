//
//  PresenceUpdateTests.cs
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

using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Tests.TestBases;
using Remora.Rest.Xunit;

namespace Remora.Discord.API.Tests.Gateway.Events;

/// <inheritdoc />
public class PresenceUpdateTests : GatewayEventTestBase<IPresenceUpdate>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PresenceUpdateTests"/> class.
    /// </summary>
    /// <param name="fixture">The test fixture.</param>
    public PresenceUpdateTests(JsonBackedTypeTestFixture fixture)
        : base(fixture)
    {
    }

    /// <inheritdoc/>
    protected override JsonAssertOptions AssertOptions { get; } = JsonAssertOptions.Default with
    {
        AllowMissing = new[]
        {
            "id", // undocumented field in "activities[]" objects
            "sync_id", // undocumented field in "activities[]" objects
            "session_id"
        }
    };
}
