//
//  IAudioTranscoderService.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.Voice.Interop.Opus;
using Remora.Results;

namespace Remora.Discord.Voice.Abstractions.Services
{
    /// <summary>
    /// Represents an interface for transcoding audio segments to Opus audio data.
    /// </summary>
    [PublicAPI]
    public interface IAudioTranscoderService
    {
        /// <summary>
        /// Gets a value indicating whether or not this instance has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the size of the sample (and in turn, buffer) that should be used for transcoding audio with this service.
        /// </summary>
        int SampleSize { get; }

        /// <summary>
        /// Initializes this instance of the <see cref="IAudioTranscoderService"/> class.
        /// </summary>
        /// <param name="audioType">The type of audio that will be encoded. Used to optimise the output.</param>
        /// <returns>A result representing the outcome of the operation.</returns>
        Result Initialize(OpusApplicationDefinition audioType = OpusApplicationDefinition.Audio);

        /// <summary>
        /// Encodes a sample as Opus audio data.
        /// </summary>
        /// <param name="sample">The sample buffer to encode.</param>
        /// <param name="opusOutput">The output buffer for encoded Opus audio data.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A result representing the number of bytes encoded to the output buffer, or an error if the operation failed.</returns>
        Task<Result<int>> EncodeAsync(ReadOnlyMemory<byte> sample, Memory<byte> opusOutput, CancellationToken ct = default);

        /// <summary>
        /// Resets this instance in preparation for transcoding a new stream.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
        /// <returns>A result indicating the outcome of the operation.</returns>
        Task<Result> ResetAsync(CancellationToken ct = default);
    }
}
