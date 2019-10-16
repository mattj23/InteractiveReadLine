using System;
using InteractiveReadLine.Abstractions;

namespace InteractiveReadLine
{
    /// <summary>
    /// Exposes a IReadLineProvider that's wrapping a IConsoleProvider object, which is by default
    /// a wrapper around the System.Console object
    /// </summary>
    public class ConsoleReadLine : IReadLineProvider
    {
        private readonly IConsoleProvider _console;

        public ConsoleReadLine() : this(new SystemConsole()) { }

        /// <summary>
        /// Instantiate a ConsoleReadLine object with an injected console provider, this mostly exists
        /// for testing.
        /// </summary>
        /// <param name="console"></param>
        public ConsoleReadLine(IConsoleProvider console)
        {
            _console = console;
        }

        public ConsoleKeyInfo ReadKey()
        {
            return _console.ReadKey();
        }

        public void SetText(string text)
        {

            _console.CursorLeft = 0;
            _console.Write(text);


        }

        public void Start(string prompt = "")
        {
            _console.WriteLine(string.Empty);
            _console.Write(prompt);
        }

        public void Finish()
        {
            _console.WriteLine(string.Empty);
        }
    }
}