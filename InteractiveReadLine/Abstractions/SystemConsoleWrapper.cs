using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Abstractions
{
    /// <summary>
    /// Presents System.Console through the IConsole interface. This default implementation originally existed
    /// to support unit testing. It is public so that callers can decorate or compose it to intercept output
    /// that read line operations write to the system console.
    /// </summary>
    public class SystemConsoleWrapper : IConsole
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
            // Break the text into pieces which have the same foreground and background colors, then write them
            // to the Console object 
            var pieces = text.SplitByFormatting();

            foreach (var piece in pieces)
            {
                if (piece.Length <= 0)
                    continue;
                
                if (piece[0].Foreground == null || piece[0].Background == null)
                    Console.ResetColor();

                if (piece[0].Foreground != null)
                    Console.ForegroundColor = (ConsoleColor) piece[0].Foreground;
                
                if (piece[0].Background != null)
                    Console.BackgroundColor = (ConsoleColor) piece[0].Background;
                
                Console.Write(piece.Text);
            }
        }

        public void WriteLine(FormattedText text)
        {
            this.Write(text);
            this.Write("\n");
        }

        public void Write(FormattedChar c)
        {
            // A single character is a one-character run of formatted text, so defer to the text overload to
            // reuse its color handling. When the character specifies foreground and background colors, this
            // also avoids resetting the console colors immediately before setting both of them.
            this.Write(new FormattedText(c));
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey(true);
        }
    }
}
