using System;

namespace InteractiveReadLine
{
    /// <summary>
    /// Provides a very basic ReadLine input, with the ability to start and finish an input instance
    /// </summary>
    public interface IReadLine : IDisposable
    {

        ConsoleKeyInfo ReadKey();

        void SetInputText(string text, int cursor);

        /// <summary>
        /// Writes a message out to the console out in the spot where the current read line input is, then
        /// immediately re-displays the line input on the next row.
        /// </summary>
        /// <param name="text">The text to write to the console, a newline char will be added automatically</param>
        void WriteMessage(string text);

    }
}