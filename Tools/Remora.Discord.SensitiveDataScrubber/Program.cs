//
//  Program.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Json;

namespace Remora.Discord.SensitiveDataScrubber
{
    /// <summary>
    /// The main class of the program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Gets or sets the program options for this execution.
        /// </summary>
        private static ProgramOptions Options { get; set; } = null!;

        /// <summary>
        /// Defines the main entrypoint of the program.
        /// </summary>
        /// <param name="args">The arguments provided to the program.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<int> Main(string[] args)
        {
            var hadErrors = false;
            Parser.Default.ParseArguments<ProgramOptions>(args)
                .WithParsed(r => Options = r)
                .WithNotParsed(_ => hadErrors = true);

            if (hadErrors)
            {
                return 1;
            }

            var services = new ServiceCollection()
                .AddLogging(c => c.AddConsole())
                .BuildServiceProvider();

            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Loading patterns...");

            if (!File.Exists("patterns.json"))
            {
                logger.LogCritical("Failed to load pattern file");
                return 1;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                Converters = { new RegexConverter() }
            };

            await using var patternsFile = File.OpenRead("patterns.json");
            var rawPatterns = await JsonSerializer.DeserializeAsync<IReadOnlyDictionary<string, SensitivePattern>>
            (
                patternsFile,
                jsonOptions
            );

            if (rawPatterns is null)
            {
                logger.LogCritical("Failed to load pattern file");
                return 1;
            }

            var patterns = rawPatterns.ToDictionary(kvp => new Regex(kvp.Key, RegexOptions.Compiled), kvp => kvp.Value);

            logger.LogInformation("Loaded {Count} patterns", patterns.Count);
            return 0;
        }
    }
}
