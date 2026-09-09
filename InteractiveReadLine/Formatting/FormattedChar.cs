using System;

namespace InteractiveReadLine.Formatting
{
    /// <summary>
    /// A struct representing a single character and its foreground and background colors. Null colors represents the
    /// system defaults.
    /// </summary>
    public struct FormattedChar : IEquatable<FormattedChar>
    {
        /// <summary>
        /// Creates a character with the specified foreground and background colors. Either color can be null
        /// to use the display default.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <param name="foreground">The foreground color, or null to use the display default.</param>
        /// <param name="background">The background color, or null to use the display default.</param>
        public FormattedChar(char c, ConsoleColor? foreground, ConsoleColor? background)
        {
            Char = c;
            Foreground = foreground;
            Background = background;
        }

        /// <summary>
        /// Gets the character without its formatting.
        /// </summary>
        public char Char { get; }
        /// <summary>
        /// Gets the foreground color, or null when the display default should be used.
        /// </summary>
        public ConsoleColor? Foreground { get; }
        /// <summary>
        /// Gets the background color, or null when the display default should be used.
        /// </summary>
        public ConsoleColor? Background { get; }

        /// <summary>
        /// Determines whether this instance and another formatted character have the same character and colors.
        /// </summary>
        public bool Equals(FormattedChar other)
        {
            return Char == other.Char && Foreground == other.Foreground && Background == other.Background;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is FormattedChar other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Char.GetHashCode();
                hashCode = (hashCode * 397) ^ Foreground.GetHashCode();
                hashCode = (hashCode * 397) ^ Background.GetHashCode();
                return hashCode;
            }
        }
    }
}
