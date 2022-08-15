//
//  IVoicePayload.cs
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

using JetBrains.Annotations;

#pragma warning disable SA1402

namespace Remora.Discord.API.Abstractions.VoiceGateway;

/// <summary>
/// Marker interface for voice payload objects.
/// </summary>
[PublicAPI]
public interface IVoicePayload
{
    /// <summary>
    /// Gets the operation code of the payload.
    /// </summary>
    VoiceOperationCode OperationCode { get; }
}

/// <summary>
/// Marker interface for payload classes.
/// </summary>
/// <typeparam name="TData">The data contained in the payload.</typeparam>
[PublicAPI]
public interface IVoicePayload<out TData> : IVoicePayload where TData : IVoiceGatewayPayloadData
{
    /// <summary>
    /// Gets the data contained in the payload.
    /// </summary>
    TData Data { get; }
}
