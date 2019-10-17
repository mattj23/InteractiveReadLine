using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Represents all of the state information in a readline view
    /// </summary>
    public struct LineState
    {
        public LineState(string text, int cursorPos)
        {
            Text = text;
            CursorPos = cursorPos;
        }

        public int CursorPos { get; }

        public string Text { get; }
    }
}