//
//  PayloadConverter.cs
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
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Bidirectional;
using Remora.Discord.API.Abstractions.Commands;
using Remora.Discord.API.Abstractions.Events;
using Remora.Discord.API.API;
using Remora.Discord.API.API.Events;
using Remora.Results;

namespace Remora.Discord.API.Json
{
    /// <inheritdoc />
    public class PayloadConverter : JsonConverter<IPayload?>
    {
        private readonly SnakeCaseNamingPolicy _snakeCase = new SnakeCaseNamingPolicy();

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Contains(typeof(IPayload)) || objectType == typeof(IPayload);
        }

        /// <inheritdoc />
        public override IPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

            if (!realDocument.RootElement.TryGetProperty("d", out var dataElement))
            {
                throw new JsonException();
            }

            var operationCode = JsonSerializer.Deserialize<OperationCode>(operationCodeProperty.GetRawText(), options);
            var obj = operationCode switch
            {
                // Commands
                OperationCode.Heartbeat => DeserializePayload<IHeartbeat>(dataElement, options),
                OperationCode.Identify => DeserializePayload<IIdentify>(dataElement, options),
                OperationCode.RequestGuildMembers => DeserializePayload<IRequestGuildMembers>(dataElement, options),
                OperationCode.Resume => DeserializePayload<IResume>(dataElement, options),
                OperationCode.PresenceUpdate => DeserializePayload<IUpdateStatus>(dataElement, options),
                OperationCode.VoiceStateUpdate => DeserializePayload<IUpdateVoiceState>(dataElement, options),

                // Events
                OperationCode.Hello => DeserializePayload<IHello>(dataElement, options),
                OperationCode.Reconnect => DeserializePayload<IReconnect>(dataElement, options),
                OperationCode.InvalidSession => DeserializePayload<IInvalidSession>(dataElement, options),
                OperationCode.HeartbeatAcknowledge => DeserializePayload<IHeartbeatAcknowledge>(dataElement, options),
                OperationCode.Dispatch => DeserializeDispatch(realDocument, dataElement, options),

                // Other
                OperationCode.Unknown => DeserializePayload<IUnknownEvent>(dataElement, options),
                _ => throw new ArgumentOutOfRangeException()
            };

            return obj;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IPayload? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var getOperationCode = GetOperationCode(value.GetType());
            if (!getOperationCode.IsSuccess)
            {
                throw new JsonException();
            }

            var operationCode = getOperationCode.Entity;

            writer.WriteStartObject();
            writer.WriteNumber("op", (int)operationCode);
            if (operationCode != OperationCode.Dispatch)
            {
                writer.WriteNull("s");
                writer.WriteNull("t");
            }
            else
            {
                if (!value.GetType().IsGenericType)
                {
                    throw new JsonException();
                }

                var genericArguments = value.GetType().GetGenericArguments();
                if (genericArguments.Length <= 0)
                {
                    throw new JsonException();
                }

                var dataType = genericArguments[0];

                var dataName = _snakeCase.ConvertName(dataType.Name).ToUpperInvariant();
                writer.WriteString("t", dataName);
            }

            writer.WritePropertyName("d");

            // We're using IHeartbeat here as a dummy type
            var payloadDataProperty = value.GetType().GetProperty(nameof(Payload<IHeartbeat>.Data));
            if (payloadDataProperty is null)
            {
                throw new JsonException();
            }

            var payloadDataPropertyGetter = payloadDataProperty.GetGetMethod();
            if (payloadDataPropertyGetter is null)
            {
                throw new JsonException();
            }

            var payloadData = payloadDataPropertyGetter.Invoke(value, new object?[] { });
            if (payloadData is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                JsonSerializer.Serialize(writer, payloadData, payloadDataProperty.PropertyType, options);
            }

            writer.WriteEndObject();
        }

        private RetrieveEntityResult<OperationCode> GetOperationCode(Type objectType)
        {
            return objectType switch
            {
                // Commands
                _ when objectType == typeof(Payload<IHeartbeat>) => OperationCode.Heartbeat,
                _ when objectType == typeof(Payload<IIdentify>) => OperationCode.Identify,
                _ when objectType == typeof(Payload<IRequestGuildMembers>) => OperationCode.RequestGuildMembers,
                _ when objectType == typeof(Payload<IResume>) => OperationCode.Resume,
                _ when objectType == typeof(Payload<IUpdateStatus>) => OperationCode.PresenceUpdate,
                _ when objectType == typeof(Payload<IUpdateVoiceState>) => OperationCode.VoiceStateUpdate,

                // Events
                _ when objectType == typeof(Payload<IHello>) => OperationCode.Hello,
                _ when objectType == typeof(Payload<IHeartbeatAcknowledge>) => OperationCode.HeartbeatAcknowledge,
                _ when objectType == typeof(Payload<IInvalidSession>) => OperationCode.InvalidSession,
                _ when objectType == typeof(Payload<IReconnect>) => OperationCode.Reconnect,
                _ when objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(EventPayload<>)
                => OperationCode.Dispatch,

                // Other
                _ => RetrieveEntityResult<OperationCode>.FromError("Unknown operation code.")
            };
        }

        private IPayload DeserializePayload<TData>(JsonElement dataProperty, JsonSerializerOptions options)
            where TData : IGatewayPayloadData
        {
            var data = JsonSerializer.Deserialize<TData>(dataProperty.GetRawText(), options);
            return new Payload<TData>(data);
        }

        private IPayload DeserializeDispatch
        (
            JsonDocument document,
            JsonElement dataElement,
            JsonSerializerOptions options
        )
        {
            if (!document.RootElement.TryGetProperty("t", out var eventNameProperty))
            {
                throw new JsonException();
            }

            if (!document.RootElement.TryGetProperty("s", out var sequenceNumberProperty))
            {
                throw new JsonException();
            }

            var sequenceNumber = sequenceNumberProperty.GetInt32();

            var eventName = eventNameProperty.GetString();
            var eventNamespace = typeof(Hello).Namespace;
            var eventTypes = typeof(Hello).Assembly.ExportedTypes.Where(t => t.Namespace == eventNamespace);

            var eventType = eventTypes.FirstOrDefault
            (
                t => _snakeCase.ConvertName(t.Name).ToUpperInvariant() == eventName
            );

            if (eventType is null)
            {
                return new EventPayload<IUnknownEvent>(new UnknownEvent(dataElement.GetRawText()), sequenceNumber);
            }

            var eventData = JsonSerializer.Deserialize
            (
                dataElement.GetRawText(), eventType, options
            );

            var payloadConstructor = typeof(EventPayload<>).GetConstructor(new[] { eventType, typeof(int) });
            if (payloadConstructor is null)
            {
                throw new JsonException();
            }

            var eventObject = payloadConstructor.Invoke(new[] { eventData, sequenceNumber });

            if (!(eventObject is IPayload))
            {
                throw new JsonException();
            }

            return (IPayload)eventData;
        }
    }
}
