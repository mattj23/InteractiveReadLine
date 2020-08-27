using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Demo.Demos.Formatters
{
    public class PasswordBarPrompt : IDemo
    {
        public string Description => "Shows a more complicated password prompt with a unique visual representation";
        
        public void Action()
        {
            Console.WriteLine("This shows a special password prompt which replaces the entered text with");
            Console.WriteLine("a unique visual representation.");

            var config = ReadLineConfig.Basic
                .SetFormatter(CommonFormatters.PasswordBar.WithFixedPrompt("Enter Password:"));

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}