//
//  DiscordJsonService.cs
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

using Newtonsoft.Json;
using Remora.Discord.API.Abstractions.Activities;
using Remora.Discord.API.Abstractions.Commands;
using Remora.Discord.API.Abstractions.Events;
using Remora.Discord.Gateway.API.Commands;
using Remora.Discord.Gateway.API.Events;
using Remora.Discord.Gateway.API.Json.ContractResolvers;
using Remora.Discord.Gateway.API.Json.Converters;
using Remora.Discord.Gateway.API.Objects;

namespace Remora.Discord.Gateway.Services
{
    /// <summary>
    /// Business logic for Discord's JSON de/serialization.
    /// </summary>
    public class DiscordJsonService
    {
        /// <summary>
        /// Gets the serializes in use by the service.
        /// </summary>
        public JsonSerializer Serializer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordJsonService"/> class.
        /// </summary>
        public DiscordJsonService()
        {
            this.Serializer = new JsonSerializer
            {
                ContractResolver = new DiscordContractResolver()
            };

            this.Serializer.Converters.Add(new PartySizeConverter());
            this.Serializer.Converters.Add(new HeartbeatConverter());
            this.Serializer.Converters.Add(new ShardIdentificationConverter());
            this.Serializer.Converters.Add(new SnowflakeConverter());
            this.Serializer.Converters.Add(new PayloadConverter());

            this.Serializer.Converters.Add(new InterfaceConverter<IHeartbeat, Heartbeat>());
            this.Serializer.Converters.Add(new InterfaceConverter<IIdentify, Identify>());
            this.Serializer.Converters.Add(new InterfaceConverter<IConnectionProperties, ConnectionProperties>());
            this.Serializer.Converters.Add(new InterfaceConverter<IShardIdentification, ShardIdentification>());
            this.Serializer.Converters.Add(new InterfaceConverter<IHeartbeat, Heartbeat>());
            this.Serializer.Converters.Add(new InterfaceConverter<IRequestGuildMembers, RequestGuildMembers>());
            this.Serializer.Converters.Add(new InterfaceConverter<IResume, Resume>());
            this.Serializer.Converters.Add(new InterfaceConverter<IUpdateStatus, UpdateStatus>());
            this.Serializer.Converters.Add(new InterfaceConverter<IUpdateVoiceState, UpdateVoiceState>());
            this.Serializer.Converters.Add(new InterfaceConverter<IHello, Hello>());
            this.Serializer.Converters.Add(new InterfaceConverter<IActivity, Activity>());
            this.Serializer.Converters.Add(new InterfaceConverter<IActivityAssets, ActivityAssets>());
            this.Serializer.Converters.Add(new InterfaceConverter<IActivityEmoji, ActivityEmoji>());
            this.Serializer.Converters.Add(new InterfaceConverter<IActivityParty, ActivityParty>());
            this.Serializer.Converters.Add(new InterfaceConverter<IPartySize, PartySize>());
            this.Serializer.Converters.Add(new InterfaceConverter<IActivitySecrets, ActivitySecrets>());
            this.Serializer.Converters.Add(new InterfaceConverter<IActivityTimestamps, ActivityTimestamps>());
        }
    }
}
