//
//  TestCommandModule.cs
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

using System.Threading.Tasks;
using Remora.Commands.Attributes;
using Remora.Commands.Modules;
using Remora.Results;

#pragma warning disable CS1591, SA1600

namespace Remora.Commands.Tests.Data.Modules
{
    [Group("test")]
    public class TestCommandModule : ModuleBase
    {
        [Command("parameterless")]
        public Task<IResult> Parameterless()
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("single-positional")]
        public Task<IResult> SinglePositional(string value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("single-optional-positional")]
        public Task<IResult> SingleOptionalPositional(string value = "dooga")
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("single-named")]
        public Task<IResult> SingleNamed([Option("value")] string value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("single-optional-named")]
        public Task<IResult> SingleOptionalNamed([Option("value")] string value = "dooga")
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("single-positional-bool")]
        public Task<IResult> SinglePositionalBool(bool value)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }

        [Command("single-named-bool")]
        public Task<IResult> SingleNamedBool([Option("enable")] bool value = false)
        {
            return Task.FromResult<IResult>(OperationResult.FromSuccess());
        }
    }
}
