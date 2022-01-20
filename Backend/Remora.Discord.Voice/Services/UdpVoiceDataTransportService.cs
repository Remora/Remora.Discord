//
//  UdpVoiceDataTransportService.cs
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
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Remora.Discord.Voice.Abstractions;
using Remora.Discord.Voice.Abstractions.Services;
using Remora.Discord.Voice.Errors;
using Remora.Discord.Voice.Interop.Sodium;
using Remora.Discord.Voice.Objects.UdpDataProtocol;
using Remora.Results;

namespace Remora.Discord.Voice.Services
{
    // TODO: Latency, heartbeating, voice speaking synthesis

    /// <summary>
    /// Represents a UDP-based transport service for voice data.
    /// </summary>
    /// <remarks>
    /// This class consumes/outputs Opus data packets, and performs the necessary encryption functions internally.
    /// </remarks>
    [PublicAPI]
    public class UdpVoiceDataTransportService : IVoiceDataTranportService, IDisposable
    {
        private static readonly IReadOnlyDictionary<string, SupportedEncryptionMode> SupportedEncryptionModes;

        private readonly IMemoryOwner<byte> _nonceMemory;
        private readonly DiscordVoiceClientOptions _options;

        private UdpClient _client;
        private SupportedEncryptionMode _encryptionMode;
        private Sodium? _encryptor;
        private IMemoryOwner<byte> _packetMemory;
        private ushort _sequence;
        private uint _ssrc;
        private uint _timestamp;

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        static UdpVoiceDataTransportService()
        {
            SupportedEncryptionModes = new Dictionary<string, SupportedEncryptionMode>()
            {
                ["xsalsa20_poly1305_lite"] = SupportedEncryptionMode.XSalsa20_Poly1305_Lite,
                ["xsalsa20_poly1305_suffix"] = SupportedEncryptionMode.XSalsa20_Poly1305_Suffix,
                ["xsalsa20_poly1305"] = SupportedEncryptionMode.XSalsa20_Poly1305
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpVoiceDataTransportService"/> class.
        /// </summary>
        /// <param name="options">The voice options to use.</param>
        /// <param name="random">A random number generator.</param>
        public UdpVoiceDataTransportService
        (
            IOptions<DiscordVoiceClientOptions> options,
            Random random
        )
        {
            _client = new UdpClient();
            _options = options.Value;
            _encryptionMode = SupportedEncryptionMode.XSalsa20_Poly1305;
            _nonceMemory = MemoryPool<byte>.Shared.Rent((int)Sodium.NonceSize);
            _packetMemory = MemoryPool<byte>.Shared.Rent(512);

            // Randomise as per the RTP specification recommendation.
            _sequence = (ushort)random.Next(0, ushort.MaxValue);
            _timestamp = (uint)random.Next(0, short.MaxValue);
        }

        /// <inheritdoc />
        public Result<string> SelectSupportedEncryptionMode(IReadOnlyList<string> encryptionModes)
        {
            foreach (string mode in encryptionModes)
            {
                if (SupportedEncryptionModes.ContainsKey(mode))
                {
                    _encryptionMode = SupportedEncryptionModes[mode];
                    return Result<string>.FromSuccess(mode);
                }
            }

            return new VoiceUdpError("None of the encryption modes offered by Discord are supported.");
        }

        /// <inheritdoc />
        public async Task<Result<IPDiscoveryResponse>> ConnectAsync(IVoiceReady voiceServerDetails, CancellationToken ct = default)
        {
            const int discoveryBufferSize = IPDiscoveryResponse.PacketLength;

            try
            {
                _client.Connect(voiceServerDetails.IP, voiceServerDetails.Port);

                using var discoveryBuffer = MemoryPool<byte>.Shared.Rent(discoveryBufferSize);

                new IPDiscoveryRequest
                (
                    voiceServerDetails.SSRC,
                    IPDiscoveryRequest.CalculateEmbeddedLength(discoveryBufferSize)
                ).Pack(discoveryBuffer.Memory.Span);

                await _client.Client.SendAsync(discoveryBuffer.Memory[..discoveryBufferSize], SocketFlags.None, ct).ConfigureAwait(false);

                using var cts = new CancellationTokenSource(1000);
                var bytesRead = await _client.Client.ReceiveAsync(discoveryBuffer.Memory, SocketFlags.None, cts.Token).ConfigureAwait(false);
                if (bytesRead != discoveryBufferSize)
                {
                    return new VoiceUdpError("Failed to receive IP discovery packet: timed out.");
                }

                this.IsConnected = true;
                _ssrc = voiceServerDetails.SSRC;

                // This method also checks that the packet is valid
                return IPDiscoveryResponse.Unpack(discoveryBuffer.Memory.Span);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <inheritdoc />
        public Result Initialize(IReadOnlyList<byte> key)
        {
            if (key.Count != Sodium.KeySize)
            {
                return new ArgumentOutOfRangeError(nameof(key), "Size of key must equal " + Sodium.KeySize);
            }

            var keyBytes = new byte[Sodium.KeySize];
            for (int i = 0; i < Sodium.KeySize; i++)
            {
                keyBytes[i] = key[i];
            }

            var createEncryptor = Sodium.Create(keyBytes);
            if (!createEncryptor.IsSuccess)
            {
                return Result.FromError(createEncryptor);
            }

            _encryptor = createEncryptor.Entity;

            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public Result Disconnect()
        {
            _client.Close(); // This directly disposes the client
            _client = new UdpClient();

            _ssrc = 0;
            _encryptor = null;
            this.IsConnected = false;

            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public async ValueTask<Result> SendFrameAsync(ReadOnlyMemory<byte> frame, int frameSize, CancellationToken ct = default)
        {
            if (!this.IsConnected)
            {
                return new InvalidOperationError("The transport service must be connected before frames can be sent.");
            }

            if (_encryptor is null)
            {
                return new InvalidOperationError("The transport service must be initialized before frames can be sent.");
            }

            try
            {
                const int rtpHeaderSize = 12;
                var encryptedFrameSize = frame.Length + (int)Sodium.MacSize;

                var packetSize = rtpHeaderSize + encryptedFrameSize;
                packetSize += _encryptionMode switch
                {
                    SupportedEncryptionMode.XSalsa20_Poly1305_Suffix => (int)Sodium.NonceSize,
                    SupportedEncryptionMode.XSalsa20_Poly1305_Lite => sizeof(uint),
                    _ => 0
                };

                if (_packetMemory.Memory.Length < packetSize)
                {
                    _packetMemory.Dispose();
                    _packetMemory = MemoryPool<byte>.Shared.Rent(packetSize);
                }

                Memory<byte> nonce = _nonceMemory.Memory[.. (int)Sodium.NonceSize];
                Memory<byte> packet = _packetMemory.Memory[..packetSize];

                WriteRtpHeader(packet.Span, frameSize);
                WriteNonce(nonce.Span, packet.Span, packet.Span[0..rtpHeaderSize]);

                var encryptionResult = _encryptor!.Encrypt(frame.Span, packet.Span.Slice(rtpHeaderSize, encryptedFrameSize), nonce.Span);
                if (!encryptionResult.IsSuccess)
                {
                    return encryptionResult;
                }

                int amountSent = await _client.Client.SendAsync(packet, SocketFlags.None, ct).ConfigureAwait(false);
                if (amountSent != packetSize)
                {
                    return new VoiceTransmitError("Size of actual sent data did not match expected length.");
                }

                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Fills a buffer with zeroes.
        /// </summary>
        /// <param name="buffer">The buffer to fill.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ZeroFill(Span<byte> buffer)
        {
            int zero = 0;
            var i = 0;

            for (; i < buffer.Length / sizeof(int); i++)
            {
                MemoryMarshal.Write(buffer, ref zero);
            }

            var remainder = buffer.Length % sizeof(int);
            if (remainder == 0)
            {
                return;
            }

            for (; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }
        }

        /// <summary>
        /// Disposes of both managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether or not to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                _client.Dispose();
                _nonceMemory.Dispose();
                _packetMemory.Dispose();
            }

            this.IsDisposed = true;
        }

        /// <summary>
        /// Writes an RTP header to the given buffer and updates the internal RTP state.
        /// </summary>
        /// <param name="buffer">The buffer to write the header to.</param>
        /// <param name="frameSize">The number of audio samples in the packet.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteRtpHeader(Span<byte> buffer, int frameSize)
        {
            buffer[0] = 0x80;
            buffer[1] = 0x78;

            if (_sequence == ushort.MaxValue)
            {
                _sequence = 0;
            }

            BinaryPrimitives.WriteUInt16BigEndian(buffer[2..], _sequence++);
            BinaryPrimitives.WriteUInt32BigEndian(buffer[4..], _timestamp);
            BinaryPrimitives.WriteUInt32BigEndian(buffer[8..], _ssrc);

            _timestamp += (uint)frameSize;
        }

        /// <summary>
        /// Generates a nonce and writes it to the correct buffer, depending on the internal encryption mode..
        /// </summary>
        /// <param name="nonceBuffer">The output nonce buffer. It is expected that this nonce buffer is the correct size.</param>
        /// <param name="packetBuffer">The packet buffer.</param>
        /// <param name="rtpHeader">The RTP header, used as the nonce when in <see cref="SupportedEncryptionMode.XSalsa20_Poly1305"/> mode.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteNonce(Span<byte> nonceBuffer, Span<byte> packetBuffer, ReadOnlySpan<byte> rtpHeader)
        {
            switch (_encryptionMode)
            {
                case SupportedEncryptionMode.XSalsa20_Poly1305:
                    ZeroFill(nonceBuffer);
                    rtpHeader.CopyTo(nonceBuffer);
                    break;
                case SupportedEncryptionMode.XSalsa20_Poly1305_Suffix:
                    Sodium.GenerateRandomBytes(nonceBuffer);
                    nonceBuffer.CopyTo(packetBuffer[^nonceBuffer.Length..]);
                    break;
                case SupportedEncryptionMode.XSalsa20_Poly1305_Lite:
                    ZeroFill(nonceBuffer);
                    BinaryPrimitives.WriteUInt32BigEndian(nonceBuffer, _timestamp);
                    nonceBuffer[..sizeof(uint)].CopyTo(packetBuffer[^sizeof(uint)..]);
                    break;
            }
        }
    }
}
