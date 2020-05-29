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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Remora.Discord.Gateway.API.Commands;
using Remora.Discord.Gateway.API.Events;

namespace Remora.Discord.Gateway.API.Json.Converters
{
    /// <inheritdoc />
    public class PayloadConverter : CustomCreationConverter<IPayload>
    {
        /// <inheritdoc/>
        public override IPayload Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPayload);
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
            var jsonObject = JObject.Load(reader);
            var operationCode = (OperationCode)jsonObject.Value<long>("op");

            return operationCode switch
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
        }

        private IPayload DeserializeDispatch(JObject jsonObject, JsonSerializer serializer)
        {
            var eventName = jsonObject.Value<string>("t");

            throw new NotImplementedException();
        }
    }
}
