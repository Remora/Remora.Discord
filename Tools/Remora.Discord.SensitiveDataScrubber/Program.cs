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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
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
                Converters = { new RegexConverter() },
                WriteIndented = true
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

            var jsonWriterOptions = new JsonWriterOptions
            {
                Indented = true
            };

            foreach (var inputFile in Options.InputFiles)
            {
                var realPath = Path.GetFullPath(inputFile);
                if (!File.Exists(realPath))
                {
                    logger.LogWarning("File not found: {File}", realPath);
                    continue;
                }

                JsonNode json;
                try
                {
                    await using var fileStream = File.OpenRead(realPath);
                    json = JsonNode.Parse(fileStream) ?? throw new InvalidOperationException();
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Failed to read {File} as a JSON document", realPath);
                    continue;
                }

                ScrubJson(patterns, json);
                logger.LogInformation("Scrubbed {File}", realPath);

                var outputFilename = Options.Overwrite
                    ? realPath
                    : Path.Combine
                        (
                            Options.OutputDirectory,
                            $"{Path.GetFileNameWithoutExtension(realPath)}.scrubbed.json"
                        );

                await using var outputFile = File.Create(outputFilename);
                await using var writer = new Utf8JsonWriter(outputFile, jsonWriterOptions);

                json.WriteTo(writer, jsonOptions);
            }

            return 0;
        }

        private static void ScrubJson(IReadOnlyDictionary<Regex, SensitivePattern> patterns, JsonNode node)
        {
            switch (node)
            {
                case JsonObject jsonObject:
                {
                    var replacements = new Dictionary<string, string>();
                    foreach (var (name, value) in jsonObject)
                    {
                        if (value is null)
                        {
                            continue;
                        }

                        var nameRegex = patterns.Keys.FirstOrDefault(key => key.IsMatch(name));
                        if (nameRegex is null)
                        {
                            continue;
                        }

                        var (valuePattern, replacement) = patterns[nameRegex];
                        var valueString = value.ToJsonString();

                        if (valuePattern.IsMatch(valueString))
                        {
                            replacements.Add(name, replacement);
                        }
                    }

                    foreach (var (name, replacement) in replacements)
                    {
                        jsonObject[name] = JsonNode.Parse(replacement);
                    }

                    break;
                }
                case JsonArray jsonArray:
                {
                    foreach (var jsonObject in jsonArray)
                    {
                        if (jsonObject is null)
                        {
                            continue;
                        }

                        ScrubJson(patterns, jsonObject);
                    }

                    break;
                }
            }
        }
    }
}
