using System;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Demo.Demos.Formatters
{
    public class PasswordStarPrompt : IDemo
    {
        public string Description => "Demonstrates a password prompt";
        public void Action()
        {
            Console.WriteLine("This shows a password prompt, which displays all text as stars.");
            var config = ReadLineConfig.Basic
                .SetFormatter(CommonFormatters.PasswordStars.WithFixedPrompt("Enter Password: "));

            var result = ConsoleReadLine.ReadLine(config);
        }
    }
}