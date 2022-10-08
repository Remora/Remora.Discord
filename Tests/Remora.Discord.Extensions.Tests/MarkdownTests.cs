//
//  MarkdownTests.cs
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
using System.Collections.Generic;
using Remora.Discord.Extensions.Formatting;
using Xunit;

namespace Remora.Discord.Extensions.Tests;

/// <summary>
/// Tests to ensure the <see cref="Markdown"/> formats inputs correctly to Discord markdown standards.
/// </summary>
public class MarkdownTests
{
    /// <summary>
    /// Tests to see if the <see cref="Markdown.Bold"/> method bolds input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("Code Geass: Hangyaku no Lelouch")]
    [InlineData("宇宙戦艦ヤマト2199")]
    public void BoldSuccess(string text)
    {
        var expected = $"**{text}**";
        var actual = Markdown.Bold(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Italicise"/> method italicises input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("Code Geass: Hangyaku no Lelouch")]
    [InlineData("宇宙戦艦ヤマト2199")]
    public void ItaliciseSuccess(string text)
    {
        var expected = $"*{text}*";
        var actual = Markdown.Italicise(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Underline"/> method underlines input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("Code Geass: Hangyaku no Lelouch")]
    [InlineData("宇宙戦艦ヤマト2199")]
    public void UnderlineSuccess(string text)
    {
        var expected = $"__{text}__";
        var actual = Markdown.Underline(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Strikethrough"/> method will strikethrough input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("Code Geass: Hangyaku no Lelouch")]
    [InlineData("宇宙戦艦ヤマト2199")]
    public void StrikethroughSuccess(string text)
    {
        var expected = $"~~{text}~~";
        var actual = Markdown.Strikethrough(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Spoiler"/> method spoilers input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("Code Geass: Hangyaku no Lelouch")]
    [InlineData("宇宙戦艦ヤマト2199")]
    public void SpoilerSuccess(string text)
    {
        var expected = $"||{text}||";
        var actual = Markdown.Spoiler(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.InlineCode"/> method inline codes input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("Code Geass: Hangyaku no Lelouch")]
    [InlineData("宇宙戦艦ヤマト2199")]
    public void InlineCodeSuccess(string text)
    {
        var expected = $"`{text}`";
        var actual = Markdown.InlineCode(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.BlockCode(string)"/> method block codes input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("public static void Main(string args){}")]
    [InlineData("func main(){}")]
    public void BlockCodeSuccess(string text)
    {
        var expected = $"```\n{text}\n```";
        var actual = Markdown.BlockCode(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.BlockCode(string, string)"/> method block codes input text with a given language
    /// based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    /// <param name="language">The language for syntax highlighting.</param>
    [Theory]
    [InlineData("public static void Main(string args){}", "cs")]
    [InlineData("func main(){}", "go")]
    public void BlockCodeWithLanguageSuccess(string text, string language)
    {
        var expected = $"```{language}\n{text}\n```";
        var actual = Markdown.BlockCode(text, language);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.BlockQuote"/> method block quotes input text based on Discord's markdown format.
    /// </summary>
    /// <param name="text">The input text to format.</param>
    [Theory]
    [InlineData("Here lies a visitor from a distant star.")]
    [InlineData("井の中の蛙、大海を知らず")]
    public void BlockQuoteSuccess(string text)
    {
        var expected = $">>> {text}\n";
        var actual = Markdown.BlockQuote(text);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.EscapeUrl(Uri)"/> method escapes a <see cref="Uri"/> based on Discord's markdown format.
    /// </summary>
    /// <param name="uri">The input uri to format.</param>
    [Theory]
    [MemberData(nameof(SampleUriTestData))]
    public void EscapeUrlWithUriSuccess(Uri uri)
    {
        var expected = $"<{uri}>";
        var actual = Markdown.EscapeUrl(uri);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.EscapeUrl(string)"/> method escapes a URL based on Discord's markdown format.
    /// </summary>
    /// <param name="url">The url to format.</param>
    [Theory]
    [InlineData("https://github.com/Nihlus/Remora.Discord")]
    [InlineData("https://leijisha.jp/")]
    public void EscapeUrlWithStringSuccess(string url)
    {
        var expected = $"<{url}>";
        var actual = Markdown.EscapeUrl(url);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Hyperlink(Uri)"/> method hyperlinks a uri to itself based on Discord's markdown
    /// format.
    /// </summary>
    /// <param name="uri">The input uri to format.</param>
    [Theory]
    [MemberData(nameof(SampleUriTestData))]
    public void HyperlinkWithUriSuccess(Uri uri)
    {
        var expected = $"[{uri}]({uri})";
        var actual = Markdown.Hyperlink(uri);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Hyperlink(string)"/> method hyperlinks a url to itself based on Discord's markdown
    /// format.
    /// </summary>
    /// <param name="url">The input url to format.</param>
    [Theory]
    [InlineData("https://github.com/Nihlus/Remora.Discord")]
    [InlineData("https://leijisha.jp/")]
    public void HyperlinkWithUrlSuccess(string url)
    {
        var expected = $"[{url}]({url})";
        var actual = Markdown.Hyperlink(url);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Hyperlink(string)"/> method hyperlinks a url to itself based on Discord's markdown
    /// format.
    /// </summary>
    /// <param name="text">The text to hyperlink to..</param>
    /// <param name="uri">The input uri to format.</param>
    [Theory]
    [MemberData(nameof(SampleTextWithUriTestData))]
    public void HyperlinkWithTextAndUriSuccess(string text, Uri uri)
    {
        var expected = $"[{text}]({uri})";
        var actual = Markdown.Hyperlink(text, uri);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Hyperlink(string)"/> method hyperlinks a url to itself based on Discord's markdown
    /// format.
    /// </summary>
    /// <param name="text">The text to hyperlink to..</param>
    /// <param name="url">The input uri to format.</param>
    [Theory]
    [InlineData("GitHub: Remora.Discord", "https://github.com/Nihlus/Remora.Discord")]
    [InlineData("Leiji Matsumoto's Personal Page", "https://leijisha.jp/")]
    public void HyperlinkWithTextAndUrlSuccess(string text, string url)
    {
        var expected = $"[{text}]({url})";
        var actual = Markdown.Hyperlink(text, url);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Timestamp(long, TimestampStyle?)"/> method formats a date based on Discord's markdown
    /// format.
    /// </summary>
    /// <param name="unixTimestamp">The time to format.</param>
    [Theory]
    [InlineData(1156738695L)]
    [InlineData(1590493200L)]
    [InlineData(1052218560L)]
    public void TimestampWithUnixTimestampSuccess(long unixTimestamp)
    {
        var expected = $"<t:{unixTimestamp}>";
        var actual = Markdown.Timestamp(unixTimestamp);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Tests to see if the <see cref="Markdown.Timestamp(long, TimestampStyle?)"/> method formats a date based on Discord's markdown
    /// format.
    /// </summary>
    /// <param name="unixTimestamp">The time to format.</param>
    /// <param name="timestampStyle">The timestamp style.</param>
    [Theory]
    [InlineData(1052218560L, TimestampStyle.ShortTime)]
    [InlineData(1052218560L, TimestampStyle.LongTime)]
    [InlineData(1052218560L, TimestampStyle.ShortDate)]
    [InlineData(1052218560L, TimestampStyle.LongDate)]
    [InlineData(1052218560L, TimestampStyle.ShortDateTime)]
    [InlineData(1052218560L, TimestampStyle.LongDateTime)]
    [InlineData(1052218560L, TimestampStyle.RelativeTime)]
    [InlineData(1052218560L, null)]
    public void TimestampWithUnixTimestampAndTimestampStyleSuccess(long unixTimestamp, TimestampStyle? timestampStyle)
    {
        var expected = timestampStyle switch
        {
            TimestampStyle.ShortTime => $"<t:{unixTimestamp}:t>",
            TimestampStyle.LongTime => $"<t:{unixTimestamp}:T>",
            TimestampStyle.ShortDate => $"<t:{unixTimestamp}:d>",
            TimestampStyle.LongDate => $"<t:{unixTimestamp}:D>",
            TimestampStyle.ShortDateTime => $"<t:{unixTimestamp}:f>",
            TimestampStyle.LongDateTime => $"<t:{unixTimestamp}:F>",
            TimestampStyle.RelativeTime => $"<t:{unixTimestamp}:R>",
            null => $"<t:{unixTimestamp}>",
            _ => throw new ArgumentOutOfRangeException(nameof(timestampStyle), timestampStyle, null)
        };

        var actual = Markdown.Timestamp(unixTimestamp, timestampStyle);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Generates sample uri test data.
    /// </summary>
    /// <returns>
    /// The generated test data.
    /// </returns>
    public static IEnumerable<object[]> SampleUriTestData()
    {
        yield return new object[] { new Uri("https://github.com/Nihlus/Remora.Discord") };
        yield return new object[] { new Uri("https://leijisha.jp/") };
    }

    /// <summary>
    /// Generates sample uri test data with an attributed text to hyperlink to.
    /// </summary>
    /// <returns>
    /// The generated test data.
    /// </returns>
    public static IEnumerable<object[]> SampleTextWithUriTestData()
    {
        yield return new object[] { "GitHub: Remora.Discord", new Uri("https://github.com/Nihlus/Remora.Discord") };
        yield return new object[] { "Leiji Matsumoto's Personal Page", new Uri("https://leijisha.jp/") };
    }
}
