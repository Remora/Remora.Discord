//
//  OptionalTypedCommands.cs
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

public class OptionalTypedCommands : CommandGroup
{
    [Command("optional-sbyte-value")]
    public Task<IResult> CommandWithOptionalSByteValue(Optional<sbyte> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-byte-value")]
    public Task<IResult> CommandWithOptionalByteValue(Optional<byte> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-short-value")]
    public Task<IResult> CommandWithOptionalShortValue(Optional<short> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-ushort-value")]
    public Task<IResult> CommandWithOptionalUShortValue(Optional<ushort> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-int-value")]
    public Task<IResult> CommandWithOptionalIntValue(Optional<int> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-uint-value")]
    public Task<IResult> CommandWithOptionalUIntValue(Optional<uint> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-long-value")]
    public Task<IResult> CommandWithOptionalLongValue(Optional<long> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-ulong-value")]
    public Task<IResult> CommandWithOptionalULongValue(Optional<ulong> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-float-value")]
    public Task<IResult> CommandWithOptionalFloatValue(Optional<float> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-double-value")]
    public Task<IResult> CommandWithOptionalDoubleValue(Optional<double> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-decimal-value")]
    public Task<IResult> CommandWithOptionalDecimalValue(Optional<decimal> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-string-value")]
    public Task<IResult> CommandWithOptionalStringValue(Optional<string> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-bool-value")]
    public Task<IResult> CommandWithOptionalBoolValue(Optional<bool> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-enum-value")]
    public Task<IResult> CommandWithOptionalEnumValue(Optional<DummyEnum> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-role-value")]
    public Task<IResult> CommandWithOptionalRoleValue(Optional<IRole> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-user-value")]
    public Task<IResult> CommandWithOptionalUserValue(Optional<IUser> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-member-value")]
    public Task<IResult> CommandWithOptionalMemberValue(Optional<IGuildMember> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-channel-value")]
    public Task<IResult> CommandWithOptionalChannelValue(Optional<IChannel> value = default)
    {
        throw new NotImplementedException();
    }

    [Command("optional-snowflake-value")]
    public Task<IResult> CommandWithOptionalSnowflakeValue(Optional<Snowflake> value)
    {
        throw new NotImplementedException();
    }
}
