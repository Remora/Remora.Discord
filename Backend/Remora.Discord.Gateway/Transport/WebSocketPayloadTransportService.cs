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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Gateway.Results;

namespace Remora.Discord.Gateway.Transport
{
    /// <summary>
    /// Represents a websocket-based transport service.
    /// </summary>
    public class WebSocketPayloadTransportService : IPayloadTransportService, IAsyncDisposable
    {
        private readonly IServiceProvider _services;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Holds the currently available websocket client.
        /// </summary>
        private ClientWebSocket? _clientWebSocket;

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
        public async Task<GatewayConnectionResult> ConnectAsync(Uri endpoint, CancellationToken ct = default)
        {
            if (!(_clientWebSocket is null))
            {
                return GatewayConnectionResult.FromError("The transport service is already connected.");
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
                        return GatewayConnectionResult.FromError("Failed to connect to the endpoint.");
                    }
                }
            }
            catch (Exception e)
            {
                socket.Dispose();
                return GatewayConnectionResult.FromError(e);
            }

            _clientWebSocket = socket;
            return GatewayConnectionResult.FromSuccess();
        }

        /// <inheritdoc />
        public async Task<SendPayloadResult> SendPayloadAsync(IPayload payload, CancellationToken ct = default)
        {
            if (_clientWebSocket is null)
            {
                return SendPayloadResult.FromError("The transport service is not connected.");
            }

            if (_clientWebSocket.State != WebSocketState.Open)
            {
                return SendPayloadResult.FromError("The socket was not open.");
            }

            await using var memoryStream = new MemoryStream();

            byte[]? buffer = null;
            try
            {
                await JsonSerializer.SerializeAsync(memoryStream, payload, _jsonOptions, ct);

                if (memoryStream.Length > 4096)
                {
                    return SendPayloadResult.FromError
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
                        return SendPayloadResult.FromError
                        (
                            "The gateway closed the connection.",
                            (GatewayCloseStatus)_clientWebSocket.CloseStatus
                        );
                    }

                    return SendPayloadResult.FromError
                    (
                        _clientWebSocket.CloseStatusDescription,
                        _clientWebSocket.CloseStatus.Value
                    );
                }
            }
            finally
            {
                if (!(buffer is null))
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            return SendPayloadResult.FromSuccess();
        }

        /// <inheritdoc />
        public async Task<ReceivePayloadResult<IPayload>> ReceivePayloadAsync(CancellationToken ct = default)
        {
            if (_clientWebSocket is null)
            {
                return ReceivePayloadResult<IPayload>.FromError("The transport service is not connected.");
            }

            if (_clientWebSocket.State != WebSocketState.Open)
            {
                return ReceivePayloadResult<IPayload>.FromError("The socket was not open.");
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
                            return ReceivePayloadResult<IPayload>.FromError
                            (
                                "The gateway closed the connection.",
                                (GatewayCloseStatus)result.CloseStatus
                            );
                        }

                        return ReceivePayloadResult<IPayload>.FromError
                        (
                            result.CloseStatusDescription,
                            result.CloseStatus.Value
                        );
                    }

                    await memoryStream.WriteAsync(buffer, 0, result.Count, ct);
                }
                while (!result.EndOfMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var payload = await JsonSerializer.DeserializeAsync<IPayload>(memoryStream, _jsonOptions, ct);
                return ReceivePayloadResult<IPayload>.FromSuccess(payload);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        /// <inheritdoc/>
        public async Task<GatewayConnectionResult> DisconnectAsync
        (
            bool reconnectionIntended,
            CancellationToken ct = default
        )
        {
            if (_clientWebSocket is null)
            {
                return GatewayConnectionResult.FromError("The transport service is not connected.");
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

                    break;
                }
            }

            _clientWebSocket.Dispose();
            _clientWebSocket = null;

            return GatewayConnectionResult.FromSuccess();
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
