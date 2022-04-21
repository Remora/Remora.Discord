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
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.IO;
using Remora.Discord.API.Abstractions.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace Remora.Discord.Gateway.Transport;

/// <summary>
/// Represents a websocket-based transport service.
/// </summary>
[PublicAPI]
public class WebSocketPayloadTransportService : IPayloadTransportService, IAsyncDisposable
{
    /// <summary>
    /// Gets the maximum size in bytes that a command may be.
    /// </summary>
    private const int MaxPayloadSize = 4096;

    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Utf8JsonWriter _payloadJsonWriter;

    private bool _isDisposed;
    private ArrayBufferWriter<byte> _payloadSendBuffer;
    private CancellationTokenSource _receiveToken;

    /// <summary>
    /// Holds the currently available websocket client.
    /// </summary>
    private ClientWebSocket? _clientWebSocket;

    /// <inheritdoc />
    public bool IsConnected => _clientWebSocket?.State is WebSocketState.Open;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketPayloadTransportService"/> class.
    /// </summary>
    /// <param name="memoryStreamManager">A <see cref="RecyclableMemoryStream"/> pool.</param>
    /// <param name="jsonOptions">The JSON options.</param>
    public WebSocketPayloadTransportService
    (
        RecyclableMemoryStreamManager memoryStreamManager,
        JsonSerializerOptions jsonOptions
    )
    {
        _memoryStreamManager = memoryStreamManager;
        _jsonOptions = jsonOptions;

        _receiveToken = new CancellationTokenSource();
        _payloadSendBuffer = new ArrayBufferWriter<byte>(MaxPayloadSize);
        _payloadJsonWriter = new Utf8JsonWriter
        (
            _payloadSendBuffer,
            new JsonWriterOptions { SkipValidation = true } // The JSON Serializer should handle everything correctly
        );

        _payloadSendBuffer.Clear();
    }

    /// <inheritdoc />
    public async Task<Result> ConnectAsync(Uri endpoint, CancellationToken ct = default)
    {
        if (this.IsConnected)
        {
            return new InvalidOperationError("The transport service is already connected.");
        }

        _receiveToken.Dispose();
        _receiveToken = new CancellationTokenSource();

        _clientWebSocket?.Dispose();
        var socket = new ClientWebSocket();

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

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    /// <remarks>This method is not thread safe.</remarks>
    public async Task<Result> SendPayloadAsync(IPayload payload, CancellationToken ct = default)
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

            await _clientWebSocket.SendAsync(data, WebSocketMessageType.Text, true, ct);

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
            _payloadSendBuffer.Clear();
            _payloadJsonWriter.Reset();
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    /// <remarks>This method is not thread safe.</remarks>
    public async Task<Result<IPayload>> ReceivePayloadAsync(CancellationToken ct = default)
    {
        if (_clientWebSocket?.State is not WebSocketState.Open)
        {
            return new InvalidOperationError("The transport service is not connected.");
        }

        try
        {
            await using var memoryStream = _memoryStreamManager.GetStream();
            using var segmentBufferOwner = MemoryPool<byte>.Shared.Rent(MaxPayloadSize);

            ValueWebSocketReceiveResult socketReceiveResult;
            do
            {
                // On the use of the _receiveToken: Cancelling a receive causes a whole manner of annoyances.
                // The entire socket will be shutdown and disposed internally. Hence, we use our own token
                // so that we only cancel receives after properly completing a close handshake.
                socketReceiveResult = await _clientWebSocket.ReceiveAsync(segmentBufferOwner.Memory, _receiveToken.Token);

                if (socketReceiveResult.MessageType is WebSocketMessageType.Close)
                {
                    return ConstructCloseError();
                }

                await memoryStream.WriteAsync(segmentBufferOwner.Memory[..socketReceiveResult.Count], ct);
            }
            while (!socketReceiveResult.EndOfMessage);

            memoryStream.Seek(0, SeekOrigin.Begin);

            var payload = await JsonSerializer.DeserializeAsync<IPayload>(memoryStream, _jsonOptions, ct);
            return payload is null
                ? new UnrecognisedPayloadError(Encoding.UTF8.GetString(memoryStream.GetBuffer()))
                : Result<IPayload>.FromSuccess(payload);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DisconnectAsync
    (
        bool reconnectionIntended,
        CancellationToken ct = default
    )
    {
        if (!this.IsConnected || _clientWebSocket is null)
        {
            return new InvalidOperationError("The transport service is not connected.");
        }

        switch (_clientWebSocket.State)
        {
            case WebSocketState.Open:
            case WebSocketState.CloseReceived:
            {
                try
                {
                    // "When you close the connection to the gateway with the close code 1000 or 1001,
                    // your session will be invalidated and your bot will appear offline.
                    // If you simply close the TCP connection, or use a different close code,
                    // the bot session will remain active and timeout after a few minutes."
                    var closeCode = reconnectionIntended
                        ? WebSocketCloseStatus.Empty
                        : WebSocketCloseStatus.NormalClosure;

                    await _clientWebSocket.CloseAsync
                    (
                        closeCode,
                        string.Empty,
                        ct
                    );
                }
                catch (WebSocketException)
                {
                    // Most likely due to some kind of premature or forced disconnection; we'll live with it
                }
                catch (OperationCanceledException)
                {
                    // We still need to cleanup
                }

                break;
            }
        }

        _receiveToken.Cancel();

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of managed resources.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_isDisposed)
        {
            return;
        }

        await DisconnectAsync(false).ConfigureAwait(false);

        await _payloadJsonWriter.DisposeAsync().ConfigureAwait(false);

        _receiveToken.Cancel();
        _receiveToken.Dispose();

        _clientWebSocket?.Dispose();

        _isDisposed = true;
    }

    /// <summary>
    /// Constructs a relevant error for the websocket having closed.
    /// </summary>
    /// <remarks>Assumes the <see cref="_clientWebSocket"/> and its CloseStatus field to be non-null.</remarks>
    /// <returns>A <see cref="GatewayDiscordError"/> or <see cref="GatewayWebSocketError"/>.</returns>
    private ResultError ConstructCloseError()
    {
        var closeValue = (int)_clientWebSocket!.CloseStatus!.Value;
        if (Enum.IsDefined(typeof(GatewayCloseStatus), closeValue))
        {
            return new GatewayDiscordError((GatewayCloseStatus)_clientWebSocket.CloseStatus.Value);
        }

        return new GatewayWebSocketError(_clientWebSocket.CloseStatus.Value);
    }
}
