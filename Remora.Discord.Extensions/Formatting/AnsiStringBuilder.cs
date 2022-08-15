//
//  AnsiStringBuilder.cs
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

using System.Text;

namespace Remora.Discord.Extensions.Formatting;

/// <summary>
/// Provides a builder to build an ANSI formatted string.
/// </summary>
public class AnsiStringBuilder
{
    private readonly StringBuilder _builder;
    private readonly StyleState _styleState;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiStringBuilder"/> class.
    /// </summary>
    public AnsiStringBuilder()
    {
        _builder = new StringBuilder();
        _styleState = new StyleState();
    }

    /// <summary>
    /// Sets the current bold state of <see cref="AnsiStringBuilder"/> whether the upcoming the should be bold.
    /// </summary>
    /// <param name="bold">Whether to upcoming text should be bold.</param>
    /// <returns>The current <see cref="AnsiStringBuilder"/> for chaining.</returns>
    public AnsiStringBuilder Bold(bool bold = true)
    {
        _styleState.IsBold = bold;
        return this;
    }

    /// <summary>
    /// Sets the current underline state of <see cref="AnsiStringBuilder"/> whether the upcoming the should be
    /// underlined.
    /// </summary>
    /// <param name="underline">Whether to upcoming text should be underlined.</param>
    /// <returns>The current <see cref="AnsiStringBuilder"/> for chaining.</returns>
    public AnsiStringBuilder Underline(bool underline = true)
    {
        _styleState.IsUnderlined = underline;
        return this;
    }

    /// <summary>
    /// Sets the current foreground color of the <see cref="AnsiStringBuilder"/> to the specific foreground color.
    /// </summary>
    /// <param name="foregroundColor">The foreground color.</param>
    /// <returns>The current <see cref="AnsiStringBuilder"/> for chaining.</returns>
    public AnsiStringBuilder Foreground(AnsiForegroundColor foregroundColor = AnsiForegroundColor.None)
    {
        _styleState.ForegroundColor = foregroundColor;
        return this;
    }

    /// <summary>
    /// Sets the current background color of the <see cref="AnsiStringBuilder"/> to the specific background color.
    /// </summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <returns>The current <see cref="AnsiStringBuilder"/> for chaining.</returns>
    public AnsiStringBuilder Background(AnsiBackgroundColor backgroundColor = AnsiBackgroundColor.None)
    {
        _styleState.BackgroundColor = backgroundColor;
        return this;
    }

    /// <summary>
    /// Resets all styling.
    /// </summary>
    /// <returns>The current <see cref="AnsiStringBuilder"/> for chaining.</returns>
    public AnsiStringBuilder Reset()
    {
        _styleState.Reset();
        return this;
    }

    /// <summary>
    /// Appends the <c>text</c> with the current styling.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The current <see cref="AnsiStringBuilder"/> for chaining.</returns>
    public AnsiStringBuilder Append(string text)
    {
        _styleState.AppendToStringBuilder(_builder);
        _builder.Append(text);

        return this;
    }

    /// <summary>
    /// Appends the text with the current styling and adds a new line to the end.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <returns>The current <see cref="AnsiStringBuilder"/> for chaining.</returns>
    public AnsiStringBuilder AppendLine(string? text = default)
    {
        if (text is not null)
        {
            Append(text);
        }

        _builder.Append('\n');
        return this;
    }

    /// <summary>
    /// Build the ansi formatted string.
    /// </summary>
    /// <returns>Returns a string containing the ansi formatting codes.</returns>
    public string Build()
    {
        return _builder.ToString();
    }

    /// <summary>
    /// Class for tracking the active styling state.
    /// </summary>
    private sealed class StyleState
    {
        private const char _escapeChar = '\u001b';

        private bool _hasChanged;
        private bool _isBold;
        private bool _isUnderlined;
        private AnsiForegroundColor _foregroundColor = AnsiForegroundColor.None;
        private AnsiBackgroundColor _backgroundColor = AnsiBackgroundColor.None;

        /// <summary>
        /// Gets or sets a value indicating whether the text should be bold.
        /// </summary>
        public bool IsBold
        {
            get => _isBold;
            set
            {
                if (_isBold == value)
                {
                    return;
                }

                _isBold = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text should be underlined.
        /// </summary>
        public bool IsUnderlined
        {
            get => _isUnderlined;
            set
            {
                if (_isUnderlined == value)
                {
                    return;
                }

                _isUnderlined = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the text.
        /// </summary>
        public AnsiForegroundColor ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                if (_foregroundColor == value)
                {
                    return;
                }

                _foregroundColor = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets the background color of the text.
        /// </summary>
        public AnsiBackgroundColor BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value)
                {
                    return;
                }

                _backgroundColor = value;
                _hasChanged = true;
            }
        }

        /// <summary>
        /// Resets the complete styling state to default values.
        /// </summary>
        public void Reset()
        {
            this.IsBold = false;
            this.IsUnderlined = false;
            this.ForegroundColor = AnsiForegroundColor.None;
            this.BackgroundColor = AnsiBackgroundColor.None;
        }

        /// <summary>
        /// Appends the ANSI formatting code when the styling has been changed.
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append to.</param>
        public void AppendToStringBuilder(StringBuilder stringBuilder)
        {
            // Do not append ANSI styling code when the style has not been changed
            if (!_hasChanged)
            {
                return;
            }

            _hasChanged = false;

            stringBuilder.Append($"{_escapeChar}[{AnsiStyle.Reset}");

            if (this.IsBold)
            {
                stringBuilder.Append($";{AnsiStyle.Bold}");
            }

            if (this.IsUnderlined)
            {
                stringBuilder.Append($";{AnsiStyle.Underline}");
            }

            if (this.BackgroundColor is not AnsiBackgroundColor.None)
            {
                stringBuilder.Append($";{(int)this.BackgroundColor}");
            }

            if (this.ForegroundColor is not AnsiForegroundColor.None)
            {
                stringBuilder.Append($";{(int)this.ForegroundColor}");
            }

            stringBuilder.Append('m');
        }
    }
}
