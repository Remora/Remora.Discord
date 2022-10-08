//
//  SampleVoiceEventDataSource.cs
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

using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Xunit;

namespace Remora.Discord.API.Tests.Services;

/// <summary>
/// Represents a source of sample data for an xUnit test.
/// </summary>
/// <typeparam name="TData">The data type.</typeparam>
public class SampleVoiceEventDataSource<TData> : TheoryData<SampleDataDescriptor> where TData : IVoiceGatewayEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleVoiceEventDataSource{TData}"/> class.
    /// </summary>
    public SampleVoiceEventDataSource()
    {
        var sampleData = new SampleDataService();

        var getSamples = sampleData.GetSampleVoiceEventDataSet<TData>();
        if (!getSamples.IsSuccess)
        {
            throw new SkipException(getSamples.Error.Message);
        }

        foreach (var sample in getSamples.Entity)
        {
            Add(sample);
        }
    }
}
