//
//  WebSocketVoicePayloadTransportService.cs
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
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Remora.Discord.API.Abstractions.VoiceGateway;
using Remora.Discord.Voice.Abstractions.Services;
using Remora.Discord.Voice.Errors;
using Remora.Results;
#if DEBUG_VOICE
using System.Diagnostics;
using System.Text;
#endif

namespace Remora.Discord.Voice.Services
{
    /// <summary>
    /// Represents a websocket-based transport service.
    /// </summary>
    [PublicAPI]
    public sealed class WebSocketVoicePayloadTransportService : IVoicePayloadTransportService, IAsyncDisposable
    {
        /// <summary>
        /// Gets the maximum size in bytes that a command may be.
        /// </summary>
        private const int MaxPayloadSize = 4096;

        private readonly IServiceProvider _services;
        private readonly RecyclableMemoryStreamManager _memoryStreamManager;
        private readonly JsonSerializerOptions _jsonOptions;

        private readonly SemaphoreSlim _payloadSendSemaphore;
        private readonly SemaphoreSlim _payloadReceiveSemaphore;
        private readonly Utf8JsonWriter _payloadJsonWriter;

        private ArrayBufferWriter<byte> _payloadSendBuffer;

        /// <summary>
        /// Holds the currently available websocket client.
        /// </summary>
        private ClientWebSocket? _clientWebSocket;

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketVoicePayloadTransportService"/> class.
        /// </summary>
        /// <param name="services">The services available to the application.</param>
        /// <param name="memoryStreamManager">A <see cref="RecyclableMemoryStream"/> pool.</param>
        /// <param name="jsonOptions">The JSON options.</param>
        public WebSocketVoicePayloadTransportService
        (
            IServiceProvider services,
            RecyclableMemoryStreamManager memoryStreamManager,
            JsonSerializerOptions jsonOptions
        )
        {
            _services = services;
            _memoryStreamManager = memoryStreamManager;
            _jsonOptions = jsonOptions;

            _payloadSendSemaphore = new SemaphoreSlim(1, 1);
            _payloadSendBuffer = new ArrayBufferWriter<byte>(MaxPayloadSize);
            _payloadJsonWriter = new Utf8JsonWriter
            (
                _payloadSendBuffer,
                new JsonWriterOptions { SkipValidation = true } // The JSON Serializer should handle everything correctly
            );

            _payloadReceiveSemaphore = new SemaphoreSlim(1, 1);
        }

        /// <inheritdoc />
        public async Task<Result> ConnectAsync(Uri endpoint, CancellationToken ct = default)
        {
            if (_clientWebSocket is not null)
            {
                return new InvalidOperationError("The transport service is already connected.");
            }

            var socket = _services.GetRequiredService<ClientWebSocket>();

            try
            {
                await socket.ConnectAsync(endpoint, ct).ConfigureAwait(false);
                switch (socket.State)
                {
                    case WebSocketState.Open:
                    case WebSocketState.Connecting:
                        {
                            break;
                        }
                    default:
                        {
                            socket.Dispose();
                            return new Gateway.Results.WebSocketError
                            (
                                socket.State,
                                "Failed to connect to the endpoint."
                            );
                        }
                }
            }
            catch (Exception e)
            {
                socket.Dispose();
                return e;
            }

            _clientWebSocket = socket;

            this.IsConnected = true;
            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public async ValueTask<Result> SendPayloadAsync<TPayload>(TPayload payload, CancellationToken ct = default) where TPayload : IVoicePayload
        {
            if (_clientWebSocket?.State is not WebSocketState.Open)
            {
                return new InvalidOperationError("The transport service is not connected.");
            }

            try
            {
                JsonSerializer.Serialize(_payloadJsonWriter, payload, _jsonOptions);

                var data = _payloadSendBuffer.WrittenMemory;

                if (data.Length > MaxPayloadSize)
                {
                    // Reset the backing buffer so we don't hold on to more memory than necessary
                    _payloadSendBuffer = new ArrayBufferWriter<byte>(MaxPayloadSize);
                    _payloadJsonWriter.Reset(_payloadSendBuffer);

                    return new NotSupportedError("The payload was too large to be accepted by the gateway.");
                }

                var semaphoreEntered = await _payloadSendSemaphore.WaitAsync(1000, ct).ConfigureAwait(false);
                if (!semaphoreEntered)
                {
                    return new OperationCanceledException("Could not enter semaphore.");
                }

#if DEBUG_VOICE
                var stringData = Encoding.UTF8.GetString(data.ToArray());
                Debug.WriteLine("Voice S: " + stringData);
#endif

                await _clientWebSocket.SendAsync(data, WebSocketMessageType.Text, true, ct).ConfigureAwait(false);

                if (_clientWebSocket.CloseStatus.HasValue)
                {
                    return ConstructCloseError();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                _payloadSendSemaphore.Release();
                _payloadSendBuffer.Clear();
                _payloadJsonWriter.Reset();
            }

            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public async ValueTask<Result<IVoicePayload>> ReceivePayloadAsync(CancellationToken ct = default)
        {
            if (_clientWebSocket?.State is not WebSocketState.Open)
            {
                return new InvalidOperationError("The transport service is not connected.");
            }

            await using var ms = _memoryStreamManager.GetStream();
            using var segmentBufferOwner = MemoryPool<byte>.Shared.Rent(MaxPayloadSize);
            var semaphoreReleased = false;

            try
            {
                var semaphoreEntered = await _payloadReceiveSemaphore.WaitAsync(1000, ct).ConfigureAwait(false);
                if (!semaphoreEntered)
                {
                    return new OperationCanceledException("Could not enter semaphore.");
                }

                ValueWebSocketReceiveResult socketReceiveResult;
                do
                {
                    socketReceiveResult = await _clientWebSocket.ReceiveAsync(segmentBufferOwner.Memory, ct).ConfigureAwait(false);

                    if (socketReceiveResult.MessageType is WebSocketMessageType.Close)
                    {
                        return ConstructCloseError();
                    }

                    await ms.WriteAsync(segmentBufferOwner.Memory[..socketReceiveResult.Count], ct).ConfigureAwait(false);
                }
                while (!socketReceiveResult.EndOfMessage);

                _payloadReceiveSemaphore.Release();
                semaphoreReleased = true;

                ms.Seek(0, SeekOrigin.Begin);

#if DEBUG_VOICE
                var tempBuffer = MemoryPool<byte>.Shared.Rent((int)ms.Length);
                await ms.ReadAsync(tempBuffer.Memory[0.. (int)ms.Length], ct).ConfigureAwait(false);
                Debug.WriteLine("Voice R: " + Encoding.UTF8.GetString(tempBuffer.Memory.Span[0.. (int)ms.Length]));
                ms.Seek(0, SeekOrigin.Begin);
#endif

                var payload = await JsonSerializer.DeserializeAsync<IVoicePayload>(ms, _jsonOptions, ct).ConfigureAwait(false);
                if (payload is null)
                {
                    return new UnrecognisedPayloadError("The received payload deserialized as a null value.");
                }

                return Result<IVoicePayload>.FromSuccess(payload);
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                if (!semaphoreReleased)
                {
                    _payloadReceiveSemaphore.Release();
                }
            }
        }

        /// <inheritdoc/>
        public async Task<Result> DisconnectAsync
        (
            bool reconnectionIntended,
            CancellationToken ct = default
        )
        {
            if (_clientWebSocket is null)
            {
                return new InvalidOperationError("The transport service is not connected.");
            }

            switch (_clientWebSocket.State)
            {
                case WebSocketState.Open:
                case WebSocketState.CloseReceived:
                case WebSocketState.CloseSent:
                    {
                        try
                        {
                            // 1012 is used here instead of normal closure, because close codes 1000 and 1001 don't
                            // allow for reconnection. 1012 is referenced in the websocket protocol as "Service restart",
                            // which makes sense for our use case.
                            var closeCode = reconnectionIntended
                                ? (WebSocketCloseStatus)1012
                                : WebSocketCloseStatus.NormalClosure;

                            await _clientWebSocket.CloseAsync
                            (
                                closeCode,
                                "Terminating connection by user request.",
                                ct
                            ).ConfigureAwait(false);
                        }
                        catch (WebSocketException)
                        {
                            // Most likely due to some kind of premature or forced disconnection; we'll live with it
                        }
                        catch (OperationCanceledException)
                        {
                            // We still need to cleanup the socket
                        }

                        break;
                    }
            }

            _clientWebSocket.Dispose();
            _clientWebSocket = null;

            this.IsConnected = false;
            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            await _payloadJsonWriter.DisposeAsync().ConfigureAwait(false);
            _payloadSendSemaphore.Dispose();
            _payloadReceiveSemaphore.Dispose();

            if (_clientWebSocket is null)
            {
                return;
            }

            await DisconnectAsync(false).ConfigureAwait(false);
        }

        /// <summary>
        /// Constructs a relevant error for the websocket having closed.
        /// </summary>
        /// <remarks>Assumes the <see cref="_clientWebSocket"/> and its CloseStatus field to be non-null.</remarks>
        /// <returns>A <see cref="VoiceGatewayDiscordError"/> or <see cref="VoiceGatewayWebSocketError"/>.</returns>
        private ResultError ConstructCloseError()
        {
            if (Enum.IsDefined(typeof(VoiceGatewayCloseStatus), (int)_clientWebSocket!.CloseStatus!.Value))
            {
                return new VoiceGatewayDiscordError((VoiceGatewayCloseStatus)_clientWebSocket.CloseStatus.Value);
            }

            return new VoiceGatewayWebSocketError(_clientWebSocket.CloseStatus.Value);
        }
    }
}
