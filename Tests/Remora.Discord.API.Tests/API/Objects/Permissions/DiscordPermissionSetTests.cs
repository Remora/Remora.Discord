//
//  DiscordPermissionSetTests.cs
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

using System.Numerics;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.API.Tests.TestBases;
using Xunit;

#pragma warning disable CS1591, SA1600

namespace Remora.Discord.API.Tests.Objects
{
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
    }
}
