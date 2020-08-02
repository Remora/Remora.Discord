//
//  TypeExtensionTests.cs
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
using System.Reflection;
using Remora.Discord.API.Extensions;
using Remora.Discord.Core;
using Xunit;

// ReSharper disable SA1600
#pragma warning disable 1591, SA1600

namespace Remora.Discord.Tests.Tests.API.Extensions
{
    /// <summary>
    /// Tests the <see cref="Discord.API.Extensions.TypeExtensions"/> class.
    /// </summary>
    public class TypeExtensionTests
    {
        public class IsOptional
        {
            [Fact]
            public void ReturnsFalseForNonOptionalType()
            {
                Assert.False(typeof(int).IsOptional());
            }

            [Fact]
            public void ReturnsFalseForNonOptionalGenericType()
            {
                Assert.False(typeof(List<int>).IsOptional());
            }

            [Fact]
            public void ReturnsTrueForOptionalType()
            {
                Assert.True(typeof(Optional<int>).IsOptional());
            }
        }

        public class IsNullable
        {
            [Fact]
            public void ReturnsFalseForNonNullableType()
            {
                Assert.False(typeof(int).IsNullable());
            }

            [Fact]
            public void ReturnsFalseForNonNullableGenericType()
            {
                Assert.False(typeof(List<int>).IsNullable());
            }

            [Fact]
            public void ReturnsTrueForNullableType()
            {
                Assert.True(typeof(int?).IsNullable());
            }
        }

        public class Unwrap
        {
            [Fact]
            public void UnwrapsOptional()
            {
                Assert.Equal(typeof(int), typeof(Optional<int>).Unwrap());
            }

            [Fact]
            public void UnwrapsOptionalNullable()
            {
                Assert.Equal(typeof(int), typeof(Optional<int?>).Unwrap());
            }

            [Fact]
            public void UnwrapsNullableOptional()
            {
                Assert.Equal(typeof(int), typeof(Optional<int>?).Unwrap());
            }

            [Fact]
            public void DoesNotChangeOtherGenericTypes()
            {
                Assert.Equal(typeof(List<int>), typeof(Optional<List<int>>).Unwrap());
            }

            [Fact]
            public void DoesNotChangeNonOptionalOrNonNullableType()
            {
                Assert.Equal(typeof(int), typeof(int).Unwrap());
            }
        }
    }
}
