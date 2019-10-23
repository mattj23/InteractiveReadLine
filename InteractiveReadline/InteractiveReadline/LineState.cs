using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Represents all of the state information in a readline view
    /// </summary>
    public class LineState
    {
        public LineState(string text, int cursor)
        {
            Text = text;
            Cursor = cursor;
        }

        public int Cursor { get; }

        public string Text { get; }
    }
}