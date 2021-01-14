//
//  SnowflakeTests.cs
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
using Remora.Discord.Core;
using Xunit;

// ReSharper disable SA1600
#pragma warning disable 1591, SA1600

namespace Remora.Discord.Tests.Tests.Core
{
    /// <summary>
    /// Tests the <see cref="Snowflake"/> struct.
    /// </summary>
    public class SnowflakeTests
    {
        /// <summary>
        /// Tests the <see cref="Snowflake.TryParse"/> method.
        /// </summary>
        public class TryParse
        {
            [Fact]
            public void ReturnsTrueForValidDiscordSnowflake()
            {
                var value = "143867839282020352";
                Assert.True(Snowflake.TryParse(value, out _));
            }

            [Fact]
            public void ReturnsFalseForStringWithLetters()
            {
                var value = "i4e867839282020e52";
                Assert.False(Snowflake.TryParse(value, out _));
            }

            [Fact]
            public void ReturnsFalseForEmptyString()
            {
                var value = string.Empty;
                Assert.False(Snowflake.TryParse(value, out _));
            }

            [Fact]
            public void ReturnsFalseForSnowflakeBeforeDiscordEpoch()
            {
                var value = "-1";
                Assert.False(Snowflake.TryParse(value, out _));
            }
        }

        /// <summary>
        /// Tests the <see cref="Snowflake.Value"/> property.
        /// </summary>
        public class Value
        {
            [Fact]
            public void ReturnsCorrectValue()
            {
                var snowflake = new Snowflake(143867839282020352u);

                Assert.Equal(143867839282020352u, snowflake.Value);
            }
        }

        /// <summary>
        /// Tests the <see cref="Snowflake.Timestamp"/> property.
        /// </summary>
        public class Timestamp
        {
            [Fact]
            public void ReturnsCorrectValue()
            {
                var time = DateTimeOffset.Parse("2016-02-01T23:59:25.820Z");
                var snowflake = new Snowflake(143867839282020352u);

                Assert.Equal(time, snowflake.Timestamp);
            }
        }

        /// <summary>
        /// Tests the <see cref="Snowflake.InternalWorkerID"/> property.
        /// </summary>
        public class InternalWorkerID
        {
            [Fact]
            public void ReturnsCorrectValue()
            {
                var workerID = 1;
                var snowflake = new Snowflake(143867839282020352u);

                Assert.Equal(workerID, snowflake.InternalWorkerID);
            }
        }

        /// <summary>
        /// Tests the <see cref="Snowflake.InternalProcessID"/> property.
        /// </summary>
        public class InternalProcessID
        {
            [Fact]
            public void ReturnsCorrectValue()
            {
                var processID = 0;
                var snowflake = new Snowflake(143867839282020352u);

                Assert.Equal(processID, snowflake.InternalProcessID);
            }
        }

        /// <summary>
        /// Tests the <see cref="Snowflake.Increment"/> property.
        /// </summary>
        public class Increment
        {
            [Fact]
            public void ReturnsCorrectValue()
            {
                var increment = 0;
                var snowflake = new Snowflake(143867839282020352u);

                Assert.Equal(increment, snowflake.Increment);
            }
        }

        /// <summary>
        /// Tests the <see cref="Snowflake.CreateTimestampSnowflake"/> method.
        /// </summary>
        public class CreateTimestampSnowflake
        {
            [Fact]
            public void CreatesSnowflakeWithUtcNowTime()
            {
                var now = DateTimeOffset.UtcNow;
                var snowflake = Snowflake.CreateTimestampSnowflake();

                // Some allowance is made here because of clock inaccuracies and the potential for scheduler differences
                // between the two above time measurements.
                var isWithinFiveSeconds = Math.Abs
                (
                    now.ToUnixTimeSeconds() - snowflake.Timestamp.ToUnixTimeSeconds()
                ) <= 5;

                Assert.True(isWithinFiveSeconds);
            }

            [Fact]
            public void CanCreateFromTimestamp()
            {
                var yearOfHell = new DateTimeOffset(2020, 1, 1, 6, 0, 0, TimeSpan.Zero);
                var snowflake = Snowflake.CreateTimestampSnowflake(yearOfHell);

                Assert.Equal(yearOfHell.ToUnixTimeMilliseconds(), snowflake.Timestamp.ToUnixTimeMilliseconds());
            }
        }

        /// <summary>
        /// Tests the <see cref="object.ToString"/> method.
        /// </summary>
        public new class ToString
        {
            [Fact]
            public void PrintsValue()
            {
                var snowflake = new Snowflake(143867839282020352u);

                Assert.Equal("143867839282020352", snowflake.ToString());
            }
        }

        /// <summary>
        /// Tests the <see cref="object.Equals(object?)"/> method.
        /// </summary>
        public new class Equals
        {
            [Fact]
            public void ReturnsFalseForNonSnowflakeObject()
            {
                var notSnowflake = "henlo";
                var snowflake = new Snowflake(143867839282020352u);

                // ReSharper disable once SuspiciousTypeConversion.Global
                Assert.False(snowflake.Equals(notSnowflake));
            }

            [Fact]
            public void ReturnsFalseForSnowflakeWithDifferentValue()
            {
                var snowflake = new Snowflake(143867839282020352u);
                var otherSnowflake = new Snowflake(169780104564572169u);

                Assert.False(snowflake.Equals(otherSnowflake));
            }

            [Fact]
            public void ReturnsTrueForSelf()
            {
                var snowflake = new Snowflake(143867839282020352u);

                Assert.True(snowflake.Equals(snowflake));
            }

            [Fact]
            public void ReturnsTrueForSnowflakeWithSameValue()
            {
                var snowflake = new Snowflake(143867839282020352u);
                var otherSnowflake = new Snowflake(143867839282020352u);

                Assert.True(snowflake.Equals(otherSnowflake));
            }
        }

        /// <summary>
        /// Tests the <see cref="object.GetHashCode"/> method.
        /// </summary>
        public new class GetHashCode
        {
            [Fact]
            public void HasSameHashAsValue()
            {
                var value = 143867839282020352u;
                var snowflake = new Snowflake(143867839282020352u);

                Assert.Equal(value.GetHashCode(), snowflake.GetHashCode());
            }
        }

        /// <summary>
        /// Tests the CompareTo method.
        /// </summary>
        public class CompareTo
        {
            [Theory]
            [InlineData(0, 0)]
            [InlineData(0, 1)]
            [InlineData(1, 0)]
            public void ComparesSameAsValue(ulong first, ulong second)
            {
                var firstSnowflake = new Snowflake(first);
                var secondSnowflake = new Snowflake(second);

                Assert.Equal(first.CompareTo(second), firstSnowflake.CompareTo(secondSnowflake));
            }
        }

        /// <summary>
        /// Tests the == operator.
        /// </summary>
        public class EqualOperator
        {
            [Theory]
            [InlineData(ulong.MinValue, ulong.MinValue)]
            [InlineData(ulong.MaxValue, ulong.MaxValue)]
            [InlineData(143867839282020352U, 143867839282020352U)]
            [InlineData(143867839282020352U, 169780104564572169U)]
            [InlineData(169780104564572169U, 143867839282020352U)]
            [InlineData(169780104564572169U, 169780104564572169U)]
            public void EquatesSameAsValue(ulong first, ulong second)
            {
                var firstSnowflake = new Snowflake(first);
                var secondSnowflake = new Snowflake(second);

                Assert.Equal(first == second, firstSnowflake == secondSnowflake);
            }
        }

        /// <summary>
        /// Tests the != operator.
        /// </summary>
        public class NotEqualOperator
        {
            [Theory]
            [InlineData(ulong.MinValue, ulong.MinValue)]
            [InlineData(ulong.MaxValue, ulong.MaxValue)]
            [InlineData(143867839282020352U, 143867839282020352U)]
            [InlineData(143867839282020352U, 169780104564572169U)]
            [InlineData(169780104564572169U, 143867839282020352U)]
            [InlineData(169780104564572169U, 169780104564572169U)]
            public void EquatesSameAsValue(ulong first, ulong second)
            {
                var firstSnowflake = new Snowflake(first);
                var secondSnowflake = new Snowflake(second);

                Assert.Equal(first != second, firstSnowflake != secondSnowflake);
            }
        }

        /// <summary>
        /// Tests the &lt; operator.
        /// </summary>
        public class LessThan
        {
            [Theory]
            [InlineData(0, 0)]
            [InlineData(0, 1)]
            [InlineData(1, 0)]
            public void ComparesSameAsValue(ulong first, ulong second)
            {
                var firstSnowflake = new Snowflake(first);
                var secondSnowflake = new Snowflake(second);

                Assert.Equal(first < second, firstSnowflake < secondSnowflake);
            }
        }

        /// <summary>
        /// Tests the &gt; operator.
        /// </summary>
        public class GreaterThan
        {
            [Theory]
            [InlineData(0, 0)]
            [InlineData(0, 1)]
            [InlineData(1, 0)]
            public void ComparesSameAsValue(ulong first, ulong second)
            {
                var firstSnowflake = new Snowflake(first);
                var secondSnowflake = new Snowflake(second);

                Assert.Equal(first > second, firstSnowflake > secondSnowflake);
            }
        }

        /// <summary>
        /// Tests the &lt;= operator.
        /// </summary>
        public class LessThanOrEqual
        {
            [Theory]
            [InlineData(0, 0)]
            [InlineData(0, 1)]
            [InlineData(1, 0)]
            public void ComparesSameAsValue(ulong first, ulong second)
            {
                var firstSnowflake = new Snowflake(first);
                var secondSnowflake = new Snowflake(second);

                Assert.Equal(first <= second, firstSnowflake <= secondSnowflake);
            }
        }

        /// <summary>
        /// Tests the &lt;= operator.
        /// </summary>
        public class GreaterThanOrEqual
        {
            [Theory]
            [InlineData(0, 0)]
            [InlineData(0, 1)]
            [InlineData(1, 0)]
            public void ComparesSameAsValue(ulong first, ulong second)
            {
                var firstSnowflake = new Snowflake(first);
                var secondSnowflake = new Snowflake(second);

                Assert.Equal(first >= second, firstSnowflake >= secondSnowflake);
            }
        }
    }
}
