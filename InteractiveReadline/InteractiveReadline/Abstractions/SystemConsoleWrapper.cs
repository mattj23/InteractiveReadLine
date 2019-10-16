using System;

namespace InteractiveReadLine.Abstractions
{
    /// <summary>
    /// A wrapper around the System.Console object. 
    /// </summary>
    public class SystemConsoleWrapper : IConsoleWrapper
    {
        public int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public int BufferHeight => Console.BufferHeight;

        public int BufferWidth => Console.BufferWidth;

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey(true);
        }
    }
}