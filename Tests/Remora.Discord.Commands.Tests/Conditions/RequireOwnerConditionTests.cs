//
//  RequireOwnerConditionTests.cs
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
using Remora.Results;
using Xunit;

namespace Remora.Discord.Commands.Tests.Conditions;

/// <summary>
/// Tests the <see cref="RequireOwnerCondition"/> class.
/// </summary>
public class RequireOwnerConditionTests
{
    /// <summary>
    /// Tests whether the condition returns an unsuccessful result if the command invoker is not the bot owner.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReturnsFalseIfInvokerIsNotBotOwner()
    {
        var contextMock = new Mock<ICommandContext>();
        contextMock.Setup(c => c.User.ID).Returns(DiscordSnowflake.New(0));

        var informationMock = new Mock<IApplication>();
        informationMock.Setup(i => i.Owner!.ID).Returns(DiscordSnowflake.New(1));

        var apiMock = new Mock<IDiscordRestOAuth2API>();
        apiMock
            .Setup(a => a.GetCurrentBotApplicationInformationAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IApplication>.FromSuccess(informationMock.Object));

        var attribute = new RequireOwnerAttribute();
        var condition = new RequireOwnerCondition(contextMock.Object, apiMock.Object);

        var result = await condition.CheckAsync(attribute);
        ResultAssert.Unsuccessful(result);
    }

    /// <summary>
    /// Tests whether the condition returns a successful result if the command invoker is the bot owner.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReturnsTrueIfInvokerIsBotOwner()
    {
        var contextMock = new Mock<ICommandContext>();
        contextMock.Setup(c => c.User.ID).Returns(DiscordSnowflake.New(0));

        var informationMock = new Mock<IApplication>();
        informationMock.Setup(i => i.Owner!.ID).Returns(DiscordSnowflake.New(0));

        var apiMock = new Mock<IDiscordRestOAuth2API>();
        apiMock
            .Setup(a => a.GetCurrentBotApplicationInformationAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IApplication>.FromSuccess(informationMock.Object));

        var attribute = new RequireOwnerAttribute();
        var condition = new RequireOwnerCondition(contextMock.Object, apiMock.Object);

        var result = await condition.CheckAsync(attribute);
        ResultAssert.Successful(result);
    }
}
