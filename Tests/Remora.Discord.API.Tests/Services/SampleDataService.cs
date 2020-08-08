//
//  SampleDataService.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Humanizer;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Results;

namespace Remora.Discord.API.Tests.Services
{
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
        public RetrieveEntityResult<IReadOnlyList<string>> GetSampleEventDataSet<TType>()
            where TType : IGatewayEvent
            => GetSampleDataSet<TType>(Path.Combine("Gateway", "Events"));

        /// <summary>
        /// Gets a sample file for the given API command type.
        /// </summary>
        /// <typeparam name="TType">The API type.</typeparam>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        public RetrieveEntityResult<IReadOnlyList<string>> GetSampleCommandDataSet<TType>()
            where TType : IGatewayCommand
            => GetSampleDataSet<TType>(Path.Combine("Gateway", "Commands"));

        /// <summary>
        /// Gets a sample file for the given API bidirectional type.
        /// </summary>
        /// <typeparam name="TType">The API type.</typeparam>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        public RetrieveEntityResult<IReadOnlyList<string>> GetSampleBidirectionalDataSet<TType>()
            where TType : IGatewayEvent, IGatewayCommand
            => GetSampleDataSet<TType>(Path.Combine("Gateway", "Bidirectional"));

        private RetrieveEntityResult<string> GetBaseSampleDataPath()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (basePath is null)
            {
                return RetrieveEntityResult<string>.FromError("Failed to retrieve the base path of the assembly.");
            }

            return Path.Combine(basePath, "Samples");
        }

        private RetrieveEntityResult<IReadOnlyList<string>> GetSampleDataSet<TType>(string subfolder)
        {
            var getBasePath = GetBaseSampleDataPath();
            if (!getBasePath.IsSuccess)
            {
                return RetrieveEntityResult<IReadOnlyList<string>>.FromError(getBasePath);
            }

            var basePath = Path.Combine(getBasePath.Entity, subfolder);
            var samplesDirectoryName = typeof(TType).Name.Underscore().Transform(To.UpperCase);

            var samplesPath = Path.Combine(basePath, samplesDirectoryName);
            if (!Directory.Exists(samplesPath))
            {
                return RetrieveEntityResult<IReadOnlyList<string>>.FromError("No valid sample data found.");
            }

            return Directory.EnumerateFiles(samplesPath, "*.json", SearchOption.AllDirectories).ToList();
        }
    }
}
