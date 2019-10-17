using System;
using InteractiveReadLine.Abstractions;

namespace InteractiveReadLine
{
    /// <summary>
    /// Exposes a IReadLine that's wrapping a IConsoleWrapper object, which is by default
    /// a wrapper around the System.Console object
    /// </summary>
    public class ConsoleReadLine : IReadLine
    {
        private readonly string _prompt;
        private readonly IConsoleWrapper _console;
        private string _lastWrittenText;
        private int _lastWrittenCursor;
        private int _startingRow;

        public ConsoleReadLine(string prompt = "") 
            : this(prompt, new SystemConsoleWrapper()) { }

        public ConsoleReadLine(string prompt, IConsoleWrapper console)
        {
            _prompt = prompt;
            _console = console;
            this.Start(_prompt);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return _console.ReadKey();
        }

        public void SetInputText(string text, int cursor)
        {
            this.SetText(_prompt + text, _prompt.Length + cursor);
        }

        private void SetText(string totalText, int cursorPos)
        {
            // The process of setting the input text requires us to find the difference between 
            // the last written text and the new text, then perform the minimum amount of character
            // writes necessary to make the two identical

            // First, we should determine what the new line needs to look like. If the new line is 
            // longer than the old line, we will write the new line exactly.  If it's shorter, we'll 
            // need to pad it out with empty characters 
            var writeText = (totalText.Length >= _lastWrittenText.Length)
                ? totalText
                : totalText + new string(' ', _lastWrittenText.Length - totalText.Length);

            // Sweep through each character in the text to write and, if the cursor is not at the
            // position to edit and an edit needs to be made, move the cursor and write the new
            // character to the console
            for (int i = 0; i < writeText.Length; i++)
            {
                if (i < _lastWrittenText.Length 
                    && writeText[i] == _lastWrittenText[i])
                    continue;

                int left = this.ColOffset(i);
                int top = this.RowOffset(i) + _startingRow;

                if (left != _console.CursorLeft)
                    _console.CursorLeft = left;
                if (top != _console.CursorTop)
                    _console.CursorTop = top;

                _console.Write(writeText[i]);
            }

            _lastWrittenText = totalText;

            // Check if we shifted down the buffer. In certain cases, if we reach the end of the buffer
            // height and we skip a line, the System.Console shifts everything up, and our starting row
            // will effectively be less than where we started.  It will never move down.
            var writtenRowOffset = this.RowOffset(_lastWrittenText.Length);
            if (writtenRowOffset + _startingRow >= _console.BufferHeight)
            {
                _startingRow = _console.BufferHeight - writtenRowOffset - 1;
            }

            _console.CursorTop = _startingRow + this.RowOffset(cursorPos);
            _console.CursorLeft = this.ColOffset(cursorPos);
        }

        /// <summary>
        /// Writes a message out to the console out in the spot where the current readline input is, then
        /// immediately redisplays the line input below
        /// </summary>
        /// <param name="text">The text to write to the console. This does not need to be terminated with a
        /// newline character, as one will be added automatically.</param>
        public void WriteMessage(string text)
        {
            var currentText = _lastWrittenText;

        }
        
        public void Dispose()
        {
            this.Finish();
        }

        private void Start(string prompt = "")
        {
            // _console.WriteLine(string.Empty);
            _startingRow = _console.CursorTop;
            _console.CursorLeft = 0;
            _console.Write(prompt);
            _lastWrittenText = prompt;
        }

        private void Finish()
        {
            _console.WriteLine(string.Empty);
        }

        private int ColOffset(int length) => length % _console.BufferWidth;

        private int RowOffset(int length) => (length - this.ColOffset(length)) / _console.BufferWidth;
    }
}