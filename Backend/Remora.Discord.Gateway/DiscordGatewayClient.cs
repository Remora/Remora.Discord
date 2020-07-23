//
//  DiscordGatewayClient.cs
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
using Remora.Discord.API.Abstractions;
using Remora.Discord.API.Abstractions.Commands;
using Remora.Discord.API.Abstractions.Events;
using Remora.Discord.API.API;
using Remora.Discord.API.API.Commands;
using Remora.Discord.Core;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace Remora.Discord.Gateway
{
    /// <summary>
    /// Represents a Discord Gateway client.
    /// </summary>
    public class DiscordGatewayClient
    {
        private readonly CancellationTokenSource _tokenSource;
        private readonly IDiscordRestGatewayAPI _gatewayAPI;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ITokenStore _tokenStore;

        /// <summary>
        /// Holds the connection status.
        /// </summary>
        private GatewayConnectionStatus _connectionStatus;

        /// <summary>
        /// Holds the websocket client.
        /// </summary>
        private ClientWebSocket _clientWebSocket;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordGatewayClient"/> class.
        /// </summary>
        /// <param name="gatewayAPI">The gateway API.</param>
        /// <param name="jsonOptions">The JSON options.</param>
        /// <param name="tokenStore">The token store.</param>
        public DiscordGatewayClient
        (
            IDiscordRestGatewayAPI gatewayAPI,
            JsonSerializerOptions jsonOptions,
            ITokenStore tokenStore
        )
        {
            _gatewayAPI = gatewayAPI;
            _jsonOptions = jsonOptions;
            _tokenStore = tokenStore;
            _tokenSource = new CancellationTokenSource();
            _clientWebSocket = new ClientWebSocket();
        }

        /// <summary>
        /// Starts and connects the gateway client. This task will not complete until cancelled (or faulted),
        /// maintaining the connection for the duration of it.
        ///
        /// If the gateway client encounters a fatal problem during the execution of this task, it will return with a
        /// failed result. If a shutdown is requested, it will gracefully terminate the connection and return a
        /// successful result.
        /// </summary>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A gateway connection result which may or may not have succeeded.</returns>
        public async Task<GatewayConnectionResult> RunAsync(CancellationToken ct)
        {
            try
            {
                if (_connectionStatus != GatewayConnectionStatus.Offline)
                {
                    return GatewayConnectionResult.FromError("Already connecting.");
                }

                var getGatewayEndpoint = await _gatewayAPI.GetGatewayBotAsync(ct);
                if (!getGatewayEndpoint.IsSuccess)
                {
                    return GatewayConnectionResult.FromError(getGatewayEndpoint);
                }

                var gatewayEndpoint = $"{getGatewayEndpoint.Entity.Url}?v=6&encoding=json";
                if (!Uri.TryCreate(gatewayEndpoint, UriKind.Absolute, out var gatewayUri))
                {
                    return GatewayConnectionResult.FromError("Failed to parse the received gateway endpoint.");
                }

                await _clientWebSocket.ConnectAsync(gatewayUri, ct);
                if (_clientWebSocket.State != WebSocketState.Open)
                {
                    return GatewayConnectionResult.FromError
                    (
                        $"Failed to connect to the gateway: {_clientWebSocket.State}"
                    );
                }

                var receiveHello = await ReceivePayloadAsync(ct);

                if (!receiveHello.IsSuccess)
                {
                    return GatewayConnectionResult.FromError(receiveHello);
                }

                if (!(receiveHello.Entity is Payload<IHello> hello))
                {
                    return GatewayConnectionResult.FromError
                    (
                        "The first payload from the gateway was not a hello. Rude!"
                    );
                }

                var heartbeatInterval = TimeSpan.FromMilliseconds(hello.Data.HeartbeatInterval);
                var lastHeartbeat = DateTime.UtcNow;

                var identifyPayload = new Payload<Identify>
                (
                    new Identify
                    (
                        _tokenStore.Token,
                        new ConnectionProperties("Remora.Discord"),
                        intents: GatewayIntents.DirectMessages,
                        compress: false
                    )
                );

                var sendIdentify = await SendPayloadAsync(identifyPayload, ct);
                if (!sendIdentify.IsSuccess)
                {
                    return GatewayConnectionResult.FromError(sendIdentify);
                }

                var receiveReady = await ReceivePayloadAsync(ct);
                if (!receiveReady.IsSuccess)
                {
                    return GatewayConnectionResult.FromError(receiveReady);
                }

                if (!(receiveReady.Entity is Payload<IReady> ready))
                {
                    return GatewayConnectionResult.FromError
                    (
                        "The payload after identification was not a Ready payload."
                    );
                }

                long? lastSequenceNumber = null;
                while (!ct.IsCancellationRequested)
                {
                    // Heartbeat, if required
                    var now = DateTime.Now;
                    if (now - lastHeartbeat > heartbeatInterval - TimeSpan.FromSeconds(1))
                    {
                        var heartbeatPayload = new Payload<IHeartbeat>(new Heartbeat(lastSequenceNumber));
                        var sendHeartbeat = await SendPayloadAsync(heartbeatPayload, ct);

                        if (!sendHeartbeat.IsSuccess)
                        {
                            return GatewayConnectionResult.FromError(sendHeartbeat);
                        }
                    }

                    // Get event
                    // Broadcast to responders
                    // await finished responders from past events
                }

                return GatewayConnectionResult.FromSuccess();
            }
            catch (Exception e)
            {
                return GatewayConnectionResult.FromError(e);
            }
        }

        /// <summary>
        /// Asynchronously sends a payload to the websocket.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A send result which may or may not have succeeded.</returns>
        private async Task<SendPayloadResult> SendPayloadAsync(IPayload payload, CancellationToken ct = default)
        {
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
                    return SendPayloadResult.FromError("The payload was too large to be accepted by the gateway.");
                }

                buffer = ArrayPool<byte>.Shared.Rent((int)memoryStream.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Send the whole payload as one chunk
                await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, ct);
            }
            catch (Exception e)
            {
                return SendPayloadResult.FromError(e);
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

        /// <summary>
        /// Asynchronously receives a payload from the websocket.
        /// </summary>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A receive result which may or may not have succeeded.</returns>
        private async Task<ReceivePayloadResult<IPayload>> ReceivePayloadAsync(CancellationToken ct = default)
        {
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
                    await memoryStream.WriteAsync(buffer, 0, result.Count, ct);
                }
                while (!result.EndOfMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var payload = await JsonSerializer.DeserializeAsync<IPayload>(memoryStream, _jsonOptions, ct);
                return ReceivePayloadResult<IPayload>.FromSuccess(payload);
            }
            catch (Exception ex)
            {
                return ReceivePayloadResult<IPayload>.FromError("Failed to receive a payload.", ex);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
