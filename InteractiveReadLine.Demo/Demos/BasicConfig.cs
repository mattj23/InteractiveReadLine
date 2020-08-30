using System;

namespace InteractiveReadLine.Demo.Demos
{
    public class BasicConfig : IDemo
    {
        public string Description => "Demonstrates a default configuration";

        public void Action()
        {
            Console.WriteLine("This is a basic, default configuration for the handler without any customizations.\n");

            ConsoleReadLine.ReadLine();
        }
    }
}