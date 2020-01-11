using System;

namespace InteractiveReadLine.Demo.Demos
{
    public class BasicConfig : IDemo
    {
        public string Path => "basic";

        public string Description => "Demonstrates a default configuration";

        public void Action()
        {
            var message = @"This is a basic, default configuration for the handler without any customizations.";
            Console.WriteLine(message);
            Console.WriteLine();

            ConsoleReadLine.ReadLine();
        }
    }
}