//
//  MentionTests.cs
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

using Moq;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Extensions.Formatting;
using Remora.Rest.Core;
using Xunit;

namespace Remora.Discord.Extensions.Tests;

/// <summary>
/// Tests to ensure the <see cref="Mention"/> formats mentions correctly to Discord standards.
/// </summary>
public class MentionTests
{
    /// <summary>
    /// Tests to see if the <see cref="Mention.User(IUser)"/> method formats a user mention based on Discord's specifications.
    /// </summary>
    /// <param name="userID">The user ID.</param>
    [Theory]
    [InlineData(330746772378877954UL)]
    [InlineData(197291773133979648UL)]
    [InlineData(135347310845624320UL)]
    [InlineData(201799098137968640UL)]
    [InlineData(209279906280898562UL)]
    [InlineData(203709726322720768UL)]
    public void UserSuccess(ulong userID)
    {
        var user = new Mock<IUser>();

        user
            .SetupGet(x => x.ID)
            .Returns(new Snowflake(userID));

        var expected = $"<@{userID}>";
        var actual = Mention.User(user.Object);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Mention.User(Snowflake)"/> method formats a channel mention based on Discord's specifications.
    /// </summary>
    /// <param name="userID">The user ID.</param>
    [Theory]
    [InlineData(330746772378877954UL)]
    [InlineData(197291773133979648UL)]
    [InlineData(135347310845624320UL)]
    [InlineData(201799098137968640UL)]
    [InlineData(209279906280898562UL)]
    [InlineData(203709726322720768UL)]
    public void UserWithIDSuccess(ulong userID)
    {
        var snowflake = new Snowflake(userID);
        var expected = $"<@{userID}>";
        var actual = Mention.User(snowflake);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Mention.Channel(IChannel)"/> method formats a channel mention based on Discord's specifications.
    /// </summary>
    /// <param name="channelID">The channel ID.</param>
    [Theory]
    [InlineData(789912892899459104UL)]
    [InlineData(789922911116984320UL)]
    [InlineData(789913136273948692UL)]
    [InlineData(789913038920744960UL)]
    [InlineData(789913059116187658UL)]
    [InlineData(858350463259115550UL)]
    public void ChannelSuccess(ulong channelID)
    {
        var channel = new Mock<IChannel>();

        channel
            .SetupGet(x => x.ID)
            .Returns(new Snowflake(channelID));

        var expected = $"<#{channelID}>";
        var actual = Mention.Channel(channel.Object);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Mention.Channel(Snowflake)"/> method formats a role mention based on Discord's specifications.
    /// </summary>
    /// <param name="channelID">The channel ID.</param>
    [Theory]
    [InlineData(789912892899459104UL)]
    [InlineData(789922911116984320UL)]
    [InlineData(789913136273948692UL)]
    [InlineData(789913038920744960UL)]
    [InlineData(789913059116187658UL)]
    [InlineData(858350463259115550UL)]
    public void ChannelWithIDSuccess(ulong channelID)
    {
        var snowflake = new Snowflake(channelID);
        var expected = $"<#{channelID}>";
        var actual = Mention.Channel(snowflake);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Mention.Role(IRole)"/> method formats a role mention based on Discord's specifications.
    /// </summary>
    /// <param name="roleID">The channel ID.</param>
    [Theory]
    [InlineData(871102387362869288UL)]
    [InlineData(301125242749714442UL)]
    [InlineData(302310311074070531UL)]
    [InlineData(460285613040730112UL)]
    [InlineData(402648568197939201UL)]
    [InlineData(372178027926519810UL)]
    public void RoleSuccess(ulong roleID)
    {
        var role = new Mock<IRole>();

        role
            .SetupGet(x => x.ID)
            .Returns(new Snowflake(roleID));

        var expected = $"<@&{roleID}>";
        var actual = Mention.Role(role.Object);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Mention.Role(Snowflake)"/> method formats a rike mention based on Discord's specifications.
    /// </summary>
    /// <param name="roleID">The role ID.</param>
    [Theory]
    [InlineData(789912892899459104UL)]
    [InlineData(789922911116984320UL)]
    [InlineData(789913136273948692UL)]
    [InlineData(789913038920744960UL)]
    [InlineData(789913059116187658UL)]
    [InlineData(858350463259115550UL)]
    public void RoleWithIDSuccess(ulong roleID)
    {
        var snowflake = new Snowflake(roleID);
        var expected = $"<@&{roleID}>";
        var actual = Mention.Role(snowflake);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Mention.SlashCommand(IApplicationCommand)"/> method formats a role mention based
    /// on Discord's specifications.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="commandID">The ID of the command.</param>
    [Theory]
    [InlineData("wooga", 871102387362869288UL)]
    public void SlashCommandSuccess(string name, ulong commandID)
    {
        var command = new Mock<IApplicationCommand>();

        command
            .SetupGet(x => x.ID)
            .Returns(new Snowflake(commandID));

        command
            .SetupGet(x => x.Name)
            .Returns(name);

        var expected = $"</{name}:{commandID}>";
        var actual = Mention.SlashCommand(command.Object);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Mention.SlashCommand(IApplicationCommand)"/> method formats a role mention based
    /// on Discord's specifications.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="commandID">The ID of the command.</param>
    [Theory]
    [InlineData("wooga", 871102387362869288UL)]
    public void SlashCommandWithNameAndIDSuccess(string name, ulong commandID)
    {
        var snowflake = new Snowflake(commandID);
        var expected = $"</{name}:{commandID}>";
        var actual = Mention.SlashCommand(name, snowflake);

        Assert.Equal(expected, actual);
    }
}
