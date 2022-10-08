//
//  RequireDiscordPermissionConditionTests.cs
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
/// Tests the <see cref="RequireDiscordPermissionCondition"/> class.
/// </summary>
public partial class RequireDiscordPermissionConditionTests
{
    private readonly Mock<IDiscordRestGuildAPI> _guildAPIMock;
    private readonly Mock<IDiscordRestChannelAPI> _channelAPIMock;
    private readonly Mock<ICommandContext> _contextMock;
    private readonly Mock<IRole> _everyoneRoleMock;
    private readonly Mock<IUser> _userMock;
    private readonly Mock<IGuildMember> _memberMock;
    private readonly Snowflake _guildID;
    private readonly Mock<IChannel> _channelMock;
    private readonly Snowflake _userID;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireDiscordPermissionConditionTests"/> class.
    /// </summary>
    public RequireDiscordPermissionConditionTests()
    {
        _guildID = DiscordSnowflake.New(0);
        _userID = DiscordSnowflake.New(1);

        var channelID = DiscordSnowflake.New(2);

        // Dependency mocks
        _guildAPIMock = new Mock<IDiscordRestGuildAPI>();
        _channelAPIMock = new Mock<IDiscordRestChannelAPI>();
        _contextMock = new Mock<ICommandContext>();

        // Result mocks
        var guildMock = new Mock<IGuild>();
        _everyoneRoleMock = new Mock<IRole>();
        _userMock = new Mock<IUser>();
        _memberMock = new Mock<IGuildMember>();
        _channelMock = new Mock<IChannel>();

        // Setup
        _contextMock.Setup(c => c.User.ID).Returns(_userID);
        _contextMock.Setup(c => c.GuildID).Returns(_guildID);
        _contextMock.Setup(c => c.ChannelID).Returns(channelID);

        guildMock.Setup(g => g.ID).Returns(_guildID);
        guildMock.Setup(g => g.OwnerID).Returns(DiscordSnowflake.New(3));

        _channelMock.Setup(c => c.ID).Returns(channelID);
        _channelMock
            .Setup(c => c.PermissionOverwrites)
            .Returns(default(Optional<IReadOnlyList<IPermissionOverwrite>>));

        _everyoneRoleMock.Setup(r => r.ID).Returns(_guildID);

        _userMock.Setup(u => u.ID).Returns(_userID);

        _memberMock.Setup(m => m.User).Returns(new Optional<IUser>(_userMock.Object));
        _memberMock.Setup(m => m.Roles).Returns(Array.Empty<Snowflake>());

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildMemberAsync
                (
                    It.Is<Snowflake>(s => s == _guildID),
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
                    It.Is<Snowflake>(s => s == _guildID),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuild>.FromSuccess(guildMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == _guildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object });

        _channelAPIMock
            .Setup(a => a.GetChannelAsync(It.Is<Snowflake>(s => s == channelID), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IChannel>.FromSuccess(_channelMock.Object));
    }

    /// <summary>
    /// Tests whether the condition behaves correctly for various cases.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task ReturnsCorrectResultForInvoker<TPermission>
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
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition behaves correctly for various cases.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task ReturnsCorrectResultForUser<TPermission>
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
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _userMock.Object);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition behaves correctly for various cases.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task ReturnsCorrectResultForMember<TPermission>
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
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _memberMock.Object);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition behaves correctly for various cases.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task ReturnsCorrectResultForRole<TPermission>
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
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _everyoneRoleMock.Object);
        Assert.Equal(expected, result.IsSuccess);
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

        var userOwnedGuildID = DiscordSnowflake.New(4);

        var userOwnedGuildMock = new Mock<IGuild>();
        userOwnedGuildMock.Setup(g => g.ID).Returns(userOwnedGuildID);
        userOwnedGuildMock.Setup(g => g.OwnerID).Returns(_userID);

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildMemberAsync
                (
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
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
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuild>.FromSuccess(userOwnedGuildMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == userOwnedGuildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object });

        _everyoneRoleMock.Setup(r => r.ID).Returns(userOwnedGuildID);
        _contextMock.Setup(c => c.GuildID).Returns(userOwnedGuildID);

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether the condition ignores the invoker being the guild owner for role targets.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task IgnoresGuildOwnerForRole<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));

        var userOwnedGuildID = DiscordSnowflake.New(4);

        var userOwnedGuildMock = new Mock<IGuild>();
        userOwnedGuildMock.Setup(g => g.ID).Returns(userOwnedGuildID);
        userOwnedGuildMock.Setup(g => g.OwnerID).Returns(_userID);

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildMemberAsync
                (
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
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
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuild>.FromSuccess(userOwnedGuildMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == userOwnedGuildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object });

        _everyoneRoleMock.Setup(r => r.ID).Returns(userOwnedGuildID);
        _contextMock.Setup(c => c.GuildID).Returns(userOwnedGuildID);

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _everyoneRoleMock.Object);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition ignores the invoker being the guild owner for user targets.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task IgnoresGuildOwnerForUser<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));
        _contextMock.Setup(c => c.User.ID).Returns(DiscordSnowflake.New(4));

        var userOwnedGuildID = DiscordSnowflake.New(5);

        var userOwnedGuildMock = new Mock<IGuild>();
        userOwnedGuildMock.Setup(g => g.ID).Returns(userOwnedGuildID);
        userOwnedGuildMock.Setup(g => g.OwnerID).Returns(_userID);

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildMemberAsync
                (
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
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
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuild>.FromSuccess(userOwnedGuildMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == userOwnedGuildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object });

        _everyoneRoleMock.Setup(r => r.ID).Returns(userOwnedGuildID);
        _contextMock.Setup(c => c.GuildID).Returns(userOwnedGuildID);

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _userMock.Object);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition ignores the invoker being the guild owner for member targets.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task IgnoresGuildOwnerForMember<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));
        _contextMock.Setup(c => c.User.ID).Returns(DiscordSnowflake.New(4));

        var userOwnedGuildID = DiscordSnowflake.New(5);

        var userOwnedGuildMock = new Mock<IGuild>();
        userOwnedGuildMock.Setup(g => g.ID).Returns(userOwnedGuildID);
        userOwnedGuildMock.Setup(g => g.OwnerID).Returns(_userID);

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildMemberAsync
                (
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
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
                    It.Is<Snowflake>(s => s == userOwnedGuildID),
                    It.IsAny<Optional<bool>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Result<IGuild>.FromSuccess(userOwnedGuildMock.Object));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == userOwnedGuildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object });

        _everyoneRoleMock.Setup(r => r.ID).Returns(userOwnedGuildID);
        _contextMock.Setup(c => c.GuildID).Returns(userOwnedGuildID);

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _userMock.Object);
        Assert.Equal(expected, result.IsSuccess);
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
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether the condition ignores the administrator permission for role targets.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task IgnoresAdministratorPermissionForRole<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        effectivePermissions = effectivePermissions.Concat(new[] { DiscordPermission.Administrator }).ToArray();

        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _everyoneRoleMock.Object);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition ignores the administrator permission for user targets.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task IgnoresAdministratorPermissionForUser<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _contextMock.Setup(c => c.User.ID).Returns(DiscordSnowflake.New(4));

        effectivePermissions = effectivePermissions.Concat(new[] { DiscordPermission.Administrator }).ToArray();
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _userMock.Object);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition ignores the administrator permission for member targets.
    /// </summary>
    /// <typeparam name="TPermission">The required permissions.</typeparam>
    /// <param name="logicalOperator">The logical operator to apply.</param>
    /// <param name="required">The permissions required by the condition.</param>
    /// <param name="effectivePermissions">The effective permissions.</param>
    /// <param name="expected">The expected result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [MemberData(nameof(Cases))]
    public async Task IgnoresAdministratorPermissionForMember<TPermission>
    (
        LogicalOperator logicalOperator,
        TPermission[] required,
        DiscordPermission[] effectivePermissions,
        bool expected
    )
        where TPermission : struct, Enum
    {
        _contextMock.Setup(c => c.User.ID).Returns(DiscordSnowflake.New(4));

        effectivePermissions = effectivePermissions.Concat(new[] { DiscordPermission.Administrator }).ToArray();
        _everyoneRoleMock.Setup(r => r.Permissions).Returns(new DiscordPermissionSet(effectivePermissions));

        var requiredPermissions = required.Cast<DiscordPermission>().ToArray();
        var attribute = new RequireDiscordPermissionAttribute(requiredPermissions)
        {
            Operator = logicalOperator
        };

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _userMock.Object);
        Assert.Equal(expected, result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the condition respects user roles.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RespectsUserRoles()
    {
        var memberRoleID = DiscordSnowflake.New(3);
        var memberRole = new Mock<IRole>();

        _everyoneRoleMock
            .Setup(r => r.Permissions)
            .Returns(new DiscordPermissionSet(DiscordPermission.SendMessages));

        memberRole
            .Setup(r => r.ID).Returns(memberRoleID);

        memberRole
            .Setup(r => r.Permissions)
            .Returns(new DiscordPermissionSet(DiscordPermission.ReadMessageHistory));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == _guildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object, memberRole.Object });

        _memberMock.Setup(m => m.Roles).Returns(new[] { memberRoleID });

        var attribute = new RequireDiscordPermissionAttribute
        (
            DiscordPermission.SendMessages,
            DiscordPermission.ReadMessageHistory
        );

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _userMock.Object);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether the condition respects member roles.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RespectsMemberRoles()
    {
        var memberRoleID = DiscordSnowflake.New(3);
        var memberRole = new Mock<IRole>();

        _everyoneRoleMock
            .Setup(r => r.Permissions)
            .Returns(new DiscordPermissionSet(DiscordPermission.SendMessages));

        memberRole
            .Setup(r => r.ID).Returns(memberRoleID);

        memberRole
            .Setup(r => r.Permissions)
            .Returns(new DiscordPermissionSet(DiscordPermission.ReadMessageHistory));

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == _guildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object, memberRole.Object });

        _memberMock.Setup(m => m.Roles).Returns(new[] { memberRoleID });

        var attribute = new RequireDiscordPermissionAttribute
        (
            DiscordPermission.SendMessages,
            DiscordPermission.ReadMessageHistory
        );

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _memberMock.Object);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether the condition respects channel role permission overwrites.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RespectsChannelRoleOverwrites()
    {
        var memberRoleID = DiscordSnowflake.New(3);
        var memberRole = new Mock<IRole>();

        var overwriteMock = new Mock<IPermissionOverwrite>();
        overwriteMock.Setup(o => o.ID).Returns(memberRoleID);
        overwriteMock.Setup(o => o.Type).Returns(PermissionOverwriteType.Role);
        overwriteMock.Setup(o => o.Allow).Returns(new DiscordPermissionSet(DiscordPermission.ReadMessageHistory));
        overwriteMock.Setup(o => o.Deny).Returns(DiscordPermissionSet.Empty);

        _everyoneRoleMock
            .Setup(r => r.Permissions)
            .Returns(new DiscordPermissionSet(DiscordPermission.SendMessages));

        memberRole
            .Setup(r => r.ID).Returns(memberRoleID);

        memberRole
            .Setup(r => r.Permissions)
            .Returns(DiscordPermissionSet.Empty);

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == _guildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object, memberRole.Object });

        _memberMock.Setup(m => m.Roles).Returns(new[] { memberRoleID });

        _channelMock
            .Setup(c => c.PermissionOverwrites)
            .Returns(new Optional<IReadOnlyList<IPermissionOverwrite>>(new[] { overwriteMock.Object }));

        var attribute = new RequireDiscordPermissionAttribute
        (
            DiscordPermission.SendMessages,
            DiscordPermission.ReadMessageHistory
        );

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _memberMock.Object);
        ResultAssert.Successful(result);
    }

    /// <summary>
    /// Tests whether the condition respects channel member permission overwrites.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RespectsChannelMemberOverwrites()
    {
        var memberRoleID = DiscordSnowflake.New(3);
        var memberRole = new Mock<IRole>();

        var overwriteMock = new Mock<IPermissionOverwrite>();
        overwriteMock.Setup(o => o.ID).Returns(_userID);
        overwriteMock.Setup(o => o.Type).Returns(PermissionOverwriteType.Member);
        overwriteMock.Setup(o => o.Allow).Returns(new DiscordPermissionSet(DiscordPermission.ReadMessageHistory));
        overwriteMock.Setup(o => o.Deny).Returns(DiscordPermissionSet.Empty);

        _everyoneRoleMock
            .Setup(r => r.Permissions)
            .Returns(new DiscordPermissionSet(DiscordPermission.SendMessages));

        memberRole
            .Setup(r => r.ID).Returns(memberRoleID);

        memberRole
            .Setup(r => r.Permissions)
            .Returns(DiscordPermissionSet.Empty);

        _guildAPIMock
            .Setup
            (
                a => a.GetGuildRolesAsync(It.Is<Snowflake>(s => s == _guildID), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new[] { _everyoneRoleMock.Object, memberRole.Object });

        _memberMock.Setup(m => m.Roles).Returns(new[] { memberRoleID });

        _channelMock
            .Setup(c => c.PermissionOverwrites)
            .Returns(new Optional<IReadOnlyList<IPermissionOverwrite>>(new[] { overwriteMock.Object }));

        var attribute = new RequireDiscordPermissionAttribute
        (
            DiscordPermission.SendMessages,
            DiscordPermission.ReadMessageHistory
        );

        var condition = new RequireDiscordPermissionCondition
        (
            _guildAPIMock.Object,
            _channelAPIMock.Object,
            _contextMock.Object
        );

        var result = await condition.CheckAsync(attribute, _memberMock.Object);
        ResultAssert.Successful(result);
    }
}
