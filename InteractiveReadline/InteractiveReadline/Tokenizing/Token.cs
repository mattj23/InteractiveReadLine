using System;

namespace InteractiveReadLine.Tokenizing
{
    public class Token
    {
        /// <summary>
        /// Creates a Token, not intended to be created by users, but rather generated from a Tokenize object
        /// </summary>
        public Token(string text, Separator next, Separator prev, int cursorPos=int.MinValue)
        {
            this.NextSeparator = next;
            this.PrevSeparator = prev;
            this.CursorPos = cursorPos;
            this.Text = text;
        }

        public Token Next { get; private set; }

        public Token Previous { get; private set; }

        public Separator NextSeparator { get; private set; }

        public Separator PrevSeparator { get; private set; }


        public string Text { get; private set;  }

        public int CursorPos { get; private set; }

        public bool HasCursor => this.CursorPos > Int32.MinValue;

        public void LinkToNext(Token next)
        {
            this.Next = next;
            next.Previous = this;
            next.PrevSeparator = this.NextSeparator;
        }

        public void ReplaceText(string newText)
        {
            this.Text = newText;
            if (this.CursorPos > this.Text.Length)
                this.CursorPos = this.Text.Length;

        }
    }
}