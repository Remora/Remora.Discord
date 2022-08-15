//
//  Program.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Rest.Json.Policies;

namespace Remora.Discord.SensitiveDataScrubber;

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
            .BuildServiceProvider(true);

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Loading patterns...");

        if (!File.Exists("patterns.json"))
        {
            logger.LogCritical("Failed to load pattern file");
            return 1;
        }

        var encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            Converters = { new RegexConverter() },
            WriteIndented = true,
            Encoder = encoder
        };

        var jsonWriterOptions = new JsonWriterOptions
        {
            Indented = true,
            Encoder = encoder
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

        var actualFiles = new List<string>();
        foreach (var inputFile in Options.InputFiles)
        {
            var realPath = Path.GetFullPath(inputFile);

            if (Directory.Exists(inputFile))
            {
                // This is a directory, so we'll enumerate json files in it
                var jsonFiles = Directory.EnumerateFiles(inputFile, "*.json", SearchOption.AllDirectories);
                actualFiles.AddRange(jsonFiles);

                continue;
            }

            if (File.Exists(realPath))
            {
                actualFiles.Add(inputFile);
                continue;
            }

            logger.LogWarning("File not found: {File}", realPath);
        }

        foreach (var actualFile in actualFiles)
        {
            JsonNode json;
            try
            {
                await using var fileStream = File.OpenRead(actualFile);
                json = JsonNode.Parse(fileStream) ?? throw new InvalidOperationException();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Failed to read {File} as a JSON document", actualFile);
                continue;
            }

            if (!ScrubJson(patterns, jsonOptions, json))
            {
                continue;
            }

            logger.LogInformation("Scrubbed {File}", actualFile);

            var outputFilename = Options.Overwrite
                ? actualFile
                : Path.Combine
                (
                    Options.OutputDirectory,
                    $"{Path.GetFileNameWithoutExtension(actualFile)}.scrubbed.json"
                );

            await using var outputFile = File.Create(outputFilename);
            await using var writer = new Utf8JsonWriter(outputFile, jsonWriterOptions);

            json.WriteTo(writer, jsonOptions);
        }

        return 0;
    }

    private static bool ScrubJson
    (
        IReadOnlyDictionary<Regex, SensitivePattern> patterns,
        JsonSerializerOptions jsonOptions,
        JsonNode node
    )
    {
        var modifiedJson = false;

        switch (node)
        {
            case JsonObject jsonObject:
            {
                var replacements = new Dictionary<string, string>();
                foreach (var (name, value) in jsonObject)
                {
                    switch (value)
                    {
                        case null:
                        {
                            continue;
                        }
                        case JsonObject or JsonArray:
                        {
                            modifiedJson |= ScrubJson(patterns, jsonOptions, value);
                            continue;
                        }
                    }

                    foreach (var key in patterns.Keys)
                    {
                        if (!key.IsMatch(name))
                        {
                            continue;
                        }

                        var (valuePattern, replacement) = patterns[key];
                        var valueString = value.ToJsonString(jsonOptions);

                        if (!valuePattern.IsMatch(valueString))
                        {
                            continue;
                        }

                        replacements.Add(name, replacement);
                        break;
                    }
                }

                foreach (var (name, replacement) in replacements)
                {
                    jsonObject[name] = JsonNode.Parse(replacement);
                    modifiedJson = true;
                }

                break;
            }
            case JsonArray jsonArray:
            {
                var replacements = new Dictionary<int, string>();
                for (var i = 0; i < jsonArray.Count; i++)
                {
                    var jsonObject = jsonArray[(Index)i];
                    switch (jsonObject)
                    {
                        case null:
                        {
                            continue;
                        }
                        case JsonObject or JsonArray:
                        {
                            modifiedJson |= ScrubJson(patterns, jsonOptions, jsonObject);
                            continue;
                        }
                    }

                    var name = jsonArray.GetPath().Split('.')[^1];
                    foreach (var key in patterns.Keys)
                    {
                        if (!key.IsMatch(name))
                        {
                            continue;
                        }

                        var (valuePattern, replacement) = patterns[key];
                        var valueString = jsonObject.ToJsonString(jsonOptions);

                        if (!valuePattern.IsMatch(valueString))
                        {
                            continue;
                        }

                        replacements.Add(i, replacement);
                        break;
                    }
                }

                foreach (var (index, replacement) in replacements)
                {
                    jsonArray[index] = JsonNode.Parse(replacement);
                    modifiedJson = true;
                }

                break;
            }
        }

        return modifiedJson;
    }
}
