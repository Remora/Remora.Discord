//
//  ImagePacker.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Results;

namespace Remora.Discord.Rest.Utility;

/// <summary>
/// Packs images into a base64 representation.
/// </summary>
[PublicAPI]
public static class ImagePacker
{
    /// <summary>
    /// Packs the given stream into a base64-encoded string, type-prefixed string.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    public static async Task<Result<Optional<string?>>> PackImageAsync
    (
        Optional<Stream?> stream,
        CancellationToken ct = default
    )
    {
        Optional<string?> imageData = default;
        if (!stream.HasValue)
        {
            return imageData;
        }

        if (stream.Value is null)
        {
            imageData = new Optional<string?>(null);
        }
        else
        {
            var packImage = await PackImageAsync(stream.Value, ct);
            if (!packImage.IsSuccess)
            {
                return Result<Optional<string?>>.FromError(packImage.Error);
            }

            imageData = packImage.Entity;
        }

        return imageData;
    }

    /// <summary>
    /// Packs the given stream into a base64-encoded string, type-prefixed string.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    public static async Task<Result<string>> PackImageAsync
    (
        Stream stream,
        CancellationToken ct = default
    )
    {
        byte[] bytes;
        if (stream is MemoryStream ms)
        {
            bytes = ms.ToArray();
        }
        else
        {
            await using var copy = new MemoryStream();
            await stream.CopyToAsync(copy, ct);

            bytes = copy.ToArray();
        }

        string? mediaType = null;
        if (bytes.IsPNG())
        {
            mediaType = "png";
        }
        else if (bytes.IsJPG())
        {
            mediaType = "jpeg";
        }
        else if (bytes.IsGIF())
        {
            mediaType = "gif";
        }

        return mediaType is null
            ? new NotSupportedError("Unknown or unsupported image format.")
            : $"data:image/{mediaType};base64,{Convert.ToBase64String(bytes)}";
    }
}
