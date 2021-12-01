//
//  DiscordVoiceClient.cs
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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.VoiceGateway;
using Remora.Discord.API.Abstractions.VoiceGateway.Commands;
using Remora.Discord.API.Abstractions.VoiceGateway.Events;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.VoiceGateway;
using Remora.Discord.API.VoiceGateway.Commands;
using Remora.Discord.Gateway;
using Remora.Discord.Voice.Abstractions.Services;
using Remora.Discord.Voice.Errors;
using Remora.Discord.Voice.Objects;
using Remora.Discord.Voice.Objects.UdpDataProtocol;
using Remora.Discord.Voice.Util;
using Remora.Rest.Core;
using Remora.Results;

// TODO: Remove requirement for operation code on VoicePayload?
namespace Remora.Discord.Voice
{
    /// <summary>
    /// Represents a Discord Voice Gateway client.
    /// </summary>
    [PublicAPI]
    public sealed class DiscordVoiceClient : IAsyncDisposable
    {
        private readonly ILogger<DiscordVoiceClient> _logger;
        private readonly DiscordGatewayClient _gatewayClient;
        private readonly DiscordVoiceClientOptions _clientOptions;
        private readonly IConnectionEstablishmentWaiterService _connectionWaiterService;
        private readonly IVoicePayloadTransportService _transportService;
        private readonly IVoiceDataTranportService _dataService;
        private readonly Random _random;
        private readonly HeartbeatData _heartbeatData;
        private readonly SemaphoreSlim _transmitSemaphore;
        private readonly IAudioTranscoderService _defaultTranscoder;
        private readonly IMemoryOwner<byte> _silenceFrameBuffer;

        /// <summary>
        /// Holds payloads that have been submitted by the application, but have not yet been sent to the gateway.
        /// </summary>
        private readonly ConcurrentQueue<IVoicePayload> _payloadsToSend;

        /// <summary>
        /// Holds payloads that have been received by the gateway, but not yet distributed to the application.
        /// </summary>
        private readonly ConcurrentQueue<IVoicePayload> _receivedPayloads;

        /// <summary>
        /// Holds the cancellation token source for internal operations.
        /// </summary>
        private CancellationTokenSource _disconnectRequestedSource;

        private Task<Result> _runLoopTask;
        private Task<Result> _sendTask;
        private Task<Result> _receiveTask;

        private UpdateVoiceState? _connectionRequestParameters;
        private VoiceConnectionEstablishmentDetails? _gatewayConnectionDetails;
        private IVoiceReady? _voiceServerConnectionDetails;

        /// <summary>
        /// Gets a value indicating whether or not the client is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the client is transmitting.
        /// </summary>
        public bool IsTransmitting => _transmitSemaphore.CurrentCount == 0;

        /// <summary>
        /// Gets the connection status of the voice gateway.
        /// </summary>
        public GatewayConnectionStatus ConnectionStatus { get; private set; }

        /// <summary>
        /// Gets the time taken for the voice gateway to respond to the last heartbeat, providing an estimate of round-trip latency.
        /// Will return zero until the first heartbeat has occured.
        /// </summary>
        public TimeSpan GatewayLatency { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordVoiceClient"/> class.
        /// </summary>
        /// <param name="logger">The logging interface.</param>
        /// <param name="gatewayClient">The gateway client.</param>
        /// <param name="clientOptions">The client options to use.</param>
        /// <param name="connectionWaiterService">The connection waiter service.</param>
        /// <param name="transportService">The voice payload transport service.</param>
        /// <param name="dataService">The voice data transport service.</param>
        /// <param name="defaultTranscoder">The default audio transcoder to use.</param>
        /// <param name="random">A random number generator.</param>
        public DiscordVoiceClient
        (
            ILogger<DiscordVoiceClient> logger,
            DiscordGatewayClient gatewayClient,
            IOptions<DiscordVoiceClientOptions> clientOptions,
            IConnectionEstablishmentWaiterService connectionWaiterService,
            IVoicePayloadTransportService transportService,
            IVoiceDataTranportService dataService,
            IAudioTranscoderService defaultTranscoder,
            Random random
        )
        {
            _logger = logger;
            _gatewayClient = gatewayClient;
            _clientOptions = clientOptions.Value;
            _connectionWaiterService = connectionWaiterService;
            _transportService = transportService;
            _dataService = dataService;
            _random = random;

            _heartbeatData = new HeartbeatData();
            _payloadsToSend = new ConcurrentQueue<IVoicePayload>();
            _receivedPayloads = new ConcurrentQueue<IVoicePayload>();

            _disconnectRequestedSource = new CancellationTokenSource();
            _runLoopTask = Task.FromResult(Result.FromSuccess());
            _sendTask = Task.FromResult(Result.FromSuccess());
            _receiveTask = Task.FromResult(Result.FromSuccess());

            _transmitSemaphore = new SemaphoreSlim(1, 1);
            _defaultTranscoder = defaultTranscoder;

            _silenceFrameBuffer = MemoryPool<byte>.Shared.Rent(3);
            _silenceFrameBuffer.Memory.Span[0] = 0xF8;
            _silenceFrameBuffer.Memory.Span[0] = 0xFF;
            _silenceFrameBuffer.Memory.Span[0] = 0xFE;

            this.ConnectionStatus = GatewayConnectionStatus.Offline;
        }

        /// <summary>
        /// Runs the voice gateway client.
        /// <para>
        /// This task will not complete until cancelled (or faulted), maintaining the connection for the duration of it.
        ///
        /// If the gateway client encounters a fatal problem during the execution of this task, it will return with a
        /// failed result. If a shutdown is requested, it will gracefully terminate the connection and return a
        /// successful result.
        /// </para>
        /// </summary>
        /// <param name="guildID">The ID of the guild to connect to.</param>
        /// <param name="channelID">The ID of the channel to connect to.</param>
        /// <param name="isSelfMuted">A value indicating whether the bot should mute itself.</param>
        /// <param name="isSelfDeafened">A value indicating whether the bot should deafen itself.</param>
        /// <param name="ct">A token by which the caller can request this method to stop.</param>
        /// <returns>A gateway connection result which may or may not have succeeded.</returns>
        public async Task<Result> RunAsync
        (
            Snowflake guildID,
            Snowflake channelID,
            bool isSelfMuted,
            bool isSelfDeafened,
            CancellationToken ct = default
        )
        {
            try
            {
                if (this.IsRunning)
                {
                    return new InvalidOperationError("Already running.");
                }

                if (!_defaultTranscoder.IsInitialized)
                {
                    _defaultTranscoder.Initialize();
                }

                _connectionRequestParameters = new UpdateVoiceState
                (
                    guildID,
                    isSelfMuted,
                    isSelfDeafened,
                    channelID
                );

                _disconnectRequestedSource.Dispose();
                _disconnectRequestedSource = new CancellationTokenSource();

                Result connectionResult = await ConnectAsync(_connectionRequestParameters, ct).ConfigureAwait(false);
                if (!connectionResult.IsSuccess)
                {
                    SendDisconnectVoiceStateUpdate(guildID);
                    await CleanupAsync().ConfigureAwait(false);
                    return connectionResult;
                }

                _runLoopTask = RunnerAsync(_connectionRequestParameters, _disconnectRequestedSource.Token);

                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                await CleanupAsync().ConfigureAwait(false);
                return ex;
            }
        }

        /// <summary>
        /// Stops the voice client.
        /// </summary>
        /// <returns>A <see cref="Result"/> representing the outcome of the operation.</returns>
        public async Task<Result> StopAsync()
        {
            try
            {
                if (!this.IsRunning)
                {
                    return new InvalidOperationError("Already stopped.");
                }

                await CleanupAsync().ConfigureAwait(false);

                Result runLoopTaskResult = await _runLoopTask.ConfigureAwait(false);
                if (!runLoopTaskResult.IsSuccess)
                {
                    return runLoopTaskResult;
                }

                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                this.ConnectionStatus = GatewayConnectionStatus.Offline;
            }
        }

        /// <summary>
        /// Enqueues a voice gateway command for sending.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command to send.</typeparam>
        /// <param name="operationCode">The operation code.</param>
        /// <param name="command">The command object.</param>
        public void EnqueueCommand<TCommand>(VoiceOperationCode operationCode, TCommand command) where TCommand : IVoiceGatewayCommand
        {
            VoicePayload<TCommand> payload = new(operationCode, command);
            _payloadsToSend.Enqueue(payload);
        }

        /// <summary>
        /// Transmits audio.
        /// </summary>
        /// <param name="audioStream">The audio data to send.</param>
        /// <param name="speakingFlags">The speaking flags to use.</param>
        /// <param name="transcoder">The audio transcoder to use.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A result representing the outcome of the operation.</returns>
        public async Task<Result> TransmitAudioAsync
        (
            Stream audioStream,
            SpeakingFlags speakingFlags = SpeakingFlags.Microphone,
            IAudioTranscoderService? transcoder = null,
            CancellationToken ct = default
        )
        {
            if (this.ConnectionStatus is not GatewayConnectionStatus.Connected || !_dataService.IsConnected)
            {
                return new InvalidOperationError("Gateway is not connected.");
            }

            if (speakingFlags is not SpeakingFlags.Microphone or SpeakingFlags.Priority)
            {
                return new InvalidOperationError($"Speaking flags must not be set to {nameof(SpeakingFlags.Microphone)} or {nameof(SpeakingFlags.Priority)}.");
            }

            if (transcoder is null)
            {
                transcoder = _defaultTranscoder;
                await transcoder.ResetAsync(ct).ConfigureAwait(false);
            }

            if (!transcoder.IsInitialized)
            {
                return new InvalidOperationError("The provided transcoder must first be initialized.");
            }

            // Take a local copy of the SSRC so we can still send a voice state update when the client is stopped
            uint ssrc = _voiceServerConnectionDetails!.SSRC;
            bool needsRelease = false;

            int sampleSize = transcoder.SampleSize;
            IMemoryOwner<byte> inputBuffer = MemoryPool<byte>.Shared.Rent(sampleSize);
            IMemoryOwner<byte> opusBuffer = MemoryPool<byte>.Shared.Rent(sampleSize);

            try
            {
                if (!await _transmitSemaphore.WaitAsync(10, ct).ConfigureAwait(false))
                {
                    return new InvalidOperationError("This client is already transmitting audio.");
                }

                needsRelease = true;

                // From https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.VoiceNext/VoiceNextConnection.cs
                double synchronizerTicks = Stopwatch.GetTimestamp();
                double synchronizerResolution = Stopwatch.Frequency * 0.005;
                double tickResolution = 10_000_000.0 / Stopwatch.Frequency;

                EnqueueCommand
                (
                    VoiceOperationCode.Speaking,
                    new VoiceSpeakingCommand
                    (
                        speakingFlags,
                        0,
                        ssrc
                    )
                );

                while (!ct.IsCancellationRequested)
                {
                    int read = await audioStream.ReadAsync
                    (
                        inputBuffer.Memory[0..sampleSize],
                        ct
                    ).ConfigureAwait(false);

                    if (read != sampleSize)
                    {
                        break; // TODO: Does this cut off audio in some cases?
                    }

                    Result<int> encodeResult = await transcoder.EncodeAsync
                    (
                        inputBuffer.Memory[..read],
                        opusBuffer.Memory[..read],
                        ct
                    ).ConfigureAwait(false);

                    if (!encodeResult.IsSuccess)
                    {
                        return Result.FromError(encodeResult);
                    }

                    // Adapted from https://github.com/DSharpPlus/DSharpPlus/blob/master/DSharpPlus.VoiceNext/VoiceNextConnection.cs
                    int durationModifier = Pcm16Util.CalculateSampleDuration(read) / 5;
                    double cts = Math.Max(Stopwatch.GetTimestamp() - synchronizerTicks, 0);

                    if (cts < synchronizerResolution * durationModifier)
                    {
                        long waitTicks = (long)(((synchronizerResolution * durationModifier) - cts) * tickResolution);

                        await Task.Delay
                        (
                            TimeSpan.FromTicks(waitTicks),
                            ct
                        ).ConfigureAwait(false);
                    }

                    synchronizerTicks += synchronizerResolution * durationModifier;

                    // We check for a client stoppage immediately before sending to minimise the chance of
                    // the data service being stopped during the synchroniser wait
                    if (_disconnectRequestedSource.IsCancellationRequested)
                    {
                        break;
                    }

                    Result sendFrameResult = await _dataService.SendFrameAsync
                    (
                        opusBuffer.Memory[0..encodeResult.Entity],
                        read,
                        ct
                    ).ConfigureAwait(false);

                    if (!sendFrameResult.IsSuccess)
                    {
                        return sendFrameResult;
                    }
                }

                Result sendSilence = SendSilenceFrames(sampleSize, ct);
                if (!sendSilence.IsSuccess)
                {
                    return sendSilence;
                }

                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                inputBuffer.Dispose();
                opusBuffer.Dispose();

                EnqueueCommand
                (
                    VoiceOperationCode.Speaking,
                    new VoiceSpeakingCommand
                    (
                        SpeakingFlags.None,
                        0,
                        ssrc
                    )
                );

                if (needsRelease)
                {
                    _transmitSemaphore.Release();
                }
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (this.ConnectionStatus is not GatewayConnectionStatus.Offline)
            {
                await StopAsync().ConfigureAwait(false);
            }

            _transmitSemaphore.Dispose();
            _silenceFrameBuffer.Dispose();
        }

        /// <summary>
        /// Restores the client to a near pre-startup state, intended for when the client is stopping or has encountered a fatal error.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task CleanupAsync()
        {
            if (!_disconnectRequestedSource.IsCancellationRequested)
            {
                _disconnectRequestedSource.Cancel();
            }

            await Task.WhenAll(_sendTask, _receiveTask, _runLoopTask).ConfigureAwait(false);
            _sendTask.Dispose();
            _receiveTask.Dispose();
            _runLoopTask.Dispose();

            if (_transportService.IsConnected)
            {
                await _transportService.DisconnectAsync(false).ConfigureAwait(false);
            }

            if (_dataService.IsConnected)
            {
                _dataService.Disconnect();
            }

            _disconnectRequestedSource.Dispose();
            _disconnectRequestedSource = new CancellationTokenSource();

            _payloadsToSend.Clear();
            _receivedPayloads.Clear();
            _connectionRequestParameters = null;
            _gatewayConnectionDetails = null;
            _voiceServerConnectionDetails = null;

            this.ConnectionStatus = GatewayConnectionStatus.Offline;
        }

        /// <summary>
        /// Runs the voice connection.
        /// </summary>
        /// <param name="connectionParameters">The parameters used to connect to the gateway.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A <see cref="Result"/> representing the outcome of the operation.</returns>
        private async Task<Result> RunnerAsync
        (
            UpdateVoiceState connectionParameters,
            CancellationToken ct
        )
        {
            this.IsRunning = true;
            await Task.Yield();

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    switch (this.ConnectionStatus)
                    {
                        case GatewayConnectionStatus.Offline:
                        case GatewayConnectionStatus.Disconnected:
                            Result connectionResult = await ConnectAsync(connectionParameters, ct).ConfigureAwait(false);
                            if (!connectionResult.IsSuccess)
                            {
                                return connectionResult;
                            }
                            break;
                        case GatewayConnectionStatus.Connected:
                            if (_sendTask.IsCompleted)
                            {
                                // TODO: Don't just blank return here.
                                Console.WriteLine("Send task failed.");
                                Result sendTaskResult = await _sendTask.ConfigureAwait(false);
                                if (!sendTaskResult.IsSuccess)
                                {
                                    return sendTaskResult;
                                }
                            }

                            if (_receiveTask.IsCompleted)
                            {
                                Console.WriteLine("Receive task failed.");
                                Result receiveTaskResult = await _receiveTask.ConfigureAwait(false);
                                if (!receiveTaskResult.IsSuccess)
                                {
                                    return receiveTaskResult;
                                }
                            }

                            // TODO: Dispatch received events
                            // TODO: Check health of voice socket
                            await Task.Delay(10, ct).ConfigureAwait(false);
                            break;
                    }
                }

                return await BeforeReturn().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Result retResult = await BeforeReturn().ConfigureAwait(false);

                if (ex is TaskCanceledException or OperationCanceledException)
                {
                    return retResult;
                }

                return ex;
            }
            finally
            {
                if (_connectionRequestParameters is not null)
                {
                    SendDisconnectVoiceStateUpdate(_connectionRequestParameters.GuildID);
                }

                this.IsRunning = false;
            }

            async Task<Result> BeforeReturn()
            {
                Result transportDisconnectResult = await _transportService.DisconnectAsync(false, ct).ConfigureAwait(false);
                Result dataDisconnectResult = _dataService.Disconnect();

                if (!transportDisconnectResult.IsSuccess)
                {
                    return transportDisconnectResult;
                }

                if (!dataDisconnectResult.IsSuccess)
                {
                    return dataDisconnectResult;
                }

                return Result.FromSuccess();
            }
        }

        /// <summary>
        /// Attempts to connect to the voice gateway and server.
        /// </summary>
        /// <param name="connectionParameters">The connection parameters.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A result representing the outcome of the operation.</returns>
        private async Task<Result> ConnectAsync
        (
            UpdateVoiceState connectionParameters,
            CancellationToken ct
        )
        {
            switch (this.ConnectionStatus)
            {
                case GatewayConnectionStatus.Offline:
                    Result initialConnectResult = await InitialConnectionAsync(connectionParameters, ct).ConfigureAwait(false);
                    if (!initialConnectResult.IsSuccess)
                    {
                        return initialConnectResult;
                    }
                    break;
                case GatewayConnectionStatus.Disconnected:
                    Result resumeResult = await ResumeConnectionAsync(ct).ConfigureAwait(false);
                    if (!resumeResult.IsSuccess)
                    {
                        return resumeResult;
                    }
                    break;
            }

            Result<string> selectedEncryptionMode = _dataService.SelectSupportedEncryptionMode(_voiceServerConnectionDetails!.Modes);
            if (!selectedEncryptionMode.IsSuccess)
            {
                return Result.FromError(selectedEncryptionMode);
            }

            Result<IPDiscoveryResponse> voiceServerConnectResult = await _dataService.ConnectAsync
            (
                _voiceServerConnectionDetails!,
                ct
            ).ConfigureAwait(false);

            if (!voiceServerConnectResult.IsSuccess)
            {
                return Result.FromError(voiceServerConnectResult);
            }

            VoiceSelectProtocol selectProtocol = new
            (
                "udp",
                new VoiceProtocolData
                (
                    voiceServerConnectResult.Entity.Address.TrimEnd('\0'),
                    voiceServerConnectResult.Entity.Port,
                    selectedEncryptionMode.Entity
                )
            );
            EnqueueCommand(VoiceOperationCode.Speaking, selectProtocol);

            DateTimeOffset startedWaitingForSessionDescription = DateTimeOffset.UtcNow;
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return new VoiceGatewayError("Operation was cancelled.", true);
                }

                Result<IVoicePayload> sessionDescriptionPayload = await _transportService.ReceivePayloadAsync(ct).ConfigureAwait(false);
                if (!sessionDescriptionPayload.IsDefined())
                {
                    return Result.FromError
                    (
                        new VoiceGatewayError("Failed to receive session description payload", true),
                        sessionDescriptionPayload
                    );
                }

                if (sessionDescriptionPayload.Entity is not IVoicePayload<IVoiceSessionDescription> sessionDescription)
                {
                    if (startedWaitingForSessionDescription.AddSeconds(2) < DateTimeOffset.UtcNow)
                    {
                        return new VoiceGatewayError("Did not receive a session description payload.", true);
                    }

                    if (sessionDescriptionPayload.Entity is not IVoicePayload<IVoiceHeartbeatAcknowledge>)
                    {
                        _receivedPayloads.Enqueue(sessionDescriptionPayload.Entity);
                    }

                    continue;
                }

                _receivedPayloads.Enqueue(sessionDescription);
                _dataService.Initialize(sessionDescription.Data.SecretKey);
                SendSilenceFrames(_defaultTranscoder.SampleSize, ct); // Can be the key to enabling voice receiving in some cases

                break;
            }

            _receiveTask = GatewayReceiverAsync(_disconnectRequestedSource.Token);
            return Result.FromSuccess();
        }

        /// <summary>
        /// Performs the initial handshake logic to connect with the voice gateway.
        /// Calls <see cref="ConnectToGatewayAndBeginSendTask(Uri, CancellationToken)"/>.
        /// </summary>
        /// <param name="connectionParameters">The connection parameters.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A <see cref="Result"/> representing the outcome of the operation.</returns>
        private async Task<Result> InitialConnectionAsync
        (
            UpdateVoiceState connectionParameters,
            CancellationToken ct
        )
        {
            _gatewayClient.SubmitCommand(connectionParameters);

            Result<VoiceConnectionEstablishmentDetails> getConnectionDetails = await _connectionWaiterService.WaitForRequestConfirmation
            (
                connectionParameters.GuildID,
                ct: ct
            ).ConfigureAwait(false);

            if (!getConnectionDetails.IsDefined())
            {
                return Result.FromError(getConnectionDetails);
            }

            _gatewayConnectionDetails = getConnectionDetails.Entity;

            // Using the full namespace here to help avoid potential confusion between the normal and voice gateway event sets.
            API.Abstractions.Gateway.Events.IVoiceStateUpdate voiceState = getConnectionDetails.Entity.VoiceState;
            API.Abstractions.Gateway.Events.IVoiceServerUpdate voiceServer = getConnectionDetails.Entity.VoiceServer;

            if (voiceServer.Endpoint is null)
            {
                return new VoiceGatewayUnavailableError();
            }

            Result<Uri> constructUriResult = ConstructVoiceGatewayEndpoint(voiceServer.Endpoint);
            if (!constructUriResult.IsDefined())
            {
                return Result.FromError(constructUriResult);
            }

            // Connect the websocket and start the send task
            Result webSocketConnectionResult = await ConnectToGatewayAndBeginSendTask
            (
                constructUriResult.Entity,
                ct
            ).ConfigureAwait(false);

            if (!webSocketConnectionResult.IsSuccess)
            {
                return webSocketConnectionResult;
            }

            EnqueueCommand
            (
                VoiceOperationCode.Identify,
                new VoiceIdentify
                (
                    voiceServer.GuildID,
                    voiceState.UserID,
                    voiceState.SessionID,
                    voiceServer.Token
                )
            );

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return new VoiceGatewayError("Operation was cancelled.", true);
                }

                Result<IVoicePayload> readyPayload = await _transportService.ReceivePayloadAsync(ct).ConfigureAwait(false);
                if (!readyPayload.IsDefined())
                {
                    return Result.FromError
                    (
                        new VoiceGatewayError("Failed to receive voice ready payload", true),
                        readyPayload
                    );
                }

                if (readyPayload.Entity is IVoicePayload<IVoiceHeartbeatAcknowledge>)
                {
                    continue;
                }

                if (readyPayload.Entity is not IVoicePayload<IVoiceReady> ready)
                {
                    return new VoiceGatewayError("The identification response payload from the gateway was not a ready message.", true);
                }

                _receivedPayloads.Enqueue(ready);
                _voiceServerConnectionDetails = ready.Data;

                break;
            }

            this.ConnectionStatus = GatewayConnectionStatus.Connected;
            return Result.FromSuccess();
        }

        /// <summary>
        /// Resumes an existing session with the voice gateway, replaying missed events.
        /// Calls <see cref="ConnectToGatewayAndBeginSendTask(Uri, CancellationToken)"/>.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/>.</param>
        /// <returns>A connection result which may or may not have succeeded.</returns>
        private async Task<Result> ResumeConnectionAsync(CancellationToken ct)
        {
            if (_gatewayConnectionDetails is null)
            {
                return new InvalidOperationError("There is no session to resume.");
            }

            // Using the full namespace here to help avoid potential confusion between the normal and voice gateway event sets.
            API.Abstractions.Gateway.Events.IVoiceStateUpdate voiceState = _gatewayConnectionDetails.VoiceState;
            API.Abstractions.Gateway.Events.IVoiceServerUpdate voiceServer = _gatewayConnectionDetails.VoiceServer;

            Result<Uri> constructUriResult = ConstructVoiceGatewayEndpoint(voiceServer.Endpoint!);
            if (!constructUriResult.IsDefined())
            {
                return Result.FromError(constructUriResult);
            }

            // Connect the websocket and start the send task
            Result webSocketConnectionResult = await ConnectToGatewayAndBeginSendTask
            (
                constructUriResult.Entity,
                ct
            ).ConfigureAwait(false);

            if (!webSocketConnectionResult.IsSuccess)
            {
                return webSocketConnectionResult;
            }

            EnqueueCommand
            (
                VoiceOperationCode.Resume,
                new VoiceResume
                (
                    voiceServer.GuildID,
                    voiceState.SessionID,
                    voiceServer.Token
                )
            );

            // Push resumed events onto the queue
            var resuming = true;
            while (resuming)
            {
                if (ct.IsCancellationRequested)
                {
                    return new VoiceGatewayError("Operation was cancelled.", true);
                }

                var receiveEvent = await _transportService.ReceivePayloadAsync(ct).ConfigureAwait(false);

                if (!receiveEvent.IsDefined())
                {
                    if (receiveEvent.Error is VoiceGatewayDiscordError)
                    {
                        // Reconnect on next iteration
                        this.ConnectionStatus = GatewayConnectionStatus.Offline;
                        return Result.FromSuccess();
                    }

                    return Result.FromError
                    (
                        new VoiceGatewayError
                        (
                            "Failed to receive a payload.",
                            true
                        ),
                        receiveEvent
                    );
                }

                switch (receiveEvent.Entity)
                {
                    case IVoicePayload<IVoiceHeartbeatAcknowledge>:
                            continue;
                    case IVoicePayload<IVoiceResumed>:
                            resuming = false;
                            break;
                }

                _receivedPayloads.Enqueue(receiveEvent.Entity);
            }

            this.ConnectionStatus = GatewayConnectionStatus.Connected;
            return Result.FromSuccess();
        }

        /// <summary>
        /// Constructs a <see cref="Uri"/> to the voice gateway endpoint.
        /// </summary>
        /// <param name="endpoint">The provided string endpoint.</param>
        /// <returns>A <see cref="Result"/> representing the outcome of the operation.</returns>
        private static Result<Uri> ConstructVoiceGatewayEndpoint(string endpoint)
        {
            endpoint = $"wss://{endpoint}?v=4";
            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var gatewayUri))
            {
                return new VoiceGatewayError
                (
                    "Failed to parse the received voice gateway endpoint.",
                    true
                );
            }

            return gatewayUri;
        }

        /// <summary>
        /// Connects the websocket and begins the send task.
        /// </summary>
        /// <param name="gatewayUri">The URI of the voice gateway.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A <see cref="Result"/> indicating the outcome of the operation.</returns>
        private async Task<Result> ConnectToGatewayAndBeginSendTask
        (
            Uri gatewayUri,
            CancellationToken ct
        )
        {
            Result connectResult = await _transportService.ConnectAsync(gatewayUri, ct).ConfigureAwait(false);
            if (!connectResult.IsSuccess)
            {
                return connectResult;
            }

            Result<IVoicePayload> helloPayload = await _transportService.ReceivePayloadAsync(ct).ConfigureAwait(false);
            if (!helloPayload.IsDefined())
            {
                return Result.FromError
                (
                    new VoiceGatewayError("Failed to receive hello payload.", true),
                    helloPayload
                );
            }

            if (helloPayload.Entity is not IVoicePayload<IVoiceHello> hello)
            {
                return new VoiceGatewayError("The first payload from the voice gateway was not a hello. Rude!", true);
            }

            _heartbeatData.Interval = hello.Data.HeartbeatInterval;
            _heartbeatData.LastSentTime = DateTimeOffset.UtcNow;
            _receivedPayloads.Enqueue(hello);
            _sendTask = GatewaySenderAsync(_disconnectRequestedSource.Token);

            return Result.FromSuccess();
        }

        /// <summary>
        /// Gets a value indicating if the gateway connection should be re-established.
        /// </summary>
        /// <param name="iterationResult">The result of the last connection iteration.</param>
        /// <param name="withNewSession">Defines whether a resume or reconnect should be attempted.</param>
        /// <returns>A value indicating whether or not the connection should be re-established.</returns>
        private bool ShouldReconnect
        (
            Result iterationResult,
            out bool withNewSession
        )
        {
            withNewSession = false;

            switch (iterationResult.Error)
            {
                case VoiceGatewayDiscordError gde:
                    {
                        switch (gde.CloseStatus)
                        {
                            case VoiceGatewayCloseStatus.AlreadyAuthenticated:
                            case VoiceGatewayCloseStatus.FailedToDecodePayload:
                            case VoiceGatewayCloseStatus.Ratelimited:
                            case VoiceGatewayCloseStatus.UnknownEncryptionMode:
                            case VoiceGatewayCloseStatus.UnknownProtocol:
                            case VoiceGatewayCloseStatus.UnknownOperationCode:
                                {
                                    return true;
                                }
                            case VoiceGatewayCloseStatus.NotAuthenticated:
                            case VoiceGatewayCloseStatus.SessionNoLongerValid:
                            case VoiceGatewayCloseStatus.SessionTimeout:
                            case VoiceGatewayCloseStatus.ServerNotFound:
                            case VoiceGatewayCloseStatus.VoiceServerCrash:
                                {
                                    withNewSession = true;
                                    return true;
                                }
                            case VoiceGatewayCloseStatus.AuthenticationFailed:
                            case VoiceGatewayCloseStatus.Disconnected:
                                {
                                    return false;
                                }
                        }

                        break;
                    }
                case VoiceGatewayWebSocketError gwe:
                    {
                        switch (gwe.CloseStatus)
                        {
                            case WebSocketCloseStatus.InternalServerError:
                            case WebSocketCloseStatus.EndpointUnavailable:
                                {
                                    withNewSession = true;
                                    return true;
                                }
                        }

                        break;
                    }
                case VoiceGatewayError gae:
                    {
                        // We'll try reconnecting on non-critical internal errors
                        return !gae.IsCritical;
                    }
                case ExceptionError exe:
                    {
                        switch (exe.Exception)
                        {
                            case System.Net.Http.HttpRequestException or WebSocketException:
                                {
                                    _logger.LogWarning
                                    (
                                        exe.Exception,
                                        "Transient error in gateway client: {Exception}",
                                        exe.Message
                                    );

                                    return true;
                                }
                            default:
                                {
                                    return false;
                                }
                        }
                    }
            }

            // We don't know what happened... try reconnecting?
            return true;
        }

        /// <summary>
        /// This method acts as the main entrypoint for the gateway sender task.
        /// It processes and sends submitted payloads to the gateway, and calls <see cref="SendHeartbeatAsync(CancellationToken)"/>.
        /// </summary>
        /// <param name="ct">A token for requests to disconnect the socket.</param>
        /// <returns>
        /// A result which may or may not have been successful. A failed result indicates that something
        /// has gone wrong when sending a payload, and that the connection has been deemed nonviable. A nonviable
        /// connection should be either terminated, reestablished, or resumed as appropriate.
        /// </returns>
        private async Task<Result> GatewaySenderAsync(CancellationToken ct)
        {
            await Task.Yield();

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    Result<TimeSpan> heartbeatResult = await SendHeartbeatAsync(ct).ConfigureAwait(false);
                    if (!heartbeatResult.IsSuccess)
                    {
                        return Result.FromError(heartbeatResult);
                    }

                    // Check if there are any user-submitted payloads to send
                    if (!_payloadsToSend.TryDequeue(out var payload))
                    {
                        // Let's sleep for a little while
                        TimeSpan sleepTime = TimeSpan.FromMilliseconds(Math.Clamp(100, 0, heartbeatResult.Entity.TotalMilliseconds));
                        await Task.Delay(sleepTime, ct).ConfigureAwait(false);
                        continue;
                    }

                    Result sendResult = await _transportService.SendPayloadAsync(payload, ct).ConfigureAwait(false);
                    if (sendResult.IsSuccess)
                    {
                        continue;
                    }

                    // Normal closures are okay
                    return sendResult.Error is VoiceGatewayWebSocketError { CloseStatus: System.Net.WebSockets.WebSocketCloseStatus.NormalClosure }
                        ? Result.FromSuccess()
                        : sendResult;
                }

                return Result.FromSuccess();
            }
            catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
            {
                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// This method acts as the main entrypoint for the gateway receiver task. It receives and processes payloads from the gateway.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>
        /// A receiver result which may or may not have been successful. A failed result indicates that
        /// something has gone wrong when receiving a payload, and that the connection has been deemed nonviable. A
        /// nonviable connection should be either terminated, reestablished, or resumed as appropriate.
        /// </returns>
        private async Task<Result> GatewayReceiverAsync(CancellationToken ct)
        {
            await Task.Yield();

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    Result<IVoicePayload> receiveResult = await _transportService.ReceivePayloadAsync(ct).ConfigureAwait(false);

                    if (!receiveResult.IsSuccess)
                    {
                        // Normal closures are okay
                        return receiveResult.Error is VoiceGatewayWebSocketError { CloseStatus: WebSocketCloseStatus.NormalClosure }
                            ? Result.FromSuccess()
                            : Result.FromError(receiveResult);
                    }

                    // Update the ack timestamp and gateway latency
                    if (receiveResult.Entity is IVoicePayload<IVoiceHeartbeatAcknowledge> heartbeatAck)
                    {
                        _heartbeatData.LastReceivedAckTime = DateTime.UtcNow;
                        _heartbeatData.LastReceivedNonce = heartbeatAck.Data.Nonce;

                        this.GatewayLatency = _heartbeatData.LastReceivedAckTime!.Value - _heartbeatData.LastSentTime!.Value;

                        continue;
                    }

                    _receivedPayloads.Enqueue(receiveResult.Entity);
                }

                return Result.FromSuccess();
            }
            catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
            {
                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Sends a heartbeat if required.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>
        /// A <see cref="Result"/> representing the outcome of the operation,
        /// and containing the maximum safe amount of time needed until the next heartbeat if the operation was successful.
        /// </returns>
        private async Task<Result<TimeSpan>> SendHeartbeatAsync(CancellationToken ct)
        {
            try
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;
                TimeSpan safetyMargin = _clientOptions.GetTrueHeartbeatSafetyMargin(_heartbeatData.Interval);

                if (_heartbeatData.LastSentTime is null || now - _heartbeatData.LastSentTime >= _heartbeatData.Interval - safetyMargin)
                {
                    if (_heartbeatData.LastReceivedAckTime < _heartbeatData.LastSentTime)
                    {
                        return new VoiceGatewayError
                        (
                            "The server did not respond in time with a heartbeat acknowledgement.",
                            false
                        );
                    }

                    if (_heartbeatData.LastSentNonce != _heartbeatData.LastReceivedNonce)
                    {
                        return new VoiceGatewayError
                        (
                            "The server did not respond with a valid heartbeat.",
                            false
                        );
                    }

                    _heartbeatData.LastSentNonce = _random.Next();
                    Result sendHeartbeatResult = await _transportService.SendPayloadAsync
                    (
                        new VoicePayload<VoiceHeartbeat>
                        (
                            VoiceOperationCode.Heartbeat,
                            new VoiceHeartbeat(_heartbeatData.LastReceivedNonce)
                        ),
                        ct
                    ).ConfigureAwait(false);

                    if (!sendHeartbeatResult.IsSuccess)
                    {
                        return Result<TimeSpan>.FromError
                        (
                            new VoiceGatewayError("Failed to send a heartbeat.", false),
                            sendHeartbeatResult
                        );
                    }

                    _heartbeatData.LastSentTime = DateTimeOffset.UtcNow;
                }

                TimeSpan safeTimeTillNext = _heartbeatData.LastSentTime.Value + _heartbeatData.Interval - safetyMargin - now;
                return Result<TimeSpan>.FromSuccess(safeTimeTillNext);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Sends an <see cref="UpdateVoiceState"/> command to the gateway, requesting that the bot be disconnected from a guild's voice channel.
        /// </summary>
        /// <param name="guildID">The guild containing the voice channel to disconnect from.</param>
        private void SendDisconnectVoiceStateUpdate(Snowflake guildID)
        {
            _gatewayClient.SubmitCommand
            (
                new UpdateVoiceState
                (
                    guildID,
                    false,
                    false
                )
            );
        }

        /// <summary>
        /// Sends five audio frames of silence to help with interpolation.
        /// </summary>
        /// <param name="sampleSize">The size of a normal audio sample for the transmission.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A result representing the outcome of the operation.</returns>
        private Result SendSilenceFrames(int sampleSize, CancellationToken ct = default)
        {
            for (int i = 0; i < 5 && !ct.IsCancellationRequested && !_disconnectRequestedSource.IsCancellationRequested; i++)
            {
                Result sendFrameResult = _dataService.SendFrame(_silenceFrameBuffer.Memory.Span[..3], sampleSize);
                if (!sendFrameResult.IsSuccess)
                {
                    return sendFrameResult;
                }
            }

            return Result.FromSuccess();
        }
    }
}
