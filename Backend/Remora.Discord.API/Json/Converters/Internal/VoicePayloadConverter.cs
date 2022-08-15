//
//  VoicePayloadConverter.cs
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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Abstractions.VoiceGateway;
using Remora.Discord.API.Abstractions.VoiceGateway.Commands;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Remora.Discord.API.VoiceGateway;
using Remora.Discord.API.VoiceGateway.Events;

namespace Remora.Discord.API.Json;

/// <inheritdoc />
internal class VoicePayloadConverter : JsonConverter<IVoicePayload?>
{
    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return objectType.GetInterfaces().Contains(typeof(IVoicePayload)) || objectType == typeof(IVoicePayload);
    }

    /// <inheritdoc />
    public override IVoicePayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var document))
        {
            throw new JsonException();
        }

        using var realDocument = document;

        if (!realDocument.RootElement.TryGetProperty("op", out var operationCodeProperty))
        {
            throw new JsonException();
        }

        var operationCode = (VoiceOperationCode)operationCodeProperty.GetInt32();

        return DeserializeFromOperationCode(operationCode, realDocument, options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IVoicePayload? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var payloadType = value.GetType();
        if (!payloadType.IsGenericType)
        {
            throw new JsonException("Invalid payload type.");
        }

        var payloadDataType = payloadType.GetGenericArguments()[0];
        var operationCode = GetVoiceOperationCode(payloadDataType);

        writer.WriteStartObject();
        writer.WriteNumber("op", (int)operationCode);

        writer.WritePropertyName("d");

        // We're using IVoiceHeartbeat here as a dummy type
        var payloadDataProperty = value.GetType().GetProperty(nameof(VoicePayload<IVoiceHeartbeat>.Data));
        if (payloadDataProperty is null)
        {
            throw new JsonException();
        }

        var payloadDataPropertyGetter = payloadDataProperty.GetGetMethod();
        if (payloadDataPropertyGetter is null)
        {
            throw new JsonException();
        }

        var payloadData = payloadDataPropertyGetter.Invoke(value, null);
        switch (payloadData)
        {
            case VoiceResumed:
                writer.WriteNullValue();
                break;
            default:
                JsonSerializer.Serialize(writer, payloadData, payloadDataProperty.PropertyType, options);
                break;
        }

        writer.WriteEndObject();
    }

    /// <summary>
    /// Deserializes a payload.
    /// </summary>
    /// <typeparam name="TData">The type of the payload data.</typeparam>
    /// <param name="operationCode">The OP code of the payload.</param>
    /// <param name="document">The JSON document to deserialize.</param>
    /// <param name="options">The JSON options to use.</param>
    /// <returns>The deserialized payload.</returns>
    /// <exception cref="JsonException">Thrown if the payload was invalid.</exception>
    protected static VoicePayload<TData> DeserializePayload<TData>(VoiceOperationCode operationCode, JsonDocument document, JsonSerializerOptions options)
        where TData : IVoiceGatewayPayloadData
    {
        if (!document.RootElement.TryGetProperty("d", out var dataProperty))
        {
            throw new JsonException();
        }

        var data = JsonSerializer.Deserialize<TData>(dataProperty.GetRawText(), options);

        if (data is null)
        {
            throw new JsonException();
        }

        return new VoicePayload<TData>(operationCode, data);
    }

    /// <summary>
    /// Gets the respective <see cref="VoiceOperationCode"/> for a payload data type.
    /// </summary>
    /// <param name="payloadDataType">The type of the payload data.</param>
    /// <returns>A result representing the determined <see cref="VoiceOperationCode"/> on success, or the given error otherwise.</returns>
    protected virtual VoiceOperationCode GetVoiceOperationCode(Type payloadDataType)
        => payloadDataType switch
        {
            // Commands
            _ when typeof(IVoiceIdentify).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Identify,

            _ when typeof(IVoiceSelectProtocol).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.SelectProtocol,

            _ when typeof(IVoiceHeartbeat).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Heartbeat,

            _ when typeof(IVoiceSpeakingCommand).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Speaking,

            _ when typeof(IVoiceResume).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Resume,

            // Events
            _ when typeof(IVoiceReady).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Ready,

            _ when typeof(IVoiceSessionDescription).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.SessionDescription,

            _ when typeof(IVoiceHeartbeatAcknowledge).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.HeartbeatAcknowledgement,

            _ when typeof(IVoiceHello).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Hello,

            _ when typeof(IVoiceResumed).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Resumed,

            // Other
            _ => throw new InvalidOperationException("Unable to determine operation code.")
        };

    /// <summary>
    /// Deserializes a payload based on the given operation code.
    /// </summary>
    /// <param name="operationCode">The operation code.</param>
    /// <param name="document">The document to deserialize.</param>
    /// <param name="options">The options to use.</param>
    /// <returns>The deserialized payload, or <c>null</c> if the operation code was not recognised.</returns>
    protected virtual IVoicePayload? DeserializeFromOperationCode(VoiceOperationCode operationCode, JsonDocument document, JsonSerializerOptions options)
        => operationCode switch
        {

            // Commands
            VoiceOperationCode.Identify => DeserializePayload<IVoiceIdentify>(VoiceOperationCode.Identify, document, options),
            VoiceOperationCode.SelectProtocol => DeserializePayload<IVoiceSelectProtocol>(VoiceOperationCode.SelectProtocol, document, options),
            VoiceOperationCode.Heartbeat => DeserializePayload<IVoiceHeartbeat>(VoiceOperationCode.Heartbeat, document, options),
            VoiceOperationCode.Speaking => DeserializePayload<IVoiceSpeakingCommand>(VoiceOperationCode.Speaking, document, options),
            VoiceOperationCode.Resume => DeserializePayload<IVoiceResume>(VoiceOperationCode.Resume, document, options),

            // Events
            VoiceOperationCode.Ready => DeserializePayload<IVoiceReady>(VoiceOperationCode.Ready, document, options),
            VoiceOperationCode.SessionDescription => DeserializePayload<IVoiceSessionDescription>(VoiceOperationCode.SessionDescription, document, options),
            VoiceOperationCode.HeartbeatAcknowledgement => DeserializePayload<IVoiceHeartbeatAcknowledge>(VoiceOperationCode.HeartbeatAcknowledgement, document, options),
            VoiceOperationCode.Hello => DeserializePayload<IVoiceHello>(VoiceOperationCode.Hello, document, options),
            VoiceOperationCode.Resumed => new VoicePayload<VoiceResumed>(VoiceOperationCode.Resumed, new VoiceResumed()),

            // If we don't recognise it (which often happens due to undocumented OP codes) we just return null.
            // It's not fantastically explicit design but it prevents generating an exception.
            _ => null
        };
}
