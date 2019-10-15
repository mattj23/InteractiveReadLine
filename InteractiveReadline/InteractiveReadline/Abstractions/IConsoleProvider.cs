using System;

namespace InteractiveReadLine.Abstractions
{
    /// <summary>
    /// Defines a provider for a console-like interface
    /// </summary>
    public interface IConsoleProvider
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

        void Write(string text);
        void WriteLine(string text);

        ConsoleKeyInfo ReadKey();


    }
}