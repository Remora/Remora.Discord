//
//  AttachmentParserTests.cs
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using OneOf;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Parsers;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Xunit;

namespace Remora.Discord.Commands.Tests.Parsers;

/// <summary>
/// Tests the <see cref="AttachmentParser"/> class.
/// </summary>
public class AttachmentParserTests
{
    private readonly Snowflake _attachmentID = Snowflake.CreateTimestampSnowflake();

    /// <summary>
    /// Tests that the <see cref="AttachmentParser"/> class cannot parse an attachment
    /// if the given context is not a <see cref="InteractionContext"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CannotParseWithoutInteractionContext()
    {
        var mockContext = new Mock<ICommandContext>();

        var parser = new AttachmentParser(mockContext.Object);

        var result = await parser.TryParseAsync(_attachmentID.ToString());

        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    /// Tests that the <see cref="AttachmentParser"/> class cannot parse an attachment
    /// if the given token is not a valid attachment ID.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CannotParseWithoutValidSnowflake()
    {
        var mockContext = new InteractionContext
            (
                default,
                default,
                Mock.Of<IUser>(),
                default,
                string.Empty,
                default,
                default,
                default,
                default,
                default
            );

        var parser = new AttachmentParser(mockContext);

        var result = await parser.TryParseAsync("invalid-snowflake");

        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    ///  Tests that the <see cref="AttachmentParser"/> class cannot parse an attachment
    /// if the given context does not contain resolved data.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CannotParseWithoutData()
    {
        var mockContext = new InteractionContext
        (
            default,
            default,
            Mock.Of<IUser>(),
            default,
            string.Empty,
            default,
            default,
            OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(Mock.Of<IApplicationCommandData>()),
            default,
            default
        );

        var parser = new AttachmentParser(mockContext);

        var result = await parser.TryParseAsync(_attachmentID.ToString());

        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    ///  Tests that the <see cref="AttachmentParser"/> class cannot parse an attachment
    /// if the given context does not contain resolved data.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CannotParseWithoutAttachment()
    {
        var mockData = new Mock<IApplicationCommandData>();
        var mockResolvedData = new Mock<IApplicationCommandInteractionDataResolved>();

        mockData.Setup(x => x.Resolved).Returns(() => new(mockResolvedData.Object));

        var mockContext = new InteractionContext
        (
            default,
            default,
            Mock.Of<IUser>(),
            default,
            string.Empty,
            default,
            default,
            OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(mockData.Object),
            default,
            default
        );

        var parser = new AttachmentParser(mockContext);
        var result = await parser.TryParseAsync(_attachmentID.ToString());

        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    ///  Tests that the <see cref="AttachmentParser"/> class cannot parse an attachment
    /// if the given context does not contain the specified attachment ID.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CannotParseWithoutProvidedAttachment()
    {
        var mockData = new Mock<IApplicationCommandData>();
        var mockResolvedData = new Mock<IApplicationCommandInteractionDataResolved>();

        mockData.Setup(x => x.Resolved).Returns(() => new(mockResolvedData.Object));

        mockResolvedData
            .Setup(x => x.Attachments)
            .Returns(() => new Dictionary<Snowflake, IAttachment>());

        var mockContext = new InteractionContext
        (
            default,
            default,
            Mock.Of<IUser>(),
            default,
            string.Empty,
            default,
            default,
            OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(mockData.Object),
            default,
            default
        );

        var parser = new AttachmentParser(mockContext);
        var result = await parser.TryParseAsync(_attachmentID.ToString());

        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    /// Tests that the <see cref="AttachmentParser"/> class can parse an attachment.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanParse()
    {
        var mockData = new Mock<IApplicationCommandData>();
        var mockAttachment = new Mock<IAttachment>();
        var mockResolvedData = new Mock<IApplicationCommandInteractionDataResolved>();

        mockData.Setup(x => x.Resolved).Returns(() => new(mockResolvedData.Object));

        mockAttachment.Setup(x => x.ID).Returns(_attachmentID);

        mockResolvedData
            .Setup(x => x.Attachments)
            .Returns(() => new Dictionary<Snowflake, IAttachment>
            {
                { _attachmentID, mockAttachment.Object }
            });

        var mockContext = new InteractionContext
        (
            default,
            default,
            Mock.Of<IUser>(),
            default,
            string.Empty,
            default,
            default,
            OneOf<IApplicationCommandData, IMessageComponentData, IModalSubmitData>.FromT0(mockData.Object),
            default,
            default
        );

        var parser = new AttachmentParser(mockContext);
        var result = await parser.TryParseAsync(_attachmentID.ToString());

        ResultAssert.Successful(result);
    }
}
