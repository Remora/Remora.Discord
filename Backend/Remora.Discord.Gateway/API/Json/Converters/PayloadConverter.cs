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
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Remora.Discord.API.Abstractions;
using Remora.Discord.Gateway.API.Commands;
using Remora.Discord.Gateway.API.Events;
using Remora.Results;

namespace Remora.Discord.Gateway.API.Json.Converters
{
    /// <inheritdoc />
    public class PayloadConverter : JsonConverter
    {
        [field: ThreadStatic]
        private static bool IsDisabled { get; set; }

        /// <inheritdoc />
        public override bool CanRead => !IsDisabled;

        /// <inheritdoc />
        public override bool CanWrite => !IsDisabled;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Contains(typeof(IPayload)) || objectType == typeof(IPayload);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            IsDisabled = true;

            var getOperationCode = GetOperationCode(value.GetType());
            if (!getOperationCode.IsSuccess)
            {
                throw new JsonException();
            }

            var operationCode = getOperationCode.Entity;
            var jsonObject = JObject.FromObject(value, serializer);

            jsonObject.AddFirst(new JProperty("op", (long)operationCode));
            if (operationCode != OperationCode.Dispatch)
            {
                jsonObject.Add("s", null);
                jsonObject.Add("t", null);
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

                var dataName = dataType.Name.Underscore().Transform(To.UpperCase);
                jsonObject.Add("t", dataName);
            }

            jsonObject.WriteTo(writer);
            IsDisabled = false;
        }

        /// <inheritdoc/>
        public override object? ReadJson
        (
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            IsDisabled = true;

            var jsonObject = JObject.Load(reader);
            var operationCode = (OperationCode)jsonObject.Value<long>("op");

            var obj = operationCode switch
            {
                // Commands
                OperationCode.Heartbeat => jsonObject.ToObject<Payload<Heartbeat>>(serializer),
                OperationCode.Identify => jsonObject.ToObject<Payload<Identify>>(serializer),
                OperationCode.RequestGuildMembers => jsonObject.ToObject<Payload<RequestGuildMembers>>(serializer),
                OperationCode.Resume => jsonObject.ToObject<Payload<Resume>>(serializer),
                OperationCode.PresenceUpdate => jsonObject.ToObject<Payload<UpdateStatus>>(serializer),
                OperationCode.VoiceStateUpdate => jsonObject.ToObject<Payload<UpdateVoiceState>>(serializer),

                // Events
                OperationCode.Hello => jsonObject.ToObject<Payload<Hello>>(serializer),
                OperationCode.Dispatch => DeserializeDispatch(jsonObject, serializer),
                OperationCode.Unknown => throw new NotImplementedException(),
                OperationCode.Reconnect => throw new NotImplementedException(),
                OperationCode.InvalidSession => throw new NotImplementedException(),
                OperationCode.HeartbeatAcknowledge => throw new NotImplementedException(),
                _ => jsonObject.ToObject<Payload<JObject>>(serializer)
            };

            IsDisabled = false;

            return obj;
        }

        private RetrieveEntityResult<OperationCode> GetOperationCode(Type objectType)
        {
            return objectType switch
            {
                // Commands
                _ when objectType == typeof(Payload<Heartbeat>) => OperationCode.Heartbeat,
                _ when objectType == typeof(Payload<Identify>) => OperationCode.Identify,
                _ when objectType == typeof(Payload<RequestGuildMembers>) => OperationCode.RequestGuildMembers,
                _ when objectType == typeof(Payload<Resume>) => OperationCode.Resume,
                _ when objectType == typeof(Payload<UpdateStatus>) => OperationCode.PresenceUpdate,
                _ when objectType == typeof(Payload<UpdateVoiceState>) => OperationCode.VoiceStateUpdate,

                // Events
                _ when objectType == typeof(Payload<Hello>) => OperationCode.Hello,
                _ when objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(EventPayload<>)
                => OperationCode.Dispatch,

                // _ when objectType == typeof(Payload<Reconnect>) => OperationCode.Reconnect,
                // _ when objectType == typeof(Payload<InvalidSession>) => OperationCode.InvalidSession,
                // _ when objectType == typeof(Payload<HeartbeatAcknowledge>) => OperationCode.HeartbeatAcknowledge,
                _ => RetrieveEntityResult<OperationCode>.FromError("Unknown operation code.")
            };
        }

        private IPayload? DeserializeDispatch(JObject jsonObject, JsonSerializer serializer)
        {
            var eventName = jsonObject.Value<string>("t");
            var eventNamespace = typeof(Hello).Namespace;
            var eventTypes = typeof(Hello).Assembly.ExportedTypes.Where(t => t.Namespace == eventNamespace);

            var eventType = eventTypes.FirstOrDefault(t => t.Name == eventName.Pascalize());
            if (eventType is null)
            {
                return jsonObject.ToObject<EventPayload<JObject>>(serializer);
            }

            var eventObject = jsonObject.ToObject(typeof(EventPayload<>).MakeGenericType(eventType), serializer);
            if (!(eventObject is IPayload))
            {
                throw new JsonException();
            }

            return (IPayload)eventObject;
        }
    }
}
