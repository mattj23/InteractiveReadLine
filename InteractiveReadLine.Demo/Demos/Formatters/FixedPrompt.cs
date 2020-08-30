using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Demo.Demos.Formatters
{
    public class FixedPrompt : IDemo
    {
        public string Description => "Demonstrates a fixed prompt";

        public void Action()
        {
            Console.WriteLine("This shows a very basic use of a formatter; to insert fixed text in front of the input area.\n");
            
            var config = ReadLineConfig.Basic
                .SetFormatter(CommonFormatters.FixedPrompt("prompt text #"));

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}