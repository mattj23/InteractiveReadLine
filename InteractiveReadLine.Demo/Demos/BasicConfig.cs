using System;

namespace InteractiveReadLine.Demo.Demos
{
    public class BasicConfig : IDemo
    {
        public string Description => "Demonstrates a default configuration";

        public void Action()
        {
            Console.WriteLine("This is a basic, default configuration for the handler without any customizations.\n");
            
            /* This demonstrates a default configuration of the ReadLine without any customizations.
             Under the hood this uses a ReadLineConfig.Basic configuration, which contains only the 
             standard key behavior (enter, backspace, home/end, arrow keys, and up/down for history)."
             
             Note that this is different from the ReadLineConfig.Empty configuration (an Empty 
             configuration is useless until at least the default and finish key behavior is added).
             
             This is the simplest way to use ReadLine, but typically you will want to use a configuration.
             */

            var result = ConsoleReadLine.ReadLine();
        }
    }
}