//
//  Pcm16AudioTranscoderService.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.Voice.Abstractions.Services;
using Remora.Discord.Voice.Interop.Opus;
using Remora.Discord.Voice.Util;
using Remora.Results;

namespace Remora.Discord.Voice.Services
{
    /// <summary>
    /// <inheritdoc/>
    /// This class deals with the transcoding of PCM-16 audio samples.
    /// </summary>
    [PublicAPI]
    public class Pcm16AudioTranscoderService : IAudioTranscoderService, IDisposable
    {
        /// <summary>
        /// Gets the duration of audio that this service transcodes with each sample.
        /// </summary>
        public const int SampleDurationMS = 40;

        private readonly SemaphoreSlim _encodeSemaphore;

        private OpusEncoder? _encoder;

        /// <inheritdoc />
        public int SampleSize { get; }

        /// <summary>
        /// Gets a value indicating whether or not this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        [MemberNotNullWhen(true, nameof(_encoder))]
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pcm16AudioTranscoderService"/> class.
        /// </summary>
        public Pcm16AudioTranscoderService()
        {
            _encodeSemaphore = new SemaphoreSlim(1, 1);
            this.SampleSize = Pcm16Util.CalculateSampleSize(SampleDurationMS);
        }

        /// <inheritdoc />
        public Result Initialize(OpusApplicationDefinition audioType = OpusApplicationDefinition.Audio)
        {
            Result<OpusEncoder> createEncoder = OpusEncoder.Create(audioType);
            if (!createEncoder.IsSuccess)
            {
                return Result.FromError(createEncoder);
            }

            _encoder = createEncoder.Entity;

            this.IsInitialized = true;
            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public async Task<Result<int>> EncodeAsync(ReadOnlyMemory<byte> sample, Memory<byte> opusOutput, CancellationToken ct = default)
        {
            if (!this.IsInitialized)
            {
                return new InvalidOperationError("The pipe must be initialized before data can be sent.");
            }

            var releaseEncode = false;

            try
            {
                releaseEncode = await _encodeSemaphore.WaitAsync(10, ct).ConfigureAwait(false);
                if (!releaseEncode)
                {
                    return new InvalidOperationError("Cannot reset while this instance is encoding.");
                }

                return _encoder.Encode(sample.Span, opusOutput.Span);
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                if (releaseEncode)
                {
                    _encodeSemaphore.Release();
                }
            }
        }

        /// <inheritdoc />
        public async Task<Result> ResetAsync(CancellationToken ct = default)
        {
            if (this.IsDisposed)
            {
                return new ObjectDisposedException(nameof(Pcm16AudioTranscoderService));
            }

            var releaseEncode = false;

            try
            {
                releaseEncode = await _encodeSemaphore.WaitAsync(10, ct).ConfigureAwait(false);
                if (!releaseEncode)
                {
                    return new InvalidOperationError("Cannot reset while this instance is encoding.");
                }

                _encoder?.Reset();

                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                if (releaseEncode)
                {
                    _encodeSemaphore.Release();
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _encodeSemaphore.Dispose();
                _encoder?.Dispose();
            }

            this.IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
