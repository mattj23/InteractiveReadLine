using System;
using System.Text;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine.KeyBehaviors
{
    /// <summary>
    /// Provides the unified target of key behaviors, exposing a standard amount of state which the
    /// behavior method can act upon
    /// </summary>
    public interface IKeyBehaviorTarget
    {
        /// <summary>
        /// Gets the ConsoleKey which was received for the current request
        /// </summary>
        ConsoleKeyInfo ReceivedKey { get; }

        /// <summary>
        /// Gets the string buffer which holds the current readline text which is being edited by the user
        /// </summary>
        StringBuilder TextBuffer { get; }

        /// <summary>
        /// Gets or sets the integer position of the cursor. Cannot be set before the beginning or after the end
        /// of the TextBuffer.
        /// </summary>
        int CursorPosition { get; set; }

        /// <summary>
        /// Invokes the auto-complete's "next" functionality, which substitutes in the next suggestion
        /// </summary>
        void AutoCompleteNext();

        /// <summary>
        /// Invokes the auto-complete's "previous" functionality, which substitutes in the previous suggestion
        /// </summary>
        void AutoCompletePrevious();

        /// <summary>
        /// Inserts text to the console out in the spot where the current read line input is, then
        /// immediately re-displays the line input on the next row. Use this to interrupt the user with a message or
        /// information while the ReadLine is still being used
        /// </summary>
        /// <param name="text">The text to write to the console, a newline char will be added automatically</param>
        void InsertText(FormattedText text);

        /// <summary>
        /// Gets the tokenized text buffer when the handler configuration has a lexer.
        /// </summary>
        /// <returns>The tokenized line, or null when the configuration has no lexer.</returns>
        TokenizedLine? GetTextTokens();

        /// <summary>
        /// Invokes the history's "next" functionality, which replaces the entire line with the next element
        /// in the history collection. If the last history element has been reached, the previously entered
        /// LineState will be reverted.
        /// </summary>
        void HistoryNext();

        /// <summary>
        /// Invokes the history's "previous" functionality, which replaces the entire line with the previous
        /// element in the history collection.
        /// </summary>
        void HistoryPrevious();

        /// <summary>
        /// Gets the text accumulated by the most recent run of cut operations. The paste behavior inserts this
        /// text at the cursor. The value is empty when nothing has been cut.
        /// </summary>
        /// <remarks>
        /// The buffer belongs to a single read line operation and does not carry over to the next one. It is
        /// populated through CutForward and CutBackward, which accumulate consecutive cuts into one piece of
        /// text that can be pasted.
        /// </remarks>
        string CutBuffer { get; }

        /// <summary>
        /// Records text removed from in front of the cursor, placing it at the end of the cut buffer.
        /// </summary>
        /// <remarks>
        /// Cut text is stored in its original order on the line, so a run of cuts can be pasted back as the
        /// original text. A behavior that removes text in front of the cursor, such as cutting to the end of
        /// the line, should report it here.
        /// </remarks>
        /// <param name="text">The text that was removed from the line.</param>
        void CutForward(string text);

        /// <summary>
        /// Records text removed from behind the cursor, placing it at the front of the cut buffer.
        /// </summary>
        /// <remarks>
        /// A behavior that removes text from behind the cursor, such as cutting to the start of the line or
        /// cutting the previous word, should report it here. Cutting three words backward one after
        /// another therefore leaves the three words in the buffer in their original order.
        /// </remarks>
        /// <param name="text">The text that was removed from the line.</param>
        void CutBackward(string text);

        /// <summary>
        /// Tells the readline handler to finish this line of input and return it
        /// </summary>
        void Finish();

        /// <summary>
        /// Tells the ReadLine handler to abandon the current line and discard the entered text. This matches
        /// conventional Ctrl+C behavior: the handler records nothing in history and completes without an error.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Tells the ReadLine handler that the user has signaled the end of input, which is what Ctrl+D on an
        /// empty line means to a shell. The line is discarded, and the caller is told that no
        /// further input is coming.
        /// </summary>
        void EndOfInput();
    }
}
