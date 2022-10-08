//
//  Markdown.cs
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
using System.Linq;
using JetBrains.Annotations;
using Remora.Discord.API.Objects;

namespace Remora.Discord.Extensions.Formatting;

/// <summary>
/// Provides helper methods to format strings into Discord Markdown.
/// </summary>
[PublicAPI]
public static partial class Markdown
{
    /// <summary>
    /// A collection of sensitive characters.
    /// </summary>
    private static readonly string[] _sensitiveCharacters = { "\\", "*", "_", "~", "`", "|", ">" };

    /// <summary>
    /// Formats a string to use Markdown Bold formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted bold string.
    /// </returns>
    public static string Bold(string text) => $"**{text}**";

    /// <summary>
    /// Formats a string to use Markdown Italicised formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted italicised string.
    /// </returns>
    public static string Italicise(string text) => $"*{text}*";

    /// <summary>
    /// Formats a string to use Markdown Underlined formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted underlined string.
    /// </returns>
    public static string Underline(string text) => $"__{text}__";

    /// <summary>
    /// Formats a string to use Markdown Strikethrough formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted strikethrough string.
    /// </returns>
    public static string Strikethrough(string text) => $"~~{text}~~";

    /// <summary>
    /// Formats a string to use Markdown Spoiler formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted spoilered string.
    /// </returns>
    public static string Spoiler(string text) => $"||{text}||";

    /// <summary>
    /// Formats a string to use Markdown Inline Code formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted inline code string.
    /// </returns>
    public static string InlineCode(string text) => $"`{text}`";

    /// <summary>
    /// Formats a string to use Markdown Block Code formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted block code string.
    /// </returns>
    public static string BlockCode(string text) => BlockCode(text, string.Empty);

    /// <summary>
    /// Formats a string to use Markdown Block Code formatting with a specified
    /// language for syntax highlighting.
    /// </summary>
    /// <remarks>
    /// Discord supports the following languages: <see href="https://github.com/highlightjs/highlight.js/tree/main/src/languages"/>.
    /// </remarks>
    /// <param name="text">The input text to format.</param>
    /// <param name="language">The language.</param>
    /// <returns>
    /// A markdown-formatted block code string.
    /// </returns>
    public static string BlockCode(string text, string language) => $"```{language}\n{text}\n```";

    /// <summary>
    /// Formats a string to use Markdown Quote Block formatting.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <returns>
    /// A markdown-formatted block quote string.
    /// </returns>
    public static string BlockQuote(string text) => $">>> {text}\n";

    /// <summary>
    /// Escapes a URL to prevent embedding of its content.
    /// </summary>
    /// <param name="uri">The URI to escape.</param>
    /// <returns>
    /// A formatted URL string that will not embed its content.
    /// </returns>
    public static string EscapeUrl(Uri uri) => EscapeUrl(uri.ToString());

    /// <summary>
    /// Escapes a URL to prevent embedding of its content.
    /// </summary>
    /// <param name="url">The URL to escape.</param>
    /// <returns>
    /// A formatted URL string that will not embed its content.
    /// </returns>
    public static string EscapeUrl(string url) => $"<{url}>";

    /// <summary>
    /// Formats a string to contain a Markdown hyperlink of itself. The hyperlink will only hyperlink inside
    /// of <see cref="Embed"/> fields, excluding the <see cref="Embed.Author"/> and <see cref="Embed.Footer"/>
    /// properties.
    /// </summary>
    /// <param name="uri">The URL to hyperlink.</param>
    /// <returns>
    /// A Markdown-formatted hyperlink string.
    /// </returns>
    public static string Hyperlink(Uri uri) => Hyperlink(uri.ToString(), uri);

    /// <summary>
    /// Formats a string to contain a Markdown hyperlink of itself. The hyperlink will only hyperlink inside
    /// of <see cref="Embed"/> fields, excluding the <see cref="Embed.Author"/> and <see cref="Embed.Footer"/>
    /// properties.
    /// </summary>
    /// <param name="url">The URL to hyperlink.</param>
    /// <returns>
    /// A Markdown-formatted hyperlink string.
    /// </returns>
    public static string Hyperlink(string url) => Hyperlink(url, url);

    /// <summary>
    /// Formats a string to contain a Markdown Hyperlink. The hyperlink will only appear inside
    /// of <see cref="Embed"/> fields, excluding the <see cref="Embed.Author"/> and <see cref="Embed.Footer"/>
    /// properties.
    /// </summary>
    /// <param name="text">The text to hyperlink.</param>
    /// <param name="uri">The URI to contain within the text.</param>
    /// <returns>
    /// A Markdown-formatted hyperlink string.
    /// </returns>
    public static string Hyperlink(string text, Uri uri) => Hyperlink(text, uri.ToString());

    /// <summary>
    /// Formats a string to contain a Markdown Hyperlink. The hyperlink will only appear inside
    /// of <see cref="Embed"/> fields, excluding the <see cref="Embed.Author"/> and <see cref="Embed.Footer"/>
    /// properties.
    /// </summary>
    /// <param name="text">The text to hyperlink.</param>
    /// <param name="url">The URL to contain within the text.</param>
    /// <returns>
    /// A Markdown-formatted hyperlink string.
    /// </returns>
    public static string Hyperlink(string text, string url) => $"[{text}]({url})";

    /// <summary>
    /// Sanitizes a string of sensitive characters.
    /// </summary>
    /// <param name="text">The text to sanitize.</param>
    /// <returns>
    /// A sanitized string.
    /// </returns>
    public static string Sanitize(string text) => _sensitiveCharacters.Aggregate(text, (current, unsafeChar) => current.Replace(unsafeChar, $@"\{unsafeChar}"));
}
