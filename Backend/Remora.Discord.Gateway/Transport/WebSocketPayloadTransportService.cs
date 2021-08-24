//
//  WebSocketPayloadTransportService.cs
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
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace Remora.Discord.Gateway.Transport
{
    /// <summary>
    /// Represents a websocket-based transport service.
    /// </summary>
    [PublicAPI]
    public class WebSocketPayloadTransportService : IPayloadTransportService, IAsyncDisposable
    {
        private readonly IServiceProvider _services;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Holds the currently available websocket client.
        /// </summary>
        private ClientWebSocket? _clientWebSocket;

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketPayloadTransportService"/> class.
        /// </summary>
        /// <param name="services">The services available to the application.</param>
        /// <param name="jsonOptions">The JSON options.</param>
        public WebSocketPayloadTransportService
        (
            IServiceProvider services,
            IOptions<JsonSerializerOptions> jsonOptions
        )
        {
            _services = services;
            _jsonOptions = jsonOptions.Value;
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
                await socket.ConnectAsync(endpoint, ct);
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
                        return new Results.WebSocketError
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
        public async Task<Result> SendPayloadAsync(IPayload payload, CancellationToken ct = default)
        {
            if (_clientWebSocket is null)
            {
                return new InvalidOperationError("The transport service is not connected.");
            }

            if (_clientWebSocket.State != WebSocketState.Open)
            {
                return new InvalidOperationError("The socket was not open.");
            }

            await using var memoryStream = new MemoryStream();

            byte[]? buffer = null;
            try
            {
                await JsonSerializer.SerializeAsync(memoryStream, payload, _jsonOptions, ct);

                if (memoryStream.Length > 4096)
                {
                    return new NotSupportedError
                    (
                        "The payload was too large to be accepted by the gateway."
                    );
                }

                buffer = ArrayPool<byte>.Shared.Rent((int)memoryStream.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Copy the data
                var bufferSegment = new ArraySegment<byte>(buffer, 0, (int)memoryStream.Length);
                await memoryStream.ReadAsync(bufferSegment, ct);

                // Send the whole payload as one chunk
                await _clientWebSocket.SendAsync(bufferSegment, WebSocketMessageType.Text, true, ct);

                if (_clientWebSocket.CloseStatus.HasValue)
                {
                    if (Enum.IsDefined(typeof(GatewayCloseStatus), (int)_clientWebSocket.CloseStatus))
                    {
                        return new GatewayDiscordError((GatewayCloseStatus)_clientWebSocket.CloseStatus);
                    }

                    return new GatewayWebSocketError(_clientWebSocket.CloseStatus.Value);
                }
            }
            finally
            {
                if (buffer is not null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public async Task<Result<IPayload>> ReceivePayloadAsync(CancellationToken ct = default)
        {
            if (_clientWebSocket is null)
            {
                return new InvalidOperationError("The transport service is not connected.");
            }

            if (_clientWebSocket.State != WebSocketState.Open)
            {
                return new InvalidOperationError("The socket was not open.");
            }

            await using var memoryStream = new MemoryStream();

            var buffer = ArrayPool<byte>.Shared.Rent(4096);

            try
            {
                WebSocketReceiveResult result;

                do
                {
                    result = await _clientWebSocket.ReceiveAsync(buffer, ct);

                    if (result.CloseStatus.HasValue)
                    {
                        if (Enum.IsDefined(typeof(GatewayCloseStatus), (int)result.CloseStatus))
                        {
                            return new GatewayDiscordError((GatewayCloseStatus)result.CloseStatus);
                        }

                        return new GatewayWebSocketError(result.CloseStatus.Value);
                    }

                    await memoryStream.WriteAsync(buffer, 0, result.Count, ct);
                }
                while (!result.EndOfMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var payload = await JsonSerializer.DeserializeAsync<IPayload>(memoryStream, _jsonOptions, ct);
                if (payload is null)
                {
                    return new NotSupportedError
                    (
                        "The received payload deserialized as a null value."
                    );
                }

                return Result<IPayload>.FromSuccess(payload);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
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
                        );
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
            if (_clientWebSocket is null)
            {
                return;
            }

            await DisconnectAsync(false);
        }
    }
}
