using System;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine.KeyBehaviors
{
    public static class CommonBehaviors
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

        public static void MoveCursorLeft(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition > 0)
                target.CursorPosition--;
        }

        public static void MoveCursorRight(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition < target.TextBuffer.Length)
                target.CursorPosition++;
        }

        /// <summary>
        /// Uses a function to create a key behavior that uses the provider's "WriteMessage" method to insert
        /// a message to the user. The handler will provide your function with the tokenized version of the
        /// current text, which will be null if the configuration doesn't have a tokenizer.
        /// </summary>
        /// <param name="message">A function which receives a Tokens object and uses it to create a string
        /// message, this is typically useful for providing help or hints to the user</param>
        /// <returns>A key behavior action which can be registered with the read line configuration</returns>
        public static Action<IKeyBehaviorTarget> WriteMessageFromTokens(Func<TokenizedLine, string> message)
        {
            return new Action<IKeyBehaviorTarget>(t => t.WriteMessage(message(t.GetTextTokens())));
        }

        public static void AutoCompleteNext(IKeyBehaviorTarget target) => target.AutoCompleteNext();

        public static void AutoCompletePrevious(IKeyBehaviorTarget target) => target.AutoCompletePrevious();

        /// <summary>
        /// Finishes the ReadLine input, instructing the handler to return the text as it is
        /// </summary>
        public static void Finish(IKeyBehaviorTarget target) => target.Finish();

        /// <summary>
        /// Inserts the received character at the cursor position if the character is a letter, digit, whitespace,
        /// punctuation, or symbol. Advances the cursor by one.
        /// </summary>
        public static void InsertCharacter(IKeyBehaviorTarget target)
        {
            if (char.IsLetterOrDigit(target.ReceivedKey.KeyChar) ||
                char.IsWhiteSpace(target.ReceivedKey.KeyChar) ||
                char.IsPunctuation(target.ReceivedKey.KeyChar) ||
                char.IsSymbol(target.ReceivedKey.KeyChar))
            {
                target.TextBuffer.Insert(target.CursorPosition, target.ReceivedKey.KeyChar);
                target.CursorPosition++;
            }
        }
    }
}