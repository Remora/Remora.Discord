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
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Gateway.Events;
using Remora.Results;

namespace Remora.Discord.API.Json
{
    /// <inheritdoc />
    internal class PayloadConverter : JsonConverter<IPayload?>
    {
        private readonly SnakeCaseNamingPolicy _snakeCase = new SnakeCaseNamingPolicy();

        /// <summary>
        /// Gets a value indicating whether unknown events are allowed to be deserialized.
        /// </summary>
        private readonly bool _allowUnknownEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadConverter"/> class.
        /// </summary>
        /// <param name="allowUnknownEvents">Whether unknown events are allowed to be deserialized.</param>
        public PayloadConverter(bool allowUnknownEvents = true)
        {
            _allowUnknownEvents = allowUnknownEvents;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Contains(typeof(IPayload)) || objectType == typeof(IPayload);
        }

        /// <inheritdoc />
        public override IPayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

                if (value is IEventPayload eventPayload)
                {
                    var dataType = genericArguments[0];
                    var dataName = _snakeCase.ConvertName(dataType.Name.Substring(1)).ToUpperInvariant();

                    var nameToWrite = eventPayload.EventName != dataName
                        ? eventPayload.EventName
                        : dataName;

                    writer.WriteString("t", nameToWrite);

                    writer.WriteNumber("s", eventPayload.SequenceNumber);
                }
                else
                {
                    writer.WriteNull("t");
                    writer.WriteNull("s");
                }
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
                if (payloadData is UnknownEvent unknownEvent)
                {
                    using var eventData = JsonDocument.Parse(unknownEvent.Data);
                    var innerData = eventData.RootElement.GetProperty("d");

                    innerData.WriteTo(writer);
                }
                else
                {
                    JsonSerializer.Serialize(writer, payloadData, payloadDataProperty.PropertyType, options);
                }
            }

            writer.WriteEndObject();
        }

        private RetrieveEntityResult<OperationCode> GetOperationCode(Type objectType)
        {
            if (!objectType.IsGenericType)
            {
                return RetrieveEntityResult<OperationCode>.FromError("Unable to determine operation code.");
            }

            if (objectType.GetGenericTypeDefinition() == typeof(EventPayload<>))
            {
                return OperationCode.Dispatch;
            }

            var dataType = objectType.GetGenericArguments()[0];

            return dataType switch
            {
                // Commands
                _ when typeof(IHeartbeat).IsAssignableFrom(dataType)
                => OperationCode.Heartbeat,

                _ when typeof(IIdentify).IsAssignableFrom(dataType)
                => OperationCode.Identify,

                _ when typeof(IRequestGuildMembers).IsAssignableFrom(dataType)
                => OperationCode.RequestGuildMembers,

                _ when typeof(IResume).IsAssignableFrom(dataType)
                => OperationCode.Resume,

                _ when typeof(IUpdateStatus).IsAssignableFrom(dataType)
                => OperationCode.PresenceUpdate,

                _ when typeof(IUpdateVoiceState).IsAssignableFrom(dataType)
                => OperationCode.VoiceStateUpdate,

                // Events
                _ when typeof(IHello).IsAssignableFrom(dataType)
                => OperationCode.Hello,

                _ when typeof(IHeartbeatAcknowledge).IsAssignableFrom(dataType)
                => OperationCode.HeartbeatAcknowledge,

                _ when typeof(IInvalidSession).IsAssignableFrom(dataType)
                => OperationCode.InvalidSession,

                _ when typeof(IReconnect).IsAssignableFrom(dataType)
                => OperationCode.Reconnect,

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
            var eventNamespace = typeof(IHello).Namespace;
            var eventTypes = typeof(IHello).Assembly.ExportedTypes
                .Where(t => t.Namespace == eventNamespace)
                .Where(t => t.IsInterface);

            var eventType = eventTypes.FirstOrDefault
            (
                t => _snakeCase.ConvertName(t.Name.Substring(1)).ToUpperInvariant() == eventName
            );

            if (eventType is null)
            {
                if (!_allowUnknownEvents)
                {
                    throw new InvalidOperationException("No matching implementation interface could be found.");
                }

                return new EventPayload<IUnknownEvent>
                (
                    eventName,
                    sequenceNumber,
                    OperationCode.Dispatch,
                    new UnknownEvent(document.RootElement.GetRawText())
                );
            }

            object? eventData;
            try
            {
                eventData = JsonSerializer.Deserialize
                (
                    dataElement.GetRawText(), eventType, options
                );
            }
            catch
            {
                if (!_allowUnknownEvents)
                {
                    throw;
                }

                return new EventPayload<IUnknownEvent>
                (
                    eventName,
                    sequenceNumber,
                    OperationCode.Dispatch,
                    new UnknownEvent(document.RootElement.GetRawText())
                );
            }

            var payloadConstructor = typeof(EventPayload<>)
                .MakeGenericType(eventType)
                .GetConstructor(new[] { typeof(string), typeof(int), typeof(OperationCode), eventType, });

            if (payloadConstructor is null)
            {
                throw new JsonException();
            }

            var eventObject = payloadConstructor.Invoke
            (
                new[]
                {
                    eventName,
                    sequenceNumber,
                    OperationCode.Dispatch,
                    eventData
                }
            );

            if (!(eventObject is IPayload))
            {
                throw new JsonException();
            }

            return (IPayload)eventObject;
        }
    }
}
