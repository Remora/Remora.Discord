//
//  CDNTestBase.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Remora.Discord.API.Errors;
using Remora.Rest.Core;
using Remora.Results;
using Xunit;

namespace Remora.Discord.API.Tests.TestBases;

/// <summary>
/// Acts as a base class for CDN endpoints.
/// </summary>
public abstract class CDNTestBase
{
    /// <summary>
    /// Gets a valid image URI without a format extension.
    /// </summary>
    protected Uri ValidUriWithoutExtension { get; }

    private readonly IReadOnlyList<CDNImageFormat> _allFormats;
    private readonly IReadOnlyList<CDNImageFormat> _supportedFormats;

    /// <summary>
    /// Initializes a new instance of the <see cref="CDNTestBase"/> class.
    /// </summary>
    /// <param name="validUriWithoutExtension">A valid static image URI.</param>
    /// <param name="supportedFormats">The supported formats.</param>
    protected CDNTestBase
    (
        Uri validUriWithoutExtension,
        IReadOnlyList<CDNImageFormat> supportedFormats
    )
    {
        this.ValidUriWithoutExtension = validUriWithoutExtension;

        _allFormats = Enum.GetValues<CDNImageFormat>();
        _supportedFormats = supportedFormats;
    }

    /// <summary>
    /// Gets a set of image URIs, using the functions under test.
    /// </summary>
    /// <param name="imageFormat">The custom image format, if any.</param>
    /// <param name="imageSize">The custom image size, if any.</param>
    /// <returns>The results.</returns>
    protected abstract IEnumerable<Result<Uri>> GetImageUris
    (
        Optional<CDNImageFormat> imageFormat = default,
        Optional<ushort> imageSize = default
    );

    /// <summary>
    /// Tests whether the correct address is returned for the simple case.
    /// </summary>
    [Fact]
    public void ReturnsCorrectAddress()
    {
        foreach (var getActual in GetImageUris())
        {
            var expected = new Uri(this.ValidUriWithoutExtension + ".png");

            Assert.True(getActual.IsSuccess);
            Assert.Equal(expected, getActual.Entity);
        }
    }

    /// <summary>
    /// Tests whether the correct address is returned when a custom image format is requested.
    /// </summary>
    [Fact]
    public void ReturnsCorrectAddressWithCustomImageFormat()
    {
        foreach (var supportedFormat in _supportedFormats)
        {
            foreach (var getActual in GetImageUris(supportedFormat))
            {
                var expected = new Uri
                (
                    this.ValidUriWithoutExtension + $".{supportedFormat.ToString().ToLowerInvariant()}"
                );

                Assert.True(getActual.IsSuccess);
                Assert.Equal(expected, getActual.Entity);
            }
        }
    }

    /// <summary>
    /// Tests whether the correct address is returned when a custom image size is requested.
    /// </summary>
    [Fact]
    public void ReturnsCorrectAddressWithCustomImageSize()
    {
        foreach (var customSize in new ushort[] { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 })
        {
            foreach (var getActual in GetImageUris(imageSize: customSize))
            {
                var expected = new Uri
                (
                    this.ValidUriWithoutExtension + $".png?size={customSize}"
                );

                Assert.True(getActual.IsSuccess);
                Assert.Equal(expected, getActual.Entity);
            }
        }
    }

    /// <summary>
    /// Tests whether an unsuccessful result is returned if the requested image size is out of range.
    /// </summary>
    [Fact]
    public void ReturnsUnsuccessfulResultIfImageSizeIsOutOfRange()
    {
        foreach (var getActual in GetImageUris(imageSize: 8))
        {
            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageSizeOutOfRangeError>(getActual.Error);
        }

        foreach (var getActual in GetImageUris(imageSize: 8192))
        {
            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageSizeOutOfRangeError>(getActual.Error);
        }
    }

    /// <summary>
    /// Tests whether an unsuccessful result is returned if the requested image size is not a power of two.
    /// </summary>
    [Fact]
    public void ReturnsUnsuccessfulResultIfImageSizeIsNotPowerOfTwo()
    {
        foreach (var getActual in GetImageUris(imageSize: 63))
        {
            Assert.False(getActual.IsSuccess);
            Assert.IsType<ImageSizeNotPowerOfTwoError>(getActual.Error);
        }
    }

    /// <summary>
    /// Tests whether an unsuccessful result is returned if the requested image format is not supported by the
    /// endpoint.
    /// </summary>
    [SkippableFact]
    public void ReturnsUnsuccessfulResultIfImageFormatIsNotSupported()
    {
        var unsupportedFormats = _allFormats.Except(_supportedFormats).ToList();
        if (unsupportedFormats.Count == 0)
        {
            throw new SkipException("There are no unsupported formats.");
        }

        foreach (var getActual in unsupportedFormats.SelectMany(unsupportedFormat => GetImageUris(unsupportedFormat)))
        {
            Assert.False(getActual.IsSuccess);
            Assert.IsType<UnsupportedImageFormatError>(getActual.Error);
        }
    }
}
