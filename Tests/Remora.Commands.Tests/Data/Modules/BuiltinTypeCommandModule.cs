//
//  BuiltinTypeCommandModule.cs
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
using System.Numerics;
using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Modules;
using Remora.Results;

#pragma warning disable CS1591, SA1600, SA1602

namespace Remora.Commands.Tests.Data.Modules
{
    [Group("test")]
    public class BuiltinTypeCommandModule : ModuleBase
    {
        public enum TestEnum
        {
            Wooga,
            Booga
        }

        [Command("bool")]
        public Task<IResult> SinglePositionalBool(bool value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("char")]
        public Task<IResult> Char(char value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("byte")]
        public Task<IResult> Byte(byte value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("sbyte")]
        public Task<IResult> SByte(sbyte value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("short")]
        public Task<IResult> Int16(short value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("ushort")]
        public Task<IResult> UInt16(ushort value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("int")]
        public Task<IResult> Int32(int value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("uint")]
        public Task<IResult> UInt32(uint value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("long")]
        public Task<IResult> Int64(long value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("ulong")]
        public Task<IResult> Uint64(ulong value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("float")]
        public Task<IResult> Single(float value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("double")]
        public Task<IResult> Double(double value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("decimal")]
        public Task<IResult> Decimal(decimal value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("big-integer")]
        public Task<IResult> BigInteger(BigInteger value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("date-time-offset")]
        public Task<IResult> DateTimeOffset(DateTimeOffset value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("enum")]
        public Task<IResult> Enum(TestEnum value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }
    }
}
