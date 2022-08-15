//
//  SampleDataService.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Humanizer;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.VoiceGateway.Commands;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Remora.Results;

namespace Remora.Discord.API.Tests.Services;

/// <summary>
/// Handles interaction with local sample data.
/// </summary>
public class SampleDataService
{
    /// <summary>
    /// Gets a sample file for the given API event type.
    /// </summary>
    /// <typeparam name="TType">The API type.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    public Result<IReadOnlyList<SampleDataDescriptor>> GetSampleEventDataSet<TType>()
        where TType : IGatewayEvent
        => GetSampleDataSet<TType>(Path.Combine("Gateway", "Events"));

    /// <summary>
    /// Gets a sample file for the given API command type.
    /// </summary>
    /// <typeparam name="TType">The API type.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    public Result<IReadOnlyList<SampleDataDescriptor>> GetSampleCommandDataSet<TType>()
        where TType : IGatewayCommand
        => GetSampleDataSet<TType>(Path.Combine("Gateway", "Commands"));

    /// <summary>
    /// Gets a sample file for the given API bidirectional type.
    /// </summary>
    /// <typeparam name="TType">The API type.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    public Result<IReadOnlyList<SampleDataDescriptor>> GetSampleBidirectionalDataSet<TType>()
        where TType : IGatewayEvent, IGatewayCommand
        => GetSampleDataSet<TType>(Path.Combine("Gateway", "Bidirectional"));

    /// <summary>
    /// Gets a sample file for the given API object type.
    /// </summary>
    /// <typeparam name="TType">The API type.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    public Result<IReadOnlyList<SampleDataDescriptor>> GetSampleObjectDataSet<TType>()
        => GetSampleDataSet<TType>("Objects");

    /// <summary>
    /// Gets a sample file for the given voice gateway event type.
    /// </summary>
    /// <typeparam name="TType">The gateway type.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    public Result<IReadOnlyList<SampleDataDescriptor>> GetSampleVoiceEventDataSet<TType>()
        where TType : IVoiceGatewayEvent
        => GetSampleDataSet<TType>(Path.Combine("VoiceGateway", "Events"));

    /// <summary>
    /// Gets a sample file for the given voice gateway command type.
    /// </summary>
    /// <typeparam name="TType">The gateway type.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    public Result<IReadOnlyList<SampleDataDescriptor>> GetSampleVoiceCommandDataSet<TType>()
        where TType : IVoiceGatewayCommand
        => GetSampleDataSet<TType>(Path.Combine("VoiceGateway", "Commands"));

    private static Result<string> GetBaseSampleDataPath()
    {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (basePath is null)
        {
            return new InvalidOperationError("Failed to retrieve the base path of the assembly.");
        }

        return Path.Combine(basePath, "Samples");
    }

    private static Result<IReadOnlyList<SampleDataDescriptor>> GetSampleDataSet<TType>(string subfolder)
    {
        var getBasePath = GetBaseSampleDataPath();
        if (!getBasePath.IsSuccess)
        {
            return Result<IReadOnlyList<SampleDataDescriptor>>.FromError(getBasePath);
        }

        var basePath = Path.Combine(getBasePath.Entity, subfolder);
        var samplesDirectoryName = typeof(TType).Name.Underscore().Transform(To.UpperCase);

        if (typeof(TType).IsInterface && samplesDirectoryName.StartsWith('I'))
        {
            samplesDirectoryName = samplesDirectoryName[2..];
        }

        var samplesPath = Path.Combine(basePath, samplesDirectoryName);
        if (!Directory.Exists(samplesPath))
        {
            return new InvalidOperationError("No valid sample data found.");
        }

        return Directory.EnumerateFiles(samplesPath, "*.json", SearchOption.AllDirectories)
            .Select(fullPath => new SampleDataDescriptor(
                basePath,
                Path.GetRelativePath(basePath, fullPath)))
            .ToArray();
    }
}
