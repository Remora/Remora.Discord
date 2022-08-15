//
//  RequireBotPermissionsConditionTests.cs
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
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Tests;
using Remora.Rest.Core;
using Remora.Results;
using Xunit;

namespace Remora.Discord.Commands.Tests.Conditions;

/// <summary>
/// Tests the <see cref="Remora.Discord.Commands.Conditions.RequireBotDiscordPermissionsCondition"/> class.
/// </summary>
public partial class RequireBotPermissionsConditionTests
{
    private readonly Mock<IDiscordRestUserAPI> _userAPIMock;
    private readonly Mock<IDiscordRestGuildAPI> _guildAPIMock;
    private readonly Mock<IDiscordRestChannelAPI> _channelAPIMock;
    private readonly Mock<ICommandContext> _contextMock;
    private readonly Mock<IGuildMember> _memberMock;
    private readonly Mock<IRole> _everyoneRoleMock;
    private readonly Mock<IChannel> _channelMock;

    private readonly Snowflake _userID;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireBotPermissionsConditionTests"/> class.
    /// </summary>
    public RequireBotPermissionsConditionTests()
    {
        var guildID = DiscordSnowflake.New(0);
        _userID = DiscordSnowflake.New(1);
        var channelID = DiscordSnowflake.New(2);

        // Dependency mocks
        _userAPIMock = new Mock<IDiscordRestUserAPI>();
        _guildAPIMock = new Mock<IDiscordRestGuildAPI>();
        _channelAPIMock = new Mock<IDiscordRestChannelAPI>();
        _contextMock = new Mock<ICommandContext>();

        // Result mocks
        var guildMock = new Mock<IGuild>();
        _memberMock = new Mock<IGuildMember>();
        _channelMock = new Mock<IChannel>();
        _everyoneRoleMock = new Mock<IRole>();

        var userMock = new Mock<IUser>();

        // Setup
        _contextMock.Setup(c => c.User.ID).Returns(_userID);
        _contextMock.Setup(c => c.GuildID).Returns(guildID);
        _contextMock.Setup(c => c.ChannelID).Returns(channelID);

        guildMock.Setup(g => g.ID).Returns(guildID);
        guildMock.Setup(g => g.OwnerID).Returns(DiscordSnowflake.New(3));

        _channelMock.Setup(c => c.ID).Returns(channelID);
        _channelMock
            .Setup(c => c.PermissionOverwrites)
            .Returns(default(Optional<IReadOnlyList<IPermissionOverwrite>>));

        _everyoneRoleMock.Setup(r => r.ID).Returns(guildID);

        userMock.Setup(u => u.ID).Returns(_userID);

        _memberMock.Setup(m => m.User).Returns(new Optional<IUser>(userMock.Object));
        _memberMock.Setup(m => m.Roles).Returns(Array.Empty<Snowflake>());

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildMemberAsync
                (
                    It.Is<Snowflake>(s => s == guildID),
                    It.Is<Snowflake>(s => s == _userID),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuildMember>.FromSuccess(_memberMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildAsync
                (
                    It.Is<Snowflake>(s => s == guildID),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuild>.FromSuccess(guildMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == guildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object });

        _channelAPIMock
            .Setup(a => a.GetChannelAsync(It.Is<Snowflake>(s => s == channelID), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(_channelMock.Object));

        _userAPIMock
            .Setup(u => u.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IUser>.FromSuccess(userMock.Object));
    }

    /// <summary>
    /// Tests whether the condition respects the owner of a guild having all permissions.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task RespectsGuildOwner<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _ = expected;
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));

        var botOwnedGuildID = DiscordSnowflake.New(4);

        var botOwnedGuildMock = new Mock<IGuild>();
        botOwnedGuildMock.Setup(g => g.ID).Returns(botOwnedGuildID);
        botOwnedGuildMock.Setup(g => g.OwnerID).Returns(_userID);

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildMemberAsync
                (
                    It.Is<Snowflake>(s => s == botOwnedGuildID),
                    It.Is<Snowflake>(s => s == _userID),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuildMember>.FromSuccess(_memberMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildAsync
                (
                    It.Is<Snowflake>(s => s == botOwnedGuildID),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuild>.FromSuccess(botOwnedGuildMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == botOwnedGuildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object });

        _everyoneRoleMock.Setup(r => r.ID).Returns(botOwnedGuildID);
        _contextMock.Setup(c => c.GuildID).Returns(botOwnedGuildID);

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireBotDiscordPermissionsAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireBotDiscordPermissionsCondition
        (
            _userAPIMock.Object,
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether the condition respects the administrator permission.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task RespectsAdministratorPermission<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _ = expected;

        effectivePermissions = effectivePermissions.Concat(new[] { DiscordPermission.Administrator }).ToArray();

        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireBotDiscordPermissionsAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireBotDiscordPermissionsCondition
        (
            _userAPIMock.Object,
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether the condition acknowledges and correctly evaluates role permissions.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task ReturnsCorrectResultForRolePermissions<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireBotDiscordPermissionsAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireBotDiscordPermissionsCondition
            (
                _userAPIMock.Object,
                _guildAPIMock.Object,
                _channelAPIMock.Object,
                _contextMock.Object
            );

        var result = await condition.CheckAsync(attribute);

        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition acknowledges and correctly evaluates channel overwrites.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task ReturnsCorrectResultForChannelRoleOverwrite<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(BigInteger.Zero));

        _channelMock
            .Setup(c => c.PermissionOverwrites)
            .Returns(new[]
            {
                new PermissionOverwrite
                (
                    _everyoneRoleMock.Object.ID,
                    PermissionOverwriteType.Role,
                    new DiscordPermissionSet(effectivePermissions),
                    new DiscordPermissionSet(BigInteger.Zero)
                )
            });

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireBotDiscordPermissionsAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireBotDiscordPermissionsCondition
        (
            _userAPIMock.Object,
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute);

        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition acknowledges and correctly evaluates user overwrites.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task ReturnsCorrectResultForChannelMemberOverwrite<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(BigInteger.Zero));

        _channelMock
            .Setup(c => c.PermissionOverwrites)
            .Returns(new[]
            {
                new PermissionOverwrite
                (
                    _userID,
                    PermissionOverwriteType.Member,
                    new DiscordPermissionSet(effectivePermissions),
                    new DiscordPermissionSet(BigInteger.Zero)
                )
            });

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireBotDiscordPermissionsAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireBotDiscordPermissionsCondition
        (
            _userAPIMock.Object,
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute);

        Assert.Equal(expected, result.IsSuccess);
    }
}
