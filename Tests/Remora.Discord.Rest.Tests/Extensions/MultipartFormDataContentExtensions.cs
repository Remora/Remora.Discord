//
//  MultipartFormDataContentExtensions.cs
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
using System.Linq;
using System.Net.Http;

namespace Remora.Discord.Rest.Tests.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="MultipartFormDataContent"/> class.
    /// </summary>
    public static class MultipartFormDataContentExtensions
    {
        /// <summary>
        /// Checks whether the multipart content contains a subpart of the given type, optionally matching the given
        /// matcher.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="matcher">The matcher.</param>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>true if the multipart content contains a matching subpart; otherwise, false.</returns>
        public static bool ContainsContent<TContent>
        (
            this MultipartFormDataContent content,
            Func<TContent, bool>? matcher = null
        )
            where TContent : HttpContent
        {
            matcher ??= _ => true;
            return content.OfType<TContent>().Where(matcher).Any();
        }

        /// <summary>
        /// Checks whether the multipart content contains a subpart of the given type with the given name, optionally
        /// matching the given matcher.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="name">The name of the content subpart.</param>
        /// <param name="matcher">The matcher.</param>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>true if the multipart content contains a matching subpart; otherwise, false.</returns>
        public static bool ContainsContent<TContent>
        (
            this MultipartFormDataContent content,
            string name,
            Func<TContent, bool>? matcher = null
        )
            where TContent : HttpContent
        {
            matcher ??= _ => true;

            return content.ContainsContent<TContent>(c =>
            {
                if (c.Headers.ContentDisposition is null)
                {
                    return false;
                }

                if (c.Headers.ContentDisposition.Name?.Trim('"') != name)
                {
                    return false;
                }

                return matcher(c);
            });
        }

        /// <summary>
        /// Checks whether the multipart content contains a <see cref="StreamContent"/> subpart with the given name and
        /// filename, optionally matching the given matcher.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="name">The name of the content subpart.</param>
        /// <param name="filename">The filename of the content subpart.</param>
        /// <param name="matcher">The matcher.</param>
        /// <returns>true if the multipart content contains a matching subpart; otherwise, false.</returns>
        public static bool ContainsContent
        (
            this MultipartFormDataContent content,
            string name,
            string filename,
            Func<StreamContent, bool>? matcher = null
        )
        {
            matcher ??= _ => true;

            return content.ContainsContent<StreamContent>(name, c =>
            {
                if (c.Headers.ContentDisposition is null)
                {
                    return false;
                }

                if (c.Headers.ContentDisposition.FileName?.Trim('"') != filename)
                {
                    return false;
                }

                return matcher(c);
            });
        }
    }
}
