//
//  DiscordPermissionSetTests.cs
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
using System.Numerics;
using Moq;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.API.Tests.TestBases;
using Xunit;

#pragma warning disable CS1591, SA1600

namespace Remora.Discord.API.Tests.Objects;

/// <inheritdoc />
public class DiscordPermissionSetTests : ObjectTestBase<IDiscordPermissionSet>
{
    [Fact]
    public void HasPermissionReturnsTrueForExistingPermission()
    {
        var permissions = new DiscordPermissionSet(BigInteger.One);
        var permission = DiscordPermission.CreateInstantInvite;

        Assert.True(permissions.HasPermission(permission));
    }

    [Fact]
    public void HasPermissionReturnsFalseForNonExistentPermission()
    {
        var permissions = new DiscordPermissionSet(BigInteger.One);
        var permission = DiscordPermission.KickMembers;

        Assert.False(permissions.HasPermission(permission));
    }

    [Fact]
    public void HasPermissionReturnsFalseForNonExistentPermissionInAnotherByte()
    {
        var permissions = new DiscordPermissionSet(BigInteger.One);
        var permission = DiscordPermission.MoveMembers;

        Assert.False(permissions.HasPermission(permission));
    }

    [Fact]
    public void HasPermissionReturnsTrueForExistingTextPermission()
    {
        var permissions = new DiscordPermissionSet(BigInteger.One);
        var permission = DiscordTextPermission.CreateInstantInvite;

        Assert.True(permissions.HasPermission(permission));
    }

    [Fact]
    public void HasPermissionReturnsFalseForNonExistentTextPermission()
    {
        var permissions = new DiscordPermissionSet(BigInteger.One);
        var permission = DiscordTextPermission.ManageChannels;

        Assert.False(permissions.HasPermission(permission));
    }

    [Fact]
    public void HasPermissionReturnsTrueForExistingVoicePermission()
    {
        var permissions = new DiscordPermissionSet(BigInteger.One);
        var permission = DiscordVoicePermission.CreateInstantInvite;

        Assert.True(permissions.HasPermission(permission));
    }

    [Fact]
    public void HasPermissionReturnsFalseForNonExistentVoicePermission()
    {
        var permissions = new DiscordPermissionSet(BigInteger.One);
        var permission = DiscordVoicePermission.ManageChannels;

        Assert.False(permissions.HasPermission(permission));
    }

    /// <summary>
    /// Tests that <see cref="DiscordPermissionSet.HasPermission(DiscordPermission)"/> works correctly for large integers that are considered signed as default by <see cref="BigInteger"/>.
    /// </summary>
    [Fact]
    public void HasPermissionReturnsTrueForLargeIntegers()
    {
        var permissions = new[]
        {
            DiscordPermission.AddReactions,
            DiscordPermission.Connect,
            DiscordPermission.UseVoiceActivity,
            DiscordPermission.SendMessagesInThreads,
            DiscordPermission.UseEmbeddedActivities
        };

        var permissionSet = new DiscordPermissionSet(permissions);
        var permission = DiscordPermission.UseEmbeddedActivities;

        Assert.True(permissionSet.HasPermission(permission));
    }

    [Fact]
    public void CanComputeMemberPermissions()
    {
        var memberID = DiscordSnowflake.New(0);

        var everyonePermissions = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        var everyoneRoleMock = new Mock<IRole>();
        everyoneRoleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(1));
        everyoneRoleMock.SetupGet(r => r.Permissions).Returns(everyonePermissions);

        var everyoneRole = everyoneRoleMock.Object;

        var actual = DiscordPermissionSet.ComputePermissions
        (
            memberID,
            everyoneRole,
            Array.Empty<IRole>()
        );

        var expected = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanComputeMemberPermissionsWithMemberAllowOverwrites()
    {
        var memberID = DiscordSnowflake.New(0);

        var everyonePermissions = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        var everyoneRoleMock = new Mock<IRole>();
        everyoneRoleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(1));
        everyoneRoleMock.SetupGet(r => r.Permissions).Returns(everyonePermissions);

        var everyoneRole = everyoneRoleMock.Object;

        var memberAllow = new DiscordPermissionSet(DiscordTextPermission.MentionEveryone);
        var memberOverwriteMock = new Mock<IPermissionOverwrite>();
        memberOverwriteMock.SetupGet(o => o.ID).Returns(memberID);
        memberOverwriteMock.SetupGet(o => o.Allow).Returns(memberAllow);
        memberOverwriteMock.SetupGet(o => o.Deny).Returns(new DiscordPermissionSet(BigInteger.Zero));

        var memberOverwrite = memberOverwriteMock.Object;

        var actual = DiscordPermissionSet.ComputePermissions
        (
            memberID,
            everyoneRole,
            Array.Empty<IRole>(),
            new[] { memberOverwrite }
        );

        var expected = new DiscordPermissionSet
        (
            DiscordTextPermission.SendMessages,
            DiscordTextPermission.MentionEveryone
        );

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanComputeMemberPermissionsWithMemberDenyOverwrites()
    {
        var memberID = DiscordSnowflake.New(0);

        var everyonePermissions = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        var everyoneRoleMock = new Mock<IRole>();
        everyoneRoleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(1));
        everyoneRoleMock.SetupGet(r => r.Permissions).Returns(everyonePermissions);

        var everyoneRole = everyoneRoleMock.Object;

        var memberDeny = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        var memberOverwriteMock = new Mock<IPermissionOverwrite>();
        memberOverwriteMock.SetupGet(o => o.ID).Returns(memberID);
        memberOverwriteMock.SetupGet(o => o.Allow).Returns(new DiscordPermissionSet(BigInteger.Zero));
        memberOverwriteMock.SetupGet(o => o.Deny).Returns(memberDeny);

        var memberOverwrite = memberOverwriteMock.Object;

        var actual = DiscordPermissionSet.ComputePermissions
        (
            memberID,
            everyoneRole,
            Array.Empty<IRole>(),
            new[] { memberOverwrite }
        );

        var expected = new DiscordPermissionSet(BigInteger.Zero);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanComputeMemberWithRolePermissions()
    {
        var memberID = DiscordSnowflake.New(0);

        var everyonePermissions = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        var everyoneRoleMock = new Mock<IRole>();
        everyoneRoleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(1));
        everyoneRoleMock.SetupGet(r => r.Permissions).Returns(everyonePermissions);

        var everyoneRole = everyoneRoleMock.Object;

        var rolePermissions = new DiscordPermissionSet(DiscordTextPermission.MentionEveryone);
        var roleMock = new Mock<IRole>();
        roleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(2));
        roleMock.SetupGet(r => r.Permissions).Returns(rolePermissions);

        var role = roleMock.Object;

        var actual = DiscordPermissionSet.ComputePermissions(memberID, everyoneRole, new[] { role });

        var expected = new DiscordPermissionSet
        (
            DiscordTextPermission.SendMessages,
            DiscordTextPermission.MentionEveryone
        );

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanComputeMemberPermissionsWithRoleAllowOverwrites()
    {
        var memberID = DiscordSnowflake.New(0);

        var everyonePermissions = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        var everyoneMock = new Mock<IRole>();
        everyoneMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(1));
        everyoneMock.SetupGet(r => r.Permissions).Returns(everyonePermissions);

        var everyoneRole = everyoneMock.Object;

        var rolePermissions = new DiscordPermissionSet(DiscordTextPermission.MentionEveryone);
        var roleMock = new Mock<IRole>();
        roleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(2));
        roleMock.SetupGet(r => r.Permissions).Returns(rolePermissions);

        var role = roleMock.Object;

        var roleAllow = new DiscordPermissionSet(DiscordTextPermission.AddReactions);
        var roleOverwriteMock = new Mock<IPermissionOverwrite>();
        roleOverwriteMock.SetupGet(o => o.ID).Returns(role.ID);
        roleOverwriteMock.SetupGet(o => o.Allow).Returns(roleAllow);
        roleOverwriteMock.SetupGet(o => o.Deny).Returns(new DiscordPermissionSet(BigInteger.Zero));

        var roleOverwrite = roleOverwriteMock.Object;

        var actual = DiscordPermissionSet.ComputePermissions
        (
            memberID,
            everyoneRole,
            new[] { role },
            new[] { roleOverwrite }
        );

        var expected = new DiscordPermissionSet
        (
            DiscordTextPermission.SendMessages,
            DiscordTextPermission.MentionEveryone,
            DiscordTextPermission.AddReactions
        );

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanComputeMemberPermissionsWithRoleDenyOverwrites()
    {
        var memberID = DiscordSnowflake.New(0);

        var everyonePermissions = new DiscordPermissionSet(DiscordTextPermission.SendMessages);
        var everyoneRoleMock = new Mock<IRole>();
        everyoneRoleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(1));
        everyoneRoleMock.SetupGet(r => r.Permissions).Returns(everyonePermissions);

        var everyoneRole = everyoneRoleMock.Object;

        var rolePermissions = new DiscordPermissionSet(DiscordTextPermission.MentionEveryone);
        var roleMock = new Mock<IRole>();
        roleMock.SetupGet(r => r.ID).Returns(DiscordSnowflake.New(2));
        roleMock.SetupGet(r => r.Permissions).Returns(rolePermissions);

        var role = roleMock.Object;

        var roleDeny = new DiscordPermissionSet(DiscordTextPermission.MentionEveryone);
        var roleOverwriteMock = new Mock<IPermissionOverwrite>();
        roleOverwriteMock.SetupGet(o => o.ID).Returns(role.ID);
        roleOverwriteMock.SetupGet(o => o.Allow).Returns(new DiscordPermissionSet(BigInteger.Zero));
        roleOverwriteMock.SetupGet(o => o.Deny).Returns(roleDeny);

        var roleOverwrite = roleOverwriteMock.Object;

        var actual = DiscordPermissionSet.ComputePermissions
        (
            memberID,
            everyoneRole,
            new[] { role },
            new[] { roleOverwrite }
        );

        var expected = new DiscordPermissionSet
        (
            DiscordTextPermission.SendMessages
        );

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanGetPermissions()
    {
        var permissions = new[] { DiscordPermission.AddReactions, DiscordPermission.Connect, DiscordPermission.UseVoiceActivity, DiscordPermission.SendMessagesInThreads, DiscordPermission.UseEmbeddedActivities };
        var permissionSet = new DiscordPermissionSet(permissions);

        Assert.Equal(permissions, permissionSet.GetPermissions());
    }
}
