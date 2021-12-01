//
//  OpusControlRequestDefinition.cs
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

namespace Remora.Discord.Voice.Interop.Opus
{
    /// <summary>
    /// Enumerates control codes for opus encoders and decoders.
    /// </summary>
    internal enum OpusControlRequestDefinition
    {
        /// <summary>
        /// A request to configure the bitrate in the encoder.
        /// </summary>
        /// <remarks>
        /// Rates from 500 to 512000 bits per second are meaningful,
        /// as well as the special values <see cref="OpusSpecialDefinition.Auto"/> and <see cref="OpusSpecialDefinition.BitrateMax"/>.
        /// The value <see cref="OpusSpecialDefinition.BitrateMax"/> can be used to cause the codec to use as much rate as it can,
        /// which is useful for controlling the rate by adjusting the output buffer size.
        /// </remarks>
        SetBitrate = 4002,

        /// <summary>
        /// A request to configures the encoder's use of inband forward error correction.
        /// </summary>
        SetInbandFec = 4012,

        /// <summary>
        /// A request to configure the encoder's expected packet loss percentage.
        /// </summary>
        SetPacketLossPercentage = 4014,

        /// <summary>
        /// A request to configure the type of signal being encoded. This is a hint which helps the encoder's mode selection.
        /// </summary>
        SetSignal = 4024,

        /// <summary>
        /// A request to reset the codec state to be equivalent to a freshly initialized state.
        /// </summary>
        /// <remarks>
        /// This should be called when switching streams in order to prevent the
        /// back to back decoding from giving different results from one at a time decoding.
        /// </remarks>
        ResetState = 4028
    }
}
