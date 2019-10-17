namespace InteractiveReadLine.Tokenizing
{
    public class Token
    {
        /// <summary>
        /// Creates a Token, not intended to be created by users, but rather generated from a Tokenize object
        /// </summary>
        public Token(string text, string prevSep, string nextSep, int cursorPos=int.MinValue)
        {
            this.NextSeparator = nextSep ?? string.Empty;
            this.CursorPos = cursorPos;
            this.PreviousSeparator = prevSep ?? string.Empty;
            this.Text = text;
        }

        public Token Next { get; private set; }

        public Token Previous { get; private set; }

        public string NextSeparator { get; }

        public string PreviousSeparator { get; }

        public string Text { get; private set;  }

        public int CursorPos { get; private set; }

        public bool HasCursor => this.CursorPos > int.MinValue;

        public void LinkToNext(Token next)
        {
            this.Next = next;
            next.Previous = this;
        }

        public void ReplaceText(string newText)
        {
            this.Text = newText;
            if (this.CursorPos > this.Text.Length + this.NextSeparator.Length)
                this.CursorPos = this.Text.Length + this.NextSeparator.Length;
        }
    }
}