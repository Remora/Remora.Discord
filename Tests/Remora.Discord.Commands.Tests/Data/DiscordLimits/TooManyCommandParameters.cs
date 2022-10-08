//
//  TooManyCommandParameters.cs
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

public class TooManyCommandParameters : CommandGroup
{
    [Command("a")]
    public Task<IResult> A
    (
        int p1,
        int p2,
        int p3,
        int p4,
        int p5,
        int p6,
        int p7,
        int p8,
        int p9,
        int p10,
        int p11,
        int p12,
        int p13,
        int p14,
        int p15,
        int p16,
        int p17,
        int p18,
        int p19,
        int p20,
        int p21,
        int p22,
        int p23,
        int p24,
        int p25,
        int p26
    )
    {
        throw new NotImplementedException();
    }
}
