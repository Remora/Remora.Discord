//
//  RequireContextConditionTests.cs
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

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Results;
using Xunit;

namespace Remora.Discord.Commands.Tests.Conditions;

/// <summary>
/// Tests the <see cref="RequireContextCondition"/> class.
/// </summary>
public class RequireContextConditionTests
{
    /// <summary>
    /// Tests that the condition behaves correctly when constrained to a context.
    /// </summary>
    /// <param name="context">The channel context.</param>
    /// <param name="channelType">The channel type to test.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [InlineData(ChannelContext.Guild, ChannelType.GuildText, true)]
    [InlineData(ChannelContext.Guild, ChannelType.DM, false)]
    [InlineData(ChannelContext.Guild, ChannelType.GuildVoice, true)]
    [InlineData(ChannelContext.Guild, ChannelType.GroupDM, false)]
    [InlineData(ChannelContext.Guild, ChannelType.GuildCategory, true)]
    [InlineData(ChannelContext.Guild, ChannelType.GuildAnnouncement, true)]
    [InlineData(ChannelContext.Guild, ChannelType.AnnouncementThread, true)]
    [InlineData(ChannelContext.Guild, ChannelType.PublicThread, true)]
    [InlineData(ChannelContext.Guild, ChannelType.PrivateThread, true)]
    [InlineData(ChannelContext.Guild, ChannelType.GuildStageVoice, true)]
    [InlineData(ChannelContext.DM, ChannelType.DM, true)]
    [InlineData(ChannelContext.GroupDM, ChannelType.GroupDM, true)]
    public async Task ReturnsCorrectResultForContext
    (
        ChannelContext context,
        ChannelType channelType,
        bool expected
    )
    {
        var contextMock = new Mock<ICommandContext>();
        contextMock.Setup(c => c.ChannelID).Returns(DiscordSnowflake.New(0));

        var channelMock = new Mock<IChannel>();
        channelMock.Setup(c => c.Type).Returns(channelType);

        var channelAPIMock = new Mock<IDiscordRestChannelAPI>();
        channelAPIMock
            .Setup(c => c.GetChannelAsync(It.IsAny<Snowflake>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(channelMock.Object));

        var attribute = new RequireContextAttribute(context);
        var condition = new RequireContextCondition(contextMock.Object, channelAPIMock.Object);

        var result = await condition.CheckAsync(attribute, default);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests that the condition behaves correctly when constrained to a type.
    /// </summary>
    /// <param name="required">The required channel type.</param>
    /// <param name="actual">The actual channel type.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [InlineData(ChannelType.GuildText, ChannelType.GuildText, true)]
    [InlineData(ChannelType.GuildText, ChannelType.GuildVoice, false)]
    public async Task ReturnsCorrectResultForType
    (
        ChannelType required,
        ChannelType actual,
        bool expected
    )
    {
        var contextMock = new Mock<ICommandContext>();
        contextMock.Setup(c => c.ChannelID).Returns(DiscordSnowflake.New(0));

        var channelMock = new Mock<IChannel>();
        channelMock.Setup(c => c.Type).Returns(actual);

        var channelAPIMock = new Mock<IDiscordRestChannelAPI>();
        channelAPIMock
            .Setup(c => c.GetChannelAsync(It.IsAny<Snowflake>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(channelMock.Object));

        var attribute = new RequireContextAttribute(required);
        var condition = new RequireContextCondition(contextMock.Object, channelAPIMock.Object);

        var result = await condition.CheckAsync(attribute, default);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition can check multiple channel contexts.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanCheckMultipleContexts()
    {
        var textContextMock = new Mock<ICommandContext>();
        textContextMock.Setup(c => c.ChannelID).Returns(DiscordSnowflake.New(0));

        var groupDMContextMock = new Mock<ICommandContext>();
        groupDMContextMock.Setup(c => c.ChannelID).Returns(DiscordSnowflake.New(1));

        var textChannelMock = new Mock<IChannel>();
        textChannelMock.Setup(c => c.Type).Returns(ChannelType.GuildText);

        var groupDMMock = new Mock<IChannel>();
        groupDMMock.Setup(c => c.Type).Returns(ChannelType.GroupDM);

        var channelAPIMock = new Mock<IDiscordRestChannelAPI>();
        channelAPIMock
            .Setup(c => c.GetChannelAsync(It.Is<Snowflake>(s => s == DiscordSnowflake.New(0)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(textChannelMock.Object));

        channelAPIMock
            .Setup(c => c.GetChannelAsync(It.Is<Snowflake>(s => s == DiscordSnowflake.New(1)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(groupDMMock.Object));

        var attribute = new RequireContextAttribute(ChannelContext.Guild, ChannelContext.GroupDM);

        var groupDMCondition = new RequireContextCondition(groupDMContextMock.Object, channelAPIMock.Object);
        var groupDMResult = await groupDMCondition.CheckAsync(attribute, default);
        ResultAssert.Successful(groupDMResult);

        var textCondition = new RequireContextCondition(textContextMock.Object, channelAPIMock.Object);
        var textResult = await textCondition.CheckAsync(attribute, default);
        ResultAssert.Successful(textResult);
    }

    /// <summary>
    /// Tests whether the condition can check multiple channel types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanCheckMultipleTypes()
    {
        var textContextMock = new Mock<ICommandContext>();
        textContextMock.Setup(c => c.ChannelID).Returns(DiscordSnowflake.New(0));

        var groupDMContextMock = new Mock<ICommandContext>();
        groupDMContextMock.Setup(c => c.ChannelID).Returns(DiscordSnowflake.New(1));

        var textChannelMock = new Mock<IChannel>();
        textChannelMock.Setup(c => c.Type).Returns(ChannelType.GuildText);

        var groupDMMock = new Mock<IChannel>();
        groupDMMock.Setup(c => c.Type).Returns(ChannelType.GroupDM);

        var channelAPIMock = new Mock<IDiscordRestChannelAPI>();
        channelAPIMock
            .Setup(c => c.GetChannelAsync(It.Is<Snowflake>(s => s == DiscordSnowflake.New(0)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(textChannelMock.Object));

        channelAPIMock
            .Setup(c => c.GetChannelAsync(It.Is<Snowflake>(s => s == DiscordSnowflake.New(1)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(groupDMMock.Object));

        var attribute = new RequireContextAttribute(ChannelType.GuildText, ChannelType.GroupDM);

        var groupDMCondition = new RequireContextCondition(groupDMContextMock.Object, channelAPIMock.Object);
        var groupDMResult = await groupDMCondition.CheckAsync(attribute, default);
        ResultAssert.Successful(groupDMResult);

        var textCondition = new RequireContextCondition(textContextMock.Object, channelAPIMock.Object);
        var textResult = await textCondition.CheckAsync(attribute, default);
        ResultAssert.Successful(textResult);
    }
}
