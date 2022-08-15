//
//  TypedCommands.cs
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
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

#pragma warning disable CS1591, SA1600, SA1402, SA1602

namespace Remora.Discord.Commands.Tests.Data.Valid;

public class TypedCommands : CommandGroup
{
    [Command("sbyte-value")]
    public Task<IResult> CommandWithSByteValue(sbyte value)
    {
        throw new NotImplementedException();
    }

    [Command("byte-value")]
    public Task<IResult> CommandWithByteValue(byte value)
    {
        throw new NotImplementedException();
    }

    [Command("short-value")]
    public Task<IResult> CommandWithShortValue(short value)
    {
        throw new NotImplementedException();
    }

    [Command("ushort-value")]
    public Task<IResult> CommandWithUShortValue(ushort value)
    {
        throw new NotImplementedException();
    }

    [Command("int-value")]
    public Task<IResult> CommandWithIntValue(int value)
    {
        throw new NotImplementedException();
    }

    [Command("uint-value")]
    public Task<IResult> CommandWithUIntValue(uint value)
    {
        throw new NotImplementedException();
    }

    [Command("long-value")]
    public Task<IResult> CommandWithLongValue(long value)
    {
        throw new NotImplementedException();
    }

    [Command("ulong-value")]
    public Task<IResult> CommandWithULongValue(ulong value)
    {
        throw new NotImplementedException();
    }

    [Command("float-value")]
    public Task<IResult> CommandWithFloatValue(float value)
    {
        throw new NotImplementedException();
    }

    [Command("double-value")]
    public Task<IResult> CommandWithDoubleValue(double value)
    {
        throw new NotImplementedException();
    }

    [Command("decimal-value")]
    public Task<IResult> CommandWithDecimalValue(decimal value)
    {
        throw new NotImplementedException();
    }

    [Command("string-value")]
    public Task<IResult> CommandWithStringValue(string value)
    {
        throw new NotImplementedException();
    }

    [Command("bool-value")]
    public Task<IResult> CommandWithBoolValue(bool value)
    {
        throw new NotImplementedException();
    }

    [Command("role-value")]
    public Task<IResult> CommandWithRoleValue(IRole role)
    {
        throw new NotImplementedException();
    }

    [Command("user-value")]
    public Task<IResult> CommandWithUserValue(IUser value)
    {
        throw new NotImplementedException();
    }

    [Command("member-value")]
    public Task<IResult> CommandWithMemberValue(IGuildMember value)
    {
        throw new NotImplementedException();
    }

    [Command("channel-value")]
    public Task<IResult> CommandWithChannelValue(IChannel value)
    {
        throw new NotImplementedException();
    }

    [Command("snowflake-value")]
    public Task<IResult> CommandWithSnowflakeValue(Snowflake value)
    {
        throw new NotImplementedException();
    }

    [Command("enum-value")]
    public Task<IResult> CommandWithEnumValue(DummyEnum value)
    {
        throw new NotImplementedException();
    }
}
