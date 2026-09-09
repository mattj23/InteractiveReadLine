using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Abstractions
{
    /// <summary>
    /// Defines a provider for a buffer backed, console-like interface.  Intended to create a seam between
    /// the actual System.Console instance and the higher level behaviors, it can also be used to wrap
    /// any character buffer interface with a moving cursor.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Gets or sets the cursor's position from the left edge of the buffer
        /// </summary>
        int CursorLeft { get; set; } 

        /// <summary>
        /// Gets or sets the cursor's position from the top of the buffer
        /// </summary>
        int CursorTop { get; set; }

        /// <summary>
        /// Gets the height of the provider's buffer, measured in rows
        /// </summary>
        int BufferHeight { get; }

        /// <summary>
        /// Gets the width of the provider's buffer, measured in columns
        /// </summary>
        int BufferWidth { get; }

        /// <summary>
        /// Gets or sets whether Ctrl+C is delivered as an ordinary keypress rather than being treated as a
        /// signal to terminate the program.
        /// </summary>
        /// <remarks>
        /// A read line operation enables this property so that a Ctrl+C key behavior can abandon the line, then
        /// restores the previous value when the operation finishes. Implementations without a system console
        /// can store the value without acting on it.
        /// </remarks>
        bool TreatControlCAsInput { get; set; }

        void Write(FormattedText text);
        void WriteLine(FormattedText text);
        void Write(FormattedChar c);

        ConsoleKeyInfo ReadKey();
    }
}
