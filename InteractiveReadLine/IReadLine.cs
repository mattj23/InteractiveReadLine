﻿using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine
{
    /// <summary>
    /// Provides a very basic ReadLine input, with the ability to start and finish an input instance
    /// </summary>
    public interface IReadLine : IDisposable
    {

        ConsoleKeyInfo ReadKey();

        void SetDisplay(LineDisplayState state);
        
        /// <summary>
        /// Inserts text to the console out in the spot where the current read line input is, then
        /// immediately re-displays the line input on the next row. Use this to interrupt the user with a message or
        /// information while the ReadLine is still being used
        /// </summary>
        /// <param name="text">The text to write to the console, a newline char will be added automatically</param>
        void InsertText(FormattedText text);

    }
}