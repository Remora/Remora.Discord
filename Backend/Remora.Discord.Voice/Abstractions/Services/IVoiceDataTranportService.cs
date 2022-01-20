//
//  IVoiceDataTranportService.cs
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Remora.Discord.Voice.Objects.UdpDataProtocol;
using Remora.Results;

namespace Remora.Discord.Voice.Abstractions.Services
{
    /// <summary>
    /// Represents an arbitrary transport service for voice data.
    /// </summary>
    /// <remarks>
    /// <summary>
    /// This interface defines the public API surface for a type that the voice gateway client can use to send and receive
    /// payloads to/from a Discord voice server. It is not specifically concerned with the actual protocol used underneath the
    /// hood, and instead only presents abstract I/O operations.
    ///
    /// Some assumptions are made in regards to the Discord voice protocol.
    /// </summary>
    /// </remarks>
    [PublicAPI]
    public interface IVoiceDataTranportService
    {
        /// <summary>
        /// Gets a value indicating whether or not the service has been connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Selects a supported encryption mode from the provided list,
        /// and sets the internal encryption mode to the chosen value.
        /// </summary>
        /// <param name="encryptionModes">The list of offered encryption modes.</param>
        /// <returns>A result representing the outcome of the operation, and containing the selected encryption mode if successful.</returns>
        Result<string> SelectSupportedEncryptionMode(IReadOnlyList<string> encryptionModes);

        /// <summary>
        /// Connects to the transport endpoint, enabling I/O operations.
        /// </summary>
        /// <param name="voiceServerDetails">The details of the voice server to connect to.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A connection result which may or may not have succeeded.</returns>
        Task<Result<IPDiscoveryResponse>> ConnectAsync(IVoiceReady voiceServerDetails, CancellationToken ct = default);

        /// <summary>
        /// Initializes the transport service.
        /// </summary>
        /// <param name="key">The key to encrypt data with.</param>
        /// <returns>A result indicating the outcome of the operation.</returns>
        Result Initialize(IReadOnlyList<byte> key);

        /// <summary>
        /// Sends an audio data frame.
        /// </summary>
        /// <param name="frame">The data frame.</param>
        /// <param name="frameSize">The number of audio samples in the packet.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A result representing the outcome of the operation.</returns>
        ValueTask<Result> SendFrameAsync(ReadOnlyMemory<byte> frame, int frameSize, CancellationToken ct = default);

        /// <summary>
        /// Disconnects from the transport endpoint.
        /// </summary>
        /// <returns>A result representing the outcome of the operation.</returns>
        Result Disconnect();
    }
}
