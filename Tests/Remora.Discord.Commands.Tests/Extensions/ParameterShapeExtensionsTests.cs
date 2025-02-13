//
//  ParameterShapeExtensionsTests.cs
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
using System.Reflection;
using Moq;
using Remora.Commands.Signatures;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;
using Xunit;

namespace Remora.Discord.Commands.Tests.Extensions;

/// <summary>
/// Tests the <see cref="ParameterShapeExtensions"/> class.
/// </summary>
public class ParameterShapeExtensionsTests
{
    /// <summary>
    /// Tests the <see cref="ParameterShapeExtensions.GetActualParameterType"/> method.
    /// </summary>
    public class GetActualParameterType
    {
        /// <summary>
        /// Tests whether the method performs as expected in multiple cases.
        /// </summary>
        /// <param name="parameterType">The parameter type.</param>
        /// <param name="expectedResult">The expected result.</param>
        [InlineData(typeof(Optional<int>), typeof(int))]
        [InlineData(typeof(int?), typeof(int))]
        [Theory]
        public void ReturnsCorrectly(Type parameterType, Type expectedResult)
        {
            var parameterShapeMock = new Mock<IParameterShape>();

            parameterShapeMock.SetupGet(p => p.ParameterType).Returns(parameterType);

            Assert.Equal(expectedResult, parameterShapeMock.Object.GetActualParameterType());
        }
    }

    /// <summary>
    /// Tests the <see cref="ParameterShapeExtensions.GetDiscordType"/> method.
    /// </summary>
    public class GetDiscordType
    {
        /// <summary>
        /// Tests whether the method performs as expected in multiple cases.
        /// </summary>
        /// <param name="parameterType">The parameter type.</param>
        /// <param name="expectedResult">The expected result.</param>
        [InlineData(typeof(bool), ApplicationCommandOptionType.Boolean)]
        [InlineData(typeof(byte), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(sbyte), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(ushort), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(short), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(uint), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(int), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(ulong), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(long), ApplicationCommandOptionType.Integer)]
        [InlineData(typeof(float), ApplicationCommandOptionType.Number)]
        [InlineData(typeof(double), ApplicationCommandOptionType.Number)]
        [InlineData(typeof(decimal), ApplicationCommandOptionType.Number)]
        [InlineData(typeof(string), ApplicationCommandOptionType.String)]
        [InlineData(typeof(IPartialRole), ApplicationCommandOptionType.Role)]
        [InlineData(typeof(IRole), ApplicationCommandOptionType.Role)]
        [InlineData(typeof(IPartialUser), ApplicationCommandOptionType.User)]
        [InlineData(typeof(IUser), ApplicationCommandOptionType.User)]
        [InlineData(typeof(IPartialGuildMember), ApplicationCommandOptionType.User)]
        [InlineData(typeof(IGuildMember), ApplicationCommandOptionType.User)]
        [InlineData(typeof(IPartialChannel), ApplicationCommandOptionType.Channel)]
        [InlineData(typeof(IChannel), ApplicationCommandOptionType.Channel)]
        [InlineData(typeof(IPartialAttachment), ApplicationCommandOptionType.Attachment)]
        [InlineData(typeof(IAttachment), ApplicationCommandOptionType.Attachment)]
        [Theory]
        public void ReturnsWithoutTypeHintAttributeCorrectly
        (
            Type parameterType,
            ApplicationCommandOptionType expectedResult
        )
        {
            var parameterShapeMock = new Mock<IParameterShape>();

            parameterShapeMock.SetupGet(p => p.Attributes).Returns(new List<Attribute>());
            parameterShapeMock.SetupGet(p => p.ParameterType).Returns(parameterType);

            Assert.Equal(expectedResult, parameterShapeMock.Object.GetDiscordType());
        }

        /// <summary>
        /// Tests whether the method performs as expected in multiple cases.
        /// </summary>
        /// <param name="parameterType">The parameter type.</param>
        [InlineData(typeof(bool))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(int))]
        [InlineData(typeof(long))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(string))]
        [InlineData(typeof(IPartialRole))]
        [InlineData(typeof(IPartialUser))]
        [InlineData(typeof(IPartialGuildMember))]
        [InlineData(typeof(IPartialChannel))]
        [InlineData(typeof(IPartialAttachment))]
        [Theory]
        public void ReturnsWithTypeHintAttributeCorrectly(Type parameterType)
        {
            var parameterShapeMock = new Mock<IParameterShape>();

            parameterShapeMock.SetupGet(p => p.Attributes)
                .Returns
                (
                    new List<Attribute>
                    {
                        new DiscordTypeHintAttribute(TypeHint.String)
                    }
                );

            parameterShapeMock.SetupGet(p => p.ParameterType).Returns(parameterType);

            Assert.Equal(ApplicationCommandOptionType.String, parameterShapeMock.Object.GetDiscordType());
        }
    }
}
