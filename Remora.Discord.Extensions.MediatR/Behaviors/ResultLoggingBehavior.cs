//
//  ResultLoggingBehavior.cs
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

using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using MediatR;
using Microsoft.Extensions.Logging;
using Remora.Results;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Remora.Discord.Extensions.MediatR.Behaviors
{
    /// <summary>
    /// A pipeline behavior which automatically logs request handling.
    /// </summary>
    /// <typeparam name="TRequest">The type of request to handle.</typeparam>
    /// <typeparam name="TResponse">The type of response to be returned as a <see cref="Result{TResponse}"/>.</typeparam>
    public class ResultLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IRequest<Result<TResponse>>
    {
        private readonly ILogger<ResultLoggingBehavior<TRequest, TResponse>> _logger;
        private static readonly string NotificationTypeName = typeof(TRequest).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultLoggingBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="logger">A logger for this instance.</param>
        public ResultLoggingBehavior(ILogger<ResultLoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse>> next)
        {
            _logger.LogInformation("Handling '{Request}'...", NotificationTypeName);

            var sw = Stopwatch.StartNew();
            var response = await next();
            sw.Stop();

            if (response.IsSuccess)
            {
                _logger.LogInformation("Successfully handled '{Request}' in {Elapsed}", NotificationTypeName, sw.Elapsed.Humanize(precision: 5));
            }
            else if (response.Error is ExceptionError exe)
            {
                _logger.LogError(exe.Exception, "Request '{Request}' failed after {Elapsed}: {Reason}", NotificationTypeName, sw.Elapsed.Humanize(precision: 5), exe.Message);
            }
            else
            {
                _logger.LogWarning("Request '{Request}' failed after {Elapsed}.", NotificationTypeName, sw.Elapsed.Humanize(precision: 5));
            }

            return response;
        }
    }
}
