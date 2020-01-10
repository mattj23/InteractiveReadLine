using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Contains the text and cursor position data produced by a user's interaction with the IReadLineProvider.
    /// </summary>
    public class LineState : IEquatable<LineState>
    {
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

        public bool Equals(LineState other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Cursor == other.Cursor && Text == other.Text;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LineState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Cursor * 397) ^ (Text != null ? Text.GetHashCode() : 0);
            }
        }
    }
}