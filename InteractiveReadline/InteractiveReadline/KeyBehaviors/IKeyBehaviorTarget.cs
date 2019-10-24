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
        /// Invokes the auto-complete's "previons" functionality, which substitutes in the previous suggestion
        /// </summary>
        void AutoCompletePrevious();

        /// <summary>
        /// Writes a message out to the console out in the spot where the current read line input is, then
        /// immediately re-displays the line input on the next row.
        /// </summary>
        /// <param name="text">The text to write to the console, a newline char will be added automatically</param>
        void WriteMessage(FormattedText text);

        /// <summary>
        /// If the handler configuration has a tokenizer, this will get the tokenization result of the text
        /// buffer
        /// </summary>
        /// <returns>Returns null if there is no tokenizer, otherwise a Tokens result</returns>
        TokenizedLine GetTextTokens();

        /// <summary>
        /// Tells the readline handler to finish this line of input and return it
        /// </summary>
        void Finish();
    }
}