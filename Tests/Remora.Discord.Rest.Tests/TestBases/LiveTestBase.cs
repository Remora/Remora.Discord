//
//  LiveTestBase.cs
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

using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.Rest.Extensions;

namespace Remora.Discord.Rest.Tests.TestBases
{
    /// <summary>
    /// Serves as a base class for tests against the live Discord service.
    /// </summary>
    /// <typeparam name="TAPI">The API under test.</typeparam>
    public abstract class LiveTestBase<TAPI>
    {
        /// <summary>
        /// Gets the API instance.
        /// </summary>
        protected TAPI API { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveTestBase{TAPI}"/> class.
        /// </summary>
        /// <param name="token">The token to use.</param>
        protected LiveTestBase(string token = "")
        {
            var services = new ServiceCollection()
                .AddDiscordRest(() => token)
                .BuildServiceProvider();

            this.API = services.GetRequiredService<TAPI>();
        }
    }
}
