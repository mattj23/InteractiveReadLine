namespace InteractiveReadLine.Tokenizing
{
    public interface IToken
    {
        string Text { get; set; }

        int? Cursor { get; set; }

        IToken Next { get; }

        IToken Previous { get; }

        IToken NextNotHidden { get; }

        IToken PreviousNotHidden { get; }

        bool IsHidden { get; }
    }
}