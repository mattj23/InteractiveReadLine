using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Represents the data produced by the user's interaction with the readline provider: the text and the current
    /// cursor position.
    /// </summary>
    public class LineState : IEquatable<LineState>
    {
        public LineState(string text, int cursor)
        {
            Text = text;
            Cursor = cursor;
        }

        public int Cursor { get; }

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