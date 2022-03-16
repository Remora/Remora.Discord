//
//  MediatorTests.cs
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

using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.API.Objects;
using Remora.Discord.Extensions.MediatR.Behaviors;
using Remora.Discord.Extensions.MediatR.Extensions;
using Remora.Discord.Extensions.MediatR.Requests;
using Remora.Discord.Extensions.MediatR.Responders;
using Remora.Discord.Gateway.Extensions;
using Remora.Rest.Core;
using Xunit;

namespace Remora.Discord.Extensions.MediatR.Tests
{
#pragma warning disable SA1600, CS1591
    public class MediatorTests
    {
        private readonly IMediator _mediator;
        private readonly ServiceProvider _serviceProvider;

        public MediatorTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMediatR(typeof(Responders.MessageCreateHandler))
                .AddTransient(typeof(RequestExceptionActionProcessorBehavior<,>), typeof(GatewayEventExceptionHandlerBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultLoggingBehavior<,>))

                .BuildServiceProvider();

            _mediator = _serviceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task HandlesEvent()
        {
            var author = new User(new Snowflake(0), "Test", 1234, null);
            var messageCreated = new MessageCreate
                (
                    new Snowflake(1),
                    new Snowflake(2),
                    default,
                    author,
                    default,
                    "Hello World!",
                    System.DateTimeOffset.Now,
                    null,
                    false,
                    false,
                    new List<IUserMention>().AsReadOnly(),
                    new List<Snowflake>().AsReadOnly(),
                    default,
                    new List<IAttachment>().AsReadOnly(),
                    new List<IEmbed>().AsReadOnly(),
                    default,
                    default,
                    false,
                    default,
                    MessageType.Default
                );

            var mediatorEventResponder = new MediatorEventResponder(_mediator);

            var result = await mediatorEventResponder.RespondAsync(messageCreated);

            Assert.True(result.IsSuccess);
        }
    }
}
