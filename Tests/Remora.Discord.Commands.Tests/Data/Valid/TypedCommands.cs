//
//  TypedCommands.cs
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

using System;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Results;

#pragma warning disable CS1591, SA1600, SA1402, SA1602

namespace Remora.Discord.Commands.Tests.Data.Valid
{
    public class TypedCommands : CommandGroup
    {
        [Command("sbyte-value")]
        public Task<Result> CommandWithIntValue(sbyte value)
        {
            throw new NotImplementedException();
        }

        [Command("byte-value")]
        public Task<Result> CommandWithByteValue(byte value)
        {
            throw new NotImplementedException();
        }

        [Command("short-value")]
        public Task<Result> CommandWithShortValue(short value)
        {
            throw new NotImplementedException();
        }

        [Command("ushort-value")]
        public Task<Result> CommandWithUShortValue(ushort value)
        {
            throw new NotImplementedException();
        }

        [Command("int-value")]
        public Task<Result> CommandWithIntValue(int value)
        {
            throw new NotImplementedException();
        }

        [Command("uint-value")]
        public Task<Result> CommandWithUIntValue(uint value)
        {
            throw new NotImplementedException();
        }

        [Command("long-value")]
        public Task<Result> CommandWithLongValue(long value)
        {
            throw new NotImplementedException();
        }

        [Command("ulong-value")]
        public Task<Result> CommandWithULongValue(ulong value)
        {
            throw new NotImplementedException();
        }

        [Command("float-value")]
        public Task<Result> CommandWithFloatValue(float value)
        {
            throw new NotImplementedException();
        }

        [Command("double-value")]
        public Task<Result> CommandWithDoubleValue(double value)
        {
            throw new NotImplementedException();
        }

        [Command("decimal-value")]
        public Task<Result> CommandWithDecimalValue(decimal value)
        {
            throw new NotImplementedException();
        }

        [Command("string-value")]
        public Task<Result> CommandWithStringValue(string value)
        {
            throw new NotImplementedException();
        }

        [Command("bool-value")]
        public Task<Result> CommandWithBoolValue(bool value)
        {
            throw new NotImplementedException();
        }

        [Command("role-value")]
        public Task<Result> CommandWithRoleValue(IRole role)
        {
            throw new NotImplementedException();
        }

        [Command("user-value")]
        public Task<Result> CommandWithUserValue(IUser value)
        {
            throw new NotImplementedException();
        }

        [Command("channel-value")]
        public Task<Result> CommandWithChannelValue(IChannel value)
        {
            throw new NotImplementedException();
        }

        [Command("member-value")]
        public Task<Result> CommandWithMemberValue(IGuildMember value)
        {
            throw new NotImplementedException();
        }

        [Command("enum-value")]
        public Task<Result> CommandWithEnumValue(TestEnum value)
        {
            throw new NotImplementedException();
        }
    }

    public enum TestEnum
    {
        Value1,
        Value2
    }
}
