//
//  VoicePayloadConverter.cs
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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Abstractions.Voice.Gateway;
using Remora.Discord.API.Abstractions.Voice.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Voice.Gateway.Commands;
using Remora.Discord.API.Abstractions.Voice.Gateway.Events;
using Remora.Discord.API.Voice.Gateway;
using Remora.Discord.API.Voice.Gateway.Events;
using Remora.Results;

namespace Remora.Discord.API.Json
{
    /// <inheritdoc />
    internal class VoicePayloadConverter : JsonConverter<IVoicePayload?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoicePayloadConverter"/> class.
        /// </summary>
        public VoicePayloadConverter()
        {
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Contains(typeof(IVoicePayload)) || objectType == typeof(IVoicePayload);
        }

        /// <inheritdoc />
        public override IVoicePayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

            var operationCode = JsonSerializer.Deserialize<VoiceOperationCode>(operationCodeProperty.GetRawText(), options);

            if (operationCode == (VoiceOperationCode)12)
            {
                System.Diagnostics.Debug.WriteLine(document.RootElement.GetRawText());
                throw new JsonException();
            }

            var obj = operationCode switch
            {
                // Bidirectional
                VoiceOperationCode.Heartbeat => DeserializePayload<IVoiceHeartbeat>(realDocument, options),
                VoiceOperationCode.HeartbeatAcknowledgement => DeserializePayload<IVoiceHeartbeatAcknowledge>(realDocument, options),
                VoiceOperationCode.Speaking => DeserializePayload<IVoiceSpeaking>(realDocument, options),

                // Commands
                VoiceOperationCode.Identify => DeserializePayload<IVoiceIdentify>(realDocument, options),
                VoiceOperationCode.Resume => DeserializePayload<IVoiceResume>(realDocument, options),
                VoiceOperationCode.SelectProtocol => DeserializePayload<IVoiceSelectProtocol>(realDocument, options),

                // Events
                VoiceOperationCode.ClientDisconnect => DeserializePayload<IVoiceClientDisconnect>(realDocument, options),
                VoiceOperationCode.Hello => DeserializePayload<IVoiceHello>(realDocument, options),
                VoiceOperationCode.Ready => DeserializePayload<IVoiceReady>(realDocument, options),
                VoiceOperationCode.Resumed => new VoicePayload<VoiceResumed>(new VoiceResumed()),
                VoiceOperationCode.SessionDescription => DeserializePayload<IVoiceSessionDescription>(realDocument, options),

                // Other
                _ => throw new ArgumentOutOfRangeException(),
            };

            return obj;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IVoicePayload? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var getVoiceOperationCode = GetVoiceOperationCode(value.GetType());
            if (!getVoiceOperationCode.IsSuccess)
            {
                throw new JsonException();
            }

            var operationCode = getVoiceOperationCode.Entity;

            writer.WriteStartObject();
            writer.WriteNumber("op", (int)operationCode);
            if (!value.GetType().IsGenericType)
            {
                throw new JsonException();
            }

            var genericArguments = value.GetType().GetGenericArguments();
            if (genericArguments.Length <= 0)
            {
                throw new JsonException();
            }

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
            JsonSerializer.Serialize(writer, payloadData, payloadDataProperty.PropertyType, options);

            writer.WriteEndObject();
        }

        private static Result<VoiceOperationCode> GetVoiceOperationCode(Type objectType)
        {
            if (!objectType.IsGenericType)
            {
                return new NotSupportedError("Unable to determine operation code.");
            }

            var dataType = objectType.GetGenericArguments()[0];

            return dataType switch
            {
                // Bidirectional
                _ when typeof(IVoiceSpeaking).IsAssignableFrom(dataType)
                => VoiceOperationCode.Speaking,

                // Commands
                _ when typeof(IVoiceHeartbeat).IsAssignableFrom(dataType)
                => VoiceOperationCode.Heartbeat,

                _ when typeof(IVoiceIdentify).IsAssignableFrom(dataType)
                => VoiceOperationCode.Identify,

                _ when typeof(IVoiceResume).IsAssignableFrom(dataType)
                => VoiceOperationCode.Resume,

                _ when typeof(IVoiceSelectProtocol).IsAssignableFrom(dataType)
                => VoiceOperationCode.SelectProtocol,

                // Events
                _ when typeof(IVoiceClientDisconnect).IsAssignableFrom(dataType)
                => VoiceOperationCode.ClientDisconnect,

                _ when typeof(IVoiceHeartbeatAcknowledge).IsAssignableFrom(dataType)
                => VoiceOperationCode.HeartbeatAcknowledgement,

                _ when typeof(IVoiceReady).IsAssignableFrom(dataType)
                => VoiceOperationCode.Hello,

                _ when typeof(IVoiceReady).IsAssignableFrom(dataType)
                => VoiceOperationCode.Ready,

                _ when typeof(IVoiceResumed).IsAssignableFrom(dataType)
                => VoiceOperationCode.Resumed,

                _ when typeof(IVoiceSessionDescription).IsAssignableFrom(dataType)
                => VoiceOperationCode.SessionDescription,

                // Other
                _ => new NotSupportedError("Unknown operation code.")
            };
        }

        private static IVoicePayload DeserializePayload<TData>(JsonDocument document, JsonSerializerOptions options)
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

            return new VoicePayload<TData>(data);
        }
    }
}
