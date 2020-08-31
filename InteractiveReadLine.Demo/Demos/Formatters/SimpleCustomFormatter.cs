using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Demo.Demos.Formatters
{
    public class SimpleCustomFormatter : IDemo
    {
        public string Description => "A simple custom formatter based on a LineState";
        public void Action()
        {
            
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