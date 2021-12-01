//
//  OpusEncoder.cs
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
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Remora.Discord.Voice.Errors;
using Remora.Discord.Voice.Util;
using Remora.Results;
using VC = Remora.Discord.Voice.Util.VoiceConstants;

#pragma warning disable SA1300 // Element should begin with upper-case letter

namespace Remora.Discord.Voice.Interop.Opus
{
    /// <summary>
    /// Represents an interface to the native libopus library.
    /// </summary>
    public sealed unsafe partial class OpusEncoder
    {
        private const string OpusLibraryName = "libopus";

        /// <summary>
        /// Encodes an opus frame.
        /// </summary>
        /// <param name="state">The encoder state.</param>
        /// <param name="pcm">Input signal (interleaved if 2 channels). length is frame_size * channels * sizeof(short).</param>
        /// <param name="frameSize">
        /// Number of samples per channel in the input signal. This must be an Opus frame size for the encoder's sampling rate.
        /// For example, at 48 kHz the permitted values are 120, 240, 480, 960, 1920, and 2880.
        /// Passing in a duration of less than 10 ms (480 samples at 48 kHz) will prevent the encoder from using the LPC or hybrid modes.
        /// </param>
        /// <param name="data">Output payload. This must contain storage for at least <paramref name="maxDataBytes"/>.</param>
        /// <param name="maxDataBytes">
        /// Size of the allocated memory for the output payload.
        /// This may be used to impose an upper limit on the instant bitrate, but should not be used as the only bitrate control.
        /// Use OPUS_SET_BITRATE to control the bitrate.
        /// </param>
        /// <returns>The length of the encoded data (in bytes) on success or a negative error code (see <see cref="OpusErrorDefinition"/>) on failure.</returns>
        [DllImport(OpusLibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int opus_encode(IntPtr state, byte* pcm, int frameSize, byte* data, int maxDataBytes);

        /// <summary>
        /// Allocates and initializes an encoder state.
        /// </summary>
        /// <param name="inputSampleRate">Sampling rate of the input signal (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
        /// <param name="channels">Number of channels (1 or 2) in the input signal.</param>
        /// <param name="audioType">Coding mode (see <see cref="OpusApplicationDefinition"/>).</param>
        /// <param name="error">Error result, if any (see <see cref="OpusErrorDefinition"/>).</param>
        /// <returns>A pointer to the encoder state.</returns>
        [DllImport(OpusLibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr opus_encoder_create(int inputSampleRate, int channels, OpusApplicationDefinition audioType, OpusErrorDefinition* error);

        /// <summary>
        /// Set a control parameter on an encoder.
        /// </summary>
        /// <remarks>
        /// This method is ONLY 'SET' compatible.
        /// </remarks>
        /// <param name="encoderState">A pointer to the encoder state.</param>
        /// <param name="request">The parameter to set.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>An <see cref="OpusErrorDefinition"/> result.</returns>
        [DllImport(OpusLibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern OpusErrorDefinition opus_encoder_ctl(IntPtr encoderState, OpusControlRequestDefinition request, int value);

        /// <summary>
        /// Frees an encoder allocation by <see cref="opus_encoder_create(int, int, OpusApplicationDefinition, OpusErrorDefinition*)"/>.
        /// </summary>
        /// <param name="st">A pointer to the encoder state.</param>
        [DllImport(OpusLibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void opus_encoder_destroy(IntPtr st);
    }

    [PublicAPI]
    public sealed unsafe partial class OpusEncoder : IDisposable
    {
        private readonly IntPtr _state;

        /// <summary>
        /// Gets a value indicating whether or not this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private OpusEncoder(IntPtr state)
        {
            _state = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpusEncoder"/> class.
        /// </summary>
        /// <param name="codingMode">The coding mode to use.</param>
        /// <returns>The created <see cref="OpusEncoder"/>, or an error if the initialization failed.</returns>
        public static Result<OpusEncoder> Create(OpusApplicationDefinition codingMode)
        {
            var error = OpusErrorDefinition.OK;
            var state = opus_encoder_create(VC.DiscordSampleRate, VC.DiscordChannelCount, codingMode, &error);

            if (error is not OpusErrorDefinition.OK)
            {
                return new OpusError(error, "Failed to create an encoder.");
            }

            error = codingMode switch
            {
                OpusApplicationDefinition.Voip => opus_encoder_ctl(state, OpusControlRequestDefinition.SetSignal, (int)OpusSignalDefinition.Voice),
                OpusApplicationDefinition.Audio => opus_encoder_ctl(state, OpusControlRequestDefinition.SetSignal, (int)OpusSignalDefinition.Music),
                _ => opus_encoder_ctl(state, OpusControlRequestDefinition.SetSignal, (int)OpusSpecialDefinition.Auto),
            };

            if (error is not OpusErrorDefinition.OK)
            {
                opus_encoder_destroy(state);
                return new OpusError(error, "Failed to set encoder signal type");
            }

            error = opus_encoder_ctl(state, OpusControlRequestDefinition.SetPacketLossPercentage, 15);
            if (error is not OpusErrorDefinition.OK)
            {
                opus_encoder_destroy(state);
                return new OpusError(error, "Failed to set expected packet loss on encoder.");
            }

            error = opus_encoder_ctl(state, OpusControlRequestDefinition.SetInbandFec, 1); // Enable
            if (error is not OpusErrorDefinition.OK)
            {
                opus_encoder_destroy(state);
                return new OpusError(error, "Failed to enable in-band forward error control on encoder.");
            }

            error = opus_encoder_ctl(state, OpusControlRequestDefinition.SetBitrate, VC.DiscordMaxBitrate);
            if (error is not OpusErrorDefinition.OK)
            {
                opus_encoder_destroy(state);
                return new OpusError(error, "Failed to set bitrate on encoder.");
            }

            return new OpusEncoder(state);
        }

        /// <summary>
        /// Encodes an audio sample.
        /// </summary>
        /// <param name="pcm16">The PCM-16 audio data to encode.</param>
        /// <param name="output">The output buffer. Must be the same length as the <paramref name="pcm16"/> buffer.</param>
        /// <returns>The length of the encoded audio, or an error if the operation failed..</returns>
        public Result<int> Encode(ReadOnlySpan<byte> pcm16, Span<byte> output)
        {
            if (pcm16.Length != output.Length)
            {
                return new ArgumentOutOfRangeError(nameof(output), "PCM and output buffers must be of equal length.");
            }

            var frameSize = Pcm16Util.CalculateFrameSize(pcm16.Length);

            int writtenLength;
            fixed (byte* pcm16Ptr = pcm16)
            {
                fixed (byte* outputPtr = output)
                {
                    writtenLength = opus_encode(_state, pcm16Ptr, frameSize, outputPtr, output.Length);
                }
            }

            return writtenLength < 0
                ? new OpusError((OpusErrorDefinition)writtenLength, "Failed to encode audio sample.")
                : writtenLength;
        }

        /// <summary>
        /// Sets the bitrate to encode at.
        /// </summary>
        /// <param name="bitrate">The new bitrate. Must be between 0 and <see cref="VC.DiscordMaxBitrate"/>.</param>
        /// <returns>A result representing the outcome of the operation.</returns>
        public Result SetBitrate(int bitrate)
        {
            if (bitrate < 0 || bitrate > VC.DiscordMaxBitrate)
            {
                return new ArgumentOutOfRangeError(nameof(bitrate), $"Bitrate must be greater than zero and less than {VC.DiscordMaxBitrate}.");
            }

            OpusErrorDefinition error = opus_encoder_ctl(_state, OpusControlRequestDefinition.SetBitrate, bitrate);
            return error is not OpusErrorDefinition.OK
                ? new OpusError(error, "Failed to set bitrate on encoder.")
                : Result.FromSuccess();
        }

        /// <summary>
        /// Resets the state of the encoder in preparation for submitting a new stream.
        /// </summary>
        /// <returns>A result representing the outcome of the operation.</returns>
        public Result Reset()
        {
            OpusErrorDefinition error = opus_encoder_ctl(_state, OpusControlRequestDefinition.ResetState, 0);
            return error is not OpusErrorDefinition.OK
                ? new OpusError(error, "Failed to reset the encoder.")
                : Result.FromSuccess();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            opus_encoder_destroy(_state);

            this.IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="OpusEncoder"/> class.
        /// </summary>
        ~OpusEncoder()
        {
            Dispose();
        }
    }
}
