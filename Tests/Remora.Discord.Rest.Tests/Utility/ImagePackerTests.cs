//
//  ImagePackerTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.Rest.Utility;
using Remora.Discord.Tests;
using Xunit;

namespace Remora.Discord.Rest.Tests.Utility;

/// <summary>
/// Tests the <see cref="ImagePacker"/> class.
/// </summary>
public class ImagePackerTests
{
    /// <summary>
    /// Tests the <see cref="ImagePacker.PackImageAsync(Stream, CancellationToken)"/> method.
    /// </summary>
    public class PackImageAsync
    {
        /// <summary>
        /// Tests whether PNG images can be packed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CanPackPNGAsync()
        {
            // Create a dummy PNG image
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            image.Position = 0;

            var result = await ImagePacker.PackImageAsync(image);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether JPEG images can be packed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CanPackJPEGAsync()
        {
            // Create a dummy JPG image
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(0x00FFD8FF);
            image.Position = 0;

            var result = await ImagePacker.PackImageAsync(image);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether GIF images can be packed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CanPackGIFAsync()
        {
            // Create a dummy GIF image
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(0x00464947);
            image.Position = 0;

            var result = await ImagePacker.PackImageAsync(image);
            ResultAssert.Successful(result);
        }

        /// <summary>
        /// Tests whether unknown formats return an error.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ReturnsErrorIfFormatIsUnknown()
        {
            // Create a dummy unknown image
            await using var image = new MemoryStream();
            await using var binaryWriter = new BinaryWriter(image);
            binaryWriter.Write(0x00000000);
            image.Position = 0;

            var result = await ImagePacker.PackImageAsync(image);
            ResultAssert.Unsuccessful(result);
        }
    }
}
