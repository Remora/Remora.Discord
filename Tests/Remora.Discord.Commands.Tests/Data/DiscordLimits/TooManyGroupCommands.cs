//
//  TooManyGroupCommands.cs
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
using Remora.Results;

#pragma warning disable CS1591, SA1600

namespace Remora.Discord.Commands.Tests.Data.DiscordLimits
{
    [Group("a")]
    public class TooManyGroupCommands : CommandGroup
    {
        [Command("c01")]
        public Task<Result> C01()
        {
            throw new NotImplementedException();
        }

        [Command("c02")]
        public Task<Result> C02()
        {
            throw new NotImplementedException();
        }

        [Command("c03")]
        public Task<Result> C03()
        {
            throw new NotImplementedException();
        }

        [Command("c04")]
        public Task<Result> C04()
        {
            throw new NotImplementedException();
        }

        [Command("c05")]
        public Task<Result> C05()
        {
            throw new NotImplementedException();
        }

        [Command("c06")]
        public Task<Result> C06()
        {
            throw new NotImplementedException();
        }

        [Command("c07")]
        public Task<Result> C07()
        {
            throw new NotImplementedException();
        }

        [Command("c08")]
        public Task<Result> C08()
        {
            throw new NotImplementedException();
        }

        [Command("c09")]
        public Task<Result> C09()
        {
            throw new NotImplementedException();
        }

        [Command("c10")]
        public Task<Result> C10()
        {
            throw new NotImplementedException();
        }

        [Command("c11")]
        public Task<Result> C11()
        {
            throw new NotImplementedException();
        }
    }
}
