//
//  ServiceCollectionExtensions.cs
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

using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Interactivity.Extensions;
using Remora.Discord.Interactivity.Services;
using Remora.Discord.Pagination.Interactions;
using Remora.Discord.Pagination.Responders;
using Remora.Rest.Core;

namespace Remora.Discord.Pagination.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required for paginated messages.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection, with pagination.</returns>
    public static IServiceCollection AddPagination(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddInteractivity();
        serviceCollection.AddSingleton(InMemoryDataService<Snowflake, PaginatedMessageData>.Instance);
        serviceCollection.AddResponder<MessageDeletedResponder>();
        serviceCollection.AddInteractionGroup<PaginationInteractions>();

        return serviceCollection;
    }
}
