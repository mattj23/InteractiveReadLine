namespace InteractiveReadLine.Tokenizing
{
    /// <summary>
    /// Represents a block of text which separates two tokens
    /// </summary>
    public class Separator
    {
        public Separator(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }

        public void ReplaceText(string newText)
        {
            this.Text = newText;
        }
    }
}