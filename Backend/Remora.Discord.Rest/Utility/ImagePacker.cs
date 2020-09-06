//
//  ImagePacker.cs
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Rest.Extensions;
using Remora.Discord.Rest.Results;
using Remora.Results;

namespace Remora.Discord.Rest.Utility
{
    /// <summary>
    /// Packs images into a base64 representation.
    /// </summary>
    public class ImagePacker
    {
        /// <summary>
        /// Packs the given stream into a base64-encoded string, type-prefixed string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A creation result which may or may not have succeeded.</returns>
        public static async Task<CreateEntityResult<string>> PackImageAsync
        (
            Stream stream,
            CancellationToken ct = default
        )
        {
            await using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream, ct);

            var imageData = memoryStream.ToArray();

            string? mediaType = null;
            if (imageData.IsPNG())
            {
                mediaType = "png";
            }
            else if (imageData.IsJPG())
            {
                mediaType = "jpeg";
            }
            else if (imageData.IsGIF())
            {
                mediaType = "gif";
            }

            if (mediaType is null)
            {
                return CreateEntityResult<string>.FromError("Unknown or unsupported image format.");
            }

            return $"data:image/{mediaType};base64,{Convert.ToBase64String(imageData)}";
        }
    }
}
