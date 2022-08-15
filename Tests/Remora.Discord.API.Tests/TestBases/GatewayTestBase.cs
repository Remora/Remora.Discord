//
//  GatewayTestBase.cs
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

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Tests.Services;

using Xunit;

namespace Remora.Discord.API.Tests.TestBases;

/// <inheritdoc />
public abstract class GatewayTestBase<TType, TEventDataSource>
    : JsonBackedTypeTestBase<IPayload, TEventDataSource> where TEventDataSource : TheoryData, new()
{
    /// <summary>
    /// Tests whether the type can be deserialized from a JSON object.
    /// </summary>
    /// <param name="sampleDataFile">The sample data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [SkippableTheory]
    [MemberData(nameof(SampleSource), DisableDiscoveryEnumeration = true)]
    public async Task DeserializedCorrectType(SampleDataDescriptor sampleDataFile)
    {
        await using var sampleData = File.OpenRead(sampleDataFile.FullPath);
        var payload = await JsonSerializer.DeserializeAsync<IPayload>(sampleData, this.Options);

        Assert.IsAssignableFrom<IPayload<TType>>(payload);
    }
}
