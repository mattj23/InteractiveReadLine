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
            // Go back to the start
            _console.CursorTop = _startingRow;
            _console.CursorLeft = 0;

            var totalText = _prompt + text;
            _console.Write(totalText);
            _lastWrittenText = totalText;

            // Check if we shifted down the buffer
            var writtenRowOffset = this.RowOffset(_lastWrittenText.Length);
            if (writtenRowOffset + _startingRow >= _console.BufferHeight)
            {
                _startingRow = _console.BufferHeight - writtenRowOffset - 1;
            }

            var cursorPos = _prompt.Length + cursor;
            _console.CursorTop = _startingRow + this.RowOffset(cursorPos);
            _console.CursorLeft = this.ColOffset(cursorPos);
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