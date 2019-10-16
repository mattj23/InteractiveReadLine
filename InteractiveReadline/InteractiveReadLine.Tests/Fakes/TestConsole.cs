using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteractiveReadLine.Abstractions;

namespace InteractiveReadLine.Tests.Fakes
{
    /// <summary>
    /// Class to simulate a System.Console text buffer
    /// </summary>
    public class TestConsole : IConsoleWrapper
    {
        private readonly char[,] _buffer;
        private readonly int _height;
        private readonly int _width;
        private readonly Queue<ConsoleKeyInfo> _keys;

        public TestConsole(int height, int width, IEnumerable<ConsoleKeyInfo> keys)
        {
            _height = height;
            _width = width;
            _buffer = new char[_height, _width];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _buffer[i, j] = char.MinValue;
                }
            }

            _keys = new Queue<ConsoleKeyInfo>(keys);
        }

        public TestConsole(int height, int width)
            : this(height, width, Enumerable.Empty<ConsoleKeyInfo>())
        {
        }

        public int CursorLeft { get; set; }

        public int CursorTop { get; set; }

        public int BufferHeight => _height;

        public int BufferWidth => _width;

        public void Write(string text)
        {
            foreach (var c in text)
                this.WriteChar(c);
        }

        public void WriteLine(string text)
        {
            this.Write(text);
            this.WriteChar('\n');
        }

        public ConsoleKeyInfo ReadKey()
        {
            return _keys.Dequeue();
        }

        public string GetRow(int row)
        {
            var rowText = new StringBuilder();
            for (int j = 0; j < _width; j++)
            {
                rowText.Append(_buffer[row, j]);
            }

            return rowText.ToString().TrimEnd(char.MinValue);
        }

        public string BufferToString()
        {
            var output = new List<string>();
            for (int i = 0; i < _height; i++)
            {
                output.Add(this.GetRow(i));
            }

            return string.Join("\n", output);
        }

        /// <summary>
        /// Writes the character at the current cursor position, then advances the cursor forward. If the cursor
        /// advances beyond the end of the buffer width, or gets a carriage return, it moves down to the next line.
        /// </summary>
        /// <param name="c"></param>
        private void WriteChar(char c)
        {
            if (c != '\n')
                _buffer[CursorTop, CursorLeft] = c;

            CursorLeft++;
            if (CursorLeft == _width || c == '\n')
            {
                // Move down to the next line
                CursorLeft = 0;
                CursorTop++;

                if (CursorTop == _height)
                {
                    // Copy the entire buffer up one so that the last row is empty
                    for (int i = 0; i < _height - 1; i++)
                    {
                        for (int j = 0; j < _width; j++)
                        {
                            _buffer[i, j] = _buffer[i + 1, j];
                        }
                    }

                    for (int j = 0; j < _width; j++)
                    {
                        _buffer[_height - 1, j] = char.MinValue;
                    }

                    // bring the top cursor back
                    CursorTop--;
                }
            }
        }

        
    }
}