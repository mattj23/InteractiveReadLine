using System;
using InteractiveReadLine.Abstractions;

namespace InteractiveReadLine
{
    /// <summary>
    /// Exposes a IReadLineProvider that's wrapping the System.Console object
    /// </summary>
    public class SystemReadLine : IReadLineProvider
    {
        private readonly IConsoleProvider _console;

        public SystemReadLine() : this(new SystemConsole()) { }

        /// <summary>
        /// Instantiate a SystemReadLine object with an injected console provider, this mostly exists
        /// for testing.
        /// </summary>
        /// <param name="console"></param>
        public SystemReadLine(IConsoleProvider console)
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

        public void Finish()
        {
            _console.WriteLine(string.Empty);
        }
    }
}