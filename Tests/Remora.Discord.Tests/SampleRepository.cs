//
//  SampleRepository.cs
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
using System.Reflection;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Rest.Extensions;

namespace Remora.Discord.Tests;

/// <summary>
/// Holds sample files.
/// </summary>
public static class SampleRepository
{
    /// <summary>
    /// Gets the loaded samples.
    /// </summary>
    private static IReadOnlyDictionary<Type, string> Samples { get; }

    /// <summary>
    /// Initializes static members of the <see cref="SampleRepository"/> class.
    /// </summary>
    static SampleRepository()
    {
        var samples = new Dictionary<Type, string>();

        var assembly = Assembly.GetExecutingAssembly();
        LoadTypeSamples(assembly, samples);

        Samples = samples;
    }

    /// <summary>
    /// Gets a JSON sample for the given type. The type may be a collection, in which a single-element array will be
    /// returned.
    /// </summary>
    /// <typeparam name="TSample">The type of the sample to get.</typeparam>
    /// <returns>The JSON data for the type.</returns>
    public static string Get<TSample>()
    {
        if (!typeof(TSample).IsGenericType || !typeof(TSample).GetGenericTypeDefinition().IsCollection())
        {
            return Samples[typeof(TSample)];
        }

        var elementType = typeof(TSample).GetGenericArguments().Single();
        return "[" + Samples[elementType] + "]";
    }

    private static void LoadTypeSamples(Assembly assembly, Dictionary<Type, string> samples)
    {
        var resourceNames = assembly.GetManifestResourceNames().Where(n => n.Contains(".Samples."));

        var interfaceTypes = typeof(IGuild).Assembly
            .GetTypes()
            .Where(t => t.Namespace is not null)
            .Where(t => t.Namespace!.Contains(".API."))
            .ToList();

        var concreteTypes = typeof(Guild).Assembly
            .GetTypes()
            .Where(t => t.Namespace is not null)
            .Where(t => t.Namespace!.Contains(".API."))
            .ToList();

        var experimentalTypes = typeof(Unstable.Extensions.ServiceCollectionExtensions).Assembly
            .GetTypes()
            .Where(t => t.Namespace is not null)
            .Where
            (
                t =>
                    t.Namespace!.Contains(".API.Abstractions.Objects") ||
                    t.Namespace!.Contains(".API.Objects")
            )
            .ToList();

        foreach (var resourceName in resourceNames)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                continue;
            }

            using var streamReader = new StreamReader(stream);
            var sampleData = streamReader.ReadToEnd();

            var resourceFileName = resourceName
                .Replace("Remora.Discord.Tests.Samples.", string.Empty)
                .Replace("_", string.Empty)
                .Replace(".json", string.Empty);

            resourceFileName = resourceFileName[(resourceFileName.LastIndexOf(".", StringComparison.Ordinal) + 1)..];

            var interfaceType = interfaceTypes.Concat(experimentalTypes).FirstOrDefault
            (
                i => i.Name.Equals("I" + resourceFileName, StringComparison.OrdinalIgnoreCase)
            );

            var concreteType = concreteTypes.Concat(experimentalTypes).FirstOrDefault
            (
                t => t.Name.Equals(resourceFileName, StringComparison.OrdinalIgnoreCase)
            );

            if (interfaceType is null || concreteType is null)
            {
                continue;
            }

            samples.TryAdd(interfaceType, sampleData);
            samples.TryAdd(concreteType, sampleData);
        }
    }
}
