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
        private string _lastInputText;

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
            _console.CursorLeft = 0;
            _console.Write(_prompt);
            _console.Write(text);
            _console.CursorLeft = cursor + _prompt.Length;
        }
        
        public void Dispose()
        {
            this.Finish();
        }

        private void Start(string prompt = "")
        {
            // _console.WriteLine(string.Empty);
            _console.CursorLeft = 0;
            _console.Write(prompt);
            _lastInputText = string.Empty;
        }

        private void Finish()
        {
            _console.WriteLine(string.Empty);
        }

    }
}