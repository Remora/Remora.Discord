//
//  ResponderDispatchOptions.cs
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

namespace Remora.Discord.Gateway;

/// <summary>
///  Represents options related to <see cref="ResponderDispatchService"/>.
/// </summary>
/// <param name="MaxItems">If <paramref name="EnableParallelDispatch"/> is <c>false</c>, how many items can be queued for dispatch at any given time.</param>
/// <param name="MaxParallelism">How many threads can concurrently be used for dispatch. If <paramref name="EnableParallelDispatch"/> is false, this has no effect.</param>
/// <param name="EnableParallelDispatch">Whether to enable paralell dispatching. While this may have positive performance impacts, events may be dispatched out of order.
/// Intra-responder dispatch order is preserved, however.</param>
/// <remarks>
/// Parallelism may improve performance, but may also cause events to be processed out of order. e.g: MessageDelete firing before a MessageCreate.
/// This can also cause some race conditions when it comes to the state of caching entities, and is only recommended if you've ensured it has no significant impact
/// on the operation of your application.
/// </remarks>
public record ResponderDispatchOptions(uint MaxItems = 100, uint? MaxParallelism = null, bool EnableParallelDispatch = false);
