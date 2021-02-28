//
//  CDNTests.cs
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
using System.Collections.Generic;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.API.Tests.TestBases;
using Remora.Discord.Core;
using Remora.Results;
using Xunit;

namespace Remora.Discord.API.Tests
{
    /// <summary>
    /// Tests the <see cref="CDN"/> class.
    /// </summary>
    public class CDNTests
    {
        /// <summary>
        /// Tests the <see cref="CDN.GetEmojiUrl(IEmoji, Optional{CDNImageFormat}, Optional{ushort})"/> method and its
        /// overloads.
        /// </summary>
        public class GetEmojiUri : CDNTestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GetEmojiUri"/> class.
            /// </summary>
            public GetEmojiUri()
                : base
                (
                    new Uri("https://cdn.discordapp.com/emojis/0"),
                    new[] { CDNImageFormat.PNG, CDNImageFormat.GIF }
                )
            {
            }

            /// <summary>
            /// Tests whether the correct address is returned when an animated emoji is provided.
            /// </summary>
            [Fact]
            public void ReturnsCorrectAddressWithAnimatedEmoji()
            {
                var expected = new Uri
                (
                    this.ValidUriWithoutExtension + ".gif"
                );

                var emoji = new Emoji(new Snowflake(0), null, IsAnimated: true);
                var getActual = CDN.GetEmojiUrl(emoji);

                Assert.True(getActual.IsSuccess);
                Assert.Equal(expected, getActual.Entity);
            }

            /// <summary>
            /// Tests whether the correct address is returned when an animated emoji is provided, but a custom format is
            /// explicitly requested.
            /// </summary>
            [Fact]
            public void ReturnsCorrectAddressWithAnimatedEmojiAndCustomFormat()
            {
                var expected = new Uri
                (
                    this.ValidUriWithoutExtension + ".png"
                );

                var emoji = new Emoji(new Snowflake(0), null, IsAnimated: true);
                var getActual = CDN.GetEmojiUrl(emoji, CDNImageFormat.PNG);

                Assert.True(getActual.IsSuccess);
                Assert.Equal(expected, getActual.Entity);
            }

            /// <summary>
            /// Tests whether an unsuccessful result is returned if the emoji isn't a custom emoji.
            /// </summary>
            [Fact]
            public void ReturnsUnsuccessfulResultIfEmojiIsNotCustomEmoji()
            {
                var emoji = new Emoji(null, "booga");
                var getActual = CDN.GetEmojiUrl(emoji);

                Assert.False(getActual.IsSuccess);
            }

            /// <inheritdoc />
            protected override IEnumerable<Result<Uri>> GetImageUris
            (
                Optional<CDNImageFormat> imageFormat = default,
                Optional<ushort> imageSize = default
            )
            {
                var emoji = new Emoji(new Snowflake(0), null);
                yield return CDN.GetEmojiUrl(emoji, imageFormat, imageSize);
                yield return CDN.GetEmojiUrl(emoji.ID!.Value, imageFormat, imageSize);
            }
        }
    }
}
