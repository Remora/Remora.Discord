//
//  TooManyGroupCommands.cs
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
using Remora.Results;

#pragma warning disable CS1591, SA1600

namespace Remora.Discord.Commands.Tests.Data.DiscordLimits;

[Group("a")]
public class TooManyGroupCommands : CommandGroup
{
    [Command("c01")]
    public Task<IResult> C01()
    {
        throw new NotImplementedException();
    }

    [Command("c02")]
    public Task<IResult> C02()
    {
        throw new NotImplementedException();
    }

    [Command("c03")]
    public Task<IResult> C03()
    {
        throw new NotImplementedException();
    }

    [Command("c04")]
    public Task<IResult> C04()
    {
        throw new NotImplementedException();
    }

    [Command("c05")]
    public Task<IResult> C05()
    {
        throw new NotImplementedException();
    }

    [Command("c06")]
    public Task<IResult> C06()
    {
        throw new NotImplementedException();
    }

    [Command("c07")]
    public Task<IResult> C07()
    {
        throw new NotImplementedException();
    }

    [Command("c08")]
    public Task<IResult> C08()
    {
        throw new NotImplementedException();
    }

    [Command("c09")]
    public Task<IResult> C09()
    {
        throw new NotImplementedException();
    }

    [Command("c10")]
    public Task<IResult> C10()
    {
        throw new NotImplementedException();
    }

    [Command("c11")]
    public Task<IResult> C11()
    {
        throw new NotImplementedException();
    }

    [Command("c12")]
    public Task<IResult> C12()
    {
        throw new NotImplementedException();
    }

    [Command("c13")]
    public Task<IResult> C13()
    {
        throw new NotImplementedException();
    }

    [Command("c14")]
    public Task<IResult> C14()
    {
        throw new NotImplementedException();
    }

    [Command("c15")]
    public Task<IResult> C15()
    {
        throw new NotImplementedException();
    }

    [Command("c16")]
    public Task<IResult> C16()
    {
        throw new NotImplementedException();
    }

    [Command("c17")]
    public Task<IResult> C17()
    {
        throw new NotImplementedException();
    }

    [Command("c18")]
    public Task<IResult> C18()
    {
        throw new NotImplementedException();
    }

    [Command("c19")]
    public Task<IResult> C19()
    {
        throw new NotImplementedException();
    }

    [Command("c20")]
    public Task<IResult> C20()
    {
        throw new NotImplementedException();
    }

    [Command("c21")]
    public Task<IResult> C21()
    {
        throw new NotImplementedException();
    }

    [Command("c22")]
    public Task<IResult> C22()
    {
        throw new NotImplementedException();
    }

    [Command("c23")]
    public Task<IResult> C23()
    {
        throw new NotImplementedException();
    }

    [Command("c24")]
    public Task<IResult> C24()
    {
        throw new NotImplementedException();
    }

    [Command("c25")]
    public Task<IResult> C25()
    {
        throw new NotImplementedException();
    }

    [Command("c26")]
    public Task<IResult> C26()
    {
        throw new NotImplementedException();
    }
}
