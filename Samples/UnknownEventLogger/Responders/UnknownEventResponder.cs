//
//  UnknownEventResponder.cs
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
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Remora.Discord.Samples.UnknownEventLogger.Responders;

/// <summary>
/// Responds to unknown events, logging them to disk.
/// </summary>
public class UnknownEventResponder : IResponder<IUnknownEvent>
{
    private readonly ILogger<UnknownEventResponder> _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownEventResponder"/> class.
    /// </summary>
    /// <param name="log">The logging instance.</param>
    public UnknownEventResponder(ILogger<UnknownEventResponder> log)
    {
        _log = log;
    }

    /// <inheritdoc/>
    public async Task<Result> RespondAsync
    (
        IUnknownEvent? gatewayEvent,
        CancellationToken ct = default
    )
    {
        if (gatewayEvent is null)
        {
            return Result.FromSuccess();
        }

        using var jsonDocument = JsonDocument.Parse(gatewayEvent.Data);
        if (!jsonDocument.RootElement.TryGetProperty("t", out var eventTypeElement))
        {
            _log.LogWarning("Failed to find an event type on the given unknown event");
            return Result.FromSuccess();
        }

        if (!jsonDocument.RootElement.TryGetProperty("s", out var eventSequenceElement))
        {
            _log.LogWarning("Failed to find an event sequence on the given unknown event");
            return Result.FromSuccess();
        }

        var eventType = eventTypeElement.GetString();
        if (eventType is null)
        {
            return new InvalidOperationError("The event type was null.");
        }

        var sequenceNumber = eventSequenceElement.GetInt64();
        var logTime = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd");

        _log.LogInformation("Received an event of type \"{EventType}\"", eventType);

        var eventDirectory = Path.GetFullPath(eventType.ToUpperInvariant());
        if (!Directory.Exists(eventDirectory))
        {
            Directory.CreateDirectory(eventDirectory);
        }

        var filename = $"{logTime}.{sequenceNumber}.json";
        var filePath = Path.Combine(eventDirectory, filename);

        await using var file = File.OpenWrite(filePath);
        await using var jsonWriter = new Utf8JsonWriter(file, new JsonWriterOptions
        {
            Indented = true
        });

        jsonDocument.WriteTo(jsonWriter);

        return Result.FromSuccess();
    }
}
