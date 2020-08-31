using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Demo.Demos.Formatters
{
    public class SimpleCustomFormatter : IDemo
    {
        public string Description => "A simple custom formatter based on a LineState";
        public void Action()
        {
            Console.WriteLine("The following formatter has both a static prompt and a suffix that displays");
            Console.WriteLine("the character count in the text buffer.\n");
            
            /* This example creates a formatter which displays a static prompt and a suffix which displays the total
             number of characters in the text buffer.
             
             The formatter is created separately here because the type inference is ambiguous between a 
             Func<LineState, LineDisplayState> and Func<TokenizedLine, LineDisplayState> when defining the formatter
             directly as a lambda.
             */
            
            var formatter = new Func<LineState, LineDisplayState>(ls =>
                {
                    var prefix = new FormattedText("prompt $ ", ConsoleColor.Red);
                    var suffix = new FormattedText($" [{ls.Text.Length} characters]", ConsoleColor.Blue);
                    return new LineDisplayState(prefix, ls.Text, suffix, ls.Cursor);
                });

            var config = ReadLineConfig.Basic
                .SetFormatter(formatter);

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}