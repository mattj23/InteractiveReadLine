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
        StringBuilder TextBuffer { get; }

        int CursorPosition { get; set; }

        void AutoCompleteNext();

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
    }
}