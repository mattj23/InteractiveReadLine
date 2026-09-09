using System;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine.KeyBehaviors
{
    /// <summary>
    /// These are common "behaviors" which are invoked by keys being pressed, and can be added to the handler
    /// either as the default behavior or to be invoked when a special key or character is pressed. They are given
    /// to the handler in the form of Action delegates which receive an IKeyBehaviorTarget, which they can operate
    /// on in order to affect some change on the line of text being written.
    ///
    /// The behaviors provided in this static class are all simple ones, they can be used directly or they can be
    /// composed together or wrapped in other methods which allow access to external state, or any other use
    /// case that can be imagined.
    /// </summary>
    public static class CommonKeyBehaviors
    {
        /// <summary>
        /// Deletes the character after the cursor
        /// </summary>
        /// <param name="target"></param>
        public static void Delete(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition < target.TextBuffer.Length)
                target.TextBuffer.Remove(target.CursorPosition, 1);
        }

        /// <summary>
        /// Deletes the character before the cursor
        /// </summary>
        public static void Backspace(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition > 0)
            {
                target.TextBuffer.Remove(target.CursorPosition - 1, 1);
                target.CursorPosition--;
            }
        }

        /// <summary>
        /// Immediately moves the cursor to the end of the line
        /// </summary>
        public static void MoveCursorToEnd(IKeyBehaviorTarget target)
        {
            target.CursorPosition = target.TextBuffer.Length;
        }

        /// <summary>
        /// Immediately moves the cursor to the beginning of the line 
        /// </summary>
        public static void MoveCursorToStart(IKeyBehaviorTarget target)
        {
            target.CursorPosition = 0;
        }

        /// <summary>
        /// Moves the cursor left (towards the start) of the line, but will not move it past the first character
        /// </summary>
        public static void MoveCursorLeft(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition > 0)
                target.CursorPosition--;
        }

        /// <summary>
        /// Moves the cursor right (towards the end) of the line, but will not move it beyond the end of the line
        /// </summary>
        public static void MoveCursorRight(IKeyBehaviorTarget target)
        {
            if (target.CursorPosition < target.TextBuffer.Length)
                target.CursorPosition++;
        }

        /// <summary>
        /// Clears the entire contents of the text buffer
        /// </summary>
        public static void ClearAll(IKeyBehaviorTarget target)
        {
            target.TextBuffer.Clear();
            target.CursorPosition = 0;
        }

        /// <summary>
        /// Uses a function to create a key behavior that uses the provider's "InsertText" method to insert
        /// a message to the user. The handler will provide your function with the tokenized version of the
        /// current text, which will be null if the configuration doesn't have a tokenizer.
        /// </summary>
        /// <param name="message">A function which receives a Tokens object and uses it to create a string
        /// message, this is typically useful for providing help or hints to the user</param>
        /// <returns>A key behavior action which can be registered with the read line configuration</returns>
        public static Action<IKeyBehaviorTarget> WriteMessageFromTokens(Func<TokenizedLine?, string> message)
        {
            return new Action<IKeyBehaviorTarget>(t => t.InsertText(message(t.GetTextTokens())));
        }

        /// <summary>
        /// Invokes the target's auto-complete next behavior, which substitutes in the next suggestion
        /// at the token where the cursor is currently residing
        /// </summary>
        public static void AutoCompleteNext(IKeyBehaviorTarget target) => target.AutoCompleteNext();

        /// <summary>
        /// Invokes the target's auto-complete previous behavior, which substitutes in the previous suggestion
        /// at the token where the cursor is currently residing
        /// </summary>
        public static void AutoCompletePrevious(IKeyBehaviorTarget target) => target.AutoCompletePrevious();

        /// <summary>
        /// Invokes the target's next history behavior, which replaces the line with the next element
        /// in the history collection. Will do nothing if no history has been provided, or replace the line
        /// with the last entered line state if the end of the history has been reached.
        /// </summary>
        /// <param name="target"></param>
        public static void HistoryNext(IKeyBehaviorTarget target) => target.HistoryNext();

        /// <summary>
        /// Invokes the target's previous history behavior, which replaces the entire line with the previous
        /// element in the history collection. Will do nothing if no history has been provided, or if the oldest
        /// element in the collection has been reached.
        /// </summary>
        /// <param name="target"></param>
        public static void HistoryPrevious(IKeyBehaviorTarget target) => target.HistoryPrevious();

        /// <summary>
        /// Finishes the ReadLine input, instructing the handler to return the text as it is
        /// </summary>
        public static void Finish(IKeyBehaviorTarget target) => target.Finish();

        /// <summary>
        /// Abandons the ReadLine input and discards the entered text. This behavior is conventionally
        /// bound to Ctrl+C.
        /// </summary>
        public static void Cancel(IKeyBehaviorTarget target) => target.Cancel();

        /// <summary>
        /// Signals that the user has no more input and discards the entered text. This behavior is conventionally
        /// behavior conventionally bound to Ctrl+D on an empty line.
        /// </summary>
        public static void EndOfInput(IKeyBehaviorTarget target) => target.EndOfInput();

        /// <summary>
        /// Deletes the character under the cursor, or signals the end of input if the line is empty. This
        /// matches shell behavior for Ctrl+D: it deletes forward when a character is available and otherwise
        /// ends the session.
        /// </summary>
        public static void DeleteOrEndOfInput(IKeyBehaviorTarget target)
        {
            if (target.TextBuffer.Length == 0)
                target.EndOfInput();
            else
                Delete(target);
        }

        /// <summary>
        /// Removes all of the text between the cursor and the end of the line
        /// </summary>
        /// <param name="target"></param>
        public static void CutToEnd(IKeyBehaviorTarget target)
        {
            var cursor = target.CursorPosition;
            var text = target.TextBuffer.ToString();
            var captured = text.Substring(0, cursor);

            target.TextBuffer.Clear();
            target.TextBuffer.Append(captured);
            target.CursorPosition = cursor;

            target.CutForward(text.Substring(cursor));
        }

        /// <summary>
        /// Removes all of the text between the cursor and the start of the line
        /// </summary>
        /// <param name="target"></param>
        public static void CutToStart(IKeyBehaviorTarget target)
        {
            var cursor = target.CursorPosition;
            var text = target.TextBuffer.ToString();
            var captured = text.Substring(cursor, target.TextBuffer.Length - cursor);

            target.TextBuffer.Clear();
            target.TextBuffer.Append(captured);
            target.CursorPosition = 0;

            target.CutBackward(text.Substring(0, cursor));
        }

        /// <summary>
        /// Using a basic split on whitespace strategy, this removes all of the text between the cursor
        /// and the end of the previous word
        /// </summary>
        /// <param name="target"></param>
        public static void CutPreviousWord(IKeyBehaviorTarget target)
        {
            var textBefore = target.TextBuffer.ToString();
            var cursorBefore = target.CursorPosition;

            var tokens =
                CommonLexers.SplitOnWhitespace(new LineState(target.TextBuffer.ToString(), target.CursorPosition));
            var token = tokens.CursorToken;
            if (token?.Cursor == null)
                return;

            int cursor = token.Cursor.Value;
            var previous = token.Previous;

            if (cursor == 0 && previous?.IsHidden == true)
            {
                // Example for "test data here", which should delete back to the beginning
                //                   ↑  

                previous.Text = "";
                if (previous.Previous != null)
                {
                    previous.Previous.Text = "";
                }
            }
            else
            {
                var captured = token.Text.Substring(cursor, token.Text.Length - cursor);
                token.Cursor = 0;
                token.Text = captured;

                if (token.IsHidden && previous != null)
                {
                    previous.Text = "";
                }
            }

            target.TextBuffer.Clear();
            target.TextBuffer.Append(tokens.Text);
            target.CursorPosition = tokens.Cursor;

            // This behavior rebuilds the line from its tokens. Recover the removed text from the span crossed
            // when the cursor moved backward.
            var removedLength = cursorBefore - target.CursorPosition;
            if (removedLength > 0 && cursorBefore <= textBefore.Length)
                target.CutBackward(textBefore.Substring(target.CursorPosition, removedLength));
            else
                target.CutBackward(string.Empty);
        }

        /// <summary>
        /// Inserts the text in the cut buffer at the cursor position and leaves the cursor at the end of
        /// the inserted text. Does nothing when nothing has been cut.
        /// </summary>
        /// <remarks>
        /// The buffer is not emptied by pasting, so the same text can be pasted repeatedly.
        /// </remarks>
        public static void Paste(IKeyBehaviorTarget target)
        {
            var text = target.CutBuffer;
            if (string.IsNullOrEmpty(text))
                return;

            target.TextBuffer.Insert(target.CursorPosition, text);
            target.CursorPosition += text.Length;
        }

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
