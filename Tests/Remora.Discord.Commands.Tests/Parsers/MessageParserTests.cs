//
//  MessageParserTests.cs
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
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Parsers;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Results;
using Xunit;

namespace Remora.Discord.Commands.Tests.Parsers;

/// <summary>
/// Tests the <see cref="MessageParser"/> class.
/// </summary>
public class MessageParserTests
{
    private readonly Mock<IDiscordRestChannelAPI> _channelAPIMock;
    private readonly MessageParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageParserTests"/> class.
    /// </summary>
    public MessageParserTests()
    {
        var messageMock = new Mock<IMessage>();
        messageMock.As<IPartialMessage>().Setup(m => m.ChannelID).Returns(DiscordSnowflake.New(0));

        var contextMock = new Mock<ITextCommandContext>();
        contextMock.Setup(c => c.Message).Returns(messageMock.Object);

        var channelAPIMock = new Mock<IDiscordRestChannelAPI>();
        channelAPIMock
            .Setup(api =>
                api.GetChannelMessageAsync
                (
                    It.IsAny<Snowflake>(),
                    It.IsAny<Snowflake>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(Result<IMessage>.FromSuccess(messageMock.Object)));

        _channelAPIMock = channelAPIMock;

        _parser = new MessageParser(contextMock.Object, channelAPIMock.Object);
    }

    /// <summary>
    /// Tests whether the parser returns error on invalid value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CannotParseInvalidValue()
    {
        var tryParse = await _parser.TryParseAsync("invalid");
        ResultAssert.Unsuccessful(tryParse);
    }

    /// <summary>
    /// Tests whether the parser can parse message given by snowflake.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanParseMessageByNumber()
    {
        var messageID = DiscordSnowflake.New(6823586735728);

        var tryParse = await _parser.TryParseAsync(messageID.ToString());
        ResultAssert.Successful(tryParse);

        _channelAPIMock.Verify(c => c
            .GetChannelMessageAsync
            (
                It.IsAny<Snowflake>(),
                It.Is<Snowflake>(s => s == messageID),
                It.IsAny<CancellationToken>()
            )
        );
    }

    /// <summary>
    /// Tests whether the parser can parse message given by message link.
    /// </summary>
    /// <param name="value">Message link that should be parsed correctly.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [InlineData("https://discord.com/channels/@me/135347310845612345/135347310845624320/")]
    [InlineData("https://discord.com/channels/123/135347310845612345/135347310845624320/")]
    [InlineData("https://canary.discord.com/channels/@me/135347310845612345/135347310845624320/")]
    [InlineData("https://canary.discord.com/channels/123/135347310845612345/135347310845624320/")]
    [InlineData("https://ptb.discord.com/channels/@me/135347310845612345/135347310845624320/")]
    [InlineData("https://ptb.discord.com/channels/123/135347310845612345/135347310845624320/")]
    [Theory]
    public async Task CanParseMessageByMessageLink(string value)
    {
        var channelID = DiscordSnowflake.New(135347310845612345);
        var messageID = DiscordSnowflake.New(135347310845624320);

        var tryParse = await _parser.TryParseAsync(value);
        ResultAssert.Successful(tryParse);

        _channelAPIMock.Verify(c => c
            .GetChannelMessageAsync
            (
                It.Is<Snowflake>(s => s == channelID),
                It.Is<Snowflake>(s => s == messageID),
                It.IsAny<CancellationToken>()
            )
        );
    }
}
