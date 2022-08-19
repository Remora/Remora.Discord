//
//  JsonBackedTypeTestBase.cs
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Extensions;
using Remora.Discord.API.Tests.Services;
using Remora.Discord.Unstable.Extensions;
using Remora.Rest.Xunit;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Remora.Discord.API.Tests.TestBases;

/// <summary>
/// Acts as a base class for testing JSON-backed types in the Discord API. This class contains common baseline
/// tests for all types.
/// </summary>
/// <typeparam name="TType">The type under test.</typeparam>
/// <typeparam name="TSampleSource">The theory data source.</typeparam>
public abstract class JsonBackedTypeTestBase<TType, TSampleSource> where TSampleSource : TheoryData, new()
{
    /// <summary>
    /// Gets the data sample source.
    /// </summary>
    public static TheoryData SampleSource => new TSampleSource();

    /// <summary>
    /// Gets the configured JSON serializer options.
    /// </summary>
    protected JsonSerializerOptions Options { get; }

    /// <summary>
    /// Gets the assertion options.
    /// </summary>
    protected virtual JsonAssertOptions AssertOptions => JsonAssertOptions.Default;

    /// <summary>
    /// Gets a value indicating whether unknown events are allowed.
    /// </summary>
    protected virtual bool AllowUnknownEvents => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonBackedTypeTestBase{TType,TSampleSource}"/> class.
    /// </summary>
    protected JsonBackedTypeTestBase()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        var services = new ServiceCollection()
            .ConfigureDiscordJsonConverters(allowUnknownEvents: this.AllowUnknownEvents)
            .AddExperimentalDiscordApi()
            .BuildServiceProvider(true);

        this.Options = services.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>()
            .Get("Discord");
    }

    /// <summary>
    /// Tests whether the type can be deserialized from a JSON object.
    /// </summary>
    /// <param name="sampleDataFile">The sample data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [SkippableTheory]
    [MemberData(nameof(SampleSource), DisableDiscoveryEnumeration = true)]
    public async Task CanDeserialize(SampleDataDescriptor sampleDataFile)
    {
        await using var sampleData = File.OpenRead(sampleDataFile.FullPath);
        var payload = await JsonSerializer.DeserializeAsync<TType>(sampleData, this.Options);
        Assert.NotNull(payload);
    }

    /// <summary>
    /// Tests whether the type can be serialized to a JSON object.
    /// </summary>
    /// <param name="sampleDataFile">The sample data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [SkippableTheory]
    [MemberData(nameof(SampleSource), DisableDiscoveryEnumeration = true)]
    public async Task CanSerialize(SampleDataDescriptor sampleDataFile)
    {
        await using var sampleData = File.OpenRead(sampleDataFile.FullPath);
        var payload = await JsonSerializer.DeserializeAsync<TType>(sampleData, this.Options);

        Assert.NotNull(payload);

        await using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, payload, this.Options);
    }

    /// <summary>
    /// Tests whether data survives being round-tripped by the type - that is, it can be deserialized and then
    /// serialized again without data loss.
    /// </summary>
    /// <param name="sampleDataFile">The sample data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [SkippableTheory]
    [MemberData(nameof(SampleSource), DisableDiscoveryEnumeration = true)]
    public async Task SurvivesRoundTrip(SampleDataDescriptor sampleDataFile)
    {
        await using var sampleData = File.OpenRead(sampleDataFile.FullPath);
        var deserialized = await JsonSerializer.DeserializeAsync<TType>(sampleData, this.Options);

        Assert.NotNull(deserialized);

        await using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, deserialized, this.Options);

        await stream.FlushAsync();

        stream.Seek(0, SeekOrigin.Begin);
        sampleData.Seek(0, SeekOrigin.Begin);

        var serialized = await JsonDocument.ParseAsync(stream);
        var original = await JsonDocument.ParseAsync(sampleData);

        JsonAssert.Equivalent(original, serialized, this.AssertOptions);
    }
}
