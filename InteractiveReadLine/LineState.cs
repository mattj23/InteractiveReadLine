using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Contains the text and cursor position data produced by a user's interaction with the IReadLineProvider.
    /// </summary>
    public class LineState : IEquatable<LineState>
    {
        /// <summary>
        /// A string can implicitly be converted to a LineState, however the cursor position will be 0 by default.
        /// </summary>
        public static implicit operator LineState(string s) => new LineState(s, 0);
        
        /// <summary>
        /// Creates a line state from the text of a line and a cursor position within it.
        /// </summary>
        /// <param name="text">The contents of the line</param>
        /// <param name="cursor">The cursor position, as an offset from the first character</param>
        public LineState(string text, int cursor)
        {
            Text = text;
            Cursor = cursor;
        }

        /// <summary>
        /// Gets the cursor position as an integer offset from the first character
        /// </summary>
        public int Cursor { get; }

        /// <summary>
        /// Gets the string contents of the line of text
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Determines whether this line state holds the same text and cursor position as another.
        /// </summary>
        public bool Equals(LineState? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Cursor == other.Cursor && Text == other.Text;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LineState) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Cursor * 397) ^ (Text != null ? Text.GetHashCode() : 0);
            }
        }
    }
}