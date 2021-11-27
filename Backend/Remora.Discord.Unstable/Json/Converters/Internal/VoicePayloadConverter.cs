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
using System.Text.Json;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.VoiceGateway;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;

namespace Remora.Discord.Json.Converters.Internal;

/// <inheritdoc />
[PublicAPI]
public class VoicePayloadConverter : API.Json.Converters.Internal.VoicePayloadConverter
{
    /// <inheritdoc />
    protected override IVoicePayload? DeserializeFromOperationCode(VoiceOperationCode operationCode, JsonDocument document, JsonSerializerOptions options)
        => operationCode switch
        {
            VoiceOperationCode.Speaking => DeserializePayload<IVoiceSpeakingEvent>(VoiceOperationCode.Speaking, document, options),
            VoiceOperationCode.ClientDisconnect => DeserializePayload<IVoiceClientDisconnect>(VoiceOperationCode.ClientDisconnect, document, options),

            _ => base.DeserializeFromOperationCode(operationCode, document, options)
        };

    /// <inheritdoc />
    protected override VoiceOperationCode GetVoiceOperationCode(Type payloadDataType)
        => payloadDataType switch
        {
            _ when typeof(IVoiceSpeakingEvent).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.Speaking,

            _ when typeof(IVoiceClientDisconnect).IsAssignableFrom(payloadDataType)
            => VoiceOperationCode.ClientDisconnect,

            // Other
            _ => base.GetVoiceOperationCode(payloadDataType)
        };
}
