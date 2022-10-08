//
//  NullableTypedCommands.cs
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

public class NullableTypedCommands : CommandGroup
{
    [Command("nullable-sbyte-value")]
    public Task<IResult> CommandWithNullableSByteValue(sbyte? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-byte-value")]
    public Task<IResult> CommandWithNullableByteValue(byte? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-short-value")]
    public Task<IResult> CommandWithNullableShortValue(short? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-ushort-value")]
    public Task<IResult> CommandWithNullableUShortValue(ushort? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-int-value")]
    public Task<IResult> CommandWithNullableIntValue(int? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-uint-value")]
    public Task<IResult> CommandWithNullableUIntValue(uint? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-long-value")]
    public Task<IResult> CommandWithNullableLongValue(long? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-ulong-value")]
    public Task<IResult> CommandWithNullableULongValue(ulong? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-float-value")]
    public Task<IResult> CommandWithNullableFloatValue(float? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-double-value")]
    public Task<IResult> CommandWithNullableDoubleValue(double? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-decimal-value")]
    public Task<IResult> CommandWithNullableDecimalValue(decimal? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-string-value")]
    public Task<IResult> CommandWithNullableStringValue(string? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-bool-value")]
    public Task<IResult> CommandWithNullableBoolValue(bool? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-enum-value")]
    public Task<IResult> CommandWithNullableEnumValue(DummyEnum? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-role-value")]
    public Task<IResult> CommandWithNullableRoleValue(IRole? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-user-value")]
    public Task<IResult> CommandWithNullableUserValue(IUser? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-member-value")]
    public Task<IResult> CommandWithNullableMemberValue(IGuildMember? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-channel-value")]
    public Task<IResult> CommandWithNullableChannelValue(IChannel? value = default)
    {
        throw new NotImplementedException();
    }

    [Command("nullable-snowflake-value")]
    public Task<IResult> CommandWithNullableSnowflakeValue(Snowflake? value)
    {
        throw new NotImplementedException();
    }
}
