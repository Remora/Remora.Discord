//
//  ImageHashTests.cs
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

using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.API.Tests.TestBases;
using Xunit;

#pragma warning disable CS1591, SA1600

namespace Remora.Discord.API.Tests.Objects;

/// <inheritdoc />
public class ImageHashTests : ObjectTestBase<IImageHash>
{
    /// <summary>
    /// Tests the <see cref="IImageHash.HasGif"/> method.
    /// </summary>
    public class HasGif
    {
        [Fact]
        public void ReturnsTrueForAnimatedHash()
        {
            var hash = new ImageHash("a_1269e74af4df7417b13759eae50c83dc");
            Assert.True(hash.HasGif);
        }

        [Fact]
        public void ReturnsFalseForStaticHash()
        {
            var hash = new ImageHash("1269e74af4df7417b13759eae50c83dc");
            Assert.False(hash.HasGif);
        }
    }
}
