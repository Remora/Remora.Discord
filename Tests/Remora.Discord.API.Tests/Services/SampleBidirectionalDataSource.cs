//
//  SampleBidirectionalDataSource.cs
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

using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Xunit;

namespace Remora.Discord.API.Tests.Services;

/// <summary>
/// Represents a source of sample data for an xUnit test.
/// </summary>
/// <typeparam name="TData">The data type.</typeparam>
public class SampleBidirectionalDataSource<TData> : TheoryData<SampleDataDescriptor> where TData : IGatewayEvent, IGatewayCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleBidirectionalDataSource{TData}"/> class.
    /// </summary>
    public SampleBidirectionalDataSource()
    {
        var sampleData = new SampleDataService();

        var getSamples = sampleData.GetSampleBidirectionalDataSet<TData>();
        if (!getSamples.IsSuccess)
        {
            throw new SkipException();
        }

        foreach (var sample in getSamples.Entity)
        {
            Add(sample);
        }
    }
}
