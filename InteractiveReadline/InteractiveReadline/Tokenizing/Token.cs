using System;

namespace InteractiveReadLine.Tokenizing
{
    public class Token
    {
        private int _cursorPos;

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

        public int CursorPos
        {
            get => this.HasCursor ? _cursorPos : Int32.MinValue;
            private set => _cursorPos = value;
        }

        public bool HasCursor
        {
            get
            {
                var limitValue = this.Text.Length + this.NextSeparator.Text.Length;
                if (this.Next == null)
                    limitValue++;
                return _cursorPos > Int32.MinValue && _cursorPos < limitValue;
            }
        }


        public void LinkToNext(Token next)
        {
            this.Next = next;
            next.Previous = this;
            next.PrevSeparator = this.NextSeparator;
        }

        public void ReplaceText(string newText)
        {
            if (this.CursorPos > newText.Length)
                this.CursorPos = newText.Length;
            this.Text = newText;
        }

        public void MoveCursorToEnd()
        {
            if (this.HasCursor)
                this.CursorPos = Text.Length;
        }
    }
}