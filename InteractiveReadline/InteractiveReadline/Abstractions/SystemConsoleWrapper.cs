using System;
using InteractiveReadLine.Formatting;

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

        public void Write(FormattedText text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                this.Write(text[i]);
            }
        }

        public void WriteLine(FormattedText text)
        {
            this.Write(text);
            this.Write("\n");
        }

        public void Write(FormattedText.FormattedChar c)
        {
            // TODO: Is there a more efficient way of dealing with this?
            Console.ResetColor();

            if (c.Foreground != null)
                Console.ForegroundColor = (ConsoleColor) c.Foreground;

            if (c.Background != null)
                Console.BackgroundColor = (ConsoleColor) c.Background;

            Console.Write(c.Char);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey(true);
        }
    }
}