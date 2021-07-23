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
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.API.Abstractions.Gateway.Bidirectional;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Gateway;
using Remora.Discord.API.Gateway.Bidirectional;
using Remora.Discord.API.Gateway.Events;
using Remora.Results;

namespace Remora.Discord.API.Json
{
    /// <inheritdoc />
    internal class PayloadConverter : JsonConverter<IPayload?>
    {
        private readonly SnakeCaseNamingPolicy _snakeCase = new();

        /// <summary>
        /// Holds a value indicating whether unknown events are allowed to be deserialized.
        /// </summary>
        private readonly bool _allowUnknownEvents;

        /// <summary>
        /// Holds a cache of event names to event types.
        /// </summary>
        private readonly ConcurrentDictionary<string, Type?> _eventTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadConverter"/> class.
        /// </summary>
        /// <param name="allowUnknownEvents">Whether unknown events are allowed to be deserialized.</param>
        public PayloadConverter(bool allowUnknownEvents = true)
        {
            _allowUnknownEvents = allowUnknownEvents;
            _eventTypes = new ConcurrentDictionary<string, Type?>();
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

            var operationCode = JsonSerializer.Deserialize<OperationCode>(operationCodeProperty.GetRawText(), options);
            var obj = operationCode switch
            {
                // Commands
                OperationCode.Heartbeat => DeserializePayload<IHeartbeat>(realDocument, options),
                OperationCode.Identify => DeserializePayload<IIdentify>(realDocument, options),
                OperationCode.RequestGuildMembers => DeserializePayload<IRequestGuildMembers>(realDocument, options),
                OperationCode.Resume => DeserializePayload<IResume>(realDocument, options),
                OperationCode.PresenceUpdate => DeserializePayload<IUpdatePresence>(realDocument, options),
                OperationCode.VoiceStateUpdate => DeserializePayload<IUpdateVoiceState>(realDocument, options),

                // Events
                OperationCode.Hello => DeserializePayload<IHello>(realDocument, options),
                OperationCode.Reconnect => new Payload<Reconnect>(new Reconnect()),
                OperationCode.InvalidSession => DeserializePayload<IInvalidSession>(realDocument, options),
                OperationCode.HeartbeatAcknowledge => new Payload<HeartbeatAcknowledge>(new HeartbeatAcknowledge()),
                OperationCode.Dispatch => DeserializeDispatch(realDocument, options),

                // Other
                OperationCode.Unknown => DeserializePayload<IUnknownEvent>(realDocument, options),
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
                    var dataName = _snakeCase.ConvertName(dataType.Name[1..]).ToUpperInvariant();

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

            var payloadData = payloadDataPropertyGetter.Invoke(value, null);
            switch (payloadData)
            {
                case Reconnect or HeartbeatAcknowledge:
                {
                    writer.WriteNullValue();
                    break;
                }
                case UnknownEvent unknownEvent:
                {
                    using var eventData = JsonDocument.Parse(unknownEvent.Data);
                    var innerData = eventData.RootElement.GetProperty("d");

                    innerData.WriteTo(writer);
                    break;
                }
                default:
                {
                    JsonSerializer.Serialize(writer, payloadData, payloadDataProperty.PropertyType, options);
                    break;
                }
            }

            writer.WriteEndObject();
        }

        private static Result<OperationCode> GetOperationCode(Type objectType)
        {
            if (!objectType.IsGenericType)
            {
                return new NotSupportedError("Unable to determine operation code.");
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

                _ when typeof(IUpdatePresence).IsAssignableFrom(dataType)
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
                _ => new NotSupportedError("Unknown operation code.")
            };
        }

        private static IPayload DeserializePayload<TData>(JsonDocument document, JsonSerializerOptions options)
            where TData : IGatewayPayloadData
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

            return new Payload<TData>(data);
        }

        private IPayload DeserializeDispatch
        (
            JsonDocument document,
            JsonSerializerOptions options
        )
        {
            if (!document.RootElement.TryGetProperty("d", out var dataProperty))
            {
                throw new JsonException();
            }

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
            if (eventName is null)
            {
                throw new JsonException();
            }

            if (!_eventTypes.TryGetValue(eventName, out var eventType))
            {
                var convertibleTypes = options.Converters.Where
                (
                    c =>
                    {
                        var converterType = c.GetType();
                        if (!converterType.IsGenericType)
                        {
                            return false;
                        }

                        var genericConverterType = converterType.GetGenericTypeDefinition();
                        return genericConverterType == typeof(DataObjectConverter<,>);
                    }
                )
                .Select(c => c.GetType().GetGenericArguments()[0]);

                eventType = convertibleTypes.FirstOrDefault
                (
                    t => _snakeCase.ConvertName(t.Name[1..]).ToUpperInvariant() == eventName
                );

                _eventTypes.TryAdd(eventName, eventType);
            }

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
                    dataProperty.GetRawText(), eventType, options
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
                .GetConstructor(new[] { typeof(string), typeof(int), typeof(OperationCode), eventType });

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

            if (eventObject is not IPayload)
            {
                throw new JsonException();
            }

            return (IPayload)eventObject;
        }
    }
}
