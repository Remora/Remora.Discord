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

using System;
using System.IO;
using System.Reflection;
using Humanizer;
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
        public RetrieveEntityResult<Stream> GetSampleEventData<TType>() => GetSampleData<TType>("Events");

        /// <summary>
        /// Gets a sample file for the given API command type.
        /// </summary>
        /// <typeparam name="TType">The API type.</typeparam>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        public RetrieveEntityResult<Stream> GetSampleCommandData<TType>() => GetSampleData<TType>("Commands");

        private RetrieveEntityResult<string> GetBaseSampleDataPath()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (basePath is null)
            {
                return RetrieveEntityResult<string>.FromError("Failed to retrieve the base path of the assembly.");
            }

            return Path.Combine(basePath, "Samples");
        }

        private RetrieveEntityResult<Stream> GetSampleData<TType>(string subfolder)
        {
            var getBasePath = GetBaseSampleDataPath();
            if (!getBasePath.IsSuccess)
            {
                return RetrieveEntityResult<Stream>.FromError(getBasePath);
            }

            var basePath = Path.Combine(getBasePath.Entity, subfolder);
            var filename = $"{typeof(TType).Name.Underscore().Transform(To.UpperCase)}.json";

            var samplePath = Path.Combine(basePath, filename);
            if (!File.Exists(samplePath))
            {
                return RetrieveEntityResult<Stream>.FromError("No valid sample data found.");
            }

            try
            {
                return File.OpenRead(samplePath);
            }
            catch (Exception e)
            {
                return RetrieveEntityResult<Stream>.FromError(e, e.Message);
            }
        }
    }
}
