namespace InteractiveReadLine.KeyBehaviors
{
    public static class StandardBehaviors
    {
        public static void Delete(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition < target.TextBuffer.Length)
                target.TextBuffer.Remove(target.CursorPosition, 1);

        }

        public static void Backspace(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition > 0)
            {
                target.TextBuffer.Remove(target.CursorPosition - 1, 1);
                target.CursorPosition--;
            }
        }

        public static void LeftArrow(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition > 0)
                target.CursorPosition--;
        }

        public static void RightArrow(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition < target.TextBuffer.Length)
                target.CursorPosition++;
        }
    }
}